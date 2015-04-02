using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameButtonScript : MonoBehaviour {
    public GameObject MusicButton;
    public GameObject SoundsButton;
    public GameObject MainMenuButton;
    public GameObject LanguageButton;
    public GameObject ResumeGameButton;
    public GameObject ExitButton;
    public GameObject NewGameButton;
    public GameObject SettingsButton;

    List<GameObject> Buttons;

	// Use this for initialization
	void Start () {
        Buttons = new List<GameObject>();
        Buttons.Add(MusicButton);
        Buttons.Add(SoundsButton);
        Buttons.Add(MainMenuButton);
        Buttons.Add(LanguageButton);
        Buttons.Add(ResumeGameButton);
        Buttons.Add(ExitButton);
        Buttons.Add(NewGameButton);
        Buttons.Add(SettingsButton);
	}
	
	// Update is called once per frame
	void Update () {
        
        if ((ST.Instanse.MenuGameMove < 0 && (NewGameButton.transform.position.x<0)) || (ST.Instanse.MenuGameMove > 0 && (LanguageButton.transform.position.x>0)))
        {
            GameObject obj;
            foreach (Object o in Buttons)
            {
                obj = o as GameObject;
                obj.transform.position = new Vector3(obj.transform.position.x - ST.Instanse.MenuGameMove, obj.transform.position.y, obj.transform.position.z);
            }
        }

	}
}
