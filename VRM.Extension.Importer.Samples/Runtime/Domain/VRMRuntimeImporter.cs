using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VRM.Extension.Samples
{
    public class VRMRuntimeImporter : MonoBehaviour
    {
        public event Action<GameObject> OnLoaded;

        private VRMImporterContext _Context;

        void OnDestroy()
        {
            _Context?.Dispose();
        }

        public void Construct(VRMImporterContext context)
        {
            _Context = context;
        }

        public async Task<GameObject> LoadAsync(string filePath)
        {
            if (_Context == null)
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

                _Context.ParseGlb(bytes);
                await _Context.LoadAsyncTask();
                _Context.ShowMeshes();

                var meta = _Context.Root.GetComponent<VRMMeta>().Meta;
                _Context.Root.name = meta.Title;

                OnLoaded?.Invoke(_Context.Root);
                return _Context.Root;
            }
            else
            {
                Debug.Log("VRM file not exists.");
                return null;
            }
        }
    }
}
