package md5e90d0fa3811c777487bb8da658a1882a;


public class SendBirdMessagingActivity_TypeTimer
	extends android.os.CountDownTimer
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTick:(J)V:GetOnTick_JHandler\n" +
			"n_onFinish:()V:GetOnFinishHandler\n" +
			"";
		mono.android.Runtime.register ("SendBirdSample.Droid.SendBirdMessagingActivity+TypeTimer, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SendBirdMessagingActivity_TypeTimer.class, __md_methods);
	}


	public SendBirdMessagingActivity_TypeTimer (long p0, long p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == SendBirdMessagingActivity_TypeTimer.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.SendBirdMessagingActivity+TypeTimer, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int64, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int64, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public void onTick (long p0)
	{
		n_onTick (p0);
	}

	private native void n_onTick (long p0);


	public void onFinish ()
	{
		n_onFinish ();
	}

	private native void n_onFinish ();

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
