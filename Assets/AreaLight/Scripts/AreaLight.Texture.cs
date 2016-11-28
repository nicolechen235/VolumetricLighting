﻿using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Text;



public partial class AreaLight : MonoBehaviour
{
    [Header("Texture")]
    public bool m_UseTextureLight;
    public Texture2D m_LightTexture;
    public Texture2D m_FilterLightTexture;
    public string m_TextureFile;

    void InitLightTexture()
    {
        if (!m_UseTextureLight) {
            Debug.LogWarning("Did not use the texture light for " + this.name);
            m_FilterLightTexture = null;
            return;
        }

        m_FilterLightTexture = AreaLightTextureManager.Instance.AddTexture(m_TextureFile);
    }
}
