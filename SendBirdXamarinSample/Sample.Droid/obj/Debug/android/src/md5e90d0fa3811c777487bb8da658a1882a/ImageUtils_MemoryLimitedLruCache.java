package md5e90d0fa3811c777487bb8da658a1882a;


public class ImageUtils_MemoryLimitedLruCache
	extends android.support.v4.util.LruCache
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_sizeOf:(Ljava/lang/Object;Ljava/lang/Object;)I:GetSizeOf_Ljava_lang_Object_Ljava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("SendBirdSample.Droid.ImageUtils+MemoryLimitedLruCache, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ImageUtils_MemoryLimitedLruCache.class, __md_methods);
	}


	public ImageUtils_MemoryLimitedLruCache (int p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == ImageUtils_MemoryLimitedLruCache.class)
			mono.android.TypeManager.Activate ("SendBirdSample.Droid.ImageUtils+MemoryLimitedLruCache, Sample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public int sizeOf (java.lang.Object p0, java.lang.Object p1)
	{
		return n_sizeOf (p0, p1);
	}

	private native int n_sizeOf (java.lang.Object p0, java.lang.Object p1);

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
