using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Format;
using Android.Views;
using Android.Widget;

using SendBird;
using SendBird.Model;
using SendBird.Query;
using SendBird.Utils;

using Java.Util;

using Sample.Droid;

namespace SendBirdSample.Droid
{
	[Android.App.Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "Messaging")]
	public class SendBirdMessagingActivity : FragmentActivity
	{
		private static SynchronizationContext mSyncContext;

		private const int REQUEST_MESSAGING_CHANNEL_LIST = 100;
		private const int REQUEST_MEMBER_LIST = 200;

		private SendBirdChatFragment mSendBirdMessagingFragment;
		private SendBirdMessagingAdapter mSendBirdMessagingAdapter;

		private ImageButton mBtnClose;
		private ImageButton mBtnSettings;
		private TextView mTxtChannelUrl;
		private View mTopBarContainer;
		private View mSettingsContainer;
		private TypeTimer mTimer;
		private Button mBtnInvite;
		private MessagingChannel mMessagingChannel;
		private Bundle mSendBirdInfo;

		public static Bundle MakeMessagingStartArgs (string appId, string uuid, string userName, string targetUserId)
		{
			return MakeMessagingStartArgs(appId, uuid, userName, new string[]{targetUserId});
		}

		public static Bundle MakeMessagingStartArgs (string appId, string uuid, string userName, string[] targetUserIds)
		{
			Bundle args = new Bundle ();
			args.PutBoolean ("start", true);
			args.PutString ("appId", appId);
			args.PutString ("uuid", uuid);
			args.PutString ("userName", userName);
			args.PutStringArray ("targetUserIds", targetUserIds);
			return args;
		}

		public static Bundle MakeMessagingJoinArgs (string appId, string uuid, string userName, string channelUrl)
		{
			Bundle args = new Bundle ();
			args.PutBoolean ("join", true);
			args.PutString ("appId", appId);
			args.PutString ("uuid", uuid);
			args.PutString ("userName", userName);
			args.PutString ("channelUrl", channelUrl);
			return args;
		}

		public class TypeTimer : Android.OS.CountDownTimer {
			private SendBirdMessagingAdapter adapter;
			public TypeTimer(long totaltime, long interval, SendBirdMessagingAdapter _adapter) : base(totaltime,interval)
			{
				adapter = _adapter;
			}

			public override void OnTick(long millisUntilFinished)
			{
				if (adapter != null) {
					if (adapter.CheckTypeStatus ()) {
						adapter.NotifyDataSetChanged ();
					}
				}
			}

			public override void OnFinish()
			{				
			}
		}

		protected override void OnResume()
		{
			base.OnResume ();

			if (mTimer != null) {
				mTimer.Cancel ();
			}

			mTimer = new TypeTimer (60 * 60 * 24 * 7 * 1000L, 1000, mSendBirdMessagingAdapter);
			mTimer.Start ();	
		}

		protected override void OnPause()
		{
			base.OnPause ();
			if (mTimer != null) {
				mTimer.Cancel ();
			}
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			GC.Collect();
			SendBirdSDK.Disconnect ();
		}

		public override void Finish ()
		{
			base.Finish ();
		}

		private void InitFragment (Bundle savedInstanceState)
		{
			mSendBirdMessagingFragment = new SendBirdChatFragment ();

			mSendBirdMessagingAdapter = new SendBirdMessagingAdapter (this);
			mSendBirdMessagingFragment.mAdapter = mSendBirdMessagingAdapter;
			mSendBirdMessagingFragment.OnChannelListClicked += (sender, e) => {
				var intent = new Intent (this, typeof(SendBirdMessagingChannelListActivity));
				StartActivityForResult (intent, REQUEST_MESSAGING_CHANNEL_LIST);
			};

			if (savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction ().Replace (Resource.Id.fragment_container, mSendBirdMessagingFragment).Commit (); // v4 fragment
			}
		}

		private void InitUIComponents ()
		{
			mTopBarContainer = FindViewById (Resource.Id.top_bar_container);
			mTxtChannelUrl = FindViewById (Resource.Id.txt_channel_url) as TextView;

			mSettingsContainer = FindViewById (Resource.Id.settings_container);
			mSettingsContainer.Visibility = ViewStates.Gone;

			mBtnClose = FindViewById (Resource.Id.btn_close) as ImageButton;
			mBtnClose.Click += (object sender, EventArgs e) => {
				Finish ();
			}; 

			mBtnInvite = FindViewById (Resource.Id.btn_invite) as Button;
			mBtnInvite.Click += (sender, e) => {
				var intent = new Intent(this, typeof(SendBirdMemberListActivity));
				Bundle args = null;
				args = SendBirdMemberListActivity.MakeSendBirdArgs(SendBirdSDK.GetAppId(), SendBirdSDK.GetUserId(), SendBirdSDK.GetUserName(), "jacob.test5");
				intent.PutExtras(args);
				StartActivityForResult(intent, REQUEST_MEMBER_LIST);
				mSettingsContainer.Visibility = ViewStates.Gone;
			};

			mBtnSettings = FindViewById (Resource.Id.btn_settings) as ImageButton;
			mBtnSettings.Click += (sender, e) => {
				if(mSettingsContainer.Visibility != ViewStates.Visible) {
					mSettingsContainer.Visibility = ViewStates.Visible;
				} else {
					mSettingsContainer.Visibility = ViewStates.Gone;
				}
			};
			ResizeMenubar ();
		}

		private void InitSendBird (Bundle extras)
		{
			mSendBirdInfo = extras;

			String appId = extras.GetString ("appId");
			String uuid = extras.GetString ("uuid");
			String userName = extras.GetString ("userName");

			mSyncContext = SynchronizationContext.Current; // required for api queries
			#region SendBirdEventHandler

			SendBirdNotificationHandler inh = new SendBirdNotificationHandler();
			inh.OnMessagingChannelUpdated += (sender, e) => {
				if(mMessagingChannel != null && mMessagingChannel.GetId() == e.MessagingChannel.GetId()) {
					mSyncContext.Post (delegate {
						UpdateMessagingChannel(e.MessagingChannel);
					}, null);
				}
			};
			inh.OnMentionUpdated += (sender, e) => {
			};

			SendBirdEventHandler ieh = new SendBirdEventHandler ();
			ieh.OnConnect += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnConnect");
				}
			};
			ieh.OnError += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh Messaging OnError");
					Console.WriteLine (e.ErrorCode);
				}
			};
			ieh.OnChannelLeft += (sender, e) => {
			};
			ieh.OnMessageReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnMessageReceived");
				}
				Console.WriteLine("CallMarkAsRead");
				SendBirdSDK.MarkAsRead();
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.AddMessageModel(e.Message);
				}, null);
			};
			ieh.OnBroadcastMessageReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnBroadcastMessageReceived");
				}
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.AddMessageModel(e.Message);
				}, null);
			};
			ieh.OnSystemMessageReceived += (sender, e) => {
				var systemMessage = (SystemMessage)e.Message;
				switch(systemMessage.category) {
				case SystemMessage.CATEGORY_TOO_MANY_MESSAGES:
					systemMessage.SetMessage("Too many messages. Please try later.");
					break;
				case SystemMessage.CATEGORY_MESSAGING_USER_BLOCKED:
					systemMessage.SetMessage("Blocked.");
					break;
				case SystemMessage.CATEGORY_MESSAGING_USER_DEACTIVATED:
					systemMessage.SetMessage("Deactivated.");
					break;
				} 

				mSendBirdMessagingAdapter.AddMessageModel(systemMessage);
			};
			ieh.OnFileReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnFileReceived");
				}
			};
			ieh.OnReadReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnReadReceived");
				}
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.SetReadStatus(e.ReadStatus.GetUserId(), e.ReadStatus.timestamp);
				}, null);
			};
			ieh.OnTypeStartReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnTypeStartReceived");
				}
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.SetTypeStatus(e.TypeStatus.GetUserId(), SendBirdUtils.GetCurrentTimeMills());
				}, null);
			};
			ieh.OnTypeEndReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnTypeEndReceived");
				}
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.SetTypeStatus(e.TypeStatus.GetUserId(), 0);
				}, null);
			};
			ieh.OnAllDataReceived += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnAllDataReceived");
				}
				mSyncContext.Post (delegate {
					mSendBirdMessagingAdapter.NotifyDataSetChanged();
					mSendBirdMessagingFragment.mListView.SetSelection(mSendBirdMessagingAdapter.Count - 1);
				}, null);
			};
			ieh.OnMessageDelivery += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnMessageDelivery");
				}
				if(!e.Sent) {
					mSendBirdMessagingFragment.mEtxtMessage.Text = e.Message;
				}
			};
			ieh.OnMessagingStarted += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnMessagingStarted");
				}
				mSendBirdMessagingAdapter.Clear();
				UpdateMessagingChannel(e.MessagingChannel);

				var channelUrl = e.MessagingChannel.GetUrl();

				MessageListQuery messageListQuery = SendBirdSDK.QueryMessageList(channelUrl);
				messageListQuery.OnResult += (sender_child, e_child) => {
					// mSyncContext.Post for RunOnUIThread
					mSyncContext.Post (delegate {
						foreach (var messageModel in e_child.Messages) {
							mSendBirdMessagingAdapter.AddMessageModel (messageModel);
						}

						mSendBirdMessagingAdapter.NotifyDataSetChanged ();
						mSendBirdMessagingFragment.mListView.SetSelection (30);

						SendBirdSDK.MarkAsRead(channelUrl);
						SendBirdSDK.Join (channelUrl);
						SendBirdSDK.Connect (mSendBirdMessagingAdapter.GetMaxMessageTimestamp());
					}, null);
				};
				messageListQuery.Prev (long.MaxValue, 50); // Excute Query
			};
			ieh.OnMessagingUpdated += (sender, e) => {
				if(SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine("ieh Messaging OnMessagingUpdated");
				}
				mSyncContext.Post (delegate {
					UpdateMessagingChannel(e.MessagingChannel);
				}, null);
			};

			#endregion

			SendBirdSDK.Init (appId);
			SendBirdSDK.Login (uuid, userName);
			SendBirdSDK.RegisterNotificationHandler (inh);
			SendBirdSDK.SetEventHandler (ieh);
		}

		private void ResizeMenubar()
		{
			ViewGroup.LayoutParams lp = mTopBarContainer.LayoutParameters;
			if(Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) {
				lp.Height = (int) (28 * Resources.DisplayMetrics.Density);
			} else {
				lp.Height = (int) (48 * Resources.DisplayMetrics.Density);
			}
			mTopBarContainer.LayoutParameters = lp;
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			ResizeMenubar ();
		}			

		protected override void OnActivityResult (int requestCode, Android.App.Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode == REQUEST_MESSAGING_CHANNEL_LIST) {
				if (resultCode == Android.App.Result.Ok && data != null) {
					SendBirdSDK.JoinMessaging (data.GetStringExtra ("channelUrl"));
				}
			} else if (requestCode == REQUEST_MEMBER_LIST) {
				if (resultCode == Android.App.Result.Ok && data != null) {
					try {
						SendBirdSDK.InviteMessaging(SendBirdSDK.GetCurrentChannel().url, new List<string>(data.GetStringArrayExtra("userIds"))); 
					} catch(Exception e) {
						Console.WriteLine (e.StackTrace);
					}
				}
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SendBirdActivityMessaging);
			this.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);

			InitFragment (savedInstanceState);
			InitUIComponents ();
			InitSendBird (this.Intent.Extras);

			if (mSendBirdInfo.GetBoolean ("start")) {
				string[] targetUserIds = mSendBirdInfo.GetStringArray ("targetUserIds");
				SendBirdSDK.StartMessaging (new List<string> (targetUserIds));
			} else if (mSendBirdInfo.GetBoolean ("join")) {
				string channelUrl = mSendBirdInfo.GetString ("channelUrl");
				SendBirdSDK.JoinMessaging (channelUrl);
			}
		}

		private void UpdateMessagingChannel(MessagingChannel messagingChannel) {
			mMessagingChannel = messagingChannel;
			mTxtChannelUrl.Text = GetDisplayMemberNames(messagingChannel.GetMembers());

			Dictionary<string, long> readStatus = new Dictionary<string, long> ();
			foreach(var member in messagingChannel.GetMembers()) {
				long currentStatus = 0L;
				mSendBirdMessagingAdapter.mReadStatus.TryGetValue(member.GetId (), out currentStatus);
				readStatus.Add (member.GetId (), Math.Max (currentStatus, messagingChannel.GetLastReadMills (member.GetId ())));
			}
			mSendBirdMessagingAdapter.ResetReadStatus (readStatus);
			mSendBirdMessagingAdapter.mMembers = messagingChannel.GetMembers ();
		}

		private static string GetDisplayMemberNames(List<MessagingChannel.Member> members)
		{
			if (members.Count < 2) {
				return "No Members";
			} else if (members.Count == 2) {
				StringBuilder names = new StringBuilder ();
				foreach (var member in members) {
					if (member.GetId ().Equals (SendBirdSDK.GetUserId ())) {
						continue;
					}

					names.Append (", " + member.name);
				}
				return names.Remove (0, 2).ToString ();
			} else {
				return "Group " + members.Count;
			}
		}

		public class SendBirdChatFragment : Fragment
		{
			private const int REQUEST_PICK_IMAGE = 100;

			public ListView mListView;
			public SendBirdMessagingAdapter mAdapter;
			public EditText mEtxtMessage;
			public Button mBtnSend;
			public ImageButton mBtnChannel;
			public ImageButton mBtnUpload;
			public ProgressBar mProgressBtnUpload;

			public delegate void OnChannelListClickedEvent (object sender, EventArgs e);

			public OnChannelListClickedEvent OnChannelListClicked;

			public SendBirdChatFragment ()
			{
			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View rootView = inflater.Inflate (Resource.Layout.SendBirdFragmentMessaging, container, false);
				InitUIComponents (rootView);
				return rootView;
			}

			private void InitUIComponents (View rootView)
			{
				mListView = rootView.FindViewById (Resource.Id.list) as ListView;
				TurnOffListViewDecoration (mListView);
				mListView.Adapter = mAdapter;

				mBtnChannel = rootView.FindViewById (Resource.Id.btn_channel) as ImageButton;
				mBtnChannel.Click += (sender, e) => {
					if (OnChannelListClicked != null) {
						OnChannelListClicked (this, new EventArgs ());
					}
				};
				mBtnSend = rootView.FindViewById (Resource.Id.btn_send) as Button;
				mBtnUpload = rootView.FindViewById (Resource.Id.btn_upload) as ImageButton;
				mProgressBtnUpload = rootView.FindViewById (Resource.Id.progress_btn_upload) as ProgressBar;
				mEtxtMessage = rootView.FindViewById (Resource.Id.etxt_message) as EditText;
				mEtxtMessage.KeyPress += (object sender, View.KeyEventArgs e) => {
					if (e.KeyCode == Keycode.Enter) {
						if (e.Event.Action == KeyEventActions.Down) {
							Send ();

							e.Handled = true;
						}
					} else {
						e.Handled = false;
					}
				};

				mBtnSend.Enabled = false;
				mBtnSend.Click += (object sender, EventArgs e) => {
					Send ();
				};

				mEtxtMessage.AfterTextChanged += (object sender, AfterTextChangedEventArgs e) => {
					mBtnSend.Enabled = e.Editable.Length() > 0 ? true : false;
					if(e.Editable.Length() > 0) {
						SendBirdSDK.TypeStart();
					} else {
						SendBirdSDK.TypeEnd();
					}
				};
				mListView.Touch += (object sender, View.TouchEventArgs e) => {
					Helper.HideKeyboard (this.Activity);
					e.Handled = false;
				};
				mListView.ScrollStateChanged += (s, args) => {
					if(args.ScrollState == ScrollState.Idle) {
						if(args.View.FirstVisiblePosition == 0 && args.View.ChildCount > 0 && args.View.GetChildAt(0).Top == 0) {
							var mMessageListQuery = SendBirdSDK.QueryMessageList (SendBirdSDK.GetChannelUrl());
							mMessageListQuery.OnResult += (sender, e) => {
								mSyncContext.Post (delegate {
									foreach (var messageModel in e.Messages) {
										mAdapter.AddMessageModel (messageModel);
									}

									mAdapter.NotifyDataSetChanged ();
									mListView.SetSelection (e.Messages.Count);
								}, null);
							};
							mMessageListQuery.Prev (mAdapter.GetMinMessageTimestamp(), 30); // Excute Query
						} else if(args.View.FirstVisiblePosition == args.View.ChildCount - 1 && args.View.ChildCount > 0) {
							var mMessageListQuery = SendBirdSDK.QueryMessageList (SendBirdSDK.GetChannelUrl());
							mMessageListQuery.OnResult += (sender, e) => {
								mSyncContext.Post (delegate {
									foreach (var messageModel in e.Messages) {
										mAdapter.AddMessageModel (messageModel);
									}

									mAdapter.NotifyDataSetChanged ();
								}, null);
							};
							mMessageListQuery.Next (mAdapter.GetMaxMessageTimestamp(), 30); // Excute Query
						}
					}
				};
			}

			private void TurnOffListViewDecoration(ListView listView)
			{
				listView.Divider = null;
				listView.DividerHeight = 0;
				listView.HorizontalFadingEdgeEnabled = false;
				listView.VerticalFadingEdgeEnabled = false;
				listView.HorizontalScrollBarEnabled = false;
				listView.VerticalScrollBarEnabled = false;
				listView.Selector = new ColorDrawable(Color.ParseColor("#ffffff"));
				listView.CacheColorHint = Color.ParseColor("#000000"); // For Gingerbread scrolling bug fix
			}

			private void Send ()
			{
				SendBirdSDK.SendMessage (mEtxtMessage.Text);
				mEtxtMessage.Text = string.Empty;

				if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) {
					Helper.HideKeyboard (this.Activity);
				}
			}

			public override void OnDestroy ()
			{
				base.OnDestroy ();
			}
		}

		public class SendBirdMessagingAdapter : BaseAdapter<Object>
		{
			private const int TYPE_UNSUPPORTED = 0;
			private const int TYPE_MESSAGE = 1;
			private const int TYPE_SYSTEM_MESSAGE = 2;
			private const int TYPE_FILELINK = 3;
			private const int TYPE_BROADCAST_MESSAGE = 4;
			private const int TYPE_TYPING_INDICATOR = 5;

			private Context mContext;
			private LayoutInflater mInflater;
			private List<Object> mItemList;

			public Dictionary<string, long> mReadStatus;
			public Dictionary<string, long> mTypeStatus;
			public List<MessagingChannel.Member> mMembers;

			internal long mMaxMessageTimestamp = long.MinValue;
			public long GetMaxMessageTimestamp()
			{
				return mMaxMessageTimestamp == long.MinValue ? long.MaxValue : mMaxMessageTimestamp;
			}
			internal long mMinMessageTimestamp = long.MaxValue;
			public long GetMinMessageTimestamp()
			{
				return mMinMessageTimestamp == long.MaxValue ? long.MinValue : mMinMessageTimestamp;
			}


			public SendBirdMessagingAdapter (Context context)
			{
				mContext = context;
				mInflater = mContext.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				mItemList = new List<Object> ();
				mReadStatus = new Dictionary<string, long>();
				mTypeStatus = new Dictionary<string, long>();
			}

			#region implemented abstract members of BaseAdapter

			public override long GetItemId (int position)
			{
				return position;
			}

			public override int ViewTypeCount {
				get {
					return 6;
				}
			}

			public override int Count {
				get {
					return mItemList.Count + ((mTypeStatus.Count <= 0) ? 0 : 1);
				}
			}

			public override Object this [int index] {
				get {
					if (index >= mItemList.Count) {
						List<string> names = new List<string> ();
						foreach (var member in mMembers) {
							if(mTypeStatus.ContainsKey(member.GetId())) {
								names.Add(member.name);
							}
						}

						return names;
					}
					return mItemList [index];
				}
			}

			public void Clear ()
			{
				mMaxMessageTimestamp = long.MinValue;
				mMinMessageTimestamp = long.MaxValue;
				mItemList.Clear ();
				mTypeStatus.Clear ();
				mItemList.Clear ();
			}

			public void ResetReadStatus(Dictionary<string, long> readStatus)
			{
				mReadStatus = readStatus;
			}

			public void SetReadStatus(string userId, long timestamp)
			{
				if (mReadStatus.ContainsKey(userId) == false || mReadStatus [userId] < timestamp) {
					mReadStatus [userId] = timestamp;
				}
			}

			public void SetTypeStatus(string userId, long timestamp)
			{
				if(userId.Equals(SendBirdSDK.GetUserId())) {
					return;
				}

				if (timestamp <= 0) {
					mTypeStatus.Remove (userId);
				} else {
					mTypeStatus[userId] = timestamp;
				}
			}

			public void AddMessageModel (MessageModel model)
			{
				if (model.IsPast ()) {
					mItemList.Insert (0, model);
				} else {
					mItemList.Add (model);
				}
				UpdateMessageTimestamp (model);
			}

			private void UpdateMessageTimestamp (MessageModel model)
			{
				mMaxMessageTimestamp = mMaxMessageTimestamp < model.messageTimestamp ? model.messageTimestamp : mMaxMessageTimestamp;
				mMinMessageTimestamp = mMinMessageTimestamp > model.messageTimestamp ? model.messageTimestamp : mMinMessageTimestamp;
			}

			public override int GetItemViewType (int position)
			{
				if (position >= mItemList.Count) {
					return TYPE_TYPING_INDICATOR;
				}

				Object item = mItemList [position];
				if (item is SendBird.Model.Message) {
					return TYPE_MESSAGE;
				} else if (item is SendBird.Model.FileLink) {
					return TYPE_FILELINK;
				} else if (item is SendBird.Model.SystemMessage) {
					return TYPE_SYSTEM_MESSAGE;
				} else if (item is SendBird.Model.BroadcastMessage) {
					return TYPE_BROADCAST_MESSAGE;
				}

				return TYPE_UNSUPPORTED;
			}

			public bool CheckTypeStatus()
			{
				foreach (var key in mTypeStatus.Keys) {
					long ts = mTypeStatus [key];
					if (SendBirdUtils.GetCurrentTimeMills () - ts > 10 * 1000L) {
						mTypeStatus.Remove (key);
						return true;
					}
				}
				return false;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ViewHolder viewHolder = null;
				Object item = this [position];

				if (convertView == null || (convertView.Tag as ViewHolder).GetViewType () != GetItemViewType (position)) {
					viewHolder = new ViewHolder ();
					viewHolder.SetViewType (GetItemViewType (position));

					switch (GetItemViewType (position)) {
					case TYPE_UNSUPPORTED:
						{
							convertView = new View (mInflater.Context);
							convertView.Tag = viewHolder;
							break;
						}
					case TYPE_MESSAGE:
						{
							TextView tv;
							ImageView iv;
							View v;

							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewMessagingMessage, parent, false);

							v = convertView.FindViewById (Resource.Id.left_container);
							viewHolder.SetView("left_container", v);
							iv = convertView.FindViewById(Resource.Id.img_left_thumbnail) as ImageView;
							viewHolder.SetView("left_thumbnail", iv);
							tv = convertView.FindViewById(Resource.Id.txt_left) as TextView;
							viewHolder.SetView("left_message", tv);
							tv = convertView.FindViewById(Resource.Id.txt_left_name) as TextView;
							viewHolder.SetView("left_name", tv);
							tv = convertView.FindViewById(Resource.Id.txt_left_time) as TextView;
							viewHolder.SetView("left_time", tv);

							v = convertView.FindViewById(Resource.Id.right_container);
							viewHolder.SetView("right_container", v);
							iv = convertView.FindViewById(Resource.Id.img_right_thumbnail) as ImageView;
							viewHolder.SetView("right_thumbnail", iv);
							tv = convertView.FindViewById(Resource.Id.txt_right) as TextView;
							viewHolder.SetView("right_message", tv);
							tv = convertView.FindViewById(Resource.Id.txt_right_name) as TextView;
							viewHolder.SetView("right_name", tv);
							tv = convertView.FindViewById(Resource.Id.txt_right_time) as TextView;
							viewHolder.SetView("right_time", tv);
							tv = convertView.FindViewById(Resource.Id.txt_right_status) as TextView;
							viewHolder.SetView("right_status", tv);

							convertView.Tag = viewHolder;
							break;
						}
					case TYPE_SYSTEM_MESSAGE:
						{
							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewSystemMessage, parent, false);
							viewHolder.SetView ("message", convertView.FindViewById (Resource.Id.txt_message) as TextView);
							convertView.Tag = viewHolder;
							break;
						}
					case TYPE_BROADCAST_MESSAGE:
						{
							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewSystemMessage, parent, false);
							viewHolder.SetView ("message", convertView.FindViewById (Resource.Id.txt_message) as TextView);
							convertView.Tag = viewHolder;
							break;
						}
					case TYPE_TYPING_INDICATOR:
						{
							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewTypingIndicator, parent, false);
							viewHolder.SetView ("message", convertView.FindViewById (Resource.Id.txt_message));
							convertView.Tag = viewHolder;
							break;
						}
					}
				}
				viewHolder = convertView.Tag as ViewHolder;
				switch (GetItemViewType (position)) {
				case TYPE_UNSUPPORTED:
					break;
				case TYPE_MESSAGE:
					{
						SendBird.Model.Message message = item as SendBird.Model.Message;
						if (message.GetSenderId ().Equals (SendBirdSDK.GetUserId ())) {
							viewHolder.GetView ("left_container").Visibility = ViewStates.Gone;
							viewHolder.GetView ("right_container").Visibility = ViewStates.Visible;

							// DisplayUrlImage()
							viewHolder.GetView<TextView> ("right_name").Text = message.GetSenderName ();
							viewHolder.GetView<TextView> ("right_message").Text = message.message;
							viewHolder.GetView<TextView> ("right_time").Text = GetDisplayDateTime (mContext, message.messageTimestamp);

							int readCount = 0;
							foreach (string key in mReadStatus.Keys) {
								if (key.Equals (message.GetSenderId ())) {
									readCount += 1;
									continue;
								}

								if (mReadStatus [key] >= message.messageTimestamp) {
									readCount += 1;
								}
							}

							if (readCount < mReadStatus.Count) {
								if (mReadStatus.Count - readCount > 1) {
									viewHolder.GetView<TextView> ("right_status").Text = "Unread " + (mReadStatus.Count - readCount);
								} else {
									viewHolder.GetView<TextView> ("right_status").Text = "Unread";
								}
							} else {
								viewHolder.GetView<TextView> ("right_status").Text = string.Empty;
							}
						} else {
							viewHolder.GetView ("left_container").Visibility = ViewStates.Visible;
							viewHolder.GetView ("right_container").Visibility = ViewStates.Gone;

							// DisplayUrlImage()
							viewHolder.GetView<TextView> ("left_name").Text = message.GetSenderName ();
							viewHolder.GetView<TextView> ("left_message").Text = message.message;
							viewHolder.GetView<TextView> ("left_time").Text = GetDisplayDateTime (mContext, message.messageTimestamp);
						}
						break;
					}
				case TYPE_SYSTEM_MESSAGE:
					{
						SendBird.Model.SystemMessage systemMessage = item as SendBird.Model.SystemMessage;
						viewHolder.GetView<TextView> ("message").TextFormatted = Html.FromHtml (systemMessage.message);
						break;
					}
				case TYPE_BROADCAST_MESSAGE:
					{
						SendBird.Model.BroadcastMessage broadcastMessage = item as SendBird.Model.BroadcastMessage;
						viewHolder.GetView<TextView> ("message").TextFormatted = Html.FromHtml (broadcastMessage.message);
						break;
					}
				case TYPE_TYPING_INDICATOR:
					{
						System.Collections.ICollection itemList = (System.Collections.ICollection)item;
						int itemCount = itemList.Count;
						string typeMsg = (item as List<string>)[0] + ((itemCount > 1) ? " +" + (itemCount - 1) : "")
						                + ((itemCount > 1) ? " are " : " is ")
						                + "typing...";
						viewHolder.GetView<TextView> ("message").Text = typeMsg;
						break;
					}
				}

				return convertView;
			}

			private class ViewHolder : Java.Lang.Object
			{
				private Dictionary<string, View> holder = new Dictionary<string, View> ();
				private int type;

				public int GetViewType ()
				{
					return this.type;
				}

				public void SetViewType (int type)
				{
					this.type = type;
				}

				public void SetView (string k, View v)
				{
					holder.Add (k, v);
				}

				public View GetView (string k)
				{
					return holder [k];
				}

				public T GetView<T> (string k)
				{
					return (T)Convert.ChangeType (GetView (k), typeof(T));
				}
			}

			#endregion
		}

		private static string GetDisplayDateTime(Context context, long milli)
		{
			var date = new Date (milli);

			if (SendBirdUtils.GetCurrentTimeMills () - milli < 60 * 60 * 24 * 1000L) {
				return DateFormat.GetTimeFormat (context).Format (date);
			}

			return DateFormat.GetDateFormat (context).Format (date) + " " + DateFormat.GetTimeFormat (context).Format (date);
		}

		public static class Helper
		{
			public static void HideKeyboard (Android.App.Activity activity)
			{
				if (activity == null || activity.CurrentFocus == null) {
					return;
				}
				var imm = activity.GetSystemService (Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
				imm.HideSoftInputFromWindow (activity.CurrentFocus.WindowToken, 0);
			}
		}
	}
}
