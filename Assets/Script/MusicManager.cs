
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    void Awake()
    {
        musicPlaying = aS1.clip;
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public AudioSource aS1, aS2;
    public float transiTime;
    public float defaultVolume;
    AudioClip musicPlaying;
    public void ChangeClip(AudioClip newMusic)
    {
        if (newMusic == musicPlaying) return;

        AudioSource nowPlaying = aS1;
        AudioSource target = aS2;

        if(nowPlaying.isPlaying == false)
        {
            nowPlaying = aS2;
            target = aS1;
        }

        StopAllCoroutines();
        StartCoroutine(MixSources(nowPlaying,target, newMusic));
        musicPlaying = newMusic;
    }

    IEnumerator MixSources(AudioSource nowPlaying, AudioSource target,AudioClip newMusic)
    {
        float percentage = 0;
        while (nowPlaying.volume >0)
        {
            nowPlaying.volume = Mathf.Lerp(defaultVolume, 0, percentage);
            percentage += Time.deltaTime/transiTime;
            yield return null;
        }

        nowPlaying.Pause();
        target.clip = newMusic;
        if (target.isPlaying == false) target.Play();
        target.UnPause();
        percentage = 0;

        while (target.volume<defaultVolume)
        {
            target.volume = Mathf.Lerp(0, defaultVolume, percentage);
            percentage += Time.deltaTime / transiTime;
            yield return null;
        }

        yield return null;
    }
}
