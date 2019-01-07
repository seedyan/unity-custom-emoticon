using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class SDKIOS : SDKTCI {

	[DllImport ("__Internal")]
	private static extern void _openPicture();
    [DllImport ("__Internal")]
    private static extern void _saveImageToPhotoAlbum(string readAddr);


    public override void Open()
    {
        Debug.Log("unity click open");
        _openPicture();
        base.Open();
    }

    public override void SaveTexture()
    {
        string path = Application.persistentDataPath + "/CustomEmoji/"+curTextureMd5+".png";
        Debug.Log(" 当前文件的路径："+path);
        _saveImageToPhotoAlbum(path);
    }
}
