using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour{

    public AudioMixer mixer;
    public Toggle audioToggle;
    void Start(){
        float Volume = PlayerPrefs.GetFloat("Audio", 0);
        mixer.SetFloat("Volume", Volume);
        audioToggle.isOn = (Volume == 0);
    }
    public void ToggleSound(bool On){
        Debug.Log("Sound Toggle");
        if(On)
            PlayerPrefs.SetFloat("Audio", 0f);
        else
            PlayerPrefs.SetFloat("Audio", -80f);

        float Volume = PlayerPrefs.GetFloat("Audio", 0);
        mixer.SetFloat("Volume", Volume);
    }
}
