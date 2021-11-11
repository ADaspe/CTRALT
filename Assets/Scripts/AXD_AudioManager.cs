using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_AudioManager : MonoBehaviour{

    public List<AXD_ClipInfos> allclipinfos;
    public List<AudioSource> audioSources;

    private void Start() {
        foreach (AXD_ClipInfos clipInfo in allclipinfos)
        {
            AudioSource source = this.gameObject.AddComponent<AudioSource>();
            source.clip = clipInfo.clip;
            source.mute = clipInfo.mute;
            source.bypassEffects = clipInfo.bypassEffects;
            source.bypassListenerEffects = clipInfo.bypassListenerEffects;
            source.bypassReverbZones = clipInfo.bypassReverbZone;
            source.playOnAwake = clipInfo.playOnAwake;
            source.loop = clipInfo.loop;
            source.priority = clipInfo.priority;
            source.volume = clipInfo.volume;
            source.pitch = clipInfo.pitch;
            source.panStereo = clipInfo.stereoPan;
            source.spatialBlend = clipInfo.spatialBend;
            source.reverbZoneMix = clipInfo.reverbZoneMix;
            source.enabled = false;
            clipInfo.linkedAudioSourceComponent = source;
        }
    }

    public void PlaySource(AudioSource source){
        source.Play();
    }

    public void PlayClip(AudioClip clip){

    }
}
