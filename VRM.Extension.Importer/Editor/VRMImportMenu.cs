#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UniGLTF;

namespace VRM.Extension.Editor
{
    public static class VRMImporterMenu
    {
        [MenuItem(VRMVersion.MENU + "/Import using UniversalToon", priority = 2)]
        static void ImportWithUniversalToon()
        {
            ImportFromMenu(new VRMImporterContext_UniversalToon());
        }

        [MenuItem(VRMVersion.MENU + "/Import using RealToon", priority = 2)]
        static void ImportWithRealToon()
        {
            ImportFromMenu(new VRMImporterContext_RealToon());
        }

        [MenuItem(VRMVersion.MENU + "/Import using NiloToon", priority = 2)]
        static void ImportWithNiloToon()
        {
            ImportFromMenu(new VRMImporterContext_NiloToon());
        }

        static void ImportFromMenu(VRMImporterContext context)
        {
            var path = EditorUtility.OpenFilePanel("open vrm", "", "vrm");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (Application.isPlaying)
            {
                // load into scene
                context.Load(path);
                context.ShowMeshes();
                context.EnableUpdateWhenOffscreen();
                Selection.activeGameObject = context.Root;
            }
            else
            {
                if (path.StartsWithUnityAssetPath())
                {
                    Debug.LogWarningFormat("disallow import from folder under the Assets");
                    return;
                }

                var assetPath = EditorUtility.SaveFilePanel("save prefab", "Assets", Path.GetFileNameWithoutExtension(path), "prefab");
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                if (!assetPath.StartsWithUnityAssetPath())
                {
                    Debug.LogWarningFormat("out of asset path: {0}", assetPath);
                    return;
                }

                // import as asset
                var prefabPath = UnityPath.FromUnityPath(assetPath);
                context.ParseGlb(File.ReadAllBytes(path));
                context.ExtractImages(prefabPath);

                EditorApplication.delayCall += () =>
                {
                    //
                    // after textures imported
                    //
                    context.Load();
                    context.SaveAsAsset(prefabPath);
                    context.EditorDestroyRoot();
                };
            }
        }
    }
}

#endif