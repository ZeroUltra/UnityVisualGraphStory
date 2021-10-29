using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    AudioSource audioMusic;
    public AudioClip _music1;//闪现声音
    public AudioClip _music2;//绊倒声音
    public AudioClip _music3;
    void Awake()
    {
        audioMusic = gameObject.GetComponent<AudioSource>();
        audioMusic.loop = false;

    }

    void PlayMusic()//播放闪现声音
    {
        audioMusic.clip = _music1;
        audioMusic.Play();
    }

    void PlayBGM()//播放绊倒声音
    {
        audioMusic.clip = _music2;
        audioMusic.Play();
    }
    void PlayM()
    {
        audioMusic.clip = _music3;
        audioMusic.Play();
    }
}
