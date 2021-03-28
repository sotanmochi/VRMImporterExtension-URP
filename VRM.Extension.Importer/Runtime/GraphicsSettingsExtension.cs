namespace UnityEngine.Rendering
{
    public enum RenderPipelineType
    {
        BuiltIn,
        URP,
        HDRP,
        Unknown
    }

    public static class GraphicsSettingsExtension
    {
        public static RenderPipelineType GetCurrentRenderPipeline()
        {
            if (GraphicsSettings.currentRenderPipeline)
            {
                string pipelineAssetName = GraphicsSettings.currentRenderPipeline.GetType().ToString();

                if (pipelineAssetName.Contains("HDRenderPipelineAsset"))
                {
                    return RenderPipelineType.HDRP;
                }
                else if (pipelineAssetName.Contains("UniversalRenderPipelineAsset"))
                {
                    return RenderPipelineType.URP;
                }
                else
                {
                    return RenderPipelineType.Unknown;
                }
            }
            else
            {
                return RenderPipelineType.BuiltIn;
            }
        }
    }
}