using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MusicAndSFX", menuName = "Music/MusicAndSFX")]
public class MusicAndSFX : ScriptableObject
{
    [Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip audioClip;
    }

    public List<NamedAudioClip> audioClips = new List<NamedAudioClip>();
}