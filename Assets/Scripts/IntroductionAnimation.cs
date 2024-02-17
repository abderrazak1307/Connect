using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionAnimation : MonoBehaviour
{
    public Vector3[] YellowPath;
    public Vector3[] PurplePath;
    public Vector3[] BluePath;
    public Vector3[] PinkPath;
    public GameObject[] YellowOutlines;
    public GameObject[] PurpleOutlines;
    public GameObject[] BlueOutlines;
    public GameObject[] PinkOutlines;

    public LineRenderer YellowLine;
    public LineRenderer PurpleLine;
    public LineRenderer BlueLine;
    public LineRenderer PinkLine;
    void Start(){
        StartCoroutine(Animate(YellowLine, YellowOutlines, YellowPath));
        StartCoroutine(Animate(PurpleLine, PurpleOutlines, PurplePath));
        StartCoroutine(Animate(BlueLine, BlueOutlines, BluePath));
        StartCoroutine(Animate(PinkLine, PinkOutlines, PinkPath));
    }

    // Update is called once per frame
    IEnumerator Animate(LineRenderer line, GameObject[] outlines, Vector3[] path){
        line.positionCount = 1;
        line.SetPosition(0, path[0]);
        outlines[0].SetActive(true);
        for(int i=1; i<path.Length; i++){
            Vector3 position = new Vector3(path[i].x, path[i].y, 0);
            Vector3 start = line.GetPosition(line.positionCount - 1);
            line.positionCount = line.positionCount + 1;
            line.SetPosition(line.positionCount - 1, start);
            StartCoroutine(AnimateLine(line, line.positionCount - 1, start, position, 0.2f));
            yield return new WaitForSeconds(0.2f);
            outlines[i].SetActive(true);
        }
        
    }

    IEnumerator AnimateLine(LineRenderer line, int position, Vector3 start, Vector3 destination, float duration){
        float counter = 0f;
        while (counter < duration && line){
            counter += Time.deltaTime;
            float time = counter / duration;
            line.SetPosition(position, Vector3.Lerp(start, destination, time));
            yield return null;
        }
    }
}
