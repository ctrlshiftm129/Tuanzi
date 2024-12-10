using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Utils

    public static IEnumerator AudioFadesOut(AudioSource audioSource, float fadeTime)
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
    }

    #endregion
    

}
