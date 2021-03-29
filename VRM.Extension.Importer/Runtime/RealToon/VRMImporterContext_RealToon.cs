using UniGLTF;

namespace VRM.Extension
{
    public class VRMImporterContext_RealToon : VRMImporterContext
    {
        bool m_useLiteShaders;

        public VRMImporterContext_RealToon()
        {
            m_useLiteShaders = false;
        }

        public VRMImporterContext_RealToon(bool useLiteShaders)
        {
            m_useLiteShaders = useLiteShaders;
        }

        public override void ParseJson(string json, IStorage storage)
        {
            base.ParseJson(json, storage);
            SetMaterialImporter(new VRMMaterialImporter_RealToon(this, VRM.materialProperties, m_useLiteShaders));
        }
    }
}
