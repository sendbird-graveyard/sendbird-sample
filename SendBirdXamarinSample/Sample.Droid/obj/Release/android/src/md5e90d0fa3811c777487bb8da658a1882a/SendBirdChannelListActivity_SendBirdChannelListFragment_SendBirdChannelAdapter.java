package md5e90d0fa3811c777487bb8da658a1882a;


public class SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter
	extends android.widget.BaseAdapter
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_getItemId:(I)J:GetGetItemId_IHandler\n" +
			"n_getCount:()I:GetGetCountHandler\n" +
			"n_getItem:(I)Ljava/lang/Object;:GetGetItem_IHandler\n" +
			"n_getView:(ILandroid/view/View;Landroid/view/ViewGroup;)Landroid/view/View;:GetGetView_ILandroid_view_View_Landroid_view_ViewGroup_Handler\n" +
			"";
		mono.android.Runtime.register ("SendBirdSample.Droid.SendBirdChannelListActivity+SendBirdChannelListFragment+SendBirdChannelAdapter, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter.class, __md_methods);
	}


	public SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.SendBirdChannelListActivity+SendBirdChannelListFragment+SendBirdChannelAdapter, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == SendBirdChannelListActivity_SendBirdChannelListFragment_SendBirdChannelAdapter.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.SendBirdChannelListActivity+SendBirdChannelListFragment+SendBirdChannelAdapter, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public long getItemId (int p0)
	{
		return n_getItemId (p0);
	}

	private native long n_getItemId (int p0);


	public int getCount ()
	{
		return n_getCount ();
	}

	private native int n_getCount ();


	public java.lang.Object getItem (int p0)
	{
		return n_getItem (p0);
	}

	private native java.lang.Object n_getItem (int p0);


	public android.view.View getView (int p0, android.view.View p1, android.view.ViewGroup p2)
	{
		return n_getView (p0, p1, p2);
	}

	private native android.view.View n_getView (int p0, android.view.View p1, android.view.ViewGroup p2);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
