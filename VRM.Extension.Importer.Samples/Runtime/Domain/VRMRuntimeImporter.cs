using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VRM.Extension.Samples
{
    public class VRMRuntimeImporter : MonoBehaviour
    {
        public event Action<GameObject> OnLoaded;

        private Type _ContextType;

        public void Construct(Type contextType)
        {
            if (contextType.IsSubclassOf(typeof(VRMImporterContext)))
            {
                _ContextType = contextType;
                Debug.Log("Importer Context Type: " + contextType);
            }
        }

        public async Task<GameObject> LoadAsync(string filePath)
        {
            if (_ContextType == null)
            {
                Debug.Log("VRMRumtimeImporter has not initialized.");
                return null;
            }

            if (File.Exists(filePath))
            {
                byte[] bytes;
                using (FileStream stream = File.Open(filePath, FileMode.Open))
                {
                    bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                }

                VRMImporterContext context = (VRMImporterContext)Activator.CreateInstance(_ContextType);

                context.ParseGlb(bytes);
                await context.LoadAsyncTask();
                context.ShowMeshes();

                var meta = context.Root.GetComponent<VRMMeta>().Meta;
                context.Root.name = meta.Title;

                OnLoaded?.Invoke(context.Root);
                return context.Root;
            }
            else
            {
                Debug.Log("VRM file not exists.");
                return null;
            }
        }
    }
}
