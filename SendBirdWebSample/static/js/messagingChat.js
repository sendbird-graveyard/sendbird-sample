
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

  $('#btn_msg_chat_list').click(function () {
    var page = 1;
    var limit = 20;
    getMessagingList(page, limit);
    $('#channel_modal').modal('show');
  });
  $('#channel_modal').on('hidden.bs.modal', function (e) {
    if (currChannelUrl == null || currChannelUrl == undefined || currChannelUrl.trim().length == 0) {
      $('#channel_modal').modal('show');
    }
  });

  $('#btn_member_list').click(function() {
    getMemberList('jia_test.Lobby');
    $('#messaging_modal').modal('show');
  });
  $('#messaging_modal').on('hidden.bs.modal', function (e) {
    if (currChannelUrl == null || currChannelUrl == undefined || currChannelUrl.trim().length == 0) {
      $('#messaging_modal').modal('show');
    }
  });

  $('#btn_curr_member_list').click(function() {
    getCurrentMemberList(currChannelUrl);
    $('#member_modal').modal('show');
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

  init();

});


function init() {
  guestId = checkGuestId();
  console.log('guestID : ', guestId);
  nickName = decodeURI(decodeURIComponent(getUrlVars()['nickname']));
  console.log(nickName);

  startSendBird(guestId, nickName);
  sendbird.setDebugMessage(false);

  if (getUrlVars()['type'] == 'start') {
    $('#btn_member_list').click();
  } else {
    $('#btn_msg_chat_list').click();
  }

}


function getMessagingList(page, limit) {
  var page = page;  // this value is setting channel list page that get from SendBird server.
  var limit = limit; // this value is setting max number of channel list that get from SendBird server. if set 0, get all list.

  // this function is get channel list from SendBird server.
  sendbird.getMessagingChannelList({
    "page": page,
    "limit": limit,
    "successFunc": function(data) {
      var channelList = data;
      if (channelList['channels'].length == 0) {
        $('#channel_canvas').html(emptyList());
      } else {
        channelListPage = channelList['page'];
        var channelListHtml = createMessagingPanel(channelList);
        $('#channel_canvas').html(channelListHtml);
      }
    }
  });
}

function emptyList() {
  var returnStr = '';

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;">' +
    '<div class="panel-body">' +
    '<div class="col-md-12" style="text-align: center;">' +
    'Empty List' +
    '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}

function createMessagingPanel(obj) {
  // channel list
  var channelListHtml = '';
  console.log(obj);
  $.each(obj['channels'], function(index, channel) {
    channelListHtml += messagingPanel(index, channel);
  });

  // load more btn
  if (obj['next'] != 0) {
    channelListHtml += messagingChannelLoadMore()
  }

  return channelListHtml;
}

function messagingPanel(index, obj) {
  var returnStr = '';
  var channelName = '';
  $.each(obj['members'], function(index, member) {
    if (index == 0) {
      channelName += member['name']
    } else {
      channelName += ', ' + member['name']
    }
  });

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;" id="messaging_panel_'+index+'" >' +
    '<div class="panel-body">' +
    '<div class="col-md-6">' +
    channelName +
    '</div>' +
    '<div class="col-md-2">' +
    '<span class="badge">' +
    obj['unread_message_count'] +
    '</span>' +
    '</div>' +
    '<div class="col-md-2">' +
    '<button class="btn btn-danger" onclick="endMessaging(\'' + index + '\', \''+obj['channel']['channel_url']+'\')">' +
    '<span class="glyphicon glyphicon-log-out" aria-hidden="true"> Leave</span>' +
    '</button>' +
    '</div>' +
    '<div class="col-md-2">' +
    '<button class="btn btn-primary" onclick="joinMessagingChannel(\''+obj['channel']['channel_url']+'\')">' +
    '<span class="glyphicon glyphicon-log-in" aria-hidden="true"> Join</span>' +
    '</button>' +
    '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}

function messagingChannelLoadMore() {
  var returnStr = '';

  returnStr += '<div style="text-align: center; margin-top: 20px;" id="messaging_channel_more_div">' +
    '<button class="btn btn-default" id="messaging_channel_more" ' +
    ' onclick="$(\'#messaging_channel_more_div\').remove(); ' +
    ' var page = channel_list_page + 1; ' +
    ' var limit = 20; ' +
    ' $(\'#channel_canvas\').html($(\'#channel_canvas\').html() + getMessagingList(page, limit));" ' +
    ' >MORE</button>' +
    '</div>';

  return returnStr;
}

function endMessaging(index, channelUrl) {
  var url = channelUrl; // this value is channel url that users wanted leave.

  // This function let user leave the channel that user belongs to.
  sendbird.endMessaging(
    url,
    {
      "successFunc": function(data) {
        var deletePanel = 'messaging_panel_'+index;
        $('#'+deletePanel).remove();

        if (currChannelUrl == channelUrl) {
          $('#btn_member_list').click();
        }
      }
    }
  );
}

function getMemberList(channelUrl) {
  // this function is get member list from SendBird server.
  sendbird.getMemberList(
    channelUrl,
    {
      "successFunc" : function(data) {
        var memberListHtml = '';
        $.each(data['members'], function(index, member) {
          if (!isCurrentUser(member['guest_id'])) {
            memberListHtml += memberPanel(index, member);
          }
        });
        $('#messaging_canvas').html(memberListHtml);
      }
    }
  );
}

function createMemberPanel(obj) {
  // member list
  var memberListHtml = '';
  $.each(obj['members'], function(index, member) {
    if ( !isCurrentUser(member['guest_id']) ) {
      memberListHtml += memberPanel(index, member);
    }
  });

  return memberListHtml;
}

function memberPanel(index, member) {
  var returnStr = '';

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;" id="member_panel_'+index+'" >' +
    '<div class="panel-body">' +
    '<div class="col-md-8">' +
    member['nickname'] +
    '</div>';

  if ( !(currChannelUrl == null || currChannelUrl == undefined || currChannelUrl.trim().length == 0) ) {
    returnStr += '<div class="col-md-2">' +
        '<button class="btn btn-success" onclick="inviteMember(\''+member['guest_id']+'\')">' +
        '<span class="glyphicon glyphicon-log-out" aria-hidden="true"> Invite</span>' +
        '</button>' +
        '</div>';
  }

  returnStr += '<div class="col-md-2">' +
    '<button class="btn btn-primary" onclick="messageMember(\''+member['guest_id']+'\')">' +
    '<span class="glyphicon glyphicon-log-in" aria-hidden="true"> Start Msg.</span>' +
    '</button>' +
    '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}

function inviteMember(guestId) {
  sendbird.inviteMessaging(
    guestId,
    {
      "successFunc": function(data) {
        currChannelInfo = data.channel;
        initChatMessage(currChannelInfo);
        sendbird.connect();
        loadMoreChatMessage();
        scrollPositionBottom();
        $('#messaging_modal').modal('hide');
        sendbird.markAsRead(currChannelInfo['channel_url']);
      }
    }
  );
}

function messageMember(guestId) {
  sendbird.startMessaging(
    guestId,
    {
      "successFunc": function(data) {
        $('#messaging_modal').modal('hide');
        currChannelInfo = data.channel;
        initChatMessage(currChannelInfo);
        sendbird.connect();
        loadMoreChatMessage();
        scrollPositionBottom();
        sendbird.markAsRead(currChannelInfo['channel_url']);
      }
    }
  );
}

function getCurrentMemberList(channelUrl) {
  // this function is get member list from SendBird server.
  sendbird.getMemberList(
    channelUrl,
    {
      "successFunc" : function(data) {
        console.log(data);
        var memberListHtml = '';
        $.each(data['members'], function(index, member) {
          memberListHtml += currentMemberPanel(index, member);
        });
        $('#member_canvas').html(memberListHtml);
      }
    }
  );
}

function createCurrentMemberPanel(obj) {
  var memberListHtml = '';
  $.each(obj['members'], function(index, member) {
    memberListHtml += currentMemberPanel(index, member);
  });

  return memberListHtml;
}

function currentMemberPanel(index, member) {
  var returnStr = '';

  returnStr += '<div class="panel panel-default" style="margin-bottom: 10px;" id="member_panel_'+index+'" >' +
    '<div class="panel-body">' +
    '<div class="col-md-8">' + member['nickname'] + '</div>' +
    '</div>' +
    '</div>';

  return returnStr;
}