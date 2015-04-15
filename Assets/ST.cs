using UnityEngine;
using System.Collections;
using System.IO;

public class ST : MonoBehaviour {
    public static ST Instanse;
    public bool Language;
    public bool Music;
    public bool Sounds;
    public bool Continue;
    public bool LoadFromSave;
    public Vector2 LeftBottomCorner;
    public Vector2 RightTopCorner;
    public bool GameMenuOpened;
    // Horisontal move
    public float MenuGameMove;
    // Vertical move
    public float SpeedMenu;
    public bool ArrowIsActive;
    public bool ArrowInTheTop;
    public string DebugFileName;
    public bool StartGame;
    public bool GameStarted;
	// Use this for initialization
	void Start () {
        LoadFromSave = false;
        Music = true;
        Sounds = true;
        LeftBottomCorner = new Vector2(-42.88f, -2.83f);
        RightTopCorner = new Vector2(-37.21f, 2.83f);
        GameMenuOpened = false;
        MenuGameMove = 0;
        Language = System.Convert.ToBoolean(File.ReadAllText("Lang.cfg"));
        SpeedMenu = 0;
        ArrowInTheTop = true;
        ArrowIsActive = true;
        StartGame = false;
        GameStarted = false;
	}
	
	// Update is called once per frame
	void Update () {
        Instanse = this;
	}
}
