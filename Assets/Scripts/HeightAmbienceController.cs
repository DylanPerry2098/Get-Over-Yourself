using System;
using UnityEngine;

public class HeightAmbienceController : MonoBehaviour
{
    [SerializeField] private AudioSource bottomAudioSource;
    [SerializeField] private AudioSource topAudioSource;

    private float _lastSampledHeight;
    private float _bottomTargetVolume = 0.5f;
    private float _topTargetVolume = 0.5f;
    
    [SerializeField] private float lowerYBound, upperYBound;

    [SerializeField] private float heightSampleRateSeconds = 5f;

    [SerializeField] private float crossfadeSmoothing = 1f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lowerYBound > upperYBound)
        {
            lowerYBound = upperYBound;
        }

        SampleHeight();
        bottomAudioSource.volume = _bottomTargetVolume;
        topAudioSource.volume = _topTargetVolume;
        InvokeRepeating("SampleHeight", heightSampleRateSeconds, heightSampleRateSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(bottomAudioSource.volume - _bottomTargetVolume) > 0.0001f)
        {
            bottomAudioSource.volume += (Math.Sign(_bottomTargetVolume - bottomAudioSource.volume) * Time.deltaTime) / crossfadeSmoothing;
        }
        
        if (Math.Abs(topAudioSource.volume - _topTargetVolume) > 0.0001f)
        {
            topAudioSource.volume += (Math.Sign(_topTargetVolume - topAudioSource.volume) * Time.deltaTime) / crossfadeSmoothing;
        }
        
        print(bottomAudioSource.volume + " | " + topAudioSource.volume);
    }

    void SampleHeight()
    {
        _lastSampledHeight = gameObject.transform.position.y;
        float ratio = Math.Min(
            1f,
            (_lastSampledHeight - lowerYBound) / (upperYBound-lowerYBound)
        );
        print("Ratio: " + ratio);
        _bottomTargetVolume = 1f - ratio;
        _topTargetVolume = ratio;
    }

}
