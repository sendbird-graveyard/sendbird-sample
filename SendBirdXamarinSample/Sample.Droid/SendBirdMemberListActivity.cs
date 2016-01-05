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

namespace SendBirdSample.Droid
{
	[Android.App.Activity (Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar", Label = "MemberList")]
	public class SendBirdMemberListActivity : FragmentActivity
	{
		private static ImageUtils.MemoryLimitedLruCache mMemoryCache;

		private SendBirdMemberListFragment mSendBirdMemberListFragment;

		private ImageButton mBtnClose;
		private Button mBtnOK;
		private TextView mTxtChannelUrl;
		private View mTopBarContainer;

		private List<Member> mSelectedMembers;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SendBirdActivityMemberList);

			this.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);

			InitFragment (savedInstanceState, this.Intent.Extras.GetString("channelUrl"));
			InitUIComponents ();
			InitSendBird(this.Intent.Extras);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy ();
			GC.Collect();
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
			
		public static Bundle MakeSendBirdArgs(String appId, String uuid, String userName, String channelUrl)
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
			mSendBirdMemberListFragment = new SendBirdMemberListFragment(channelUrl);
			mSendBirdMemberListFragment.OnMemberSelected += (sender, e) => {
				mSelectedMembers = e.Members as List<Member>;
				if(mSelectedMembers.Count <= 0) {
					mBtnOK.SetTextColor(Color.ParseColor("#6f5ca7"));
				} else {
					mBtnOK.SetTextColor(Color.ParseColor("#35f8ca"));
				}
			};
			if(savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragment_container, mSendBirdMemberListFragment).Commit(); // v4 fragment
			}
		}

		private void InitUIComponents() 
		{
			mTopBarContainer = FindViewById(Resource.Id.top_bar_container) as View;
			mTxtChannelUrl = FindViewById (Resource.Id.txt_channel_url) as TextView;

			mBtnClose = FindViewById (Resource.Id.btn_close) as ImageButton;
			mBtnClose.Click += (sender, e) => {
				Finish();
			}; 

			mBtnOK = FindViewById (Resource.Id.btn_ok) as Button;
			mBtnOK.Click += (sender, e) =>  {
				if(mSelectedMembers.Count > 0) {
					string[] memberIds = new string[mSelectedMembers.Count];
					for(var i = 0; i < memberIds.Length; i++) {
						Console.WriteLine(mSelectedMembers[i].GetId());
						memberIds[i] = mSelectedMembers[i].GetId();
					}
					Intent data = new Intent();
					data.PutExtra("userIds", memberIds);
					SetResult(Android.App.Result.Ok, data);
				} else {
					SetResult(Android.App.Result.Canceled);
				}
				Finish();
			};

			ResizeMenubar ();

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

		private void InitSendBird(Bundle extras)
		{
			if(extras != null) {
				String appId = extras.GetString("appId");
				String uuid = extras.GetString("uuid");
				String userName = extras.GetString("userName");
				
				SendBirdSDK.Init(appId);
				SendBirdSDK.Login(uuid, userName);
			}
		}

		public class SendBirdMemberListFragment : Fragment 
		{
			public delegate void OnMemberSelectedEvent(object sender, MemberListEventArgs e);
			public OnMemberSelectedEvent OnMemberSelected;

			private string mChannelUrl;
			private ListView mListView;
			private MemberListQuery mMemberListQuery;
			private SendBirdMemberAdapter mAdapter;
			private List<Member> mSelectedMembers;

			public SendBirdMemberListFragment() 
			{
			}

			public SendBirdMemberListFragment(string channelUrl)
			{
				mChannelUrl = channelUrl;
			}

			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View rootView = inflater.Inflate(Resource.Layout.SendBirdFragmentMemberList, container, false);
				InitUIComponents(rootView);

				mMemberListQuery = SendBirdSDK.QueryMemberList (mChannelUrl);
				mMemberListQuery.OnResult += (sender, e) =>  {
					mAdapter.AddAll(e.Members);
					if(e.Members.Count <= 0) {
						Toast.MakeText(this.Activity, "No members.", ToastLength.Short).Show();
					}
				};
				mMemberListQuery.Next (); // actually query to get member list via API Client

				return rootView;
			}

			public override void OnResume()
			{
				base.OnResume ();

				mAdapter.NotifyDataSetChanged ();
			}

			public override void OnDestroy()
			{
				base.OnDestroy ();
			}
				
			private void InitUIComponents(View rootView)
			{
				mSelectedMembers = new List<Member>();
				mListView = rootView.FindViewById (Resource.Id.list) as ListView;
				mAdapter = new SendBirdMemberAdapter(this.Activity, this);
				mListView.ItemClick += (sender, e) => {
				};

				mListView.Adapter = mAdapter;
			}

			public class SendBirdMemberAdapter : BaseAdapter<Member>
			{
				private Context mContext;
				private SendBirdMemberListFragment mFragment;
				private LayoutInflater mInflater;
				private List<Member> mItemList;

				public SendBirdMemberAdapter (Context context, SendBirdMemberListFragment fragment)
				{
					mContext = context;
					mFragment = fragment;
					mInflater = mContext.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
					mItemList = new List<Member> ();
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
				public override Member this [int index]
				{
					get {
						return mItemList [index];
					}
				}
				public void AddAll(List<Member> members)
				{
					mItemList.AddRange(members);
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

						convertView = mInflater.Inflate (Resource.Layout.SendBirdViewMember, parent, false);
						viewHolder.SetView ("root_view", convertView);
						viewHolder.SetView("img_thumbnail", convertView.FindViewById(Resource.Id.img_thumbnail));
						viewHolder.SetView("txt_name", convertView.FindViewById(Resource.Id.txt_name));
						viewHolder.SetView("chk_select", convertView.FindViewById(Resource.Id.chk_select));

						convertView.Tag = viewHolder;
					}

					Member item = this[position];
					viewHolder = convertView.Tag as ViewHolder;
					DisplayUrlImage(viewHolder.GetView<ImageView> ("img_thumbnail"), item.imageUrl);
					viewHolder.GetView<TextView> ("txt_name").Text = item.name;
					viewHolder.GetView<CheckBox> ("chk_select").CheckedChange += (sender, e) => {
						if(e.IsChecked) {
							mFragment.mSelectedMembers.Add(item);
						} else {
							mFragment.mSelectedMembers.Remove(item);
						}

						if(mFragment.OnMemberSelected != null) {
							mFragment.OnMemberSelected(this, new MemberListEventArgs(mFragment.mSelectedMembers));
						}
					};
					viewHolder.GetView<CheckBox> ("chk_select").Checked = mFragment.mSelectedMembers.Contains(item);

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
					webClient.Dispose();
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


