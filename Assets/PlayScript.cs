using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class PlayScript : MonoBehaviour
{
    // Left bottom white figure
    public GameObject WhiteFigure;

    // Right top black figure
    public GameObject BlackFigure;

    List<GameObject> FiguresHaveToHit;
    public bool IsHaveToHit;

    // Distance between the nearest cells
    public const float DISTANCE = (float)(0.81);

    public GameObject RedSignal;
    public GameObject GreenSignal;
    List<GameObject> RedSignals;
    // All figures
    List<GameObject> WhiteCheckers;
    List<GameObject> BlackCheckers;

    // Mouse position in the screen coordinates
    Vector3 MousePos;

    // Checker coordinate in the l.b.corner
    Vector2 LeftBottomCorner;

    // Checker coordinate in the r.t.corner
    Vector2 RightTopCorner;

    const int SIZE_OF_MATRIX = 8;

    // Cheskers board. 1 - white checker, 2 - black checker, 0 - empty cell
    int[,] Array;
    GameObject obj;
    // If we can hit one more time
    bool MultiHit;

    // MouseDown - 1, MouseUp - 0
    bool MouseClicked;

    // White - 1, Black - 0
    bool WhiteMoveExpected;

    // Last checker position
    Vector3 ChosenCheckerOldPos;
    Vector2 OldIntCheckerPos;

    // Chosen chesker object
    GameObject ChosenChecker;
    // Now matrix coordinates
    int x, y;
    // Old matrix coordinates
    int old_x;
    int old_y;
    readonly string DebugFileName = System.DateTime.Now.Day.ToString() + "_" + System.DateTime.Now.Month.ToString() + "_" + System.DateTime.Now.Year.ToString() + "_" + System.DateTime.Now.Hour.ToString() + "_" + System.DateTime.Now.Minute.ToString() + "_" + System.DateTime.Now.Second.ToString();

    void SetRedSignalInCheckerPos(int x, int y)
    {
        bool exist = false;
        foreach (Object o in RedSignals)
        {
            obj = (GameObject)o;
            if (ConvertxToIntCoordinate(obj.transform.position.x) == x && ConvertyToIntCoordinate(obj.transform.position.y) == y)
            {
                exist = true;
            }
        }
        if (!exist)
        {
            obj = Instantiate(RedSignal);
            obj.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -0.25f);
            RedSignals.Add(obj);
        }
    }

    void SetGreenSignalInCheckerPos(int x, int y)
    {
        GreenSignal.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -0.25f);
    }

    void HideRedSignals()
    {
        for (int i = 0; i < RedSignals.Count; i++)
        {
            obj = RedSignals[i];
            RedSignals.RemoveAt(i);
            Destroy(obj);
        }
    }

    void HideGreenSignal()
    {
        GreenSignal.transform.position = new Vector3(GreenSignal.transform.position.x, GreenSignal.transform.position.y, 1f);
    }
    void SetCheckersInTheFirstPosition()
    {

        if (!ST.Instanse.Continue)
        {
            WhiteMoveExpected = true;
            Array = new int[8, 8];
            GameObject instanceWF = Instantiate(WhiteFigure);
            GameObject instanceBF = Instantiate(BlackFigure);
            WhiteFigure.transform.position = new Vector3(LeftBottomCorner.x, LeftBottomCorner.y, -1f);
            BlackFigure.transform.position = new Vector3(RightTopCorner.x, RightTopCorner.y, -1f);
            WhiteCheckers.Add(WhiteFigure);
            Array[0, 0] = 1;
            BlackCheckers.Add(BlackFigure);
            Array[7, 7] = 2;
            for (int i = 0; i < 4; i++)
            {

                if (i != 0)
                {
                    obj = Instantiate(WhiteFigure);
                    obj.transform.position = new Vector3(WhiteFigure.transform.position.x + DISTANCE * 2 * i, WhiteFigure.transform.position.y, WhiteFigure.transform.position.z);
                    WhiteCheckers.Add(obj);
                    Array[0, i * 2] = 1;
                    obj = Instantiate(BlackFigure);
                    obj.transform.position = new Vector3(BlackFigure.transform.position.x + DISTANCE * 2 * (-i), BlackFigure.transform.position.y, BlackFigure.transform.position.z);
                    BlackCheckers.Add(obj);
                    Array[7, i * 2 - 1] = 2;
                }
                obj = Instantiate(WhiteFigure);
                obj.transform.position = new Vector3(WhiteFigure.transform.position.x + DISTANCE * 2 * i + DISTANCE, WhiteFigure.transform.position.y + DISTANCE, WhiteFigure.transform.position.z);
                WhiteCheckers.Add(obj);
                Array[1, i * 2 + 1] = 1;
                obj = Instantiate(WhiteFigure);
                obj.transform.position = new Vector3(WhiteFigure.transform.position.x + DISTANCE * 2 * i, WhiteFigure.transform.position.y + DISTANCE * 2, WhiteFigure.transform.position.z);
                WhiteCheckers.Add(obj);
                Array[2, i * 2] = 1;
                obj = Instantiate(BlackFigure);
                obj.transform.position = new Vector3(BlackFigure.transform.position.x + DISTANCE * 2 * (-i) - DISTANCE, BlackFigure.transform.position.y - DISTANCE, BlackFigure.transform.position.z);
                BlackCheckers.Add(obj);
                Array[6, i * 2] = 2;
                obj = Instantiate(BlackFigure);
                obj.transform.position = new Vector3(BlackFigure.transform.position.x + DISTANCE * 2 * (-i), BlackFigure.transform.position.y - DISTANCE * 2, BlackFigure.transform.position.z);
                BlackCheckers.Add(obj);
                Array[5, i * 2 + 1] = 2;
            }
            WhiteFigure = instanceWF;
            BlackFigure = instanceBF;
            //Save();
        }

    }

    /// <summary>
    /// Convert matrix chessboard 'x' coordinate to world 'x' coordinate
    /// </summary>
    /// <param name="coord">Chessboard 'x' coordinate</param>
    /// <returns>Converted 'x' coordinate</returns>
    float ConvertxToFloatCoordinate(int coord)
    {
        return LeftBottomCorner.x + coord * DISTANCE;
    }

    /// <summary>
    /// Convert matrix chessboard 'y' coordinate to world 'y' coordinate
    /// </summary>
    /// <param name="coord">Chessboard 'y' coordinate</param>
    /// <returns>Converted 'y' coordinate</returns>
    float ConvertyToFloatCoordinate(int coord)
    {
        return LeftBottomCorner.y + coord * DISTANCE;
    }

    /// <summary>
    /// Convert world coordinate to matrix array coordinate
    /// </summary>
    /// <param name="coord">World coordinate</param>
    /// <returns>Matrix chessboard coordinate</returns>
    int ConvertxToIntCoordinate(float coord)
    {
        return (int)((coord - LeftBottomCorner.x) / (DISTANCE) + DISTANCE * 0.5);
    }
    int ConvertyToIntCoordinate(float coord)
    {
        return (int)((coord - LeftBottomCorner.y) / (DISTANCE) + DISTANCE * 0.5);
    }

    /// <summary>
    /// Change WhiteMoveExpected to the oposit value
    /// </summary>
    void WhiteMoveExpectedChange()
    {
        if (WhiteMoveExpected)
        {
            WhiteMoveExpected = false;
        }
        else
        {
            WhiteMoveExpected = true;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    void Start2()
    {
        LeftBottomCorner = ST.Instanse.LeftBottomCorner;
        RightTopCorner = ST.Instanse.RightTopCorner;
        if (ST.Instanse.Continue)
        {
            Vector3 v;
            int wc = WhiteCheckers.Count;
            int bc = BlackCheckers.Count;
            for (int i = wc - 1; i >= 0; i--)
            {
                v = WhiteCheckers[i].transform.position;
                DeleteChecker(ConvertxToIntCoordinate(v.x), ConvertyToIntCoordinate(v.y), false);
            }
            for (int i = bc - 1; i >= 0; i--)
            {
                v = BlackCheckers[i].transform.position;
                DeleteChecker(ConvertxToIntCoordinate(v.x), ConvertyToIntCoordinate(v.y), true);
            }
            ST.Instanse.Continue = false;
        }
        if (RedSignals != null)
        {
            HideRedSignals();
        }
        if (GreenSignal != null)
        {
            HideGreenSignal();
        }
        WhiteCheckers = new List<GameObject>();
        BlackCheckers = new List<GameObject>();
        MouseClicked = false;
        MultiHit = false;
        IsHaveToHit = false;
        SetCheckersInTheFirstPosition();
        /*if (!File.Exists("Save"))
        {
            File.WriteAllText("Save", string.Empty);
        }*/
        obj = new GameObject();
        FiguresHaveToHit = new List<GameObject>();
        RedSignals = new List<GameObject>();
    }

    /// <summary>
    /// Checking if we can hit back
    /// </summary>
    /// <returns>Return true if we can</returns>
    bool CanHitBack()
    {
        int x = ConvertxToIntCoordinate(ChosenChecker.transform.position.x);
        int y = ConvertyToIntCoordinate(ChosenChecker.transform.position.y);
        if (WhiteMoveExpected)
        {
            if (CheckBlackChecker(x - 1, y - 1) && CheckEmptyCell(x - 2, y - 2))
            {
                if (Array[y - 1, x - 1] == 2 && Array[y - 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckBlackChecker(x + 1, y - 1) && CheckEmptyCell(x + 2, y - 2))
            {
                if (Array[y - 1, x + 1] == 2 && Array[y - 2, x + 2] == 0)
                {
                    return true;
                }
            }
        }
        else
        {
            if (CheckWhiteChecker(x - 1, y + 1) && CheckEmptyCell(x - 2, y + 2))
            {
                if (Array[y + 1, x - 1] == 1 && Array[y + 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckWhiteChecker(x + 1, y + 1) && CheckEmptyCell(x + 2, y + 2))
            {
                if (Array[y + 1, x + 1] == 1 && Array[y + 2, x + 2] == 0)
                {
                    return true;
                }
            }

        }
        return false;
    }

    /// <summary>
    /// Checking if we can hit
    /// </summary>
    /// <returns>Return true if we can</returns>
    bool CanHit()
    {

        int x = ConvertxToIntCoordinate(ChosenChecker.transform.position.x);
        int y = ConvertyToIntCoordinate(ChosenChecker.transform.position.y);
        if (WhiteMoveExpected)
        {
            if (CheckBlackChecker(x - 1, y + 1) && CheckEmptyCell(x - 2, y + 2))
            {
                if (Array[y + 1, x - 1] == 2 && Array[y + 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckBlackChecker(x - 1, y - 1) && CheckEmptyCell(x - 2, y - 2))
            {
                if (Array[y - 1, x - 1] == 2 && Array[y - 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckBlackChecker(x + 1, y + 1) && CheckEmptyCell(x + 2, y + 2))
            {
                if (Array[y + 1, x + 1] == 2 && Array[y + 2, x + 2] == 0)
                {
                    return true;
                }
            }
            if (CheckBlackChecker(x + 1, y - 1) && CheckEmptyCell(x + 2, y - 2))
            {
                if (Array[y - 1, x + 1] == 2 && Array[y - 2, x + 2] == 0)
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            if (CheckWhiteChecker(x - 1, y - 1) && CheckEmptyCell(x - 2, y - 2))
            {
                if (Array[y - 1, x - 1] == 1 && Array[y - 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckWhiteChecker(x - 1, y + 1) && CheckEmptyCell(x - 2, y + 2))
            {
                if (Array[y + 1, x - 1] == 1 && Array[y + 2, x - 2] == 0)
                {
                    return true;
                }
            }
            if (CheckWhiteChecker(x + 1, y - 1) && CheckEmptyCell(x + 2, y - 2))
            {
                if (Array[y - 1, x + 1] == 1 && Array[y - 2, x + 2] == 0)
                {
                    return true;
                }
            }
            if (CheckWhiteChecker(x + 1, y + 1) && CheckEmptyCell(x + 2, y + 2))
            {
                if (Array[y + 1, x + 1] == 1 && Array[y + 2, x + 2] == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // x,y in matrix coordinates, white - 1, black = 0
    void DeleteChecker(int x, int y, bool color)
    {
        GameObject obj;
        if (color)
        {
            for (int i = 0; i < BlackCheckers.Count; i++)
            {
                obj = BlackCheckers[i];
                if (x == ConvertxToIntCoordinate(obj.transform.position.x) && y == ConvertyToIntCoordinate(obj.transform.position.y))
                {
                    Array[y, x] = 0;
                    BlackCheckers.RemoveAt(i);
                    Destroy(obj);

                }
            }
        }
        else
        {
            for (int i = 0; i < WhiteCheckers.Count; i++)
            {
                obj = WhiteCheckers[i];
                if (x == ConvertxToIntCoordinate(obj.transform.position.x) && y == ConvertyToIntCoordinate(obj.transform.position.y))
                {
                    Array[y, x] = 0;
                    WhiteCheckers.RemoveAt(i);
                    Destroy(obj);
                }
            }
        }
    }

    [System.Serializable]
    struct Point
    {
        public float x, y;
        public bool WhiteMove;
    }
    // Save game
    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        List<Point> wc = new List<Point>();
        List<Point> bc = new List<Point>();

        Point p;

        foreach (Object o in WhiteCheckers)
        {
            obj = o as GameObject;
            p.x = obj.transform.position.x;
            p.y = obj.transform.position.y;
            p.WhiteMove = WhiteMoveExpected;
            wc.Add(p);
        }
        foreach (Object o in BlackCheckers)
        {
            obj = o as GameObject;
            p.x = obj.transform.position.x;
            p.y = obj.transform.position.y;
            p.WhiteMove = WhiteMoveExpected;
            bc.Add(p);
        }
        using (var fStream = new FileStream(System.Environment.CurrentDirectory + @"\Save", FileMode.Create, FileAccess.Write, FileShare.None))
        {
            formatter.Serialize(fStream, wc);
            formatter.Serialize(fStream, bc);
        }

    }


    void DebugSave()
    {
        string s;
        for (int i = SIZE_OF_MATRIX - 1; i >= 0; i--)
        {
            s = string.Empty;
            for (int j = 0; j < SIZE_OF_MATRIX; j++)
            {
                if (Array[i, j] == 1)
                {
                    s += "@";
                }
                if (Array[i, j] == 2)
                {
                    s += "#";
                }
                if (Array[i, j] == 0)
                {
                    s += "-";
                }
            }
            File.AppendAllText(System.Environment.CurrentDirectory + @"\DebugSave\" + DebugFileName + ".log", s + System.Environment.NewLine);
        }
        File.AppendAllText(System.Environment.CurrentDirectory + @"\DebugSave\" + DebugFileName + ".log", "----------------------------" + System.Environment.NewLine);

    }

    // Load game
    void Load()
    {
        Array = new int[8, 8];
        BinaryFormatter formatter = new BinaryFormatter();
        List<Point> wc;
        List<Point> bc;

        using (FileStream inStr = new FileStream(System.Environment.CurrentDirectory + @"\Save", FileMode.Open))
        {
            wc = formatter.Deserialize(inStr) as List<Point>;
            bc = formatter.Deserialize(inStr) as List<Point>;
        }
        WhiteCheckers = new List<GameObject>();
        BlackCheckers = new List<GameObject>();
        GameObject obj;
        foreach (Point p in wc)
        {
            WhiteMoveExpected = p.WhiteMove;
            obj = Instantiate(WhiteFigure);
            obj.transform.position = new Vector3(p.x, p.y, -1);
            Array[ConvertyToIntCoordinate(p.y), ConvertxToIntCoordinate(p.x)] = 1;
            WhiteCheckers.Add(obj);

        }
        foreach (Point p in bc)
        {
            WhiteMoveExpected = p.WhiteMove;
            obj = Instantiate(BlackFigure);
            obj.transform.position = new Vector3(p.x, p.y, -1);
            Array[ConvertyToIntCoordinate(p.y), ConvertxToIntCoordinate(p.x)] = 2;
            BlackCheckers.Add(obj);
        }

    }
    //Go to position in the array
    void GoTo(int x, int y)
    {
        HideGreenSignal();

        if (MouseClicked && Array[y, x] == 0)
        {
            Array[ConvertyToIntCoordinate(ChosenCheckerOldPos.y), ConvertxToIntCoordinate(ChosenCheckerOldPos.x)] = 0;
            if (WhiteMoveExpected)
            {
                Array[y, x] = 1;
            }
            else
            {
                Array[y, x] = 2;
            }
            MouseClicked = false;
            ChosenChecker.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -1);
        }

    }

    // Return true if white checker in position
    bool CheckWhiteChecker(int posX, int posY)
    {
        if (posX < SIZE_OF_MATRIX && posY < SIZE_OF_MATRIX && posX >= 0 && posY >= 0)
        {
            if (Array[posY, posX] == 1)
                return true;
        }
        return false;
    }

    // Return true if black checker in position
    bool CheckBlackChecker(int posX, int posY)
    {
        if (posX < SIZE_OF_MATRIX && posY < SIZE_OF_MATRIX && posX >= 0 && posY >= 0)
        {
            if (Array[posY, posX] == 2)
                return true;
        }
        return false;
    }

    // Return true if position is empty
    bool CheckEmptyCell(int posX, int posY)
    {
        if (posX < SIZE_OF_MATRIX && posY < SIZE_OF_MATRIX && posX >= 0 && posY >= 0)
        {
            if (Array[posY, posX] == 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Method checks if a mouse is over the area
    /// </summary>
    /// <returns>Return true if the mouse is over the chessboard</returns>
    bool MousePosIsInTheGameSquare()
    {
        if (MousePos.x > LeftBottomCorner.x - DISTANCE * 0.5 && MousePos.y < RightTopCorner.y + DISTANCE * 0.5 && MousePos.x < RightTopCorner.x + DISTANCE * 0.5 && MousePos.y > LeftBottomCorner.y - DISTANCE * 0.5)
        {

            return true;
        }
        else
        {

            return false;
        }
    }

    void TryToChooseChecker(List<GameObject> checkers)
    {
        foreach (Object o in checkers)
        {
            obj = o as GameObject;
            if (Mathf.Abs(obj.transform.position.x - MousePos.x) < DISTANCE / 2 && Mathf.Abs(obj.transform.position.y - MousePos.y) < DISTANCE / 2)
            {
                MouseClicked = true;
                ChosenChecker = obj;
                ChosenCheckerOldPos = obj.transform.position;
                SetGreenSignalInCheckerPos(ConvertxToIntCoordinate(ChosenChecker.transform.position.x), ConvertyToIntCoordinate(ChosenChecker.transform.position.y));
                break;
            }
        }
    }
    void TryToChooseCheckerToHit()
    {
        HideRedSignals();
        foreach (Object o in FiguresHaveToHit)
        {
            obj = o as GameObject;
            if (Mathf.Abs(obj.transform.position.x - MousePos.x) < DISTANCE / 2 && Mathf.Abs(obj.transform.position.y - MousePos.y) < DISTANCE / 2)
            {
                ChosenChecker = obj;
                if (CanHit() || CanHitBack())
                {
                    MouseClicked = true;
                    ChosenChecker = obj;
                    ChosenCheckerOldPos = obj.transform.position;
                    SetGreenSignalInCheckerPos(ConvertxToIntCoordinate(ChosenChecker.transform.position.x), ConvertyToIntCoordinate(ChosenChecker.transform.position.y));

                }
            }
        }
        foreach (Object o in FiguresHaveToHit)
        {
            obj = o as GameObject;
            if (!MouseClicked)
            {
                SetRedSignalInCheckerPos(ConvertxToIntCoordinate(obj.transform.position.x), ConvertyToIntCoordinate(obj.transform.position.y));
            }
            else
            {
                HideRedSignals();
            }
        }

    }

    void MoveCheckerAfterTheCursor()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ChosenChecker.transform.position = new Vector3(pos.x, pos.y, -2);
    }

    void Hit(int kill_x, int kill_y)
    {
        DeleteChecker(kill_x, kill_y, WhiteMoveExpected);
        GoTo(x, y);
        if (CanHit() || CanHitBack())
        {
            ChosenCheckerOldPos = ChosenChecker.transform.position;
            MouseClicked = true;
            MultiHit = true;
        }
        else
        {
            MultiHit = false;
            WhiteMoveExpectedChange();
            HideRedSignals();
        }
        FiguresHaveToHit.Clear();
        //Save();
    }

    bool FindFigureToHit(int x, int y, List<GameObject> figures)
    {
        foreach (Object o in figures)
        {
            obj = o as GameObject;
            if (ConvertxToIntCoordinate(obj.transform.position.x) == x && ConvertyToIntCoordinate(obj.transform.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    bool TryHit()
    {
        if ((x - old_x == 2) &&
           ((WhiteMoveExpected && CheckWhiteChecker(x - 2, y + 2) && CheckBlackChecker(x - 1, y + 1)) ||
           (!WhiteMoveExpected && CheckBlackChecker(x - 2, y + 2) && CheckWhiteChecker(x - 1, y + 1))))
        {
            Hit(x - 1, y + 1);
            return true;
        }
        if (old_x - x == 2 &&
            ((WhiteMoveExpected && CheckWhiteChecker(x + 2, y + 2) && CheckBlackChecker(x + 1, y + 1)) ||
            (!WhiteMoveExpected && CheckBlackChecker(x + 2, y + 2) && CheckWhiteChecker(x + 1, y + 1))))
        {
            Hit(x + 1, y + 1);
            return true;
        }

        if (x - old_x == 2 &&
            (!WhiteMoveExpected && CheckBlackChecker(x - 2, y - 2) && CheckWhiteChecker(x - 1, y - 1)) ||
            (WhiteMoveExpected && CheckWhiteChecker(x - 2, y - 2) && CheckBlackChecker(x - 1, y - 1)))
        {
            Hit(x - 1, y - 1);
            return true;
        }
        if (old_x - x == 2 &&
            (!WhiteMoveExpected && CheckBlackChecker(x + 2, y - 2) && CheckWhiteChecker(x + 1, y - 1)) ||
            (WhiteMoveExpected && CheckWhiteChecker(x + 2, y - 2) && CheckBlackChecker(x + 1, y - 1)))
        {
            Hit(x + 1, y - 1);
            return true;
        }
        return false;
    }


    bool Click()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            return true;
        }
        return false;
    }

    // Initialize variables every moment
    void Init()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        OldIntCheckerPos = new Vector2(ConvertxToIntCoordinate(ChosenCheckerOldPos.x), ConvertyToIntCoordinate(ChosenCheckerOldPos.y));
    }


    bool HaveToHit(List<GameObject> checkers)
    {
        FiguresHaveToHit.Clear();
        IsHaveToHit = false;
        foreach (Object o in checkers)
        {
            obj = o as GameObject;
            ChosenChecker = obj;
            if (CanHit() || CanHitBack())
            {
                IsHaveToHit = true;
                FiguresHaveToHit.Add(obj);
            }
        }
        if (IsHaveToHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // General playing method
    void Playing()
    {
        // White checker choose
        if (WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {

            if (!HaveToHit(WhiteCheckers))
            {

                TryToChooseChecker(WhiteCheckers);
                HideRedSignals();

            }
            else
            {

                TryToChooseCheckerToHit();
            }
            return;
        }

        // Black checker choose
        if (!WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {
            if (!HaveToHit(BlackCheckers))
            {
                TryToChooseChecker(BlackCheckers);
            }
            else
            {
                TryToChooseCheckerToHit();
            }
            return;
        }

        // Checker move
        if (!Click() && MouseClicked)
        {

            /*
            MoveCheckerAfterTheCursor();
            return;*/
        }

        // Checker set
        if (Click() && MouseClicked)
        {
            old_x = ConvertxToIntCoordinate(ChosenCheckerOldPos.x);
            old_y = ConvertyToIntCoordinate(ChosenCheckerOldPos.y);

            // If we in the game square
            if (MousePosIsInTheGameSquare())
            {
                x = ConvertxToIntCoordinate(MousePos.x);
                y = ConvertyToIntCoordinate(MousePos.y);

                // Maked a mistake with checker setting
                /*
                if (!MultiHit && x == old_x && y == old_y)
                {
                    ChosenChecker.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -1);
                    MouseClicked = false;
                    return;
                }*/
                if (!MultiHit && WhiteMoveExpected && CheckWhiteChecker(x, y))
                {
                    TryToChooseChecker(WhiteCheckers);
                    SetGreenSignalInCheckerPos(x, y);
                    return;
                }
                if (!MultiHit && !WhiteMoveExpected && CheckBlackChecker(x, y))
                {
                    TryToChooseChecker(BlackCheckers);
                    SetGreenSignalInCheckerPos(x, y);
                    return;
                }
                if (MultiHit)
                {
                    SetGreenSignalInCheckerPos(ConvertxToIntCoordinate(ChosenChecker.transform.position.x), ConvertyToIntCoordinate(ChosenChecker.transform.position.y));
                }

                // Chosen cell is empty
                if (CheckEmptyCell(x, y))
                {
                    // One step
                    if (!IsHaveToHit && Mathf.Abs(x - old_x) == 1 && Mathf.Abs(y - old_y) == 1)
                    {

                        if ((WhiteMoveExpected && old_y < y) || (!WhiteMoveExpected && old_y > y))
                        {
                            GoTo(x, y);
                            WhiteMoveExpectedChange();
                            //Save();
                            return;
                        }
                    }
                    // Hit
                    if (Mathf.Abs(x - old_x) == 2 && Mathf.Abs(y - old_y) == 2)
                    {
                        if (TryHit())
                        {
                            return;
                        }
                    }
                }
            }

        }

    }
    void StartGameWithPlayer()
    {
        SetCheckersInTheFirstPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (ST.Instanse.GameStarted)
        {
            Init();
            Playing();


        }
        if (ST.Instanse.StartGame)
        {
            Start2();
            ST.Instanse.StartGame = false;
            ST.Instanse.GameStarted = true;
        }

    }
}
