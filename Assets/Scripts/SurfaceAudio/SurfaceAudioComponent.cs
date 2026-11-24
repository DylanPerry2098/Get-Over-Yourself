using System;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SurfaceAudioComponent : MonoBehaviour
{
    [SerializeField]
    private SurfaceAudioLibrary audioLibrary;

    [SerializeField]
    private Dictionary<SurfaceAudioLibrary.SurfaceType, RandomAudioClip> randomAudioClipBySurface = new Dictionary<SurfaceAudioLibrary.SurfaceType, RandomAudioClip>();

    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (audioLibrary == null)
        {
            throw new MissingFieldException("Missing audio library!");
        }
        
        foreach (KeyValuePair<SurfaceAudioLibrary.SurfaceType, AudioClip[]> entry in audioLibrary.SoundsBySurface)
        {
            
            RandomAudioClip rac = gameObject.AddComponent<RandomAudioClip>();
            rac.clips = entry.Value;
            randomAudioClipBySurface[entry.Key] = rac;
        }
    }

    public void PlaySound(SurfaceAudioLibrary.SurfaceType surfaceType)
    {
        if (!randomAudioClipBySurface.ContainsKey(surfaceType))
        {
            Debug.LogWarning("Surface type " + surfaceType + " not found in library");
        }

        audioSource.clip = randomAudioClipBySurface[surfaceType].GetClip();
        audioSource.Play();
    }
}