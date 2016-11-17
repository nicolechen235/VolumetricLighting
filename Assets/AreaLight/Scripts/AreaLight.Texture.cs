using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Text;



public partial class AreaLight: MonoBehaviour
{
    [Header("Texture")]
    public Texture2D m_LightTexture;
    public Texture2D m_FilterLightTexture;

    private static bool initTexture = false;
    void InitLightTexture()
    {

        //#TODO save this texture in a struct
        if (initTexture) {
            Debug.Log("Init Again");
            return;
        }

        initTexture = true;
        BinaryReader reader = new BinaryReader(File.Open("o2.dds", FileMode.Open, FileAccess.Read));

        int offset = 128;
        int mipmapCount = 12;
        int size = 1 << (mipmapCount - 1);

        m_FilterLightTexture = new Texture2D(2048, 2048, TextureFormat.RGBA32, true);
        m_FilterLightTexture.wrapMode = TextureWrapMode.Clamp;

        for (int mIndex = 0; mIndex < mipmapCount; mIndex++) {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] tmpBytes = reader.ReadBytes(size * size * 4);
            Texture2D tempTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tempTex.LoadRawTextureData(tmpBytes);
            tempTex.Apply();

            Graphics.CopyTexture(tempTex, 0, 0, m_FilterLightTexture, 0, mIndex);

            offset += size * size * 4;
            size /= 2;
        }
        
        m_FilterLightTexture.Apply(false);

    }
}
