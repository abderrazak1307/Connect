using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameColors {
    Orange = 0,
    Pink = 1,
    Yellow = 2,
    Green = 3,
    Blue = 4,
    Purple = 5,
    Navy = 6
}
[ExecuteAlways]
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public Color[] colors;
    public bool isDragging;
    [HideInInspector]
    public List<SquareScript> comboSquares = new List<SquareScript>();

    public List<List<SquareScript>> Moves = new List<List<SquareScript>>();
    [HideInInspector]
    public List<LineRenderer> lines = new List<LineRenderer>();
    public List<List<SquareScript>> redoList = new List<List<SquareScript>>();
    public GameObject linePrefab;
    public GameObject confettiEffect;
    public AudioClip wonAudio1;
    public AudioClip wonAudio2;
    public Button UndoButton;
    public Button RedoButton;
    public bool haveWon = false;

    void Awake(){
        instance = this;
    }
    void Start(){
        AdsManager.CanLoadNextScene = false;
    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(1);

        if(Input.GetMouseButtonUp(0)){
            if(comboSquares.Count > 0){
                SquareScript firstSquare = comboSquares[0];
                SquareScript lastSquare = comboSquares[comboSquares.Count - 1];
                if (isDragging && lastSquare.type == SquareType.Start && firstSquare != lastSquare)
                    checkValidate();
                else
                    cancelAction();
            }
        }
    }
    void CreateLine(Vector3 pos, Color c){
        GameObject lineClone = (GameObject)Instantiate(linePrefab);
        LineRenderer line = lineClone.GetComponent<LineRenderer>();
        line.material.SetColor("_Color", c);

        if(GridScript.instance.GridSize == 8) line.startWidth = 0.15f;
        else if(GridScript.instance.GridSize == 7) line.startWidth = 0.2f;
        else if(GridScript.instance.GridSize == 6) line.startWidth = 0.25f;
        else line.startWidth = 0.3f;
            
        line.positionCount = 1;
        Vector3 position = new Vector3(pos.x, pos.y, 0);
        line.SetPosition(0, position);
        lines.Add(line);
    }
    void AddAPoint(Vector3 pos){
        LineRenderer line = lines[lines.Count - 1];
        Vector3 position = new Vector3(pos.x, pos.y, 0);
        Vector3 start = line.GetPosition(line.positionCount -1);
        line.positionCount = line.positionCount+1;
        line.SetPosition(line.positionCount - 1, start);
        StartCoroutine(AnimateLine(line, line.positionCount -1, start, position, 0.2f));
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
    void RemoveLastLine(){
        if(lines.Count == 0) return;

        LineRenderer line = lines[lines.Count - 1];
        lines.RemoveAt(lines.Count-1);
        Destroy(line.gameObject);
    }
    void CreateEntireLine(List<SquareScript> list){
        GameObject lineClone = (GameObject)Instantiate(linePrefab);
        LineRenderer line = lineClone.GetComponent<LineRenderer>();
        line.material.SetColor("_Color", ConvertColor(list[0].color));
        line.positionCount = list.Count;
        for(int i=0; i<list.Count; i++){
            Vector3 position = new Vector3(list[i].transform.position.x, list[i].transform.position.y, 0);
            line.SetPosition(i, position);
        }
        lines.Add(line);
    }

    public static Color ConvertColor(GameColors color){
        if(!instance) instance = FindObjectOfType<GameManager>();
        if (instance)
            return instance.colors[(int) color];
        else
            return Color.black;
    }

    public void RegisterStart(SquareScript sc){
        isDragging = true;
        comboSquares.Clear();
        comboSquares.Add(sc);
        CreateLine(sc.transform.position, ConvertColor(sc.color));
    }
    public void RegisterMiddle(SquareScript sc){
        SquareScript lastSquare = comboSquares[comboSquares.Count - 1];
        if(GridScript.instance.CheckAdjacency(lastSquare, sc) && !sc.colored){
            if(sc.type != SquareType.Start){
                sc.Colorify(comboSquares[0].color);
                comboSquares.Add(sc);
                AddAPoint(sc.transform.position);
            }else{
                sc.Colorify(comboSquares[0].color);
                comboSquares.Add(sc);
                AddAPoint(sc.transform.position);
                checkValidate();
            }
        }else{
            cancelAction();
        }
    }
    void CheckUndoRedo(){
        if(Moves.Count>0)UndoButton.interactable = true;
        else UndoButton.interactable = false;

        if(redoList.Count>0)RedoButton.interactable = true;
        else RedoButton.interactable = false;
    }
    public void UndoMoveContaining(SquareScript sc){
        int moveIndex = 0;
        foreach (var move in Moves){
            foreach(var step in move){
                if(step == sc) goto found;
            }
            moveIndex++;
        }
        found:
        if(moveIndex >= Moves.Count) return;

        List<SquareScript> moveToUndo = Moves[moveIndex];
        redoList.Add(moveToUndo);
        Moves.RemoveAt(moveIndex);
        foreach (var square in moveToUndo)
            square.Reset();
        Destroy(lines[moveIndex].gameObject);
        lines.RemoveAt(moveIndex);
        isDragging = false;
        CheckUndoRedo();
    }
    public void Undo(){
        if(Moves.Count == 0) return;

        Debug.Log("Undoing last move");
        List<SquareScript> lastValidMove = Moves[Moves.Count - 1];
        redoList.Add(lastValidMove);
        Moves.RemoveAt(Moves.Count-1);
        foreach (var sc in lastValidMove)
            sc.Reset();
        RemoveLastLine();
        isDragging = false;
        CheckUndoRedo();
    }
    public void Redo(){
        if (redoList.Count == 0) return;

        Debug.Log("Undoing last move");
        List<SquareScript> lastValidMove = redoList[redoList.Count - 1];
        Moves.Add(lastValidMove);
        redoList.RemoveAt(redoList.Count - 1);

        foreach (var sc in lastValidMove)
            sc.Colorify(lastValidMove[0].color);

        CreateEntireLine(lastValidMove);
        isDragging = false;
        CheckUndoRedo();
    }
    public void checkValidate(){
        Debug.Log("Checking validate...");
        SquareScript firstSquare = comboSquares[0];
        SquareScript lastSquare = comboSquares[comboSquares.Count - 1];
        if(firstSquare.color == lastSquare.color && firstSquare != lastSquare){
            Moves.Add(new List<SquareScript>(comboSquares));
            isDragging = false;
            redoList.Clear();
            Debug.Log("Valid move!");
            CheckUndoRedo();
            CheckWinCon();
        }else{
            cancelAction();
            Debug.Log("Invalid, resetting!");
        }
    }
    public void CheckWinCon(){
        Debug.Log("Grid has: "+GridScript.Grid);
        foreach (var square in GridScript.Grid){
            if(square && square.type != SquareType.Dead && !square.colored){
                Debug.Log("You don't win" + square.gameObject.name + " colored?: " + square.colored);
                return;
            }
        }
        haveWon = true;
        UndoButton.interactable = false;
        RedoButton.interactable = false;
        Invoke("Confetti", 0.2f);
    }

    public GameObject WonUI;
    public TMPro.TextMeshProUGUI WonText;
    private string[] WonMessages = {"Nice",  "Well done", "Great", "Perfect"};
    void Confetti(){
        confettiEffect.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(wonAudio1);
        Invoke("SecondSound", 2.8f);
        Invoke("Won", 1f);
    }
    void SecondSound(){
        GetComponent<AudioSource>().PlayOneShot(wonAudio2);
    }
    void Won(){
        int index;
        if(Time.timeSinceLevelLoad > 60f) index = 0;
        else if (Time.timeSinceLevelLoad > 40f) index = 1;
        else if (Time.timeSinceLevelLoad > 20f) index = 2;
        else index = 3;

        WonText.text = WonMessages[index];
        WonUI.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex > PlayerPrefs.GetInt("LastLevel", 1))
            PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel(){
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings){
            StartCoroutine(LoadSceneInBackground(SceneManager.GetActiveScene().buildIndex + 1));
            AdsManager.ShowInterstitial();
        }else{
            Home();
        }
    }
    public void cancelAction(){
        if(!isDragging) return;

        foreach(var sc in comboSquares)
            sc.Reset();
        RemoveLastLine();
        isDragging = false;
    }
    public void Retry(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Home(){
        SceneManager.LoadScene(1);
    }

    private IEnumerator LoadSceneInBackground(int sceneNumber){
        yield return new WaitForSeconds(0.5f);
        var loading = SceneManager.LoadSceneAsync(sceneNumber);
        while (!loading.isDone)
            yield return new WaitForSeconds(0.1f);
            
        yield return new WaitUntil(() => AdsManager.CanLoadNextScene);
    }
}