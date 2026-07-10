using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("オーディオソース")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("BGMリスト（後で音源をセットする）")]
    [SerializeField] private AudioClip stageBGM;

    [Header("SEリスト（後で音源をセットする）")]
    [SerializeField] private AudioClip jumpSE;
    [SerializeField] private AudioClip landSE;
    [SerializeField] private AudioClip grappleSE; 
    [SerializeField] private AudioClip releaseSE;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && stageBGM != null)
        {
            bgmSource.clip = stageBGM;
            bgmSource.loop = true; 
            bgmSource.Play();
        }
    }

    public void PlayJumpSE() { PlaySE(jumpSE); }
    public void PlayLandSE() { PlaySE(landSE); }
    public void PlayGrappleSE() { PlaySE(grappleSE); }
    public void PlayReleaseSE() { PlaySE(releaseSE); }

    private void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }
}