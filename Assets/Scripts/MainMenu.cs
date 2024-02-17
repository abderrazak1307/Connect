using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    const int baseLevel = 1;
    public TMPro.TextMeshProUGUI[] LevelPackTexts;
    public TMPro.TextMeshProUGUI[] LevelPackTitles;
    public string[] LevelPackNames;
    public int[] LevelsPerPack;
    public GameObject[] LevelPackGrids;
    public GameObject levelButton;

    public void Generate(){
        #if UNITY_EDITOR
        if(LevelPackNames.Length != LevelPackTexts.Length || LevelPackNames.Length != LevelsPerPack.Length) return;
        
        // Update Levelpack buttons
        for(int i=0; i<LevelPackTexts.Length; i++)
            LevelPackTexts[i].text = LevelPackNames[i] + " ("+LevelsPerPack[i] +"/"+LevelsPerPack[i]+")";
        
        // Update titles
        for(int i=0; i<LevelPackTitles.Length; i++)
            LevelPackTitles[i].text = LevelPackNames[i];

        // Create grids
        int levelNumber = 1;
        for(int i=0; i<LevelPackGrids.Length; i++){
            foreach(Transform child in LevelPackGrids[i].transform)
                DestroyImmediate(child.gameObject);
            
            for(int j=0; j<LevelsPerPack[i]; j++){
                GameObject level = (GameObject) PrefabUtility.InstantiatePrefab(levelButton);
                level.transform.SetParent(LevelPackGrids[i].transform);
                level.transform.localScale = Vector3.one;
                level.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ""+(j+1);
                level.name = "Level " + levelNumber;
                level.GetComponent<LevelLoader>().Level = levelNumber;
                levelNumber++;
            }
        }
        #endif
    }

    void Start(){
        AdsManager.CanLoadNextScene = false;

        if (LevelPackNames.Length != LevelPackTexts.Length || LevelPackNames.Length != LevelsPerPack.Length) return;

        if(LevelPackTexts.Length == 0) return;

        int LastLevel = PlayerPrefs.GetInt("LastLevel", 1);
        int i = 0;

        // Update Levelpack buttons
        while (LastLevel > 0 && i < LevelPackTexts.Length)
        {
            if (LastLevel > LevelsPerPack[i])
                LevelPackTexts[i].text = LevelPackNames[i] + " (" + LevelsPerPack[i] + "/" + LevelsPerPack[i] + ")";
            else
                LevelPackTexts[i].text = LevelPackNames[i] + " (" + LastLevel + "/" + LevelsPerPack[i] + ")";
            LastLevel = LastLevel - LevelsPerPack[i];
            i++;
        }
        while (i < LevelPackTexts.Length){
            LevelPackTexts[i].text = LevelPackNames[i] + " (0/" + LevelsPerPack[i] + ")";
            i++;
        }
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
            SceneManager.LoadScene(0);
    }
    public void LoadSplashScreen(){
        SceneManager.LoadScene(0);
    }
    public void LoadLastLevel(){
        int LastLevel = PlayerPrefs.GetInt("LastLevel", 1);
        if (baseLevel + LastLevel < SceneManager.sceneCountInBuildSettings){
            StartCoroutine(LoadSceneInBackground(baseLevel + LastLevel));
            AdsManager.ShowInterstitial();
        }else{
            StartCoroutine(LoadSceneInBackground(1));
            AdsManager.ShowInterstitial();
        }
    }
    private IEnumerator LoadSceneInBackground(int sceneNumber){
        yield return new WaitForSeconds(0.5f);
        var loading = SceneManager.LoadSceneAsync(sceneNumber);
        while (!loading.isDone)
            yield return new WaitForSeconds(0.1f);

        yield return new WaitUntil(() => AdsManager.CanLoadNextScene);
    }
}
