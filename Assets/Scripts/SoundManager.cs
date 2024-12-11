using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    //serialized to enable editing in the inspector.
    [Header("Sources")]
    [SerializeField] private AudioSource effect;
    [SerializeField] private AudioSource music;

    [SerializeField] private AudioSource citynoise;

    [Header("Clips")]
    [SerializeField] public AudioClip plane;
    [SerializeField] public AudioClip drop;
    [SerializeField] public AudioClip rowClear;
    [SerializeField] private AudioClip background;
    [SerializeField] private AudioClip background2;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
            Destroy(gameObject);
        
    }

    void Start()
    {
        PlayMusic(background);
        PlayCity(background2);

    }

    public void PauseTracks()
    {
        citynoise.Pause();
        music.Pause();
    }

    public void PlayPlaneSound()
    {
        PlayEffect(plane);
    }

    public void PlayDropSound()
    {
        PlayEffect(drop);
    }

    public void PlayLineClearSound()
    {
        PlayEffect(rowClear);
    }

   

    public void PlayEffect(AudioClip clip)
    {
        if (clip != null && effect != null)
        {
            effect.PlayOneShot(clip);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && music != null)
        {
            music.clip = clip;
            music.loop = true;
            music.Play();
        }
    }
    private void PlayCity(AudioClip clip)
    {
        if (clip != null && music != null)
        {
            citynoise.clip = clip;
            citynoise.loop = true;
            citynoise.Play();
        }
    }
}
