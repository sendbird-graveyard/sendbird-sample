using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;

using SendBird;
using SendBird.Model;
using SendBird.Query;

using Sample.Droid;
using System.IO;

namespace SendBirdSample.Droid
{
	[Android.App.Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "ChannelList")]
	public class SendBirdChannelListActivity : FragmentActivity
	{
		private static ImageUtils.MemoryLimitedLruCache mMemoryCache;

		private SendBirdChannelListFragment mSendBirdChannelListFragment;

		private ImageButton mBtnClose;
		private ImageButton mBtnSettings;
		private TextView mTxtChannelUrl;
		private View mTopBarContainer;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SendBirdActivityChannelList);
			this.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);

			InitFragment (savedInstanceState, this.Intent.Extras.GetString("channelUrl"));
			InitUIComponents ();
			InitSendBird(this.Intent.Extras);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy ();
		}

		protected override void OnResume()
		{
			base.OnResume ();
		}

		protected override void OnPause() 
		{
			base.OnPause ();
		}

		public override void Finish () 
		{
			base.Finish ();
		}
			
		public static Bundle MakeSendBirdArgs(string appId, string uuid, string userName, string channelUrl)
		{
			Bundle args = new Bundle();
			args.PutString("appId", appId);
			args.PutString("uuid", uuid);
			args.PutString("userName", userName);
			args.PutString("channelUrl", channelUrl);
			return args;
		}

		private void InitFragment(Bundle savedInstanceState, string channelUrl) 
		{
			mSendBirdChannelListFragment = new SendBirdChannelListFragment(channelUrl);
			mSendBirdChannelListFragment.OnChannelSelected += (sender, e) => {
				Intent data = new Intent();
				data.PutExtra("channelUrl", e.Channel.url);
				SetResult(Android.App.Result.Ok, data);
				Finish();
			};
			if(savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragment_container, mSendBirdChannelListFragment).Commit(); // v4 fragment
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
				builder.SetTitle ("SendBird");
				builder.SetMessage("SendBird In App version " + SendBird.CommonVar.VERSION);
				builder.SetPositiveButton ("OK", (sender_child, e_child) => {
				});

				var dialog = builder.Create ();
				dialog.Show();
			};
			var maxMemory = (int)(Java.Lang.Runtime.GetRuntime().MaxMemory() / 1024);
			// Use 1/8th of the available memory for this memory cache.
			int cacheSize = maxMemory / 8;

			mMemoryCache = new ImageUtils.MemoryLimitedLruCache(cacheSize);
   		}

		private void InitSendBird(Bundle extras)
		{
			String appId = extras.GetString("appId");
			String uuid = extras.GetString("uuid");
			String userName = extras.GetString("userName");
			
			SendBirdSDK.Init(appId);
			SendBirdSDK.Login(uuid, userName);
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

		public class SendBirdChannelListFragment : Fragment 
		{
			public delegate void OnChannelSelectedEvent(object sender, ChannelEventArgs e);
			public OnChannelSelectedEvent OnChannelSelected;

			private string mChannelUrl;
			private ListView mListView;
			private ChannelListQuery mChannelListQuery;
			private ChannelListQuery mChannelListSearchQuery;
			private SendBirdChannelAdapter mCurrentAdapter;
			private SendBirdChannelAdapter mAdapter;
			private SendBirdChannelAdapter mSearchAdapter;
			private EditText mEtxtSearch;

			public SendBirdChannelListFragment() 
			{
			}

			public SendBirdChannelListFragment(string channelUrl)
			{
				mChannelUrl = channelUrl;
			}

			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View rootView = inflater.Inflate(Resource.Layout.SendBirdFragmentChannelList, container, false);
				InitUIComponents(rootView);
				mChannelListQuery = SendBirdSDK.QueryChannelList ();
				mChannelListQuery.OnResult += (sender, e) =>  {
					mAdapter.AddAll(e.Channels);
				};
				mChannelListQuery.Next (); // actually query to get channel list via API Client

				ShowChannelList();

				return rootView;
			}

			private void LoadMoreChannels()
			{
				if(mCurrentAdapter == mAdapter) {
					if(mChannelListQuery != null && mChannelListQuery.HasNext() && !mChannelListQuery.IsLoading()) {
						mChannelListQuery.OnResult += (sender, e) => {
							mAdapter.AddAll(e.Channels);
							mAdapter.NotifyDataSetChanged();
						};
						mChannelListQuery.Next ();
					}
				} else if(mCurrentAdapter == mSearchAdapter) {
					if(mChannelListSearchQuery != null && mChannelListSearchQuery.HasNext() && !mChannelListSearchQuery.IsLoading()) {
						mChannelListSearchQuery.OnResult += (sender, e) => {
							mSearchAdapter.AddAll(e.Channels);
							mSearchAdapter.NotifyDataSetChanged();
						};
						mChannelListSearchQuery.Next ();
					}
				}
			}

			public override void OnResume()
			{
				base.OnResume ();

				mAdapter.NotifyDataSetChanged ();
				mSearchAdapter.NotifyDataSetChanged ();
			}

			public override void OnDestroy()
			{
				base.OnDestroy ();
			}
				
			private void InitUIComponents(View rootView)
			{
				mListView = rootView.FindViewById (Resource.Id.list) as ListView;
				mAdapter = new SendBirdChannelAdapter(this.Activity);
				mSearchAdapter = new SendBirdChannelAdapter (this.Activity);
				mListView.ItemClick += (sender, e) => {
					Channel channel = mCurrentAdapter[e.Position];
					if(OnChannelSelected != null) {
						OnChannelSelected(this, new ChannelEventArgs(channel));
					}
				};

				mEtxtSearch = rootView.FindViewById (Resource.Id.etxt_search) as EditText;
				mEtxtSearch.AfterTextChanged += (sender, e) => {
					if(e.Editable.Length() <= 0) {
						ShowChannelList();
					} else {
						ShowSearchList();
					}
				};
				mEtxtSearch.KeyPress += (sender, e) => {
					if(e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter) {
						Search(mEtxtSearch.Text);

						e.Handled = true;
					} else {
						e.Handled = false;
					}
				};
			}

			private void ShowChannelList()
			{
				mCurrentAdapter = mAdapter;
				mListView.Adapter = mCurrentAdapter;
				mCurrentAdapter.NotifyDataSetChanged ();
			}

			private void ShowSearchList()
			{
				mCurrentAdapter = mSearchAdapter;
				mListView.Adapter = mCurrentAdapter;
				mCurrentAdapter.NotifyDataSetChanged();
			}
				
			private void Search(string keyword)
			{
				if (keyword == null || keyword.Length <= 0) {
					ShowChannelList ();
					return;
				}

				ShowSearchList ();

				mChannelListSearchQuery = SendBirdSDK.QueryChannelList(keyword);
				mChannelListSearchQuery.OnResult += (sender, e) => {
					mSearchAdapter.Clear();
					mSearchAdapter.AddAll(e.Channels);
					mSearchAdapter.NotifyDataSetChanged();
					if(e.Channels.Count <= 0) {
						Toast.MakeText(this.Activity, "No channels were found.", ToastLength.Short).Show();
					}
				};
				mChannelListSearchQuery.Next ();
			}

			public class SendBirdChannelAdapter : BaseAdapter<Channel>
			{
				private LayoutInflater mInflater;
				private List<Channel> mItemList;

				public SendBirdChannelAdapter (Context context)
				{
					mInflater = context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
					mItemList = new List<Channel> ();
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

				public override Channel this [int index]
				{
					get {
						return mItemList [index];
					}
				}

				public void Add(Channel channel)
				{
					mItemList.Add(channel);
					NotifyDataSetChanged ();
				}

				public void AddAll(List<Channel> channels)
				{
					mItemList.AddRange(channels);
					NotifyDataSetChanged ();
				}

				public void Clear()
				{
					mItemList.Clear ();
				}

				public override View GetView (int position, View convertView, ViewGroup parent)
				{
					ViewHolder viewHolder = null;

					if (convertView == null) {
						viewHolder = new ViewHolder ();

						convertView = mInflater.Inflate (Resource.Layout.SendBirdViewChannel, parent, false);
						viewHolder.SetView ("selected_container", convertView.FindViewById(Resource.Id.selected_container));
						viewHolder.GetView ("selected_container").Visibility = ViewStates.Gone;
						viewHolder.SetView("img_thumbnail", convertView.FindViewById(Resource.Id.img_thumbnail));
						viewHolder.SetView("txt_topic", convertView.FindViewById(Resource.Id.txt_topic));
						viewHolder.SetView("txt_desc", convertView.FindViewById(Resource.Id.txt_desc));

						convertView.Tag = viewHolder;
					}

					Channel item = this[position];
					viewHolder = convertView.Tag as ViewHolder;

					DisplayUrlImage(viewHolder.GetView<ImageView> ("img_thumbnail"), item.coverUrl);

					viewHolder.GetView<TextView> ("txt_topic").Text = "#" + item.GetUrlWithoutAppPrefix();
					viewHolder.GetView<TextView> ("txt_desc").Text = ("" + item.memberCount + ((item.memberCount <= 1) ? " Member" : " Members"));

					if(item.url.Equals(SendBirdSDK.GetChannelUrl())) {
						viewHolder.GetView("selected_container").Visibility = ViewStates.Visible;
					} else {
						viewHolder.GetView("selected_container").Visibility = ViewStates.Gone;
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


