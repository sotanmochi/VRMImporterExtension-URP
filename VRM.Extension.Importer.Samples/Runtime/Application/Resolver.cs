using UnityEngine;

namespace VRM.Extension.Samples
{
    public class Resolver : MonoBehaviour, IInitializableBeforeSceneLoad
    {
        [SerializeField] ShaderType _ShaderType = ShaderType.NiloToon;

        public void InitializeBeforeSceneLoad()
        {
            Debug.Log("<color=orange>Resolver.InitializeBeforeSceneLoad()</color>");

            UnityEngine.QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 60;
            UnityEngine.Application.runInBackground = true;

            // Domain
            var vrmRuntimeImporter = GameObject.FindObjectOfType<VRMRuntimeImporter>();
            switch(_ShaderType)
            {
                case ShaderType.UniversalToon:
                    vrmRuntimeImporter.Construct(typeof(VRMImporterContext_UniversalToon));
                    break;
                case ShaderType.RealToon:
                    vrmRuntimeImporter.Construct(typeof(VRMImporterContext_RealToon));
                    break;
                case ShaderType.NiloToon:
                    vrmRuntimeImporter.Construct(typeof(VRMImporterContext_NiloToon));
                    break;
                default:
                    vrmRuntimeImporter.Construct(typeof(VRMImporterContext_UniversalToon));
                    break;
            }

            var animatorControllerProvider = GameObject.FindObjectOfType<AnimatorControllerProvider>();

            // View
            var importEventTrigger = ApplicationInitializer.FindObjectOfInterface<IVRMImportEventTrigger>();
            var shaderSelectEventTrigger = ApplicationInitializer.FindObjectOfInterface<IShaderSelectEventTrigger>();

            // Presenter
            var importPresenter = GameObject.FindObjectOfType<VRMImportPresenter>();
            importPresenter.Construct(importEventTrigger, vrmRuntimeImporter, animatorControllerProvider, shaderSelectEventTrigger);
        }
    }
}