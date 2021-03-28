using UniGLTF;

namespace VRM.Extension
{
    public class VRMImporterContext_NiloToon : VRMImporterContext
    {
        public override void ParseJson(string json, IStorage storage)
        {
            base.ParseJson(json, storage);
            SetMaterialImporter(new VRMMaterialImporter_NiloToon(this, VRM.materialProperties));
        }
    }
}
