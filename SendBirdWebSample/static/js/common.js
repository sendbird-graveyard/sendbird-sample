
var appId = 'A7A2672C-AD11-11E4-8DAA-0A18B21C2D82'; // Sample SendBird app id
var sendbird = null;
var currChannelInfo = {};
var currChannelUrl = null;
var loadMoreScroll = false;
var currScrollHeight = 0;
var FILE_ICON = ['ppt', 'xls', 'pdf', 'doc'];

function notifyMe() {
  if (window.Notification && Notification.permission === "granted") {
    console.log("Notification is already granted.");
  } else if (window.Notification && Notification.permission !== "denied") {
    Notification.requestPermission(function (status) {
      if (Notification.permission !== status) {
        Notification.permission = status;
      }

      if (status === "granted") {
        console.log("Notification is granted.");
      } else {
        console.log("Notification is denied.");
      }
    });
  }
}

function isCurrentUser(guestId) {
  return (getGuestId()==guestId) ? true : false;
}

function checkGuestId() {
  var name = getGuestId();
  if (name.trim().length == 0) {
    return generateUUID();
  }
  return name;
}

function getGuestId() {
  var name = 'guest_id=';
  var ca = document.cookie.split(';');
  for (var i=0 ; i<ca.length ; i++) {
    var c = ca[i];
    while (c.charAt(0)==' ') c = c.substring(1);
    if (c.indexOf(name) == 0) {
      return c.substring(name.length,c.length);
    }
  }
  return '';
}

function generateUUID() {
  var d = new Date().getTime();
  var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = (d + Math.random()*16)%16 | 0;
      d = Math.floor(d/16);
      return (c=='x' ? r : (r&0x3|0x8)).toString(16);
  });
  return setCookieGuestId(uuid);
}

function setCookieGuestId(uuid) {
  document.cookie = "guest_id=" + uuid + '; expires=Fri, 31 Dec 9999 23:59:59 GMT';
  return uuid;
}

function startSendBird(guestId, nickName) {

  /** SendBird Settings */
  var option = {
    app_id: appId,  // Sample SendBird app
    guest_id: guestId, // Unique key
    user_name: nickName,  // Generate User Nickname
    image_url: '' // if you  want setting user image, set this value. if you don't, you delete this value or set empty value
  };
  sendbird = new SendBird(option); // create SendBird

  // this event is a function that run when receive message
  sendbird.events.onMessageReceived = function(obj) {
    setChatMessage(obj);
  };

  // this event is a function that run when receive system message
  sendbird.events.onSystemMessageReceived = function(obj) {
    setSysMessage(obj);
  };

  // this event is a function that run when receive file message
  sendbird.events.onFileMessageReceived = function(obj) {
    if (obj.type.indexOf('image') < 0) {
      setFileMessage(obj);
    } else {
      setImageMessage(obj);
    }
  };

  // this event is a function that run when receive broadcast message
  sendbird.events.onBroadcastMessageReceived = function(obj) {
    setBroadcastMessage(obj);
  };

  sendbird.events.onMessagingChannelUpdateReceived = function(obj) {
    console.log(obj);
  };
  /** END SendBird Settings */


}

function scrollPositionBottom() {
  var scrollHeight = $("#chat_canvas")[0].scrollHeight;
  $("#chat_canvas")[0].scrollTop = scrollHeight;
  currScrollHeight = scrollHeight;
}

function scrollAfterImageLoad(obj) {
  $("#chat_canvas")[0].scrollTop = $("#chat_canvas")[0].scrollTop + obj.height;
}


function leaveChannel(index, channelUrl) {
  var url = channelUrl; // this value is channel url that users wanted leave.

  // This function let user leave the channel that user belongs to.
  sendbird.leaveChannel(
    url,
    {
      "successFunc": function(data) {
        console.log(data);
      }
    }
  );
}

function joinChannel(channelUrl) {
  var url = channelUrl; // this value is channel url that users wanted join.

  // This function let user join the channel that user belongs to.
  sendbird.joinChannel(
    url,
    {
     "successFunc" : function(data) {
       currChannelInfo = data;
       initChatMessage(currChannelInfo);
       $('#channel_modal').modal('hide');
       sendbird.connect();
       loadMoreChatMessage();
       scrollPositionBottom();
     }
    }
  );
}

function joinMessagingChannel(channelUrl) {
  var url = channelUrl; // this value is channel url that users wanted join.

  // This function let user join the channel that user belongs to.
  sendbird.joinMessagingChannel(
    url,
    {
      "successFunc" : function(data) {
        currChannelInfo = data.channel;
        initChatMessage(currChannelInfo);
        $('#channel_modal').modal('hide');
        sendbird.connect();
        loadMoreChatMessage();
        scrollPositionBottom();
        sendbird.markAsRead(url);
     }
    }
  );
}

function initChatMessage(info) {
  $('#chat_canvas').html('');
  currChannelUrl = info['channel_url']; // set global value
  setWelcomeMessage(info['name']);
}

function setWelcomeMessage(channelName) {
  var welcomeStr = '<h1 style="text-align: center;">Welcome to <b style="color: #000;">' + channelName + '</b></h1>';
  $('#welcome_canvas').html(welcomeStr);
  $('#welcome_modal').modal('show');
  setTimeout(function() { $('#welcome_modal').modal('hide'); }, 1000);
}

function setChatMessage(obj) {
  var msgList = messageList(obj);

  $('#chat_canvas').append(msgList);

  scrollPositionBottom();
}

function notifyMessage(message) {
  var iconUrl = location.protocol + '//' + location.host + '/static/img/icon-jiver-120px.png';
  if (window.Notification && Notification.permission === "granted") {
    var noti = new Notification("SendBird | " + currChannelUrl, {
      icon: iconUrl,
      body: message,
      tag: currChannelUrl
    });

    noti.onclick = function(data){
      window.focus();
    }

  }
}

function messageList(obj) {

  var msgList = '';

  // this function is that compare to current user using users id that input parameter.
  if (isCurrentUser(obj['user']['guest_id'])) {
    msgList += userMessage(obj);
  } else {
    msgList += memberMessage(obj);

    if (!document.hasFocus()) {
      notifyMessage(obj['message']);
    }

  }
  return msgList;
}

function userMessage(obj) {
  return '<li class="list-group-item chat-user"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b> ' + convertLinkMessage(obj['message']) + '</li>';
}

function memberMessage(obj) {
  return '<li class="list-group-item chat-member"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b> ' + convertLinkMessage(obj['message']) + '</li>';
}

function nameInjectionCheck(name) {
  name = name.replace(/</g, '&lt;');
  name = name.replace(/>/g, '&gt;');
  return name;
}

function convertLinkMessage(msg) {
  var returnString = '';

  msg = msg.replace(/</g, '&lt;');
  msg = msg.replace(/>/g, '&gt;');
  
  var urlexp = new RegExp('(http|ftp|https)://[a-z0-9\-_]+(\.[a-z0-9\-_]+)+([a-z0-9\-\.,@\?^=%&;:/~\+#]*[a-z0-9\-@\?^=%&;/~\+#])?', 'i');
  if (urlexp.test(msg)) {
    returnString += '<a href="' + msg + '" target="_blank">' + msg + '</a>';
  } else {
    returnString += msg;
  }

  return returnString;
}

function loadMoreChatMessage() {
  var scrollHeight = $("#chat_canvas")[0].scrollHeight;

  // this function is that get message from server.
  var moreMessage = {};
  sendbird.getMessageLoadMore({
    "successFunc": function(data) {
      var msgList = '';
      moreMessage = data["messages"];
      $.each(moreMessage.reverse(), function(index, msg) {
        var item = sendbird.commandSeparate(msg);
        if (sendbird.isMessage(item.cmd)) {
          msgList += messageList(item.payload);
        } else if (sendbird.isFileMessage(item.cmd)) {
          if (!sendbird.hasImage(item.payload)) {
            msgList += fileMessageList(item.payload);
          } else {
            msgList += imageMessageList(item.payload);
          }
        }
      });
      $('#chat_canvas').prepend(msgList);
    }
  });

  loadMoreScroll = true;
  $("#chat_canvas")[0].scrollTop = $("#chat_canvas")[0].scrollHeight - scrollHeight;
}

function setSysMessage(obj) {
  var msgList = sysMessage(obj);

  $('#chat_canvas').append(msgList);
  scrollPositionBottom();
}

function sysMessage(obj) {
  return '<li class="list-group-item chat-system">' + obj['message'] + '</li>';
}

function setBroadcastMessage(obj) {
  var msgList = broadcastMessage(obj);

  $('#chat_canvas').append(msgList);
  scrollPositionBottom();
}

function broadcastMessage(obj) {
  return '<li class="list-group-item chat-broadcast"><b>' + obj['message'] + '</b></li>';
}

function setFileMessage(obj) {
  var msgList = fileMessageList(obj);

  $('#chat_canvas').append(msgList);
  scrollPositionBottom();
}

function fileMessageList(obj) {
  var msgList = '';

  var icon = obj['type'].substring(obj['type'].indexOf('/')+1);
  icon = $.inArray( icon, FILE_ICON ) < 0 ? 'etc' : icon;

  // this function is that compare to current user using users id that input parameter.
  if (isCurrentUser(obj['user']['guest_id'])) {
    msgList += userFileMessage(obj, icon);
  } else {
    msgList += memberFileMessage(obj, icon);
  }
  return msgList;
}

function userFileMessage(obj, icon) {
  var returnMsgString = '';
  returnMsgString += '<li class="list-group-item chat-user"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b>' +
    obj['name'] +
    '<br>' +
    '<div class="chat-user">' +
    '<a href="' + obj['url'] + '" target="_blank">' +
    '<img src="static/img/file-icon/' + icon + '.png" onload="scrollAfterImageLoad(this)">' +
    '</a>' +
    '</div>' +
    '</li>';
  return returnMsgString;
}

function memberFileMessage(obj, icon) {
  var returnMsgString = '';
  returnMsgString += '<li class="list-group-item chat-member"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b>' +
    obj['name'] +
    '<br>' +
    '<div class="chat-member">' +
    '<a href="' + obj['url'] + '" target="_blank">' +
    '<img src="static/img/file-icon/' + icon + '.png" onload="scrollAfterImageLoad(this)">' +
    '</a>' + 
    '</div>' +
    '</li>';
  return returnMsgString;
}

function setImageMessage(obj) {
  var msgList = imageMessageList(obj);

  $('#chat_canvas').append(msgList);
  scrollPositionBottom();
}

function imageMessageList(obj) {
  var msgList = '';

  // this function is that compare to current user using users id that input parameter.
  if (isCurrentUser(obj['user']['guest_id'])) {
    msgList += userImageMessage(obj);
  } else {
    msgList += memberImageMessage(obj);
  }
  return msgList;
}

function userImageMessage(obj) {
  var returnMsgString = '';
  returnMsgString += '<li class="list-group-item chat-user"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b>' + obj['name'] + '<br>' +
    '<a href="' + obj['url'] + '" target="_blank">' +
    '<img src="' + obj['url'] + '" width="400px" onload="scrollAfterImageLoad(this)">' +
    '</a>' +
    '</li>';
  return returnMsgString;
}

function memberImageMessage(obj) {
  var returnMsgString = '';
  returnMsgString += '<li class="list-group-item chat-member"><b>'+ nameInjectionCheck(obj['user']['name']) +' : </b>' + obj['name'] + '<br>' +
    '<a href="' + obj['url'] + '" target="_blank">' +
    '<img src="' + obj['url'] + '" width="400px" onload="scrollAfterImageLoad(this)">' +
    '</a>' +
    '</li>';
  return returnMsgString;
}


function getUrlVars() {
  var vars = [], hash;
  var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
  for(var i = 0; i < hashes.length; i++) {
    hash = hashes[i].split('=');
    vars.push(hash[0]);
    vars[hash[0]] = hash[1];
  }
  return vars;
}