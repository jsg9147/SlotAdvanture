using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePlayDelayed : MonoBehaviour
{
    public AudioClip audioClip;
    public float delayTime;

    void Awake()
    {
        // AudioSource 컴포넌트 가져오기 또는 추가하기
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        float delay = delayTime;
        // AudioClip 설정
        if(audioClip != null)
            audioSource.clip = audioClip;

        // 딜레이 설정 후 재생
        if (GameManager.Instance.doubleSpeed)
        {
            delay = delay * 0.5f;
        }
        audioSource.PlayDelayed(delay);
    }
}
