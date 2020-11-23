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

            float newMinBorder = EditorGUILayout.Slider("Min border", target.MinBorder, 0f, 1f);
            if (newMinBorder != target.MinBorder && target.IsMaterialsCached)
                target.MinBorder = newMinBorder;

            float newMaxBorder = EditorGUILayout.Slider("Max border", target.MaxBorder, 0f, 1f);
            if (newMaxBorder != target.MaxBorder && target.IsMaterialsCached)
                target.MaxBorder = newMaxBorder;

            bool newIsAppearing = EditorGUILayout.Toggle("Is Appearing", target.IsAppearing);
            if (newIsAppearing != target.IsAppearing && target.IsMaterialsCached)
                target.IsAppearing = newIsAppearing;

            bool newIsDisappeared = EditorGUILayout.Toggle("Is Disappeared", target.IsDisappeared);
            if (newIsDisappeared != target.IsDisappeared && target.IsMaterialsCached)
                target.IsDisappeared = newIsDisappeared;

        }
    }

}
