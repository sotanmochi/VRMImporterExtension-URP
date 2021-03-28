using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRM.Extension.Samples
{
    public class VRMImportView : MonoBehaviour, IVRMImportEventTrigger
    {
        [SerializeField] Button _Import;

        public event Action<string> OnTriggerVRMImportEvent;

        void Awake()
        {
            _Import.onClick.AddListener(() => 
            {
#if UNITY_STANDALONE_WIN
                var path = VRM.Samples.FileDialogForWindows.FileDialog("open VRM", ".vrm");
#elif UNITY_EDITOR
                var path = UnityEditor.EditorUtility.OpenFilePanel("Open VRM", "", "vrm");
#else
                var path = Application.dataPath + "/default.vrm";
#endif
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                OnTriggerVRMImportEvent?.Invoke(path);
            });
        }
    }
}