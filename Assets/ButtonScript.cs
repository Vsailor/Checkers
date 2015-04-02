using UnityEngine;
using System.Collections;
using System.IO;

public class ButtonScript : MonoBehaviour
{
    public GameObject MainBackground;
    public GameObject Button;
    Vector3 OldMainBackgroundPos;
    double Button_x1;
    double Button_x2;
    double Button_y1;
    double Button_y2;
    public float CameraMove;
    public Texture EnLightTexture;
    public Texture EnDarkTexture;
    public Texture UALightTexture;
    public Texture UADarkTexture;
    bool Language;
    bool Music;
    bool Sounds;

    Vector3 MousePos;
    // Use this for initialization
    void Start()
    {
        OldMainBackgroundPos = MainBackground.transform.position;
        CameraMove = 0;
    }
    void Init()
    {
        try
        {

            Language = ST.Instanse.Language;
            Music = ST.Instanse.Music;
            Sounds = ST.Instanse.Sounds;
        }
        catch (System.NullReferenceException)
        { }
        Button_x1 = Button.transform.localPosition.x - Mathf.Abs(Button.transform.localScale.x) * 0.5;
        Button_x2 = Button_x1 + Mathf.Abs(Button.transform.localScale.x);
        Button_y1 = Button.transform.localPosition.y - Mathf.Abs(Button.transform.localScale.y) * 0.5;
        Button_y2 = Button_y1 + Mathf.Abs(Button.transform.localScale.y);
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Language)
        {
            Button.GetComponent<Renderer>().material.mainTexture = EnDarkTexture;
        }
        else
        {
            Button.GetComponent<Renderer>().material.mainTexture = UADarkTexture;
        }

        if (Button.name == "SoundsButton" && !Sounds)
        {
            if (Language)
            {
                Button.GetComponent<Renderer>().material.mainTexture = EnDarkTexture;
            }
            else
            {
                Button.GetComponent<Renderer>().material.mainTexture = UADarkTexture;
            }
        }
        else if (Button.name == "SoundsButton" && Sounds)
        {
            if (Language)
            {
                Button.GetComponent<Renderer>().material.mainTexture = EnLightTexture;
            }
            else
            {
                Button.GetComponent<Renderer>().material.mainTexture = UALightTexture;
            }
        }


        if (Button.name == "MusicButton" && !Music)
        {
            if (Language)
            {
                Button.GetComponent<Renderer>().material.mainTexture = EnDarkTexture;
            }
            else
            {
                Button.GetComponent<Renderer>().material.mainTexture = UADarkTexture;
            }
        }
        else if (Button.name == "MusicButton" && Music)
        {
            if (Language)
            {
                Button.GetComponent<Renderer>().material.mainTexture = EnLightTexture;
            }
            else
            {
                Button.GetComponent<Renderer>().material.mainTexture = UALightTexture;
            }
        }
        if (!Sounds)
        {
            ST.Instanse.Music = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Init();


        if (MousePos.x > Button_x1 && MousePos.x < Button_x2 &&
            MousePos.y > Button_y1 && MousePos.y < Button_y2)
        {
            if (Language && (Button.name != "MusicButton" || Button.name != "SoundsButton"))
            {
                Button.GetComponent<Renderer>().material.mainTexture = EnLightTexture;
            }
            else
            {
                if (Button.name != "MusicButton" || Button.name != "SoundsButton")
                {
                    Button.GetComponent<Renderer>().material.mainTexture = UALightTexture;
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Button.name == "NewGameButton")
                {
                    ST.Instanse.Continue = false;
                    ST.Instanse.StartGame = true;
                    Camera.main.transform.position = new Vector3(-40.0f, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    File.WriteAllText("Save", string.Empty);
                }
                if (Button.name == "ResumeGameButton")
                {
                    if (Application.loadedLevelName == "Game")
                    {

                    }
                    else
                    {
                        ST.Instanse.Continue = true;
                        Application.LoadLevel("Game");
                    }
                }
                if (Button.name == "ExitButton")
                {
                    if (Application.loadedLevelName == "Cheskers")
                    {
                        Application.Quit();
                    }
                    else
                    {
                        Application.LoadLevel("Cheskers");
                    }
                }
                if (Button.name == "SettingsButton")
                {

                    CameraMove = 0.8f;
                }
                if (Button.name == "MainMenuButton")
                {

                    CameraMove = -0.8f;

                }
                if (Button.name == "LanguageButton")
                {
                    if (Language)
                    {
                        ST.Instanse.Language = false;
                    }
                    else
                    {
                        ST.Instanse.Language = true;
                    }
                    File.WriteAllText("Lang.cfg", ST.Instanse.Language.ToString());
                }
                if (Button.name == "MusicButton")
                {
                    if (Language)
                    {
                        if (Music)
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = EnDarkTexture;
                            ST.Instanse.Music = false;
                        }
                        else
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = EnLightTexture;
                            ST.Instanse.Music = true;
                        }
                    }
                    if (!Language)
                    {
                        if (Music)
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = UADarkTexture;
                            ST.Instanse.Music = false;
                        }
                        else
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = UALightTexture;
                            ST.Instanse.Music = true;
                        }
                    }
                }
                if (Button.name == "SoundsButton")
                {
                    if (Language)
                    {
                        if (ST.Instanse.Sounds)
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = EnDarkTexture;
                            ST.Instanse.Sounds = false;
                        }
                        else
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = EnLightTexture;
                            ST.Instanse.Sounds = true;
                        }
                    }
                    if (!Language)
                    {
                        if (Sounds)
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = UADarkTexture;
                            ST.Instanse.Sounds = false;
                        }
                        else
                        {
                            Button.GetComponent<Renderer>().material.mainTexture = UALightTexture;
                            ST.Instanse.Sounds = true;
                        }
                    }
                }
            }

        }

        if (Application.loadedLevelName == "Cheskers")
        {

            if ((CameraMove > 0 && Camera.main.transform.position.x < 26.3) || (CameraMove < 0 && Camera.main.transform.position.x > 0))
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + CameraMove, Camera.main.transform.position.y, Camera.main.transform.position.z);
                MainBackground.transform.position = new Vector3(MainBackground.transform.position.x + CameraMove, MainBackground.transform.position.y + Camera.main.transform.position.y, MainBackground.transform.position.z);
            }
            else
            {
                CameraMove = 0;
            }
        }

        if (Application.loadedLevelName == "Game")
        {

            if (OldMainBackgroundPos.y != MainBackground.transform.position.y)
            {
                if (OldMainBackgroundPos.y < MainBackground.transform.position.y)
                {
                    CameraMove = 0.4f;
                }
                else
                {
                    CameraMove = -0.4f;
                }
                Button.transform.position = new Vector3(Button.transform.position.x, Button.transform.position.y + CameraMove, Button.transform.position.z);
            }
            else
            {
                CameraMove = 0;
            }

            OldMainBackgroundPos = MainBackground.transform.position;
        }
    }
}
