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

    private void Awake()
    {
        _instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
