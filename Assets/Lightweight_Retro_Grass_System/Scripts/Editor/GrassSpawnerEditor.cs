using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace grassGeneration
{
    [CustomEditor(typeof(GrassSpawner))]
    public class GrassSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GrassSpawner spawner = (GrassSpawner)target;

            DrawDefaultInspector();
            if (true)
            {

            }


            if (GUILayout.Button("Generate"))
            {
                spawner.GenerateGrass();
            }

            if (GUILayout.Button("Clear"))
            {
                spawner.ClearGrass();
            }
        }
    }
}

