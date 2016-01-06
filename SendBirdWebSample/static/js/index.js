
var userNickName = null;

$(document).ready(function() {

  init();

  $('#nick_name').keyup(function(e) {

    userNickName = $('#nick_name').val();
    if (userNickName == null || userNickName == undefined || userNickName.replace(/ /gi, '').length == 0) {
      $('#btn_open_chat_list').prop('disabled', true);
      $('#btn_start_msg_chat').prop('disabled', true);
      $('#btn_Msg_Chat_List').prop('disabled', true);
    } else {
      $('#btn_open_chat_list').prop('disabled', false);
      $('#btn_start_msg_chat').prop('disabled', false);
      $('#btn_Msg_Chat_List').prop('disabled', false);
    }

  });

  $('#btn_open_chat_list').click(function() {
    window.location.href = 'open-chat.html?nickname=' + encodeURI(encodeURIComponent(userNickName));
  });

  $('#btn_start_msg_chat').click(function() {
    window.location.href = 'msg-chat.html?nickname=' + encodeURI(encodeURIComponent(userNickName)) + '&type=start';
  });

  $('#btn_Msg_Chat_List').click(function() {
    window.location.href = 'msg-chat.html?nickname=' + encodeURI(encodeURIComponent(userNickName)) + '&type=list';
  });

});

function init() {
  $('#nick_name').val('');

  $('#btn_open_chat_list').prop('disabled', true);
  $('#btn_start_msg_chat').prop('disabled', true);
  $('#btn_Msg_Chat_List').prop('disabled', true);

  $('#nick_name').focus();
}