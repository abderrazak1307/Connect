using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusic : MonoBehaviour
{
    private static AudioSource _audioSource;
    private void Awake(){
        DontDestroyOnLoad(transform.gameObject);
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    public void PlayMusic(){
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic(){
        _audioSource.Stop();
    }
}