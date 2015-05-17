using UnityEngine;
using System.Collections;
using System.IO;

public class ST : MonoBehaviour {
    public AudioSource Audio;
    // 1 - white, 0 - black, -1 - NoWinner
    public int Winner;
    public static ST Instanse;
    // 1 - en, 0 - ua
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
        Winner = -1;
        LoadFromSave = false;
        Music = false;
        Sounds = true;
        LeftBottomCorner = new Vector2(-42.88f, -2.83f);
        RightTopCorner = new Vector2(-37.21f, 2.83f);
        GameMenuOpened = false;
        MenuGameMove = 0;

            Language = true;
        
        SpeedMenu = 0;
        ArrowInTheTop = true;
        ArrowIsActive = true;
        StartGame = false;
        GameStarted = false;
	}
	
	// Update is called once per frame
	void Update () {
        Instanse = this;
        if (Music)
        {
            Audio.mute = false;
        }
        else
        {
            Audio.mute = true;
        }
	}
}
