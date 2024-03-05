using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    public static MusicManager Instance
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
    private void Awake()
    {
        _instance = this;
    }

    private AudioSource musicSource;
    
    [Header("Music")]
    [SerializeField] public AudioClip music_classroom;
    [SerializeField] public AudioClip music_FPS;
    
    public void PlayMusic(AudioClip musicClip)
    {
        // TODO UNCOMMENT LATER
        // if (musicSource.isPlaying)
        // {
        //     musicSource.Stop();
        // }
        // musicSource.clip = musicClip;
        // musicSource.Play();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        musicSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
