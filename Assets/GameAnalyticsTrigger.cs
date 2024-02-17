using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsTrigger : MonoBehaviour
{
    public static GameAnalyticsTrigger instance = null;
    private void Awake(){
        if(instance == null){
            instance=this;
        }else{
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start(){
        GameAnalytics.Initialize();
        GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Started Game");
    }
}
