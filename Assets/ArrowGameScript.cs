using UnityEngine;
using System.Collections;

public class ArrowGameScript : MonoBehaviour
{
    public GameObject GameSquare;
    public GameObject BlackFoneMenu;
    public GameObject Arrow;
    public float SpeedMenu;
    double Button_x1;
    double Button_x2;
    double Button_y1;
    double Button_y2;
    double Distance;
    Vector3 MousePos;
    bool ArrowInTheTop;

    // Use this for initialization
    void Start()
    {
        SetInTheFirstPos();
        Distance = 13.72f;
    }

    void HideArrow()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 1);
        ST.Instanse.ArrowIsActive = false;
    }

    void ShowArrow()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -2);
        ST.Instanse.ArrowIsActive = true;
    }



    void SetInTheFirstPos()
    {
        SpeedMenu = 0;
        ArrowInTheTop = true;
    }

    void BlackFoneMenuMove()
    {
        BlackFoneMenu.transform.position = new Vector3(BlackFoneMenu.transform.position.x, BlackFoneMenu.transform.position.y + SpeedMenu, BlackFoneMenu.transform.position.z);
    }

    void Init()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Button_x1 = Arrow.transform.localPosition.x - 0.4f;
        Button_x2 = Button_x1 + 0.8f;
        Button_y1 = Arrow.transform.localPosition.y - 0.4f;
        Button_y2 = Button_y1 + 0.8f;
    }

    void OnClick()
    {
        if (MousePos.x > Button_x1 && MousePos.x < Button_x2 &&
           MousePos.y > Button_y1 && MousePos.y < Button_y2)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ST.Instanse.ArrowIsActive = false;
                if (ST.Instanse.ArrowInTheTop)
                {
                    SpeedMenu = -0.4f;
                    BlackFoneMenuMove();
                    ST.Instanse.GameMenuOpened = true;
                }
                else
                {
                    SpeedMenu = 0.4f;
                    BlackFoneMenuMove();
                    ST.Instanse.GameMenuOpened = false;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Init();
        if (ST.Instanse.ArrowIsActive)
        {
            ShowArrow();
        }
        else
        {
            HideArrow();
        }
        OnClick();
        if (SpeedMenu != 0)
        {
            if (BlackFoneMenu.transform.position.y > 0 && BlackFoneMenu.transform.position.y < Distance)
            {
                BlackFoneMenuMove();
            }
            else
            {
                SpeedMenu = 0f;
                if (ST.Instanse.ArrowInTheTop)
                {
                    ShowArrow();
                    Arrow.transform.position = new Vector3(Arrow.transform.position.x, Arrow.transform.position.y * (-1), -2);
                    Arrow.transform.localScale = new Vector3(Arrow.transform.localScale.x, Arrow.transform.localScale.y * (-1), Arrow.transform.localScale.z);
                    ST.Instanse.ArrowInTheTop = false;
                }
                else
                {
                    ShowArrow();
                    Arrow.transform.position = new Vector3(Arrow.transform.position.x, Arrow.transform.position.y * (-1), -2);
                    Arrow.transform.localScale = new Vector3(Arrow.transform.localScale.x, Arrow.transform.localScale.y * (-1), Arrow.transform.localScale.z);
                    ST.Instanse.ArrowInTheTop = true;
                }
            }
        }

    }
}
