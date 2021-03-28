// Copyright (c) 2021 Soichiro Sugimoto
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniGLTF;

namespace VRM.Extension
{
    public class VRMMaterialImporter_NiloToon : VRMMaterialImporter
    {
        List<glTF_VRM_Material> m_materials;

        public VRMMaterialImporter_NiloToon(ImporterContext context, List<glTF_VRM_Material> materials) : base(context, materials)
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

            if((i==0 && m_materials.Count == 0) || (currentRenderPipeline != RenderPipelineType.URP))
            {
                // Dummy
                return new Material(Shader.Find(shaderName));
            }

            // Debug.Log("Restore VRM Material.");

            // Restore VRM material
            var vrmMaterial = base.CreateMaterial(i, src, hasVertexColor);
            if (vrmMaterial.shader != Shader.Find("VRM/MToon"))
            {
                return vrmMaterial;
            }

            // NiloToon shader
            var shaderPath = $"SimpleURPToonLitExample(With Outline)";

            return CreateNiloToonMaterial(vrmMaterial, shaderPath);
        }

        /// <summary>
        /// Convert MToon to NiloToon
        /// </summary>
        /// <param name="vrmMaterial"></param>
        /// <param name="shaderPath"></param>
        /// <returns></returns>
        public Material CreateNiloToonMaterial(Material vrmMaterial, string shaderPath)
        {
            var material = new Material(Shader.Find(shaderPath));
            material.name = vrmMaterial.name;
            material.renderQueue = vrmMaterial.renderQueue;
            material.doubleSidedGI = vrmMaterial.doubleSidedGI;

            var mainTexture = vrmMaterial.GetTexture(_MainTexture);
            var mainColor = vrmMaterial.GetColor(_MainColor);
            var shadeShift = vrmMaterial.GetFloat(_ShadeShift);
            var shadeToony = vrmMaterial.GetFloat(_ShadeToony);
            var alphaCutoff = vrmMaterial.GetFloat(_AlphaCutoff);
            var emissionTexture = vrmMaterial.GetTexture(_EmissionMap);
            var emissionColor = vrmMaterial.GetColor(_EmissionColor);
            var outlineWidth = vrmMaterial.GetFloat(_OutlineWidth);
            var outlineColor = vrmMaterial.GetColor(_OutlineColor);

            material.SetTexture(_BaseMap, mainTexture);
            material.SetColor(_BaseColor, mainColor);
            material.SetFloat(_CelShadeMidPoint, shadeShift);
            material.SetFloat(_CelShadeSoftness, 1.0f - shadeToony);
            material.SetFloat(_AlphaCutoff, alphaCutoff);
            material.SetTexture(_EmissionMap, emissionTexture);
            material.SetColor(_EmissionColor, emissionColor);
            material.SetFloat(_OutlineWidth, outlineWidth);
            material.SetColor(_OutlineColor, outlineColor);

            var blendMode = (int) vrmMaterial.GetFloat(_BlendMode);
            var cullMode = (int) vrmMaterial.GetFloat(_CullMode);
            if (blendMode != 0)
            {
                material.EnableKeyword("_UseAlphaClipping");
                material.SetFloat(_UseAlphaClipping, 1);
            }

            material.SetFloat(_DirectLightMultiplier, 1.0f);
            material.SetFloat(_DirectLightMultiplier, 1.0f);

            return material;
        }

        // Shared shader properties
        private static readonly int _AlphaCutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int _EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int _OutlineColor = Shader.PropertyToID("_OutlineColor");

        // MToon shader properties
        private static readonly int _BlendMode = Shader.PropertyToID("_BlendMode");
        private static readonly int _CullMode = Shader.PropertyToID("_CullMode");
        private static readonly int _MainTexture = Shader.PropertyToID("_MainTex");
        private static readonly int _MainColor = Shader.PropertyToID("_Color");
        private static readonly int _ShadeShift = Shader.PropertyToID("_ShadeShift");
        private static readonly int _ShadeToony = Shader.PropertyToID("_ShadeToony");

        // NiloToon shader properties
        private static readonly int _DirectLightMultiplier = Shader.PropertyToID("_DirectLightMultiplier");
        private static readonly int _UseAlphaClipping = Shader.PropertyToID("_UseAlphaClipping");
        private static readonly int _BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int _BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int _CelShadeMidPoint = Shader.PropertyToID("_CelShadeMidPoint");
        private static readonly int _CelShadeSoftness = Shader.PropertyToID("_CelShadeSoftness");
    }
}
