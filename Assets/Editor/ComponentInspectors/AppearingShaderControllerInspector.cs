using UnityEditor;
using SandboxMoba.Shaders;
using UnityEngine;

namespace SandboxMoba.Editor.ComponentInspectors
{
    [CustomEditor(typeof(AppearingShaderController))]
    [CanEditMultipleObjects]
    public class AppearingShaderControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            AppearingShaderController target = (AppearingShaderController)this.target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Create and cach new materials"))
            {
                target.CacheNewMaterials();
            }
            if (!target.IsMaterialsCached)
                EditorGUI.BeginDisabledGroup(true);

            target.SyncInEditor = EditorGUILayout.Toggle("Sync in editor", target.SyncInEditor);

            float minBorder = target.MinBorder;
            float newMinBorder = EditorGUILayout.Slider("Min border", minBorder, 0f, 1f);
            if (newMinBorder != minBorder && target.IsMaterialsCached)
                target.MinBorder = newMinBorder;
            minBorder = newMinBorder;

            float maxBorder = target.MaxBorder;
            float newMaxBorder = EditorGUILayout.Slider("Max border", maxBorder, 0f, 1f);
            if (newMaxBorder != maxBorder && target.IsMaterialsCached)
                target.MaxBorder = newMaxBorder;
            maxBorder = newMaxBorder;
        }
    }

}
