using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Audio Manager is NULL");
            }

            return _instance;
        }
    }

    [Header("Gun SFX")] 
    [SerializeField] public AudioClip sfx_GunFire;
    
    [Header("Foley SFX")] 
    [SerializeField] public AudioClip sfx_HurtVocal;
    [SerializeField] public AudioClip sfx_DialogueReaction;
    
    [Header("Music And SFX Scriptable Object")]
    [SerializeField] public MusicAndSFX musicAndSfxSO;
    
    
    // List to store AudioSource components
    private List<AudioSource> audioSources = new List<AudioSource>();


    private void Awake()
    {
        _instance = this;
    }

    public void PlaySFX(AudioClip sfxClip, GameObject sfxCaller)
    {
        AudioSource sfxSource = sfxCaller.GetComponent<AudioSource>();
        if (!sfxSource)
        {
            sfxSource = sfxCaller.AddComponent<AudioSource>();
        }
        sfxSource.clip = sfxClip;
        
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.GetFpsLoaded())
        {
            sfxSource.spatialBlend = 1.0f;
        }
        else
        {
            sfxSource.spatialBlend = 0.0f;
        }
        sfxSource.Play();
    }

    [YarnCommand("dialogueReact")]
    public void PlayDialogueReaction()
    {
        AudioSource sfxSource = GetComponent<AudioSource>();
        if (!sfxSource)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        AudioClip sfxClip = sfx_DialogueReaction;
        sfxSource.clip = sfxClip;
        sfxSource.Play();
    }
    
    // Call this to play music that is listed in the MusicAndSFX ScriptableObject. First variable is whether it loops.
    [YarnCommand("playSound")]
    public void PlaySound(bool isLooping, string name)
    {
        // Create a new AudioSource component
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        
        // Add the new AudioSource to the list
        audioSources.Add(sfxSource);

        // Find the AudioClip with the specified name
        AudioClip sfxClip = null;
        foreach (var sound in musicAndSfxSO.audioClips)
        {
            if (sound.name == name)
            {
                sfxClip = sound.audioClip;
                break;
            }
        }

        // Check if AudioClip is found
        if (sfxClip == null)
        {
            Debug.LogError("AudioClip with name " + name + " not found in MusicAndSFX ScriptableObject.");
            return;
        }

        // Set AudioClip and loop mode
        sfxSource.clip = sfxClip;
        sfxSource.loop = isLooping;

        // Play the sound
        sfxSource.Play();

        // Start coroutine to remove AudioSource after playback
        StartCoroutine(RemoveAudioSourceAfterPlayback(sfxSource));
    }

    // Coroutine to remove AudioSource after playback
    private IEnumerator RemoveAudioSourceAfterPlayback(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSources.Remove(audioSource);
        Destroy(audioSource);
    }
}
