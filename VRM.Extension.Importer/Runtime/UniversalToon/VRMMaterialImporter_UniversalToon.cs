// Copyright (c) 2021 Soichiro Sugimoto
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniGLTF;

namespace VRM.Extension
{
    public class VRMMaterialImporter_UniversalToon : VRMMaterialImporter
    {
        List<glTF_VRM_Material> m_materials;

        public VRMMaterialImporter_UniversalToon(ImporterContext context, List<glTF_VRM_Material> materials) : base(context, materials)
        {
            m_materials = materials;
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

            if(i==0 && m_materials.Count == 0 || (currentRenderPipeline != RenderPipelineType.URP))
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

            // Universal Toon Shader
            var shaderPath = $"Universal Render Pipeline/Toon";

            return CreateUniversalToonMaterial(vrmMaterial, shaderPath);
        }

        /// <summary>
        // Convert MToon to UniversalToon
        /// </summary>
        /// <param name="vrmMaterial"></param>
        /// <param name="shaderPath"></param>
        /// <returns></returns>
        public Material CreateUniversalToonMaterial(Material vrmMaterial, string shaderPath)
        {
            var material = new Material(Shader.Find(shaderPath));
            material.name = vrmMaterial.name;
            material.renderQueue = vrmMaterial.renderQueue;
            material.doubleSidedGI = vrmMaterial.doubleSidedGI;

            var mainTexture = vrmMaterial.GetTexture(_MainTexture);
            var mainColor = vrmMaterial.GetColor(_Color);
            var shadeTexture = vrmMaterial.GetTexture(_ShadeTexture);
            var shadeColor = vrmMaterial.GetColor(_ShadeColor);
            var shadeToony = vrmMaterial.GetFloat(_ShadeToony);
            var alphaCutoff = vrmMaterial.GetFloat(_AlphaCutoff);
            var normalMapTexture = vrmMaterial.GetTexture(_BumpMap);
            var bumpScale = vrmMaterial.GetFloat(_BumpScale);
            var emissionTexture = vrmMaterial.GetTexture(_EmissionMap);
            var emissionColor = vrmMaterial.GetColor(_EmissionColor);
            var matcapTexture = vrmMaterial.GetTexture(_SphereAdd);
            var rimLightColor = vrmMaterial.GetColor(_RimColor);
            var outlineWidth = vrmMaterial.GetFloat(_OutlineWidth);
            var outlineWidthTexture = vrmMaterial.GetTexture(_OutlineWidthTexture);
            var outlineColor = vrmMaterial.GetColor(_OutlineColor);

            material.SetFloat(_EnableOutline, 1);
            material.SetFloat(_EnableMatCap, 1);
            material.SetFloat(_EnableRimLight, 1);
            material.EnableKeyword(_EnableEmissionKeyword);

            material.SetTexture(_MainTexture, mainTexture);
            material.SetColor(_BaseColor, mainColor);
            material.SetTexture(_1st_ShadeMap, shadeTexture);
            material.SetColor(_1st_ShadeColor, shadeColor);
            material.SetFloat(_BaseColor_Step, 1.0f);
            material.SetFloat(_BaseShade_Feather, 1.0f - shadeToony);
            material.SetTexture(_NormalMap, normalMapTexture);
            material.SetFloat(_BumpScale, bumpScale);
            material.SetTexture(_Emissive_Tex, emissionTexture);
            material.SetColor(_Emissive_Color, emissionColor);
            material.SetTexture(_MatCap_Sampler, matcapTexture);
            material.SetColor(_RimLightColor, rimLightColor);
            material.SetFloat(_Outline_Width, outlineWidth);
            material.SetColor(_Outline_Color, outlineColor);
            material.SetTexture(_OutlineTex, outlineWidthTexture);

            // CullMode: Off = 0, Front = 1, Back = 2
            var cullMode = (int) vrmMaterial.GetFloat(_CullMode);
            material.SetInt(_CullMode, cullMode);

            // BlendMode: Opaque = 0, Cutout = 1, Transparent = 2, TransparentWithZWrite = 3
            var blendMode = (int) vrmMaterial.GetFloat(_BlendMode);
            if (blendMode != 0)
            {
                material.SetInt(_ClippingMode, 2);
                material.SetFloat(_IsBaseMapAlphaAsClippingMask, 1);
                material.SetFloat(_AlphaCutoff, alphaCutoff);
                material.DisableKeyword(ShaderDefineIS_CLIPPING_OFF);
                material.DisableKeyword(ShaderDefineIS_CLIPPING_MODE);
                material.EnableKeyword(ShaderDefineIS_CLIPPING_TRANSMODE);
                material.DisableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_NO);
                material.EnableKeyword(ShaderDefineIS_OUTLINE_CLIPPING_YES);
            }

            return material;
        }

        // Shared shader properties
        private static readonly int _CullMode = Shader.PropertyToID("_CullMode");
        private static readonly int _AlphaCutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int _MainTexture = Shader.PropertyToID("_MainTex");
        private static readonly int _BumpScale = Shader.PropertyToID("_BumpScale");

        // MToon shader properties
        private static readonly int _BlendMode = Shader.PropertyToID("_BlendMode");
        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _ShadeTexture = Shader.PropertyToID("_ShadeTexture");
        private static readonly int _ShadeColor = Shader.PropertyToID("_ShadeColor");
        private static readonly int _ShadeToony = Shader.PropertyToID("_ShadeToony");
        private static readonly int _BumpMap = Shader.PropertyToID("_BumpMap");
        private static readonly int _EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _SphereAdd = Shader.PropertyToID("_SphereAdd");
        private static readonly int _RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int _OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int _OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int _OutlineWidthTexture = Shader.PropertyToID("_OutlineWidthTexture");

        // UniversalToon shader properties
        private static readonly int _BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int _1st_ShadeMap = Shader.PropertyToID("_1st_ShadeMap");
        private static readonly int _1st_ShadeColor = Shader.PropertyToID("_1st_ShadeColor");
        private static readonly int _BaseColor_Step = Shader.PropertyToID("_BaseColor_Step");
        private static readonly int _BaseShade_Feather = Shader.PropertyToID("_BaseShade_Feather");
        private static readonly int _NormalMap = Shader.PropertyToID("_NormalMap");
        private static readonly int _Emissive_Tex = Shader.PropertyToID("_Emissive_Tex");
        private static readonly int _Emissive_Color = Shader.PropertyToID("_Emissive_Color");
        private static readonly int _MatCap_Sampler = Shader.PropertyToID("_MatCap_Sampler");
        private static readonly int _RimLightColor = Shader.PropertyToID("_RimLightColor");
        private static readonly int _Outline_Width = Shader.PropertyToID("_Outline_Width");
        private static readonly int _Outline_Color = Shader.PropertyToID("_Outline_Color");
        private static readonly int _OutlineTex = Shader.PropertyToID("_OutlineTex");

        private static readonly int _IsBaseMapAlphaAsClippingMask = Shader.PropertyToID("_IsBaseMapAlphaAsClippingMask");
        private static readonly int _ClippingMode = Shader.PropertyToID("_ClippingMode");
        private static readonly int _EnableOutline = Shader.PropertyToID("_Is_OutlineTex");
        private static readonly int _EnableMatCap = Shader.PropertyToID("_MatCap");
        private static readonly int _EnableRimLight = Shader.PropertyToID("_RimLight");
        private static readonly string _EnableEmissionKeyword = "_EMISSION";

        private static string ShaderDefineIS_OUTLINE_CLIPPING_NO = "_IS_OUTLINE_CLIPPING_NO";
        private static string ShaderDefineIS_OUTLINE_CLIPPING_YES = "_IS_OUTLINE_CLIPPING_YES";
        private static string ShaderDefineIS_CLIPPING_OFF = "_IS_CLIPPING_OFF";
        private static string ShaderDefineIS_CLIPPING_MODE = "_IS_CLIPPING_MODE";
        private static string ShaderDefineIS_CLIPPING_TRANSMODE = "_IS_CLIPPING_TRANSMODE";
    }
}
