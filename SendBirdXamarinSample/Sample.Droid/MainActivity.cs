using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

using Java.Security;

using Sample.Droid;

namespace SendBirdSample.Droid
{
	[Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "SendBirdSample", MainLauncher = true, Icon = "@mipmap/sendbird_ic_launcher")]
	public class MainActivity : Activity
	{
		private const int REQUEST_SENDBIRD_CHAT_ACTIVITY = 100;

		private const int REQUEST_SENDBIRD_CHANNEL_LIST_ACTIVITY = 101;

		private const int REQUEST_SENDBIRD_MESSAGING_ACTIVITY = 200;

		private const int REQUEST_SENDBIRD_MESSAGING_CHANNEL_LIST_ACTIVITY = 201;

		private const int REQUEST_SENDBIRD_MEMBER_LIST_ACTIVITY = 300;

		string appId = "A7A2672C-AD11-11E4-8DAA-0A18B21C2D82";

		private static string userId = GenerateDeviceUUID ();

		private static string userName = "user-" + GenerateDeviceUUID ().Substring(0, 5);

		string channelUrl = "jia_test.lobby";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			FindViewById<TextView> (Resource.Id.etxt_nickname).TextChanged += (object sender, TextChangedEventArgs e) => {
				userName = e.Text.ToString ();
			};
			FindViewById (Resource.Id.btn_start_chat).Click += delegate {
				StartChat (channelUrl);
			};
			FindViewById (Resource.Id.btn_start_channel_list).Click += delegate {
				StartChannelList ();
			};
			FindViewById (Resource.Id.btn_start_messaging).Click += delegate {
				StartMemberList ();
			};
			FindViewById (Resource.Id.btn_start_messaging_channel_list).Click += delegate {
				StartMessagingChannelList ();
			};
		}

		internal void StartChat (string channelUrl)
		{
			var intent = new Intent (this, typeof(SendBirdChatActivity));
			Bundle args = SendBirdChatActivity.MakeSendBirdArgs (appId, userId, userName, channelUrl);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_CHAT_ACTIVITY);
		}

		internal void StartChannelList ()
		{
			var intent = new Intent (this, typeof(SendBirdChannelListActivity));
			Bundle args = SendBirdChannelListActivity.MakeSendBirdArgs (appId, userId, userName, channelUrl);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_CHANNEL_LIST_ACTIVITY);
		}

		internal void StartMemberList ()
		{
			var intent = new Intent (this, typeof(SendBirdMemberListActivity));
			Bundle args = SendBirdMemberListActivity.MakeSendBirdArgs (appId, userId, userName, channelUrl);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_MEMBER_LIST_ACTIVITY);
		}

		internal void StartMessaging (string[] targetUserIds)
		{
			var intent = new Intent (this, typeof(SendBirdMessagingActivity));
			Bundle args = SendBirdMessagingActivity.MakeMessagingStartArgs (appId, userId, userName, targetUserIds);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_MESSAGING_ACTIVITY);
		}

		internal void JoinMessaging (string channelUrl)
		{
			var intent = new Intent (this, typeof(SendBirdMessagingActivity));
			Bundle args = SendBirdMessagingActivity.MakeMessagingJoinArgs (appId, userId, userName, channelUrl);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_MESSAGING_ACTIVITY);
		}

		internal void StartMessagingChannelList ()
		{
			var intent = new Intent (this, typeof(SendBirdMessagingChannelListActivity));
			Bundle args = SendBirdMessagingChannelListActivity.MakeSendBirdArgs (appId, userId, userName);
			intent.PutExtras (args);
			StartActivityForResult (intent, REQUEST_SENDBIRD_MESSAGING_CHANNEL_LIST_ACTIVITY);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			Console.WriteLine (requestCode);

			if (resultCode == Result.Ok && requestCode == REQUEST_SENDBIRD_MESSAGING_CHANNEL_LIST_ACTIVITY && data != null) {
				JoinMessaging (data.GetStringExtra ("channelUrl"));
			}
			if (resultCode == Result.Ok && requestCode == REQUEST_SENDBIRD_MEMBER_LIST_ACTIVITY && data != null) {
				StartMessaging (data.GetStringArrayExtra ("userIds"));
			}
			if (resultCode == Result.Ok && requestCode == REQUEST_SENDBIRD_CHAT_ACTIVITY && data != null) {
				StartMessaging (data.GetStringArrayExtra ("userIds"));
			}
			if (resultCode == Result.Ok && requestCode == REQUEST_SENDBIRD_CHANNEL_LIST_ACTIVITY && data != null) {
				StartChat (data.GetStringExtra ("channelUrl"));
			}
		}

		public static string GenerateDeviceUUID ()
		{
			string serial = Build.Serial;
			string androidID = Settings.Secure.AndroidId;
			string deviceUUID = serial + androidID;
			MessageDigest digest;
			byte[] result;
			try {
				digest = MessageDigest.GetInstance ("SHA-1");
				result = digest.Digest (Encoding.UTF8.GetBytes (deviceUUID));
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				return null;
			}
			StringBuilder sb = new StringBuilder ();
			foreach (byte b in result) {
				sb.Append (b.ToString ("X2"));
			}
			return sb.ToString ();
		}
	}
}
