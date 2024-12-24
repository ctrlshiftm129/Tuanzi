using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Manager
{
    public AudioSource bgmSource;
    public AudioSource seSource;
    
    [Space]
    public AudioClip bossBgm;
    public List<AudioClip> normalBgm = new();
    public List<AudioClip> se = new();

    private void Awake()
    {
        Register2Locator();
    }

    #region Game Audio

    public void PlayNormalBgm(int id)
    {
        bgmSource.clip = normalBgm[id];
        bgmSource.loop = true;
        bgmSource.Play();
    }
    
    public void PlayBossBgm()
    {
        bgmSource.clip = bossBgm;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayGlobalSe(int id)
    {
        seSource.clip = se[id];
        seSource.loop = false;
        seSource.Play();
    }

    public void FadesOutCurrentBgm(Action callback = null)
    {
        if (!bgmSource.isPlaying) return;
        StartCoroutine(AudioFadesOut(bgmSource, 4, callback));
    }

    #endregion
    
    #region Utils

    public static IEnumerator AudioFadesOut(AudioSource audioSource, float fadeTime, Action callback = null)
    {
        // 计算每秒降低的音量
        var fadeOutVolumeDelta = 1f / fadeTime;
        // 在指定的时间内降低音量，直到音量为0
        while (audioSource.volume > 0)
        {
            // 降低音量
            audioSource.volume -= fadeOutVolumeDelta * Time.deltaTime;
            // 等待一帧
            yield return null;
        }
 
        // 音乐音量降至0，关闭音乐或进行其他操作
        audioSource.Stop();
        audioSource.volume = 1;
        callback?.Invoke();
    }

    #endregion
}
