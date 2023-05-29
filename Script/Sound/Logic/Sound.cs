using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void SetSound(SoundDetail soundDetail)
    {
        audioSource.clip = soundDetail.soundClip;
        audioSource.volume = soundDetail.soundVolime;
        audioSource.pitch = Random.Range(soundDetail.soundPitchMin, soundDetail.soundPitchMax);
    }
}
