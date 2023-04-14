using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PoolableAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    private Queue<PoolableAudioSource> pool;

    private AudioClip currentClip => audioSource.clip;
    private bool hasClip;

    
    public void Init(Queue<PoolableAudioSource> audioPool)
    {
        audioSource = GetComponent<AudioSource>();
        pool = audioPool;
        gameObject.SetActive(false);
    }


    public void Play(AudioClip clip)
    {
        gameObject.SetActive(true);
        hasClip = true;
        audioSource.PlayOneShot(clip);
    }


    private void Update()
    {
        if(!hasClip) return;
        if (currentClip) return;
        
        ReturnPool();
        hasClip = false;

    }


    public void ReturnPool()
    {
        pool.Enqueue(this);
        gameObject.SetActive(false);
    }
}
