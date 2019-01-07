using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Util {

    public static void SaveTextureToPng(Texture2D texture, string pngName,string path)
    {
        byte[] data = texture.EncodeToPNG();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        FileStream file = File.Open(path + pngName, FileMode.Create);

        using (BinaryWriter bw = new BinaryWriter(file))
        {
            bw.Write(data);
            file.Close();
        }
    }
}
