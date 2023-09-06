using System;
using UnityEngine;

[Serializable]
public class Sound {
    public SoundName name;

    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    [Range(.1f, 3f)] public float pitch;

    [HideInInspector]
    public AudioSource source;
}

public enum SoundName {
    Move,
    Coin,
    Death,
    Reveal
}