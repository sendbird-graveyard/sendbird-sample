using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

using Android.Content;
using Android.Graphics;
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
	[Android.App.Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "MessagingChannelList")]
	public class SendBirdMessagingChannelListActivity : FragmentActivity
	{
		private static SynchronizationContext mSyncContext;
		private static ImageUtils.MemoryLimitedLruCache mMemoryCache;

		private SendBirdMessagingChannelListFragment mSendBirdMessagingChannelListFragment;
		private SendBirdMessagingChannelAdapter mSendBirdMessagingChannelAdapter;

		private ImageButton mBtnClose;
		private ImageButton mBtnSettings;
		private TextView mTxtChannelUrl;
		private View mTopBarContainer;
			
		public static Bundle MakeSendBirdArgs(string appId, string uuid, string userName)
		{
			Bundle args = new Bundle();
			args.PutString("appId", appId);
			args.PutString("uuid", uuid);
			args.PutString("userName", userName);
			return args;
		}

		private void InitFragment(Bundle savedInstanceState) 
		{
			mSendBirdMessagingChannelAdapter = new SendBirdMessagingChannelAdapter (this);

			mSendBirdMessagingChannelListFragment = new SendBirdMessagingChannelListFragment();
			mSendBirdMessagingChannelListFragment.mAdapter = mSendBirdMessagingChannelAdapter;
			mSendBirdMessagingChannelListFragment.OnMessagingChannelSelected += (sender, e) => {
				Intent data = new Intent();
				data.PutExtra("channelUrl", e.MessagingChannel.GetUrl());
				SetResult(Android.App.Result.Ok, data);
				Finish();
			};
			if(savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragment_container, mSendBirdMessagingChannelListFragment).Commit(); // v4 fragment
			}
		}

		private void InitUIComponents() 
		{
			mTopBarContainer = FindViewById(Resource.Id.top_bar_container) as View;
			mTxtChannelUrl = FindViewById (Resource.Id.txt_channel_url) as TextView;

			mBtnClose = FindViewById (Resource.Id.btn_close) as ImageButton;
			mBtnClose.Click += (object sender, EventArgs e) => {
				Finish();
			}; 
			mBtnSettings = FindViewById (Resource.Id.btn_settings) as ImageButton;
			mBtnSettings.Click += (sender, e) => {
				var builder = new Android.App.AlertDialog.Builder (this);
				builder.SetTitle ("SENDBIRD");
				builder.SetMessage("SendBird In App version " + SendBird.CommonVar.VERSION);
				builder.SetPositiveButton ("OK", (sender_child, e_child) => {
				});

				var dialog = builder.Create ();
				dialog.Show();
			};
			ResizeMenubar ();

			// Register Cache
			// Get max available VM memory, exceeding this amount will throw an OutOfMemory exception.
			// Stored in kilobytes as LruCache takes an int in its constructor.
			var cacheSize = (int)(Java.Lang.Runtime.GetRuntime().MaxMemory() / 16);

			mMemoryCache = new ImageUtils.MemoryLimitedLruCache(cacheSize);
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

		private void InitSendBird(Bundle extras)
		{
			if (extras != null) {
				String appId = extras.GetString("appId");
				String uuid = extras.GetString("uuid");
				String userName = extras.GetString("userName");

				mSyncContext = SynchronizationContext.Current; // required for ui update

				SendBirdNotificationHandler jnh = new SendBirdNotificationHandler();
				jnh.OnMessagingChannelUpdated += (sender, e) => {
					mSendBirdMessagingChannelAdapter.Replace(e.MessagingChannel);
				};
				jnh.OnMentionUpdated += (sender, e) => {

				};

				SendBirdEventHandler ieh = new SendBirdEventHandler ();
				ieh.OnConnect += (sender, e) => {
				};
				ieh.OnError += (sender, e) => {
					if (SendBird.CommonVar.IS_DEBUG) {
						Console.WriteLine ("ieh Messaging OnError");
						Console.WriteLine (e.ErrorCode);
					}
				};
				ieh.OnMessagingEnded += (sender, e) => {
				};

				SendBirdSDK.Init (appId);
				SendBirdSDK.Login (uuid, userName);
				SendBirdSDK.RegisterNotificationHandler (jnh);
				SendBirdSDK.SetEventHandler (ieh);
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SendBirdActivityMessagingChannelList);
			this.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);

			InitFragment (savedInstanceState);
			InitUIComponents ();

			InitSendBird(this.Intent.Extras);

			Toast.MakeText(this, "Long press the channel to leave.", ToastLength.Long).Show();
		}

		protected override void OnResume()
		{
			base.OnResume ();
			var mMessageChannelListQuery = SendBirdSDK.QueryMessagingList ();
			mMessageChannelListQuery.OnResult += (sender, e) => {
				mSyncContext.Post (delegate {
					mSendBirdMessagingChannelAdapter.Clear();
					mSendBirdMessagingChannelAdapter.AddAll(e.MessagingChannels);
					if(e.MessagingChannels.Count <= 0) {
						Toast.MakeText(this, "No messaging channels were found.", ToastLength.Short).Show();
					}

					mSendBirdMessagingChannelAdapter.NotifyDataSetChanged ();

					SendBirdSDK.Join("");
					SendBirdSDK.Connect();
				}, null);
			};
			mMessageChannelListQuery.Excute (); // Excute Query
		}

		protected override void OnPause() 
		{
			base.OnPause ();
			SendBirdSDK.Disconnect ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			GC.Collect();
		}

		public override void Finish () 
		{
			base.Finish ();
		}

		public class SendBirdMessagingChannelListFragment : Fragment 
		{
			public delegate void OnMessagingChannelSelectedEvent(object sender, MessagingChannelEventArgs e);
			public OnMessagingChannelSelectedEvent OnMessagingChannelSelected;
			private ListView mListView;
			internal SendBirdMessagingChannelAdapter mAdapter;

			public SendBirdMessagingChannelListFragment() 
			{
			}

			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View rootView = inflater.Inflate(Resource.Layout.SendBirdFragmentMessagingChannelList, container, false);
				InitUIComponents(rootView);
				return rootView;
			}

			public override void OnResume()
			{
				base.OnResume ();
				mAdapter.NotifyDataSetChanged ();
			}
				
			private void InitUIComponents(View rootView)
			{
				mListView = rootView.FindViewById (Resource.Id.list) as ListView;
				mListView.Adapter = mAdapter;
				mListView.ItemClick += (sender, e) => {
					MessagingChannel messagingChannel = mAdapter[e.Position];
					if(OnMessagingChannelSelected != null) {
						OnMessagingChannelSelected(this, new MessagingChannelEventArgs(messagingChannel));
					}
				};
				mListView.ItemLongClick += (sender, e) => {
					MessagingChannel messagingChannel = mAdapter[e.Position];
					var builder = new Android.App.AlertDialog.Builder (this.Activity);
					builder.SetTitle ("Leave");
					builder.SetMessage("Do you want to leave this channel?");
					builder.SetPositiveButton ("Leave", (sender_child, e_child) => {
						mAdapter.Remove(e.Position);
						mAdapter.NotifyDataSetChanged();
						SendBirdSDK.EndMessaging(messagingChannel.GetUrl());
					});
					builder.SetNeutralButton ("Hide", (sender_child, e_child) => {
						mAdapter.Remove(e.Position);
						mAdapter.NotifyDataSetChanged();
						SendBirdSDK.HideMessaging(messagingChannel.GetUrl());
					});
					builder.SetNegativeButton("Cancel", (EventHandler<DialogClickEventArgs>)null);

					var dialog = builder.Create ();
					dialog.Show();
				};
			}
		}

		public class SendBirdMessagingChannelAdapter : BaseAdapter<MessagingChannel>
		{
			private Context mContext;
			private LayoutInflater mInflater;
			private List<MessagingChannel> mItemList;
			internal long mCurrentChannelId;

			public SendBirdMessagingChannelAdapter (Context context)
			{
				mContext = context;
				mInflater = context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				mItemList = new List<MessagingChannel> ();
			}

			#region implemented abstract members of BaseAdapter

			public override long GetItemId (int position)
			{
				return position;
			}
			public override int Count
			{
				get {
					return mItemList.Count;
				}
			}
			public override MessagingChannel this [int index]
			{
				get {
					return mItemList [index];
				}
			}
			public void Add(MessagingChannel channel)
			{
				mItemList.Add(channel);
				NotifyDataSetChanged ();
			}
			public void AddAll(List<MessagingChannel> channels)
			{
				mItemList.AddRange(channels);
				NotifyDataSetChanged ();
			}
			public void Replace(MessagingChannel newChannel)
			{
				foreach (MessagingChannel oldChannel in mItemList) {
					if (oldChannel.GetId () == newChannel.GetId ()) {
						mItemList.Remove (oldChannel);
						break;
					}
				}
				mItemList.Insert (0, newChannel);
				NotifyDataSetChanged ();
			}
			public void Clear()
			{
				mItemList.Clear ();
			}
			public void Remove(int index)
			{
				mItemList.RemoveAt (index);
			}
			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ViewHolder viewHolder = null;

				if (convertView == null) {
					viewHolder = new ViewHolder ();

					convertView = mInflater.Inflate (Resource.Layout.SendBirdViewMessagingChannel, parent, false);
					viewHolder.SetView ("selected_container", convertView.FindViewById(Resource.Id.selected_container));
					viewHolder.GetView ("selected_container").Visibility = ViewStates.Gone;
					viewHolder.SetView("img_thumbnail", convertView.FindViewById(Resource.Id.img_thumbnail));
					viewHolder.SetView("txt_topic", convertView.FindViewById(Resource.Id.txt_topic));
					viewHolder.SetView("txt_member_count", convertView.FindViewById(Resource.Id.txt_member_count));
					viewHolder.SetView("txt_unread_count", convertView.FindViewById(Resource.Id.txt_unread_count));
					viewHolder.SetView("txt_date", convertView.FindViewById(Resource.Id.txt_date));
					viewHolder.SetView("txt_desc", convertView.FindViewById(Resource.Id.txt_desc));

					convertView.Tag = viewHolder;
				}
				MessagingChannel item = this[position];
				viewHolder = convertView.Tag as ViewHolder;
				DisplayUrlImage(viewHolder.GetView<ImageView> ("img_thumbnail"), GetDisplayCoverImageUrl(item.GetMembers()));
				viewHolder.GetView<TextView> ("txt_topic").Text = GetDisplayMemberNames (item.GetMembers ());

				if(item.unreadMessageCount > 0) {
					viewHolder.GetView<TextView> ("txt_unread_count").Visibility = ViewStates.Visible;
					viewHolder.GetView<TextView> ("txt_unread_count").Text = "" + item.unreadMessageCount.ToString();
				} else {
					viewHolder.GetView<TextView> ("txt_unread_count").Visibility = ViewStates.Gone;
				}

				if(item.IsGroupMessageChannel()) {
					viewHolder.GetView<TextView> ("txt_member_count").Visibility = ViewStates.Visible;
					viewHolder.GetView<TextView> ("txt_member_count").Text = "" + item.GetMemberCount().ToString();
				} else {
					viewHolder.GetView<TextView> ("txt_member_count").Visibility = ViewStates.Gone;
				}

				if(item.HasLastMessage()) {
					SendBird.Model.Message message = item.lastMessage;
					viewHolder.GetView<TextView> ("txt_date").Text = GetDisplayTimeOrDate (mContext, message.messageTimestamp);
					viewHolder.GetView<TextView> ("txt_desc").Text = string.Empty + message.message;
				} else {
					viewHolder.GetView<TextView> ("txt_date").Text = string.Empty;
					viewHolder.GetView<TextView> ("txt_desc").Text = string.Empty;
				}

				return convertView;
			}

			private class ViewHolder : Java.Lang.Object
			{
				private Dictionary<string, View> holder = new Dictionary<string, View>();
				private int type;

				public int GetViewType()
				{
					return this.type;
				}

				public void SetViewType(int type)
				{
					this.type = type;
				}
				public void SetView(string k, View v)
				{
					holder.Add(k, v);
				}

				public View GetView(string k)
				{
					return holder[k];
				}

				public T GetView<T> (string k)
				{
					return (T)Convert.ChangeType(GetView(k), typeof(T));
				}
			}
			#endregion
		}

		private static string GetDisplayCoverImageUrl(List<MessagingChannel.Member> members)
		{
			foreach (MessagingChannel.Member member in members) {
				if(member.id.Equals(SendBirdSDK.GetUserId())) {
					continue;
				}

				return member.imageUrl;
			}

			return string.Empty;
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

		private static string GetDisplayTimeOrDate(Context context, long milli)
		{
			Date date = new Date (milli);

			if (SendBirdUtils.GetCurrentTimeMills () - milli > 60 * 60 * 24 * 1000L) {
				return DateFormat.GetDateFormat(context).Format(date);
			} else {
				return DateFormat.GetTimeFormat(context).Format(date);
			}
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
			public static void HideKeyboard(Android.App.Activity activity)
			{
				if (activity == null || activity.CurrentFocus == null) {
					return;
				}
				var imm = activity.GetSystemService(Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
				imm.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, 0);
			}
		}
	}
}


