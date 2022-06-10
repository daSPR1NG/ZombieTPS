using UnityEditor;
using UnityEngine;

namespace Khynan_Coding
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Spawner), true)]
    public class SpawnerEditor : Editor
    {
        public GameObject WhatToSpawn;
        public Transform AtPosition;

        public override void OnInspectorGUI()
        {
            #region Init
            base.OnInspectorGUI();

            Spawner spawner = (Spawner)target;

            EditorGUILayout.BeginHorizontal();
            WhatToSpawn = (GameObject)EditorGUILayout.ObjectField(WhatToSpawn, typeof(GameObject), true);
            AtPosition = (Transform)EditorGUILayout.ObjectField(AtPosition, typeof(Transform), true);
            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUI.BeginDisabledGroup(!WhatToSpawn || !AtPosition);
            SpawnEntity(WhatToSpawn, AtPosition);
            EditorGUI.EndDisabledGroup();
        }

        private void SpawnEntity(GameObject go, Transform pos)
        {
            if (GUILayout.Button(new GUIContent("Spawn Something", "Spawn something at the given position.")))
            {
                Instantiate(go, pos.position, pos.rotation);
            }
        }
    }
#endif
}