using UnityEngine;

namespace VRM.Extension.Samples
{
    public class VRMImportPresenter : MonoBehaviour
    {
        private IVRMImportEventTrigger _ImportEventTrigger;
        private VRMRuntimeImporter _RuntimeImporter;
        private AnimatorControllerProvider _AnimatorControllerProvider;
        private IShaderSelectEventTrigger _ShaderSelectEventTrigger;

        public void Construct(
            IVRMImportEventTrigger eventTrigger, 
            VRMRuntimeImporter runtimeImporter, 
            AnimatorControllerProvider animatorControllerProvider = null,
            IShaderSelectEventTrigger shaderSelectEventTrigger = null)
        {
            _ImportEventTrigger = eventTrigger;
            _RuntimeImporter = runtimeImporter;
            _AnimatorControllerProvider = animatorControllerProvider;
            _ShaderSelectEventTrigger = shaderSelectEventTrigger;
        }

        private void Awake()
        {
            _ImportEventTrigger.OnTriggerVRMImportEvent += async(filePath) =>
            {
                // Debug.Log("OnTrigger@Presenter: " + filePath);
                var vrm = await _RuntimeImporter.LoadAsync(filePath);
                vrm.transform.SetParent(_RuntimeImporter.transform);
                // Debug.Log("OnLoaded");
            };

            _RuntimeImporter.OnLoaded += (vrm) =>
            {
                // Debug.Log("OnLoaded callback");
                // vrm.transform.SetParent(_RuntimeImporter.transform);
                Animator animator = vrm.GetComponent<Animator>();
                if (animator != null && _AnimatorControllerProvider != null)
                {
                    animator.runtimeAnimatorController = _AnimatorControllerProvider.GetAnimatorController();
                }
            };

            if (_ShaderSelectEventTrigger != null)
            {
                _ShaderSelectEventTrigger.OnTriggerShaderSelectEvent += (shaderTypeName) =>
                {
                    switch(shaderTypeName)
                    {
                        case "UniversalToon":
                            _RuntimeImporter.Construct(typeof(VRMImporterContext_UniversalToon));
                            break;
                        case "RealToon":
                            _RuntimeImporter.Construct(typeof(VRMImporterContext_RealToon));
                            break;
                        case "NiloToon":
                            _RuntimeImporter.Construct(typeof(VRMImporterContext_NiloToon));
                            break;
                        default:
                            _RuntimeImporter.Construct(typeof(VRMImporterContext_UniversalToon));
                            break;
                    }
                };
            }
        }
    }
}