using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class AreaLightTextureManager : MonoBehaviour
{

    private static AreaLightTextureManager s_Instance;

    private Dictionary<string, Texture2D> m_Textures = new Dictionary<string, Texture2D>();


    public static AreaLightTextureManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = (AreaLightTextureManager) FindObjectOfType(typeof(AreaLightTextureManager));
            }
            return s_Instance;
        }
    }

    public static Dictionary<string, Texture2D> GetTextures()
    {
        Debug.Log("Textures initialize");
        AreaLightTextureManager instance = Instance;
        return instance == null ? new Dictionary<string, Texture2D>() : instance.m_Textures;
    }

    public Texture2D GetTexture(string texName) {
        if (m_Textures.ContainsKey(texName))
        {
            return m_Textures[texName];
        }
        else
        {
            if (!m_Textures.ContainsKey("White")) {
                m_Textures.Add("White", Texture2D.whiteTexture);
            }
            return m_Textures["White"];
        }
    }

    public Texture2D AddTexture(string texName) {
        if (m_Textures.ContainsKey(texName)) {
            return m_Textures[texName];
        }
        Texture2D filteredLightTexture;
        BinaryReader reader = new BinaryReader(File.Open(texName, FileMode.Open, FileAccess.Read));

        reader.BaseStream.Seek(12, SeekOrigin.Begin);
        int texHeight = reader.ReadInt32();

        reader.BaseStream.Seek(16, SeekOrigin.Begin);
        int texWidth = reader.ReadInt32();

        reader.BaseStream.Seek(28, SeekOrigin.Begin);
        int mipmapCount = reader.ReadInt32();

        int offset = 128;
        int size = 1 << (mipmapCount - 1);

        filteredLightTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, true);
        filteredLightTexture.wrapMode = TextureWrapMode.Clamp;
        for (int mIndex = 0; mIndex < mipmapCount; mIndex++)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] tmpBytes = reader.ReadBytes(size * size * 4);
            Texture2D tempTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tempTex.LoadRawTextureData(tmpBytes);
            tempTex.Apply();

            Graphics.CopyTexture(tempTex, 0, 0, filteredLightTexture, 0, mIndex);

            offset += size * size * 4;
            size /= 2;
        }

        filteredLightTexture.Apply(false);

        m_Textures.Add(texName, filteredLightTexture);
        return filteredLightTexture; 
    }
}
