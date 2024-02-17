using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum SquareType{Normal, Start, Dead}

[ExecuteAlways]
public class SquareScript : MonoBehaviour{
    
    [Header("Settings")]
    public GameColors color;
    public SquareType type;
    [HideInInspector]
    public bool colored = false;

    [Header("Debug")]
    public Image outlineImage;
    public Image deadImage;
    public Image startImage;

    void Start(){
        Reset();
    }
    void OnValidate(){
        Reset();
    }
    public void Colorify(GameColors c){
        Color color = GameManager.ConvertColor(c);
        outlineImage.color = new Color(color.r, color.g, color.b, 0.8f);
        outlineImage.gameObject.SetActive(true);
        colored = true;
    }
    public void Reset(){
        if (type == SquareType.Dead){
            deadImage.gameObject.SetActive(true);
        }
        else {
            Color c = GameManager.ConvertColor(color);
            // Start Dot
            startImage.color = c;
            startImage.gameObject.SetActive(type==SquareType.Start);
            // Outline && Dead Image
            outlineImage.gameObject.SetActive(false);
            deadImage.gameObject.SetActive(false);
        }
        colored = false;
    }

    public void BeginDrag(){
        if(GameManager.instance.haveWon)
            return;

        if(colored)
            GameManager.instance.UndoMoveContaining(this);

        if(type != SquareType.Start) return;

        GameManager.instance.RegisterStart(this);
        Colorify(color);
    }

    public void Drag(){
        if (GameManager.instance.haveWon)
            return;

        if(type == SquareType.Dead || !GameManager.instance.isDragging || GameManager.instance.comboSquares.Contains(this))
            return;

        GameManager.instance.RegisterMiddle(this);
    }
}
