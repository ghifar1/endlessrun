using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public static AudioScript current;

    [Header("BGM Music")]
    public AudioClip summer;
    public AudioClip dessert;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        AudioSource audioSource = GetComponent<AudioSource>();

        if (EnvironmentControl.current.activeSeasonIndex == 0)
        {
            audioSource.Stop();
            audioSource.clip = summer;
            audioSource.Play();
        } else
        {
            audioSource.Stop();
            audioSource.clip = dessert;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
