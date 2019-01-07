/// create by ydl

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SDKCallBack : MonoBehaviour
{
    public RawImage orginImage;

    public RawImage image1000;

    public RawImage image100;

    private string customEmojiFilePath = null;

    private string filePath = null;

    string md5Str = null;

    void Awake()
    {

        customEmojiFilePath = "file://" + Application.persistentDataPath + "/CustomEmoji/";
        filePath = Application.persistentDataPath + "/CustomEmoji/";

        if(RuntimePlatform.WindowsEditor == Application.platform)
         EditorTest();

    }

    void Start()
    {
       
    }

    public void EditorTest()
    {
        return;
        string path = "file://" + "C:/Users/yandalong.seedyan/Desktop/IMG_2672.JPG";
        string temp = "C:/Users/yandalong.seedyan/Desktop/IMG_2672.JPG";
        string temp1 = temp.Replace("/", "\\");
        Debug.Log(temp1);
        byte[] imageByte = File.ReadAllBytes(temp1);
        md5Str = GetMd5(imageByte);
        StartCoroutine(LoadTexture(path));
    }

    public void GetBase64(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            Debug.Log(content);
            Texture2D orginTexture = Base64ToTex(content);
            if(orginTexture==null) return;
            Texture2D texture1000 = CompressTexture(orginTexture, 1000);

            image1000.texture = texture1000;
            image1000.SetNativeSize();
            Texture2D texture100 = CompressTexture(orginTexture, 100);
            image100.texture = texture100;
            image100.SetNativeSize();
            //  Debug.Log();

            SDKTCI.Instance.curTexture = texture1000;
            SDKTCI.Instance.curTextureMd5 = md5Str;
            Debug.Log(customEmojiFilePath);
            Util.SaveTextureToPng(texture1000, md5Str + ".png", filePath);
            Util.SaveTextureToPng(texture100, md5Str + "_small.png", filePath);
        }
    }

    public void MessageCallBack(string str)
    {
        Debug.Log("unity message:" + str);
        //注解1
        string path = "file://" + Application.persistentDataPath + "/CustomEmoji/" + str;
        byte[] imageByte = File.ReadAllBytes(filePath + str);
       
        Debug.Log(md5Str);
        //在Android插件中通知Unity开始去指定路径中找图片资源
        StartCoroutine(LoadTexture(path));
    }

    IEnumerator LoadTexture(string path)
    {
        using (WWW www = new WWW(path))
        {
            yield return www;

            if (www.isDone && www.error == null)
            {
                //orginImage.texture = www.texture;
                //orginImage.SetNativeSize();
                Texture2D texture1000 = CompressTexture(www.texture, 1000);

                image1000.texture = texture1000;
                image1000.SetNativeSize();
                Texture2D texture100 = CompressTexture(www.texture, 100);
                image100.texture = texture100;
                image100.SetNativeSize();

                //Resources.UnloadAsset(www.texture);
                Destroy(www.texture);
              //  Debug.Log();

                SDKTCI.Instance.curTexture = texture1000;
                Debug.Log(customEmojiFilePath);
                Util.SaveTextureToPng(texture1000, md5Str+".png", filePath);
                Util.SaveTextureToPng(texture100, md5Str+"_small.png", filePath);
                //www.
                //Debug.Log("11111111:" + www.texture.name);
            }
            else
            {
                Debug.Log("www.error:" + www.error);
            }
        }
    }


    Texture2D CompressTexture(Texture2D texture, int targetLenth)
    {
        float orginW = texture.width;
        float orginH = texture.height;
        Debug.Log("压缩之前的图" + orginW + "  " + orginH);
        

        float maxLenth = Mathf.Max(orginW, orginH);

        float radio = targetLenth / maxLenth;

        int targetW = 0;
        int targetH = 0;
        if (radio >= 1)
        {
            targetW = (int)orginW;
            targetH = (int)orginH;
        }
        else
        {
            targetW = (int)(orginW * radio);
            targetH = (int)(orginH * radio);

        }
        Debug.Log("压缩之后的图" + targetW + "  " + targetH);

        //Texture2D txe2d = new Texture2D(targetW, targetH, TextureFormat.ARGB4444, false);
        //www.LoadImageIntoTexture(txe2d);



        return ScaleTexture(texture, targetW,targetH);
    }

    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        } 
        result.Apply(); 
        return result;
    }

    string GetMd5(byte[] imageByte)
    {
      
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(imageByte);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            stringBuilder.Append(Convert.ToString(result[i],16));
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// base64编码文本转换成图片
    /// </summary>
    public Texture2D Base64ToTex(string recordBase64String)
    {
        Texture2D tex2D = null;
        string base64 = recordBase64String;
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            md5Str = GetMd5(bytes);
            tex2D = new Texture2D(100, 100,TextureFormat.RGBA4444,false);
            tex2D.LoadImage(bytes);
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex);
        }
        return tex2D;
    }

    public void SavePictureCallBack(string content)
    {
        if(string.IsNullOrEmpty(content))
        {
            Debug.Log("图片保存成功");
        }
        else
        {
            Debug.Log("图片保存失败");
        }
    }

    public void CommonTip(string content)
    {
        Debug.Log(content);
    }


    public void PcLoadImage(string path)
    {
        string path1 = "file://" + path;
        string temp1 = path.Replace("/", "\\");
        Debug.Log(temp1);
        byte[] imageByte = File.ReadAllBytes(temp1);
        md5Str = GetMd5(imageByte);

        StartCoroutine(LoadTexture(path1));//加载图片到panle
    }
}
