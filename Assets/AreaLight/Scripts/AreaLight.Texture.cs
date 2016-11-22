using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Text;



public partial class AreaLight : MonoBehaviour
{
    [Header("Texture")]
    public Texture2D m_LightTexture;
    private Texture2D m_FilterLightTexture;
    public string m_TextureFile;
    private bool initTexture = false;
    private int m_texHeight;
    private int m_texWidth;

    void InitLightTexture()
    {

        //#TODO save this texture in a struct
        if (initTexture) {
            Debug.Log("Init Again");
            return;
        }

        initTexture = true;
        BinaryReader reader = new BinaryReader(File.Open(m_TextureFile, FileMode.Open, FileAccess.Read));

        reader.BaseStream.Seek(12, SeekOrigin.Begin);
        m_texHeight = reader.ReadInt32();

        reader.BaseStream.Seek(16, SeekOrigin.Begin);
        m_texWidth = reader.ReadInt32();

        reader.BaseStream.Seek(28, SeekOrigin.Begin);
        int mipmapCount = reader.ReadInt32();

        int offset = 128;
        int size = 1 << (mipmapCount - 1);

        m_FilterLightTexture = new Texture2D(m_texWidth, m_texHeight, TextureFormat.RGBA32, true);
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
