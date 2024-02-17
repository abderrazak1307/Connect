using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour{
    const int baseLevel = 1;
    public int Level;
    public GameObject Lock;

    void Start(){
        int LastLevel = PlayerPrefs.GetInt("LastLevel", 1);
        if(Level > 0){
            if(Level > LastLevel  || Level >= SceneManager.sceneCountInBuildSettings-1){
                Lock.SetActive(true);
                GetComponent<Button>().interactable = false;
            }else{
                Lock.SetActive(false);
                GetComponent<Button>().interactable = true;
            }
        }
    }
    public void LoadLevel(){
        StartCoroutine(LoadSceneInBackground(baseLevel + Level));
        AdsManager.ShowInterstitial();
    }
    private IEnumerator LoadSceneInBackground(int sceneNumber){
        yield return new WaitForSeconds(0.5f);
        var loading = SceneManager.LoadSceneAsync(sceneNumber);
        while (!loading.isDone)
            yield return new WaitForSeconds(0.1f);

        yield return new WaitUntil(() => AdsManager.CanLoadNextScene);
        AdsManager.CanLoadNextScene = false;
    }
}
