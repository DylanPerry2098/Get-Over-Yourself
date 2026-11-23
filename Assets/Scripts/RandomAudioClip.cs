using UnityEngine;
using System.IO;

/**
 * Keeps a collection of random audio clips, and plays them at random.
 */
public class RandomAudioClip : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] clips;
    
    public AudioClip GetClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
