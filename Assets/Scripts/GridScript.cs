using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class GridScript : MonoBehaviour
{
    public int GridSize = 5;
    public Transform gridGO;
    public static GridScript instance;
    public static List<SquareScript> Grid = new List<SquareScript>();  
    public GameObject square;
    void Awake(){
        instance = this;
        Grid.Clear();
        foreach (Transform tr in gridGO)
            Grid.Add(tr.GetComponent<SquareScript>());
    }
    public void Generate(){
        #if UNITY_EDITOR
        foreach(Transform tr in gridGO)
            DestroyImmediate(tr.gameObject);
        
        for(int i=0; i<GridSize; i++){
            for (int j = 0; j < GridSize; j++){
                GameObject sc = (GameObject) PrefabUtility.InstantiatePrefab(square);
                sc.transform.SetParent(gridGO.transform);
                sc.transform.localPosition = new Vector3(sc.transform.localPosition.x, sc.transform.localPosition.y, 0);
                sc.transform.localScale = Vector3.one;
                sc.name = "Square "+ (i+1) + "-" + (j+1);
            }
        }
        #endif
    }
    public void Refresh(){
        foreach (Transform sc in gridGO)
            sc.GetComponent<SquareScript>().Reset();
    }
    public bool CheckAdjacency(SquareScript s1, SquareScript s2){
        int x1 = Grid.IndexOf(s1) / GridSize;
        int y1 = Grid.IndexOf(s1) % GridSize;
        
        int x2 = Grid.IndexOf(s2) / GridSize;
        int y2 = Grid.IndexOf(s2) % GridSize;

        return (Mathf.Abs(x1 - x2) == 1  && Mathf.Abs(y1 - y2) == 0) ||
               (Mathf.Abs(y1 - y2) == 1 && Mathf.Abs(x1 - x2) == 0);
    }
}
