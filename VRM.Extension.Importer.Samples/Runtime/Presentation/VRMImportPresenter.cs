using UnityEngine;

namespace VRM.Extension.Samples
{
    public class VRMImportPresenter : MonoBehaviour
    {
        private IVRMImportEventTrigger _ImportEventTrigger;
        private VRMRuntimeImporter _RuntimeImporter;

        public void Construct(IVRMImportEventTrigger eventTrigger, VRMRuntimeImporter runtimeImporter)
        {
            _ImportEventTrigger = eventTrigger;
            _RuntimeImporter = runtimeImporter;
        }

        private void Awake()
        {
            _ImportEventTrigger.OnTriggerVRMImportEvent += async(filePath) =>
            {
                Debug.Log("OnTrigger@Presenter: " + filePath);
                var vrm = await _RuntimeImporter.LoadAsync(filePath);
                vrm.transform.SetParent(_RuntimeImporter.transform);
                Debug.Log("OnLoaded");
            };

            _RuntimeImporter.OnLoaded += (vrm) =>
            {
                Debug.Log("OnLoaded callback");
                // vrm.transform.SetParent(_RuntimeImporter.transform);
            };
        }
    }
}