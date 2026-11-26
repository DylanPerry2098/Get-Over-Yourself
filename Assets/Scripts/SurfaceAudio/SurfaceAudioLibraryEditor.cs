using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/**
 * Loads SurfaceAudioLibrary
 */
[CustomEditor(typeof(SurfaceAudioLibrary))]
public class SurfaceAudioLibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal Inspector fields
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate Surface Library"))
        {
            GenerateLibrary();
        }
    }

    private void GenerateLibrary()
    {
        SurfaceAudioLibrary library = (SurfaceAudioLibrary)target;

        Undo.RecordObject(library, "Generate Surface Library");

        foreach (KeyValuePair<SurfaceAudioLibrary.SurfaceType, string> entry in library.FilePathBySurface)
        {
            library.SoundsBySurface[entry.Key] = LoadClips(entry.Value, library.mustContainString);
        }
        
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();

        Debug.Log("Surface Library Generated!");
    }

    private AudioClip[] LoadClips(string folderPath, string searchTerm)
    {
        string[] guids = AssetDatabase.FindAssets(
            searchTerm + " t:AudioClip",
            new[] { folderPath }
        );
        
        AudioClip[] clips = new AudioClip[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            clips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        return clips;
    }
}