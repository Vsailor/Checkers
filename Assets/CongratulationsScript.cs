using UnityEngine;
using System.Collections;

public class CongratulationsScript : MonoBehaviour {
    public GameObject BlackUA;
    public GameObject BlackEN;
    public GameObject WhiteUA;
    public GameObject WhiteEN;
    public GameObject GameMainMenuButton;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        if (ST.Instanse.Winner != -1 && Camera.main.transform.position.x == -40.04f)
        {
            ST.Instanse.GameStarted = false;
            this.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -3f);
            switch (ST.Instanse.Winner)
            {
                case 0:
                    if (ST.Instanse.Language)
                    {
                        BlackEN.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 1f, -4f);
                    }
                    else
                    {
                        BlackUA.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 1f, -4f);
                    }
                    break;
                case 1:
                    if (ST.Instanse.Language)
                    {
                        WhiteEN.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 1f, -4f);
                    }
                    else
                    {
                        WhiteUA.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 1f, -4f);
                    }
                    break;
            }
            GameMainMenuButton.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y-4.5f, -4f);
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 1f);
            BlackEN.transform.position = new Vector3(BlackEN.transform.position.x, BlackEN.transform.position.y, 1f);
            BlackUA.transform.position = new Vector3(BlackUA.transform.position.x, BlackUA.transform.position.y, 1f);
            WhiteEN.transform.position = new Vector3(WhiteEN.transform.position.x, WhiteEN.transform.position.y, 1f);
            WhiteUA.transform.position = new Vector3(WhiteUA.transform.position.x, WhiteUA.transform.position.y, 1f);
            GameMainMenuButton.transform.position = new Vector3(GameMainMenuButton.transform.position.x, 10f, GameMainMenuButton.transform.position.z);
        }
	}
}
