using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ClipInfo",menuName ="ScriptableObjects/AXD_ClipInfo",order =3)]
public class AXD_ClipInfos : ScriptableObject
{
    public AudioSource linkedAudioSourceComponent;
    public AudioClip clip;
    public bool mute = false;
    public bool bypassEffects = false;
    public bool bypassListenerEffects = false;
    public bool bypassReverbZone = false;
    public bool playOnAwake = true;
    public bool loop = false;

    [Range(0,256)] public int priority = 128;

    [Range(0,1)] public float volume = 1;

    [Range(-3,3)] public float pitch = 1;

    [Range(-1,1)] public float stereoPan = 0;
    [Range(0,1)] public float spatialBend = 0;

    [Range(0,1.1f)] public float reverbZoneMix = 1;

}
