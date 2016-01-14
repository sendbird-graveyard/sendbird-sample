package md5e90d0fa3811c777487bb8da658a1882a;


public class SendBirdMemberListActivity_SendBirdMemberListFragment
	extends android.support.v4.app.Fragment
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onDestroy:()V:GetOnDestroyHandler\n" +
			"";
		mono.android.Runtime.register ("SendBirdSample.Droid.SendBirdMemberListActivity+SendBirdMemberListFragment, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SendBirdMemberListActivity_SendBirdMemberListFragment.class, __md_methods);
	}


	public SendBirdMemberListActivity_SendBirdMemberListFragment () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SendBirdMemberListActivity_SendBirdMemberListFragment.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.SendBirdMemberListActivity+SendBirdMemberListFragment, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public SendBirdMemberListActivity_SendBirdMemberListFragment (java.lang.String p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == SendBirdMemberListActivity_SendBirdMemberListFragment.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.SendBirdMemberListActivity+SendBirdMemberListFragment, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();


	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();

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
