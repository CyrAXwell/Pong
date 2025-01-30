using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SFX_VOLUME = "SFXVolume";
    private const float MIN_VOLUME = 0.0001f;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _sFXSource;
    
    public AudioClip WallBounce;
    public AudioClip PlayerBounce;
    public AudioClip Goal;
    public AudioClip StartGame;
    public AudioClip GameOver;
    public AudioClip ButtonSelect;
    public AudioClip ButtonClick;
    private int _volume = 5;

    public static AudioManager Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _volume = PlayerPrefs.GetInt(PLAYER_PREFS_SFX_VOLUME, 5);
            if (_volume == 0)
                _audioMixer.SetFloat(PLAYER_PREFS_SFX_VOLUME, Mathf.Log10(MIN_VOLUME)*20);
            else
                _audioMixer.SetFloat(PLAYER_PREFS_SFX_VOLUME, Mathf.Log10(_volume / 10f)*20);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeVolume()
    {
        _volume ++;
        if (_volume > 10)
        {
            _volume = 0;
            _sFXSource.mute = true;
            _audioMixer.SetFloat(PLAYER_PREFS_SFX_VOLUME, Mathf.Log10(MIN_VOLUME)*20);
        }
        else
        {
            _sFXSource.mute = false;
            _audioMixer.SetFloat(PLAYER_PREFS_SFX_VOLUME, Mathf.Log10(_volume / 10f)*20);
        }

        PlayerPrefs.SetInt(PLAYER_PREFS_SFX_VOLUME, _volume);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        _sFXSource.PlayOneShot(audioClip, volume);
    }

    public float GetVolume() => _volume;
}
