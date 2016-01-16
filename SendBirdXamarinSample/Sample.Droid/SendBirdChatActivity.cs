using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Android.Content;
using Android.Database;
using Android.Graphics;

using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Util;
using Android.Text;
using Android.Views;
using Android.Widget;

using SendBird;
using SendBird.Model;
using SendBird.Query;

using Sample.Droid;

namespace SendBirdSample.Droid
{
	[Android.App.Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "Chat")]
	public class SendBirdChatActivity : FragmentActivity
	{
		private static SynchronizationContext mSyncContext;
		private static ImageUtils.MemoryLimitedLruCache mMemoryCache;

		private const int REQUEST_CHANNEL_LIST = 100;

		private SendBirdChatFragment mSendBirdChatFragment;
		private SendBirdChatAdapter mSendBirdChatAdapter;

		private MessageListQuery mMessageListQuery;

		private ImageButton mBtnClose;
		private ImageButton mBtnSettings;
		private TextView mTxtChannelUrl;
		private View mTopBarContainer;
		private View mSettingsContainer;
		private Button mBtnLeave;
		private string mChannelUrl;
		private bool mDoNotDisconnect;

		public static Bundle MakeSendBirdArgs (string appId, string uuid, string userName, string channelUrl)
		{
			Bundle args = new Bundle ();
			args.PutString ("appId", appId);
			args.PutString ("uuid", uuid);
			args.PutString ("userName", userName);
			args.PutString ("channelUrl", channelUrl);
			return args;
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			if (!mDoNotDisconnect) {
				SendBirdSDK.Disconnect ();
			}
		}

		public override void Finish ()
		{
			base.Finish ();
		}

		private void InitFragment (Bundle savedInstanceState)
		{
			mSendBirdChatFragment = new SendBirdChatFragment ();

			mSendBirdChatAdapter = new SendBirdChatAdapter (this);
			mSendBirdChatFragment.mAdapter = mSendBirdChatAdapter;
			mSendBirdChatFragment.OnChannelListClicked += (sender, e) => {
				var intent = new Intent (this, typeof(SendBirdChannelListActivity));
				intent.PutExtras (this.Intent.Extras);
				StartActivityForResult (intent, REQUEST_CHANNEL_LIST);
			};

			if (savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction ().Replace (Resource.Id.fragment_container, mSendBirdChatFragment).Commit (); // v4 fragment
			}
		}

		private void InitUIComponents ()
		{
			mTopBarContainer = FindViewById (Resource.Id.top_bar_container);
			mTxtChannelUrl = FindViewById (Resource.Id.txt_channel_url) as TextView;

			mSettingsContainer = FindViewById (Resource.Id.settings_container);
			mSettingsContainer.Visibility = ViewStates.Gone;

			mBtnLeave = FindViewById (Resource.Id.btn_leave) as Button;
			mBtnLeave.Click += (sender, e) => {
				mSettingsContainer.Visibility = ViewStates.Gone;
				SendBirdSDK.Leave(SendBirdSDK.GetChannelUrl());
				Finish();
			};

			mBtnClose = FindViewById (Resource.Id.btn_close) as ImageButton;
			mBtnClose.Click += (object sender, EventArgs e) => {
				Finish ();
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

		private void InitSendBird (Bundle extras)
		{
			String appId = extras.GetString ("appId");
			String uuid = extras.GetString ("uuid");
			String userName = extras.GetString ("userName");
			mChannelUrl = extras.GetString ("channelUrl");

			mSyncContext = SynchronizationContext.Current; // required for ui update
			#region SendBirdEventHandler

			SendBirdEventHandler ieh = new SendBirdEventHandler ();
			ieh.OnConnect += (sender, e) => {
				Channel channel = e.Channel;
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh onConnect");
				}
				mTxtChannelUrl.Text = "#" + mChannelUrl;
			};
			ieh.OnError += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh onError");
					Console.WriteLine (e.ErrorCode);
				}
			};
			ieh.OnChannelLeft += (sender, e) => {
			};
			ieh.OnMessageReceived += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh onMessageReceived");
				}
				mSendBirdChatAdapter.AddMessageModel (e.Message);
				mSyncContext.Post (delegate {
					mSendBirdChatAdapter.NotifyDataSetChanged ();
				}, null);
			};
			ieh.OnBroadcastMessageReceived += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh onBroadcastMessageReceived");
				}
				mSendBirdChatAdapter.AddMessageModel(e.Message);
				mSyncContext.Post (delegate {
					mSendBirdChatAdapter.NotifyDataSetChanged ();
				}, null);
			};
			ieh.OnSystemMessageReceived += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh onSystemMessageReceived");
				}
				mSendBirdChatAdapter.AddMessageModel(e.Message);
				mSyncContext.Post (delegate {
					mSendBirdChatAdapter.NotifyDataSetChanged ();
				}, null);
			};
			ieh.OnFileReceived += (sender, e) => {
				if (SendBird.CommonVar.IS_DEBUG) {
					Console.WriteLine ("ieh OnFileReceived");
				}
				mSendBirdChatAdapter.AddMessageModel(e.FileLink);
				mSyncContext.Post (delegate {
					mSendBirdChatAdapter.NotifyDataSetChanged ();
				}, null);
			};
			ieh.OnAllDataReceived += (sender, e) => {
				mSyncContext.Post (delegate {
					mSendBirdChatAdapter.NotifyDataSetChanged();
					mSendBirdChatFragment.mListView.SetSelection(mSendBirdChatAdapter.Count);
				}, null);
			};
			ieh.OnMessageDelivery += (sender, e) => {
				if(!e.Sent) {
					mSendBirdChatFragment.mEtxtMessage.Text = e.Message;
				}
			};
			ieh.OnMessagingEnded += (sender, e) => {
			};


			#endregion

			SendBirdSDK.Init (appId);
			SendBirdSDK.Login (uuid, userName);
			SendBirdSDK.SetEventHandler (ieh);
		}

		protected override void OnActivityResult (int requestCode, Android.App.Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode == REQUEST_CHANNEL_LIST) {
				if (resultCode == Android.App.Result.Ok && data != null) {
					mChannelUrl = data.GetStringExtra ("channelUrl");

					mSendBirdChatAdapter.Clear ();
					mSendBirdChatAdapter.NotifyDataSetChanged ();

					mMessageListQuery = SendBirdSDK.QueryMessageList (mChannelUrl);
					mMessageListQuery.OnResult += (sender, e) => {
						// mSyncContext.Post for RunOnUIThread
						mSyncContext.Post (delegate {
							foreach (var messageModel in e.Messages) {
								mSendBirdChatAdapter.AddMessageModel (messageModel);
							}

							mSendBirdChatAdapter.NotifyDataSetChanged ();
							mSendBirdChatFragment.mListView.SetSelection (mSendBirdChatAdapter.Count);

							SendBirdSDK.Join (mChannelUrl);
							SendBirdSDK.Connect (mSendBirdChatAdapter.GetMaxMessageTimestamp());
						}, null);
					};
					mMessageListQuery.Prev (long.MaxValue, 50); // Excute Query
				}
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SendBirdActivityChat);
			this.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);

			InitFragment (savedInstanceState);
			InitUIComponents ();
			InitSendBird (this.Intent.Extras);

			mMessageListQuery = SendBirdSDK.QueryMessageList (mChannelUrl);
			mMessageListQuery.OnResult += (sender, e) => {
				// mSyncContext.Post for RunOnUIThread
				mSyncContext.Post (delegate {
					foreach (var messageModel in e.Messages) {
						mSendBirdChatAdapter.AddMessageModel (messageModel);
					}

					mSendBirdChatFragment.mAdapter.NotifyDataSetChanged ();
					mSendBirdChatFragment.mListView.SetSelection (mSendBirdChatAdapter.Count);

					SendBirdSDK.Join (mChannelUrl);
					SendBirdSDK.Connect (mSendBirdChatAdapter.GetMaxMessageTimestamp());
				}, null);
			};
			mMessageListQuery.Prev (long.MaxValue, 50); // Excute Query
		}

		public class SendBirdChatFragment : Fragment
		{
			private const int REQUEST_PICK_IMAGE = 100;

			public ListView mListView;
			public SendBirdChatAdapter mAdapter;
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
				View rootView = inflater.Inflate (Resource.Layout.SendBirdFragmentChat, container, false);
				InitUIComponents (rootView);
				return rootView;
			}

			private void InitUIComponents (View rootView)
			{
				mListView = rootView.FindViewById (Resource.Id.list_view) as ListView;
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
				mBtnUpload.Click += delegate {
					Intent intent = new Intent();
					intent.SetType("image/*");
					intent.SetAction(Intent.ActionGetContent);
					StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), REQUEST_PICK_IMAGE);
				};
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
					mBtnSend.Enabled = true;
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
								// mSyncContext.Post for RunOnUIThread
								mSyncContext.Post (delegate {
									foreach (var messageModel in e.Messages) {
										mAdapter.AddMessageModel (messageModel);
									}

									mAdapter.NotifyDataSetChanged ();
									mListView.SetSelection (e.Messages.Count);
								}, null);
							};
							mMessageListQuery.Prev (mAdapter.GetMinMessageTimestamp(), 30); // Excute Query
						}
					}
				};

				// Register Cache
				// Get max available VM memory, exceeding this amount will throw an OutOfMemory exception.
				// Stored in kilobytes as LruCache takes an int in its constructor.
				var cacheSize = (int)(Java.Lang.Runtime.GetRuntime().MaxMemory() / 16);

				mMemoryCache = new ImageUtils.MemoryLimitedLruCache(cacheSize);
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
				SendBirdSDK.SendMessage (mEtxtMessage.Text.ToString ());
				mEtxtMessage.Text = string.Empty;
			}

			public override void OnActivityResult(int requestCode, int resultCode, Intent data)
			{
				base.OnActivityResult(requestCode, resultCode, data);
				if (resultCode == (int)Android.App.Result.Ok) {
					if (requestCode == REQUEST_PICK_IMAGE && data != null && data.Data != null) {
						Upload(data.Data);
					}
				}
			}

			private void Upload(Android.Net.Uri uri)
			{
				try {
					// The projection contains the columns we want to return in our query.
					string[] projection = new[] { 
						Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data,
						Android.Provider.MediaStore.Images.Media.InterfaceConsts.MimeType,
						Android.Provider.MediaStore.Images.Media.InterfaceConsts.Size
					};
					using (ICursor cursor = this.Activity.ContentResolver.Query(uri, projection, null, null, null))
					{
						if (cursor != null)
						{
							cursor.MoveToFirst();
							string path = cursor.GetString(cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data));
							string mime = cursor.GetString(cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.MimeType));
							int size = cursor.GetInt(cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Size));
							cursor.Close();

							if(path == null) {
								Toast.MakeText(this.Activity, "Uploading file must be located in local storage.", ToastLength.Long).Show();
							} else {
								SendBirdSDK.UploadFile(path, mime, size, "", new SendBirdFileUploadEventHandler(
									(sender, e) => {
										if(e.Exception != null) {
											Console.WriteLine(e.Exception.StackTrace);
											Toast.MakeText(this.Activity, "Fail to upload the file.", ToastLength.Long).Show();
										}

										SendBirdSDK.SendFile(e.FileInfo);
									}
								));
							}
						}
					}
				} catch (Exception e) {
					Console.WriteLine(e.StackTrace);
					Toast.MakeText(this.Activity, "Fail to upload the file.", ToastLength.Long).Show();
				}
			}

			public override void OnDestroy ()
			{
				base.OnDestroy ();
				GC.Collect();
			}
		}

		public class SendBirdChatAdapter : BaseAdapter<Object>
		{
			private const int TYPE_UNSUPPORTED = 0;
			private const int TYPE_MESSAGE = 1;
			private const int TYPE_SYSTEM_MESSAGE = 2;
			private const int TYPE_FILELINK = 3;
			private const int TYPE_BROADCAST_MESSAGE = 4;

			private Context mContext;
			private LayoutInflater mInflater;
			private List<Object> mItemList;

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

			public SendBirdChatAdapter (Context context)
			{
				mContext = context;
				mInflater = mContext.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				mItemList = new List<Object> ();
			}

			#region implemented abstract members of BaseAdapter

			public override long GetItemId (int position)
			{
				return position;
			}

			public override int Count {
				get {
					return mItemList.Count;
				}
			}

			public override Object this [int index] {
				get {
					return mItemList [index];
				}
			}

			public void Clear ()
			{
				mMaxMessageTimestamp = long.MinValue;
				mMinMessageTimestamp = long.MaxValue;
				mItemList.Clear ();
			}

			public void AddMessageModel (MessageModel model)
			{
                if (model == null)
                    return;
                
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
							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewMessage, parent, false);
							viewHolder.SetView ("message", convertView.FindViewById (Resource.Id.txt_message) as TextView);
							viewHolder.SetView ("img_op_icon", (ImageView)convertView.FindViewById (Resource.Id.img_op_icon));
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
					case TYPE_FILELINK:
						{
							TextView tv;
							convertView = mInflater.Inflate (Resource.Layout.SendBirdViewFileLink, parent, false);
							tv = convertView.FindViewById (Resource.Id.txt_sender_name) as TextView;

							viewHolder.SetView ("txt_sender_name", tv);
							viewHolder.SetView("img_op_icon", convertView.FindViewById(Resource.Id.img_op_icon) as ImageView);
							viewHolder.SetView("img_file_container", convertView.FindViewById(Resource.Id.img_file_container));
							viewHolder.SetView("image_container", convertView.FindViewById(Resource.Id.image_container));
							viewHolder.SetView("img_thumbnail", convertView.FindViewById(Resource.Id.img_thumbnail));
							viewHolder.SetView("txt_image_name", convertView.FindViewById(Resource.Id.txt_image_name));
							viewHolder.SetView("txt_image_size", convertView.FindViewById(Resource.Id.txt_image_size));

							viewHolder.SetView("file_container", convertView.FindViewById(Resource.Id.file_container));
							viewHolder.SetView("txt_file_name", convertView.FindViewById(Resource.Id.txt_file_name));
							viewHolder.SetView("txt_file_size", convertView.FindViewById(Resource.Id.txt_file_size));

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
						viewHolder.GetView<ImageView> ("img_op_icon").Visibility = ViewStates.Gone;
						var messageView = viewHolder.GetView<TextView> ("message");
						messageView.TextFormatted = Html.FromHtml ("<font color='#824096'><b>" + message.GetSenderName () + "</b></font>: " + message.message);
						if (!messageView.HasOnClickListeners) {
							// To prevent mutiple click listners
							messageView.Click += (sender, e) => {
								var builder = new Android.App.AlertDialog.Builder (mContext);
								builder.SetTitle ("SENDBIRD");
								builder.SetMessage("Do you want to start 1:1 messaging with " + message.GetSenderName() + "?");
								builder.SetPositiveButton ("OK", (s, ev) => {
									Intent data = new Intent();
									data.PutExtra("userIds", new string[]{message.GetSenderId()});
									((SendBirdChatActivity)mContext).SetResult(Android.App.Result.Ok, data);
									((SendBirdChatActivity)mContext).mDoNotDisconnect = true;
									((SendBirdChatActivity)mContext).Finish();
								});
								builder.SetNegativeButton ("Cancel", (EventHandler<DialogClickEventArgs>)null);

								var dialog = builder.Create ();
								dialog.Show();
							};
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
				case TYPE_FILELINK:
					{
						FileLink fileLink = item as FileLink;
						if(fileLink.isOpMessage) {
							viewHolder.GetView<ImageView> ("img_op_icon").Visibility = ViewStates.Visible;
							viewHolder.GetView<TextView> ("txt_sender_name").TextFormatted = Html.FromHtml("&nbsp;&nbsp;&nbsp;<font color='#824096'><b>" + fileLink.GetSenderName() + "</b></font>: ");
						} else {
							viewHolder.GetView<ImageView> ("img_op_icon").Visibility = ViewStates.Gone;
							viewHolder.GetView<TextView>("txt_sender_name").TextFormatted = Html.FromHtml("<font color='#824096'><b>" + fileLink.GetSenderName() + "</b></font>: ");
						}
						if(fileLink.fileInfo.type.ToLower().StartsWith("image")) {
							viewHolder.GetView("file_container").Visibility = ViewStates.Gone;

							viewHolder.GetView("image_container").Visibility = ViewStates.Visible;
							viewHolder.GetView<TextView>("txt_image_name").Text = fileLink.fileInfo.name;
							viewHolder.GetView<TextView>("txt_image_size").Text = fileLink.fileInfo.size.ToString();
							if (fileLink.fileInfo.url != null && fileLink.fileInfo.url != "null") {
								DisplayUrlImage (viewHolder.GetView<ImageView> ("img_thumbnail"), fileLink.fileInfo.url);
							}
						} else {
							viewHolder.GetView("image_container").Visibility = ViewStates.Gone;

							viewHolder.GetView("file_container").Visibility = ViewStates.Visible;
							viewHolder.GetView<TextView>("txt_file_name").Text = fileLink.fileInfo.name;
							viewHolder.GetView<TextView>("txt_file_size").Text = "" + fileLink.fileInfo.size.ToString();
						}
						viewHolder.GetView("txt_sender_name").Click += (sender, e) => {
							var builder = new Android.App.AlertDialog.Builder (mContext);
							builder.SetTitle ("SENDBIRD");
							builder.SetMessage("Do you want to start 1:1 messaging with " + fileLink.GetSenderName() + "?");
							builder.SetPositiveButton ("OK", (s, ev) => {
								Intent data = new Intent();
								data.PutExtra("userIds", new string[]{fileLink.GetSenderId()});
								((SendBirdChatActivity)mContext).SetResult(Android.App.Result.Ok, data);
								((SendBirdChatActivity)mContext).mDoNotDisconnect = true;
								((SendBirdChatActivity)mContext).Finish();
							});
							builder.SetNegativeButton ("Cancel", (EventHandler<DialogClickEventArgs>)null);

							var dialog = builder.Create ();
							dialog.Show();
						};
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

		private static void DisplayUrlImage(ImageView imageView, string url)
		{
			int targetHeight = 256;
			int targetWidth = 256;

			if (mMemoryCache.Get (url) != null) {
				Bitmap cachedBM = (Bitmap)mMemoryCache.Get (url);
				imageView.SetImageBitmap (cachedBM);
			} else {
				WebClient webClient = new WebClient();
				webClient.DownloadDataCompleted += (sender, e) => {
					try {
						if(e.Error != null) {
							Console.WriteLine(e.Error.InnerException.StackTrace);
							Console.WriteLine(e.Error.InnerException.Message);
						} else {
							BitmapFactory.Options options = new BitmapFactory.Options ();
							options.InJustDecodeBounds = true; // <-- This makes sure bitmap is not loaded into memory.
							// Then get the properties of the bitmap
							BitmapFactory.DecodeByteArray(e.Result, 0, e.Result.Length, options);
							options.InSampleSize = ImageUtils.CalculateInSampleSize (options, targetWidth, targetHeight);
							options.InJustDecodeBounds = false;
							// Now we are loading it with the correct options. And saving precious memory.
							Bitmap bm = BitmapFactory.DecodeByteArray(e.Result, 0, e.Result.Length, options);
							mMemoryCache.Put(url, bm);
							imageView.SetImageBitmap(bm);
						}
					} catch(Exception ex) {
						Console.WriteLine(ex.StackTrace);
					}
					webClient.Dispose ();
				};
				webClient.DownloadDataAsync(new Uri(url));
			}
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
