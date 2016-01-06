
var nickName = null;
var guestId = null;
var channelListPage = 0;

$(document).ready(function() {

  notifyMe();

  $('#file_input_field').change(function() {
    if ($('#file_input_field').val().trim().length == 0) return;

    var file = $('#file_input_field')[0].files[0];

    var fileInfo = {
      "name": file.name,
      "type": file.type,
      "size": file.size,
      "custom": ''
    };
    sendbird.uploadFile(file, fileInfo, {"async": true});

    $('#file_input_field').val('');
  });

  $('#btn_curr_member_list').click(function() {
    getMemberList(currChannelUrl);
    $('#member_modal').modal('show');
  });

  $('#btn_open_chat_list').click(function () {
    var page = 1;
    var limit = 20;

    getChannelList(page, limit);

    $('#channel_modal').modal('show');
  });
  $('#channel_modal').on('hidden.bs.modal', function (e) {
    if (currChannelUrl == null || currChannelUrl == undefined || currChannelUrl.trim().length == 0) {
      $('#channel_modal').modal('show');
    }
  });

  // Message Input KeyUp
  $('#msg_input').keydown(function (event) {
    if (event.keyCode == 13 && event.shiftKey) {
      //console.log("enter");
    } else if (event.keyCode == 13 && !event.shiftKey) {
      event.preventDefault();
      if ($.trim(this.value) != '') {
        event.preventDefault();
        this.value = $.trim(this.value);
        var chatMessage = $.trim(this.value);
        sendbird.message(chatMessage);

        scrollPositionBottom();
      }
      this.value = "";
    }
  });
  $('#send_msg').click(function() {
    // this function is user send the message.
    if($.trim($('#msg_input').val()) != '') {
      var value = $.trim($('#msg_input').val());
      var chatMessage = $.trim(value);
      sendbird.message(chatMessage);

      scrollPositionBottom();
    }
    $('#msg_input').val('');
  });

  $('#chat_canvas').on('scroll', function() {
    var currHeight = $('#chat_canvas').scrollTop();
    if (currHeight == 0) {
      loadMoreChatMessage();
    }
  });

  $('#channel_search_btn').on('click', function() {
    var query = $('#channel_search_input').val();

    if (query == null || query == undefined || query.trim().length == 0) {
      if(!confirm('Empty string is run that search all channel list.\nAre you want?')) {
        return false;
      }
    }

    getChannelSearch(query);

  });

  init();

});


function init() {
  guestId = checkGuestId();
  console.log('guestID : ', guestId);
  nickName = decodeURI(decodeURIComponent(getUrlVars()['nickname']));
  console.log('nickname : ', nickName);

  startSendBird(guestId, nickName);
  sendbird.setDebugMessage(true);

  $('#btn_open_chat_list').click();
}


function getChannelList(page, limit) {
  var page = page;  // this value is setting channel list page that get from SendBird server.
  var limit = limit; // this value is setting max number of channel list that get from SendBird server.

  var channelListHtml = '';
  // this function is get channel list from SendBird server.
  sendbird.getChannelList({
    "page": page,
    "limit": limit,
    "successFunc" : function(data) {
      channelListHtml = createChannelPanel(data);
      channelListPage = data['page'];
      $('#channel_canvas').append(channelListHtml);
    }
  });
}

function getChannelSearch(query) {
  var query = query; // this value is the string that users wanted search.

  // this function is that search channel list that user typed as input field.
  sendbird.getChannelSearch({
    "query": query,
    "successFunc" : function(data) {
      console.log(data);
      $('#channel_canvas').html();
      var channelListHtml = createChannelPanel(data);
      channelListPage = data['page'];
      $('#channel_canvas').html(channelListHtml);
    }
  });
}

function createChannelPanel(obj) {
  // channel list
  var channelListHtml = '';
  $.each(obj['channels'], function(index, channel) {
    if ( !(channel['channel_url'] == currChannelUrl) ) {
      channelListHtml += channelPanel(index, channel);
    }
  });

  // load more btn
  if (obj['next'] != 0) {
    channelListHtml += channelLoadMore()
  }

  return channelListHtml;
}

function channelPanel(index, obj) {
  var returnStr = '';

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;" id="channel_panel_'+index+'" >' +
    '<div class="panel-body">' +
    '<div class="col-md-8">' +
    obj['name'] +
    '</div>' +
    '<div class="col-md-2">' +
    '<button class="btn btn-danger" onclick="leaveChannel(\'' + index + '\', \''+obj['channel_url']+'\')">' +
    '<span class="glyphicon glyphicon-log-out" aria-hidden="true"> Leave</span>' +
    '</button>' +
    '</div>' +
    '<div class="col-md-2">' +
    '<button class="btn btn-primary" onclick="joinChannel(\''+obj['channel_url']+'\')">' +
    '<span class="glyphicon glyphicon-log-in" aria-hidden="true"> Join</span>' +
    '</button>' +
    '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}

function channelLoadMore() {
  var returnStr = '';

  returnStr += '<div style="text-align: center; margin-top: 20px;" id="channel_more_div">' +
    '<button class="btn btn-default" id="channel_more" ' +
    ' onclick="$(\'#channel_more_div\').remove(); ' +
    ' var page = channelListPage + 1; ' +
    ' var limit = 20; ' +
    ' getChannelList(page, limit);"' +
    ' >MORE</button>' +
    '</div>';

  return returnStr;
}


function getMemberList(channelUrl) {
  // this function is get member list from SendBird server.
  sendbird.getMemberList(
    channelUrl,
    {
      "successFunc" : function(data) {
        var memberListHtml = '';
        $.each(data['members'], function(index, member) {
          memberListHtml += memberPanel(index, member);
        });
        $('#member_canvas').html(memberListHtml);
      }
    }
  );
}

function memberPanel(index, member) {
  var returnStr = '';

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;" id="member_panel_'+index+'" >' +
    '<div class="panel-body">' +
    '<div class="col-md-8">' + member['nickname'] + '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}