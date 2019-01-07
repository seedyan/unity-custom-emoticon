package com.sdg.emoji;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import com.unity3d.player.UnityPlayer;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.ContentUris;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.Build;
import android.provider.DocumentsContract;
import android.provider.MediaStore;
import android.support.v4.content.CursorLoader;
import android.text.TextUtils;
import android.util.Base64;
import android.util.Log;



public class CustomEmojiUtil 
{
	

    /**
     * @param activity    当前activity
     * @param imageUri    拍照后照片存储路径
     * @param requestCode 调用系统相机请求码
     */
    public static void takePicture(Activity activity, Uri imageUri, int requestCode) { //调用系统相机
        Intent intentCamera = new Intent();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) { 
        	intentCamera.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION); //添加这一句表示对目标应用临时授权该Uri所代表的文件
        } 
        intentCamera.setAction(MediaStore.ACTION_IMAGE_CAPTURE);
        //将拍照结果保存至photo_file的Uri中，不保留在相册中
        intentCamera.putExtra(MediaStore.EXTRA_OUTPUT, imageUri);
        activity.startActivityForResult(intentCamera, requestCode);
    }
     /**
     * @param activity    当前activity
     * @param requestCode 打开相册的请求码
     */
    public static void openPic(Activity activity, int requestCode) {
    	Intent photoPickerIntent = new Intent(Intent.ACTION_GET_CONTENT);
        photoPickerIntent.setType("image/*");
        activity.startActivityForResult(photoPickerIntent, requestCode);
    }

	//获取图片压缩比
	public static int GetBitMapSampleSize(int reqWidth, int reqHeight)
	{
		float maxLength = Math.max(reqWidth, reqHeight);
		int be = 1;
		if(maxLength>1024)
		{
			be = (int)(maxLength / (float)1024); 
		}
		return be;
	}
	
    //压缩图片
    public static Bitmap lessenUriImage(String path){ 
		BitmapFactory.Options options = new BitmapFactory.Options(); 
		options.inJustDecodeBounds = true; 
		Bitmap bitmap = BitmapFactory.decodeFile(path, options); //此时返回 bm 为空 
		options.inJustDecodeBounds = false; //缩放比。由于是固定比例缩放，只用高或者宽其中一个数据进行计算即可
		int be = CustomEmojiUtil.GetBitMapSampleSize(options.outWidth,options.outHeight);
		Log.d("test", be+":be");
		options.inSampleSize = be; //重新读入图片，注意此时已经把 options.inJustDecodeBounds 设回 false 了 
		
		bitmap=BitmapFactory.decodeFile(path,options); 
		int w = bitmap.getWidth(); 
		int h = bitmap.getHeight(); 
		System.out.println(w+" "+h); //after zoom
		return bitmap;
	}
	/**
	 * 根据Uri获取图片的绝对路径
	 *
	 * @param context 上下文对象
	 * @param uri     图片的Uri
	 * @return 如果Uri对应的图片存在, 那么返回该图片的绝对路径, 否则返回null
	 */
	public static String getRealPathFromUri(Context context, Uri uri) {
	    int sdkVersion = Build.VERSION.SDK_INT;
	    Log.d("test","sdkVersion:"+sdkVersion );
	    if (sdkVersion >= 19) { // api >= 19
	        return getRealPathFromUriAboveApi19(context, uri);
	    } else { // api < 19
	        return getRealPathFromUriBelowAPI19(context, uri);
	    }
	}
	
	/**
     * 适配api19及以上,根据uri获取图片的绝对路径
     *
     * @param context 上下文对象
     * @param uri     图片的Uri
     * @return 如果Uri对应的图片存在, 那么返回该图片的绝对路径, 否则返回null
     */
    @SuppressLint("NewApi")
    private static String getRealPathFromUriAboveApi19(Context context, Uri uri) {
        String filePath = null;
        Log.d("test", "context:"+context+"    uri:"+uri);
        if (DocumentsContract.isDocumentUri(context, uri)) {
        	 Log.d("test", "DocumentsContract");
            // 如果是document类型的 uri, 则通过document id来进行处理
            String documentId = DocumentsContract.getDocumentId(uri);
            if (isMediaDocument(uri)) { // MediaProvider
                // 使用':'分割
                String id = documentId.split(":")[1];

                String selection = MediaStore.Images.Media._ID + "=?";
                String[] selectionArgs = {id};
                filePath = getDataColumn(context, MediaStore.Images.Media.EXTERNAL_CONTENT_URI, selection, selectionArgs);
            } else if (isDownloadsDocument(uri)) { // DownloadsProvider
                Uri contentUri = ContentUris.withAppendedId(Uri.parse("content://downloads/public_downloads"), Long.valueOf(documentId));
                filePath = getDataColumn(context, contentUri, null, null);
            }
        } else if ("content".equalsIgnoreCase(uri.getScheme())){
        	 Log.d("test", "content");
            // 如果是 content 类型的 Uri
            filePath = getDataColumn(context, uri, null, null);
        } else if ("file".equals(uri.getScheme())) {
        	 Log.d("test", "file");
            // 如果是 file 类型的 Uri,直接获取图片对应的路径
            filePath = uri.getPath();
        }
        return filePath;
    }
    
    /**
     * @param uri the Uri to check
     * @return Whether the Uri authority is MediaProvider
     */
    private static boolean isMediaDocument(Uri uri) {
        return "com.android.providers.media.documents".equals(uri.getAuthority());
    }
    
    /**
     * @param uri the Uri to check
     * @return Whether the Uri authority is DownloadsProvider
     */
    private static boolean isDownloadsDocument(Uri uri) {
        return "com.android.providers.downloads.documents".equals(uri.getAuthority());
    }
	
	/**
     * 适配api19以下(不包括api19),根据uri获取图片的绝对路径
     *
     * @param context 上下文对象
     * @param uri     图片的Uri
     * @return 如果Uri对应的图片存在, 那么返回该图片的绝对路径, 否则返回null
     */
    private static String getRealPathFromUriBelowAPI19(Context context, Uri uri) {
        return getDataColumn(context, uri, null, null);
    }
    
    /**
     * 获取数据库表中的 _data 列，即返回Uri对应的文件路径
     * @return
     */
    private static String getDataColumn(Context context, Uri uri, String selection, String[] selectionArgs) {
        String path = null;
        Log.d("test", "getDataColumn");
        String[] projection = new String[]{MediaStore.Images.Media.DATA};
        Cursor cursor = null;
        try {
        	Log.d("test","try:"+context +"   context.getContentResolver():"+context.getContentResolver());
        	
            cursor = context.getContentResolver().query(uri, null, selection, selectionArgs, null);
            Log.d("test", "cursor:"+cursor+"  cursor.moveToFirst():"+cursor.moveToFirst()+"  projection[0]:"+projection[0]+"  cursor.getColumnIndexOrThrow(projection[0]):"+cursor.getColumnIndexOrThrow(projection[0]));
            
            if (cursor != null && cursor.moveToFirst()) {
                int columnIndex = cursor.getColumnIndexOrThrow(projection[0]);
                path = cursor.getString(columnIndex);
            }
        } catch (Exception e) {
        	Log.d("test", e.toString());
            if (cursor != null) {
                cursor.close();
            }
        }
        return path;
    }
    
    /**
     * 读取图片属性：旋转的角度
     *
     * @param path 图片绝对路径
     * @return degree旋转的角度
     */
    public static int readPictureDegree(String path) {
        int degree = 0;
        try {
            ExifInterface exifInterface = new ExifInterface(path);
            int orientation = exifInterface.getAttributeInt(
                    ExifInterface.TAG_ORIENTATION,
                    ExifInterface.ORIENTATION_NORMAL);
            switch (orientation) {
                case ExifInterface.ORIENTATION_ROTATE_90:
                    degree = 90;
                    break;
                case ExifInterface.ORIENTATION_ROTATE_180:
                    degree = 180;
                    break;
                case ExifInterface.ORIENTATION_ROTATE_270:
                    degree = 270;
                    break;
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return degree;
    }
    /**
     * 将图片按照某个角度进行旋转
     *
     * @param bm
     *            需要旋转的图片
     * @param degree
     *            旋转角度
     * @return 旋转后的图片
     */
    public static Bitmap rotateBitmapByDegree(Bitmap bm, int degree) {
        Bitmap returnBm = null;
 
        // 根据旋转角度，生成旋转矩阵
        Matrix matrix = new Matrix();
        matrix.postRotate(degree);
        try {
            // 将原始图片按照旋转矩阵进行旋转，并得到新的图片
            returnBm = Bitmap.createBitmap(bm, 0, 0, bm.getWidth(),
                    bm.getHeight(), matrix, true);
        } catch (OutOfMemoryError e) {
        }
        if (returnBm == null) {
            returnBm = bm;
        }
        if (bm != returnBm) {
            bm.recycle();
        }
        return returnBm;
    }
    
    ///保存图片到沙盒文件
	public static void SaveBitmap(Context context, Bitmap bitmap,String fileName,String ImageName) throws IOException {
		Log.d("test", "SaveBitmap");
		FileOutputStream fOut = null;
		String packageName = context.getPackageName();
		Log.d("test", "packageName:"+packageName);
		//注解1
		String path = "/mnt/sdcard/Android/data/"+context.getPackageName()+"/files"+fileName;
		Log.d("test", "path："+path);
		try 
		{
		  File destDir = new File(path);
		  if (!destDir.exists())
		  {
			  destDir.mkdirs();
		  }
		  fOut = new FileOutputStream(path + ImageName);
		  //将Bitmap对象写入本地路径中，Unity在去相同的路径来读取这个文件
		  bitmap.compress(Bitmap.CompressFormat.PNG, 50, fOut);
		  UnityPlayer.UnitySendMessage("SDKCallBackObj","MessageCallBack",ImageName);
		  Log.d("test", "UnitySendMessage");
		  try 
		  {
			  fOut.flush();
		  } 
		  catch (IOException e) 
		  {
			  e.printStackTrace();
		  }
		  
		  try 
		  {
			  fOut.close();
			  bitmap.recycle();	// 回收bitmap的内存
			  bitmap = null;
		  } 
		  catch (IOException e) 
		  {
			  e.printStackTrace();
		  }
		} 
		catch (FileNotFoundException e) 
		{
			e.printStackTrace();
		}
	}
	
	 /**
     * bitmap转为base64
     *
     * @param bitmap
     * @return
     */ public static void bitmapToBase64(Bitmap bitmap)
     {
    	 String result = null; 
    	 ByteArrayOutputStream baos = null;
    	 try 
    	 { 
    		 if (bitmap != null)
    		 { 
	    		 baos = new ByteArrayOutputStream();
	    		 bitmap.compress(Bitmap.CompressFormat.JPEG, 100, baos);
	    		 baos.flush(); baos.close(); byte[] bitmapBytes = baos.toByteArray();
	    		 result = Base64.encodeToString(bitmapBytes, Base64.DEFAULT); 
    		 } 
    	 }
    	 catch (IOException e)
    	 { 
    		 e.printStackTrace(); 
    	 }
    	 finally 
    	 { 
    		 try
    		 { 
    			 if (baos != null)
			 	 {
    				 baos.flush();
    				 baos.close();
				 }
    		 } 
    		 catch (IOException e) 
    		 { 
    			 e.printStackTrace();
    		 } 
    	 }
    	 	UnityPlayer.UnitySendMessage("SDKCallBackObj","GetBase64",result);
    }
     
     ///检测图片尺寸
     public static boolean CheckOutImageSize(Bitmap bitmap)
     {
    	 if(bitmap == null) return false;
    	 float w = bitmap.getWidth();
    	 float h = bitmap.getHeight();
    	 float min = Math.min(w, h);
    	 if(min<50)
    	 {
    		 Log.d("test","min::"+min);
    		 return false;
    	 }
    	 float value = w / h;
    	 if(value < 1/3f || value > 67/10f)
    	 {
    		 Log.d("test","value < 1/3f || value > 67/10f::"+value );
    		 return false;
    	 }
    	 return true;
     }
}