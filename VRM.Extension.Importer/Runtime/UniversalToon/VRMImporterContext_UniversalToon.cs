using UniGLTF;

namespace VRM.Extension
{
    public class VRMImporterContext_UniversalToon : VRMImporterContext
    {
        public override void ParseJson(string json, IStorage storage)
        {
            base.ParseJson(json, storage);
            SetMaterialImporter(new VRMMaterialImporter_UniversalToon(this, VRM.materialProperties));
        }
    }
}
