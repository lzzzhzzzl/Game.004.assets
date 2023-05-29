using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("音乐数据库")]
    public GameSoundData_SO gameSoundData_SO;
    public ScneneSoundData_SO scneneSoundData_SO;

    [Header("音乐组件")]
    public AudioSource ambientSource;
    public AudioSource gameSource;


    public float MusicStartSecond => Random.Range(5f, 15f);
    private float musicTransitionSecond = 8f;

    private Coroutine soundRoutine;
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapShot;
    public AudioMixerSnapshot ambientSnapShot;
    public AudioMixerSnapshot muteSnapShot;

    private void OnEnable()
    {
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }


    private void OnPlaySoundEvent(SoundName soundName)
    {
        SoundDetail soundDetail = gameSoundData_SO.GetGameSoundDetail(soundName);
        if (soundDetail != null)
        {
            EventHandler.CallInitSoundEffect(soundDetail);
        }
    }


    private void OnAfterSceneDataLoadEvent()
    {
        string currentScene = TransitionManager.Instance.GetCurrentMapID();
        SceneSoundItem sceneSound = scneneSoundData_SO.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;

        SoundDetail ambient = gameSoundData_SO.GetGameSoundDetail(sceneSound.ambient);
        SoundDetail music = gameSoundData_SO.GetGameSoundDetail(sceneSound.music);
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));
    }
    private IEnumerator PlaySoundRoutine(SoundDetail music, SoundDetail ambient)
    {
        if (music != null && ambient != null)
        {
            PlayAmbientClip(ambient, 1f);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music, musicTransitionSecond);
        }
    }


    private void PlayMusicClip(SoundDetail soundDetails, float TransitionTime)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolime));
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled)
        {
            gameSource.Play();
        }
        normalSnapShot.TransitionTo(TransitionTime);
    }
    private void PlayAmbientClip(SoundDetail soundDetails, float TransitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolime));
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();
        }
        ambientSnapShot.TransitionTo(TransitionTime); ;
    }


    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }


    private void OnEndGameEvent()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        muteSnapShot.TransitionTo(1f);
    }
}
