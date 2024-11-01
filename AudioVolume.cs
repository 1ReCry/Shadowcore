using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolume : MonoBehaviour
{
    [Header("Это музыка?")]
    public bool isMusic;
    [Header("Всегда обновлять (фиксированная громкость)")]
    public bool alwaysUpdate;

    private AudioSource[] audioSources;

    void Start()
    {
        // Получаем все AudioSource компоненты на объекте
        audioSources = GetComponents<AudioSource>();


        if(!alwaysUpdate)
        {
            if (!isMusic)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.volume *= Globals.soundVolume;
                }
            }

            if (isMusic)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.volume *= Globals.musicVolume;
                }
            }
        }
    }

    void Update()
    {
        if(alwaysUpdate)
        {
            if (!isMusic)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.volume = 1*Globals.soundVolume;
                }
            }

            if (isMusic)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.volume = 1*Globals.musicVolume;
                }
            }
        }
    }
}
