using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;


public class SDKAndroid : SDKTCI
{


    private AndroidJavaObject jo;

    private const string SDKAndroidFuncName = "GetUntiyMessage";
    public SDKAndroid()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }




    /// <summary>
    /// Unity调用对应平台的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private T SDKCall<T>(string method, params object[] param)
    {
        try
        {
            return jo.Call<T>(method, param);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return default(T);
    }

    /// <summary>
    /// Unity调用对应平台的方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="param"></param>
    private void SDKCall(string method, params object[] param)
    {
        try
        {
            jo.Call(method, param);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public override void Open()
    {
        SDKCall(SDKAndroidFuncName,100,null);
        base.Open();
    }

    public override void SaveTexture()
    {
        if (curTexture != null)
            SaveImage(curTexture);
        base.SaveTexture();
    }
    //截屏并保存
    void SaveImage(Texture2D tex)
    {
        string imageName = getLongTime()+ ".png";
        Debug.Log("imageName:" + imageName);
        //图片大小  
        byte[] byt = tex.EncodeToPNG();
        string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android")) + "/DCIM/Camera/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += imageName;
        Debug.Log("path:" + path);
        File.WriteAllBytes(path  , byt);
        if (path == null) return;
            SDKCall(SDKAndroidFuncName, 101, path);
    }


    long getLongTime()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
        return timeStamp;
    }
}
