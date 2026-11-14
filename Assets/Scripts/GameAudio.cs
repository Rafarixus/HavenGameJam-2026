using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip finish;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.2f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 0.8f;

    private static GameAudio instance;
    private AudioSource musicSource;
    private AudioSource effectsSource;
    private bool finishPlayed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance) return;
        var prefab = Resources.Load<GameAudio>("GameAudio");
        if (prefab) Instantiate(prefab);
    }

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource = CreateSource(musicVolume);
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.priority = 200;
        effectsSource = CreateSource(effectsVolume);
        if (music) musicSource.Play();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private AudioSource CreateSource(float volume)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = volume;
        return source;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single) finishPlayed = false;
    }

    public static void PlayShoot()
    {
        if (instance && instance.shoot) instance.effectsSource.PlayOneShot(instance.shoot);
    }

    public static void PlayFinish()
    {
        if (!instance || instance.finishPlayed) return;
        instance.finishPlayed = true;
        if (instance.finish) instance.effectsSource.PlayOneShot(instance.finish);
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        instance = null;
    }
}
