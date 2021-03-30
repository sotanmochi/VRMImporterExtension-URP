// Copyright (c) 2021 Soichiro Sugimoto
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniGLTF;

namespace VRM.Extension
{
    public class VRMMaterialImporter_RealToon : VRMMaterialImporter
    {
        List<glTF_VRM_Material> m_materials;
        bool m_useLiteShaders;

        public VRMMaterialImporter_RealToon(ImporterContext context, List<glTF_VRM_Material> materials, bool useLiteShaders) : base(context, materials)
        {
            m_materials = materials;
            m_useLiteShaders = useLiteShaders;
        }

        public override Material CreateMaterial(int i, glTFMaterial src, bool hasVertexColor)
        {
            var currentRenderPipeline = GraphicsSettingsExtension.GetCurrentRenderPipeline();
            var renderPipelinePath = "";
            var shaderName = "Standard";

            switch (currentRenderPipeline)
            {
                case RenderPipelineType.BuiltIn:
                    renderPipelinePath = "";
                    shaderName = "Standard";
                    break;

                case RenderPipelineType.URP:
                    renderPipelinePath = "Universal Render Pipeline/";
                    shaderName = "Universal Render Pipeline/Lit";
                    break;

                case RenderPipelineType.HDRP:
                    renderPipelinePath = "HDRP/";
                    shaderName = "HDRP/Lit";
                    break;
            }

            // Debug.Log("RenderPipeline: " + renderPipelinePath);

            if(i==0 && m_materials.Count == 0)
            {
                // Dummy
                return new Material(Shader.Find(shaderName));
            }

            // Restore VRM material
            var vrmMaterial = base.CreateMaterial(i, src, hasVertexColor);
            if (vrmMaterial.shader != Shader.Find("VRM/MToon"))
            {
                return vrmMaterial;
            }

            // RealToon shader
            var shaderCategory = (currentRenderPipeline == RenderPipelineType.BuiltIn && m_useLiteShaders) ? "Lite" : "Default";
            var shaderPath = $"{renderPipelinePath}RealToon/Version 5/{shaderCategory}";

            if (currentRenderPipeline == RenderPipelineType.URP)
            {
                shaderPath = $"{shaderPath}/Default";
            }
            else if (currentRenderPipeline != RenderPipelineType.HDRP)
            {
                // [MToon Rendering Types/BlendModes]
                // Opaque = 0, Cutout = 1, Transparent = 2, TransparentWithZWrite = 3
                var renderingTypeBlendMode = (int) vrmMaterial.GetFloat(_BlendMode);
                if (renderingTypeBlendMode == 2 || renderingTypeBlendMode == 3)
                {
                    shaderPath = $"{shaderPath}/Fade Transparency";
                }
                else
                {
                    shaderPath = $"{shaderPath}/Default";
                }
            }

            return CreateRealToonMaterial(vrmMaterial, shaderPath);
        }

        /// <summary>
        // Convert MToon to RealToon
        /// </summary>
        /// <param name="vrmMaterial"></param>
        /// <param name="shaderPath"></param>
        /// <returns></returns>
        public Material CreateRealToonMaterial(Material vrmMaterial, string shaderPath)
        {
            var material = new Material(Shader.Find(shaderPath));
            material.name = vrmMaterial.name;
            material.renderQueue = vrmMaterial.renderQueue;

            var mainTexture = vrmMaterial.GetTexture(_MainTexture);
            var mainColor = vrmMaterial.GetColor(_Color);
            var shadeColor = vrmMaterial.GetColor(_ShadeColor);
            var alphaCutoff = vrmMaterial.GetFloat(_AlphaCutoff);
            var normalMapTexture = vrmMaterial.GetTexture(_BumpMap);
            var normalMapIntensity = vrmMaterial.GetFloat(_BumpScale);
            var emissionTexture = vrmMaterial.GetTexture(_EmissionMap);
            var emissionColor = vrmMaterial.GetColor(_EmissionColor);
            var matcapTexture = vrmMaterial.GetTexture(_SphereAdd);
            var rimLightColor = vrmMaterial.GetColor(_RimColor);
            var outlineWidth = vrmMaterial.GetFloat(_OutlineWidth);
            var outlineWidthTexture = vrmMaterial.GetTexture(_OutlineWidthTexture);
            var outlineColor = vrmMaterial.GetColor(_OutlineColor);

            material.EnableKeyword(_EnableOutlineKeyword);
            material.SetFloat(_EnableOutline, 1);
            material.EnableKeyword(_EnableNormalMapKeyword);
            material.SetFloat(_EnableNormalMap, 1);
            material.EnableKeyword(_EnableMatCapKeyword);
            material.SetFloat(_EnableMatCap, 1);
            material.EnableKeyword(_EnableGlossKeyword);
            material.SetFloat(_EnableGloss, 1);
            material.EnableKeyword(_EnableGlossTextureKeyword);
            material.SetFloat(_EnableGlossTexture, 1);
            material.EnableKeyword(_EnableRimLightKeyword);
            material.SetFloat(_EnableRimLight, 1);

            material.SetTexture(_MainTexture, mainTexture);
            material.SetColor(_MainColor, mainColor);
            material.SetColor(_OverallShadowColor, shadeColor);
            material.SetFloat(_SelfShadowThreshold, 1);
            material.SetFloat(_AlphaCutout, alphaCutoff);
            material.SetTexture(_NormalMap, normalMapTexture);
            material.SetFloat(_NormalMapIntensity, normalMapIntensity);
            material.SetTexture(_GlossTexture, emissionTexture);
            material.SetColor(_GlossColor, emissionColor);
            material.SetTexture(_MatCap, matcapTexture);
            material.SetFloat(_MatCapSpecMode, 1); // Specular Mode: ON
            material.SetFloat(_MatCapSpecPower, 5);
            material.SetColor(_RimLightColor, rimLightColor);
            material.SetFloat(_OutlineWidth, outlineWidth);
            material.SetColor(_OutlineColor, outlineColor);
            material.SetTexture(_OutlitWidthControl, outlineWidthTexture);

            // CullMode: Off = 0, Front = 1, Back = 2
            var cullMode = (int) vrmMaterial.GetFloat(_CullMode);
            material.SetInt(_Culling, cullMode);

            // BlendMode: Opaque = 0, Cutout = 1, Transparent = 2, TransparentWithZWrite = 3
            var blendMode = (int) vrmMaterial.GetFloat(_BlendMode);
            if (blendMode != 0)
            {
                material.EnableKeyword(_EnableTransmodeKeyword);
                material.SetFloat(_EnableTransmode, 1);
                material.EnableKeyword(_EnableCutoutKeyword);
                material.SetFloat(_EnableCutout, 1);
            }

            return material;
        }

        // Shared shader properties
        private static readonly int _MainTexture = Shader.PropertyToID("_MainTex");
        private static readonly int _OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int _OutlineColor = Shader.PropertyToID("_OutlineColor");

        // MToon shader properties
        private static readonly int _BlendMode = Shader.PropertyToID("_BlendMode");
        private static readonly int _CullMode = Shader.PropertyToID("_CullMode");
        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _AlphaCutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int _ShadeColor = Shader.PropertyToID("_ShadeColor");
        private static readonly int _BumpMap = Shader.PropertyToID("_BumpMap");
        private static readonly int _BumpScale = Shader.PropertyToID("_BumpScale");
        private static readonly int _EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _SphereAdd = Shader.PropertyToID("_SphereAdd");
        private static readonly int _RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int _OutlineWidthTexture = Shader.PropertyToID("_OutlineWidthTexture");

        // RealToon shader properties
        private static readonly int _Culling = Shader.PropertyToID("_Culling");
        private static readonly int _MainColor = Shader.PropertyToID("_MainColor");
        private static readonly int _AlphaCutout = Shader.PropertyToID("_Cutout");
        private static readonly int _OverallShadowColor = Shader.PropertyToID("_OverallShadowColor");
        private static readonly int _SelfShadowThreshold = Shader.PropertyToID("_SelfShadowThreshold");
        private static readonly int _NormalMap = Shader.PropertyToID("_NormalMap");
        private static readonly int _NormalMapIntensity = Shader.PropertyToID("_NormalMapIntensity");
        private static readonly int _GlossTexture = Shader.PropertyToID("_GlossTexture");
        private static readonly int _GlossColor = Shader.PropertyToID("_GlossColor");
        private static readonly int _MatCap = Shader.PropertyToID("_MCap");
        private static readonly int _MatCapSpecMode = Shader.PropertyToID("_SPECMODE");
        private static readonly int _MatCapSpecPower = Shader.PropertyToID("_SPECIN");
        private static readonly int _RimLightColor = Shader.PropertyToID("_RimLightColor");
        private static readonly int _OutlitWidthControl = Shader.PropertyToID("_OutlitWidthControl");

        private static readonly string _EnableTransmodeKeyword = "N_F_TRANS_ON";
        private static readonly int _EnableTransmode = Shader.PropertyToID("_TRANSMODE");
        private static readonly string _EnableCutoutKeyword = "N_F_CO_ON";
        private static readonly int _EnableCutout = Shader.PropertyToID("_N_F_CO");
        private static readonly string _EnableOutlineKeyword = "N_F_O_ON";
        private static readonly int _EnableOutline = Shader.PropertyToID("_N_F_O");
        private static readonly string _EnableNormalMapKeyword = "N_F_NM_ON";
        private static readonly int _EnableNormalMap = Shader.PropertyToID("_N_F_NM");
        private static readonly string _EnableMatCapKeyword = "N_F_MC_ON";
        private static readonly int _EnableMatCap = Shader.PropertyToID("_N_F_MC");
        private static readonly string _EnableGlossKeyword = "N_F_GLO_ON";
        private static readonly int _EnableGloss = Shader.PropertyToID("_N_F_GLO");
        private static readonly string _EnableGlossTextureKeyword = "N_F_GLOT_ON";
        private static readonly int _EnableGlossTexture = Shader.PropertyToID("_N_F_GLOT");
        private static readonly string _EnableRimLightKeyword = "N_F_RL_ON";
        private static readonly int _EnableRimLight = Shader.PropertyToID("_N_F_RL");
    }
}
