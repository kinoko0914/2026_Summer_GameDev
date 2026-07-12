using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("オーディオソース")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("BGMリスト")]
    [SerializeField] private AudioClip stageBGM;

    [Header("SEリスト")]
    [SerializeField] private AudioClip jumpSE;
    [SerializeField] private AudioClip landSE;
    [SerializeField] private AudioClip grappleSE;
    [SerializeField] private AudioClip releaseSE;
    [SerializeField] private AudioClip damageSE;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            PlayBGM();
        }
        else
        {
            StopBGM(); 
        }
    }

    void Start()
    {
    }

    public void PlayBGM()
    {
        if (bgmSource != null && stageBGM != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = stageBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void PlayJumpSE() { PlaySE(jumpSE); }
    public void PlayLandSE() { PlaySE(landSE); }
    public void PlayGrappleSE() { PlaySE(grappleSE); }
    public void PlayReleaseSE() { PlaySE(releaseSE); }
    public void PlayDamageSE() { PlaySE(damageSE); }

    private void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }
}