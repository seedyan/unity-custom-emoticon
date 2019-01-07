using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class DllTest
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOpenFileName1([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }
}
public class SDKDefault : SDKTCI
{

    private bool flag = false;

    public void OpenDocument()
    {


    }


    public SDKDefault()
    {
    }


    public override void Open()
    {

        OpenFileDialog od = new OpenFileDialog();
        od.Title = "请选择图片";
        od.Multiselect = false;
        od.Filter = "图片文件(*.jpg,*.png,*.bmp,*.gif)|*.jpg;*.png;*.bmp;*.gif";
        if (od.ShowDialog() == DialogResult.OK)
        {
            //Debug.Log(od.FileName);
            SDKCallBack callback = GameObject.Find("SDKCallBackObj").GetComponent<SDKCallBack>();
            callback.PcLoadImage(od.FileName);
        }

        return;
        //OpenFileName ofn = new OpenFileName();

        //ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "图片文件(*.jpg,*.png,*.bmp,*.gif)|*.jpg;*.png;*.bmp;*.gif";

        //ofn.file = new string(new char[256]);

        //ofn.maxFile = ofn.file.Length;

        //ofn.fileTitle = new string(new char[64]);

        //ofn.maxFileTitle = ofn.fileTitle.Length;

        //ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径

        ////注意 一下项目不一定要全选 但是0x00000008项不要缺少
        //ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;    //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        //if (DllTest.GetOpenFileName(ofn))
        //{


        //    Debug.Log("Selected file with full path: {0}" + ofn.file);
        //    flag = true;
        //}
        //if (flag == true)
        //{
        //    System.Diagnostics.Process.Start(ofn.file);

        //}
        //base.Open();
    }

}
