using UnityEngine;

public class MusicManager : MonoBehaviour {

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float volume = .3f;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        //初始化我们的声音
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        //由于我们不能保证其他函数是否是先等这里awake，再调用GetVolume()修改相关声音的初始化设置
        audioSource.volume = volume;

    }

    public void ChangeVolume() {
        volume += .1f;
        if (volume > 1f) {
            volume = 0f;
        }
        audioSource.volume = volume;

        //保存声音的相关设置
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return volume;
    }
}