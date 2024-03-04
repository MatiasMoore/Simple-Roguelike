using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource _source;

    private string _basePath = "Audio";

    public enum SFX
    {
        testSFX,
        CoinPickup
    }

    static public AudioController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    [ContextMenu("Test Audio")]
    public void TestAudio()
    {
        this.PlaySFX(SFX.testSFX);
    }

    public void PlaySFX(SFX sfx)
    {
        var clip = GetAudioClip(sfx);
        _source.PlayOneShot(clip);
    }

    private AudioClip GetAudioClip(SFX sfx)
    {
        var path = GetPathForSFX(sfx);
        return GetAudioClip(path);
    }

    private string GetPathForSFX(SFX sfx)
    {
        return _basePath + "/" + sfx.ToString();
    }

    private AudioClip GetAudioClip(string path)
    {
        return UnityEngine.Resources.Load<AudioClip>(path);
    }
}
