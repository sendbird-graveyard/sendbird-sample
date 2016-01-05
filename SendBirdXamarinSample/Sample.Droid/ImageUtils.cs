using System;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Util;


namespace SendBirdSample.Droid
{
	public class ImageUtils
	{
		public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}
		/// <summary>
		/// LruCache limited by memory footprint in KB rather than number of items.
		/// </summary>
		public class MemoryLimitedLruCache : LruCache
		{
			public MemoryLimitedLruCache(int size) : base(size) {}

			protected override int SizeOf(Java.Lang.Object key, Java.Lang.Object value)
			{
				// android.graphics.Bitmap.getByteCount() method isn't currently implemented in Xamarin. Invoke Java method.
				IntPtr classRef = JNIEnv.FindClass("android/graphics/Bitmap");
				var getBytesMethodHandle = JNIEnv.GetMethodID(classRef, "getByteCount", "()I");
				var byteCount = JNIEnv.CallIntMethod(value.Handle, getBytesMethodHandle);

				return byteCount / 1024;
			}
		}
	}
}