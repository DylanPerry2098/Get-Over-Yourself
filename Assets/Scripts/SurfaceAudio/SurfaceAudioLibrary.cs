using UnityEngine;
using System.Collections.Generic;

/**
 * A ScriptableObject for storing a multiple groups of audio events based on surface type.
 * Can autoload groups of sound effects from the editor using [SurfaceAudioLibraryGenerator].
 */
[CreateAssetMenu(
    fileName = "SurfaceAudioLibrary",
    menuName = "Audio/Footstep Library"
)]
public class SurfaceAudioLibrary : ScriptableObject
{
    public enum SurfaceType
    {
        Dirt,
        Grass,
        Stone,
        Wood,
        Snow,
        Mud,
    }

    [SerializeField] public Dictionary<SurfaceType, AudioClip[]> SoundsBySurface = new Dictionary<SurfaceType, AudioClip[]>();

    [SerializeField] public Dictionary<SurfaceType, string> FilePathBySurface = new Dictionary<SurfaceType, string>();
    
    
}