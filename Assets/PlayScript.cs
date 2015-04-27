using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class PlayScript : MonoBehaviour
{
    public GameObject WhiteFigure;

    public GameObject BlackFigure;

    public GameObject WhiteQueen;
    public GameObject BlackQueen;

    List<GameObject> FiguresHaveToHit;
    public bool IsHaveToHit;

    // Distance between the nearest cells
    public const float DISTANCE = (float)(0.81);
    public GameObject HelpSignal;
    public GameObject RedSignal;
    public GameObject GreenSignal;
    List<GameObject> RedSignals;
    // All figures
    List<GameObject> HelpSignals;
    List<GameObject> WhiteCheckers;
    List<GameObject> BlackCheckers;
    List<GameObject> WhiteQueens;
    List<GameObject> BlackQueens;
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
    string DebugFileName;
    
    List<GameObject> BlackCheckersKilled;
    List<GameObject> WhiteCheckersKilled;

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
        if (ST.Instanse.LoadFromSave)
        {
            Load();
            ST.Instanse.LoadFromSave = false;
        }
        else
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
                Save();
            }
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

    void ShowHelpSignals(bool color)
    {
        if (HelpSignals.Count == 0)
        {
            if (color)
            {
                foreach (var v in WhiteCheckers)
                {
                    HelpSignals.Add(Instantiate(HelpSignal));
                    HelpSignals[HelpSignals.Count - 1].transform.position = new Vector3(v.transform.position.x, v.transform.position.y, -0.25f);
                }
            }
            else
            {
                foreach (var v in BlackCheckers)
                {
                    HelpSignals.Add(Instantiate(HelpSignal));
                    HelpSignals[HelpSignals.Count - 1].transform.position = new Vector3(v.transform.position.x, v.transform.position.y, -0.25f);
                }
            }
        }
    }

    void HideHelpSignals()
    {
        for (int i = 0; i < HelpSignals.Count; i++)
        {
            Destroy(HelpSignals[i]);
        }
        HelpSignals.Clear();
    }

    void Start2()
    {
        LeftBottomCorner = ST.Instanse.LeftBottomCorner;
        RightTopCorner = ST.Instanse.RightTopCorner;

        if (WhiteCheckers != null || BlackCheckers != null || WhiteQueens != null || BlackQueens != null)
        {
            Vector3 v;
            if (WhiteCheckers != null)
            {
                int wc = WhiteCheckers.Count;
                for (int i = wc - 1; i >= 0; i--)
                {
                    v = WhiteCheckers[i].transform.position;
                    DeleteChecker(ConvertxToIntCoordinate(v.x), ConvertyToIntCoordinate(v.y), false);
                }
            }
            if (BlackCheckers != null)
            {
                int bc = BlackCheckers.Count;
                for (int i = bc - 1; i >= 0; i--)
                {
                    v = BlackCheckers[i].transform.position;
                    DeleteChecker(ConvertxToIntCoordinate(v.x), ConvertyToIntCoordinate(v.y), true);
                }
            }
            if (WhiteQueens != null)
            {
                for (int i = 0; i < WhiteQueens.Count; i++)
                {
                    Destroy(WhiteQueens[i]);
                }
                WhiteQueens.Clear();
            }
            if (BlackQueens != null)
            {
                for (int i = 0; i < BlackQueens.Count; i++)
                {
                    Destroy(BlackQueens[i]);
                }
                BlackQueens.Clear();
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


        if (WhiteCheckersKilled != null)
        {
            for (int i = 0; i < WhiteCheckersKilled.Count; i++)
            {
                Destroy(WhiteCheckersKilled[i]);
            }
            WhiteCheckersKilled.Clear();
        }

        if (BlackCheckersKilled != null)
        {
            for (int i = 0; i < BlackCheckersKilled.Count; i++)
            {
                Destroy(BlackCheckersKilled[i]);
            }
            BlackCheckersKilled.Clear();
        }
        if (HelpSignals != null)
        {
            HideHelpSignals();
        }
        WhiteQueens = new List<GameObject>();
        BlackQueens = new List<GameObject>();
        WhiteCheckers = new List<GameObject>();
        BlackCheckers = new List<GameObject>();
        WhiteCheckersKilled = new List<GameObject>();
        BlackCheckersKilled = new List<GameObject>();
        HelpSignals = new List<GameObject>();
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
        }


        if (ChosenQueen())
        {
            int qx = x;
            int qy = y;
            for (int i = 1; x + i < 8 && y + i < 8; i++)
            {
                qx = x + i;
                qy = y + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (WhiteMoveExpected && CheckBlackChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2))
                    {
                        return true;
                    }
                    if (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; x - i >= 0 && y - i >= 0; i--)
            {
                qx = x - i;
                qy = y - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (WhiteMoveExpected && CheckBlackChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2))
                    {
                        return true;
                    }
                    if (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; x + i < 8 && y - i >= 0; i++)
            {
                qx = x + i;
                qy = y - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (WhiteMoveExpected && CheckBlackChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2))
                    {
                        return true;
                    }
                    if (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; x - i >= 0 && y + i < 8; i++)
            {
                qx = x - i;
                qy = y + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (WhiteMoveExpected && CheckBlackChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2))
                    {
                        return true;
                    }
                    if (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        return false;
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
                    GameObject o;
                    for (int j = 0; j < BlackQueens.Count; j++)
                    {
                        o = BlackQueens[j];
                        if (x == ConvertxToIntCoordinate(o.transform.position.x) && y == ConvertyToIntCoordinate(o.transform.position.y))
                        {
                            BlackQueens.RemoveAt(j);
                            j = 0;
                            o.transform.position = new Vector3(o.transform.position.x, o.transform.position.y, 3);
                            Destroy(o);
                            print("Black queens count = " + BlackQueens.Count);

                        }
                    }
                    Destroy(obj);
                    i = 0;
                    print("Black checkers count = " + BlackCheckers.Count);

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
                    GameObject o;
                    for (int j = 0; j < WhiteQueens.Count; j++)
                    {
                        o = WhiteQueens[j];
                        if (x == ConvertxToIntCoordinate(o.transform.position.x) && y == ConvertyToIntCoordinate(o.transform.position.y))
                        {
                            WhiteQueens.RemoveAt(j);
                            o.transform.position = new Vector3(o.transform.position.x, o.transform.position.y, 3);
                            Destroy(o);
                            j = 0;
                            print("White queens count = " + WhiteQueens.Count);
                        }
                    }
                    Destroy(obj);
                    i = 0;
                    print("White checkers count = " + WhiteCheckers.Count);
                }
            }
        }
    }
    [System.Serializable]
    struct KilledChecker
    {
        public float x, y, z;
        public bool isQueen;
        public bool color;
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
        List<Point> wq = new List<Point>();
        List<Point> bq = new List<Point>();
        Point p;
        KilledChecker k;
        List<KilledChecker> l = new List<KilledChecker>();
        foreach (var v in WhiteCheckersKilled)
        {
            k.x = v.transform.position.x;
            k.y = v.transform.position.y;
            k.z = v.transform.position.z;
            if (v.name.CompareTo("WhiteQueen") == 1)
            {
                k.isQueen = true;
            }
            else
            {
                k.isQueen = false;
            }
            k.color = true;
            l.Add(k);
        }
        foreach (var v in BlackCheckersKilled)
        {
            k.x = v.transform.position.x;
            k.y = v.transform.position.y;
            k.z = v.transform.position.z;
            if (v.name.CompareTo("BlackQueen") == 1)
            {
                k.isQueen = true;
            }
            else
            {
                k.isQueen = false;
            }
            k.color = false;
            l.Add(k);
        }
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
        foreach (Object o in WhiteQueens)
        {
            obj = o as GameObject;
            p.x = obj.transform.position.x;
            p.y = obj.transform.position.y;
            p.WhiteMove = WhiteMoveExpected;
            wq.Add(p);
        }
        foreach (Object o in BlackQueens)
        {
            obj = o as GameObject;
            p.x = obj.transform.position.x;
            p.y = obj.transform.position.y;
            p.WhiteMove = WhiteMoveExpected;
            bq.Add(p);
        }
        using (var fStream = new FileStream(Application.persistentDataPath + @"\Save", FileMode.Create, FileAccess.Write, FileShare.None))
        {
            formatter.Serialize(fStream, wc);
            formatter.Serialize(fStream, bc);
            formatter.Serialize(fStream, wq);
            formatter.Serialize(fStream, bq);
            formatter.Serialize(fStream, l);

        }

    }




    void DebugSave()
    {
        string s;
        if (!File.Exists(Application.persistentDataPath + DebugFileName + ".log"))
        {
            (new FileStream(Application.persistentDataPath + DebugFileName + ".log", FileMode.Create, FileAccess.Write, FileShare.None)).Close();
        }
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
            File.AppendAllText(Application.persistentDataPath + DebugFileName + ".log", s + System.Environment.NewLine);
        }
        File.AppendAllText(Application.persistentDataPath + DebugFileName + ".log", "----------------------------" + System.Environment.NewLine);

    }

    // Load game
    void Load()
    {
        Array = new int[8, 8];
        BinaryFormatter formatter = new BinaryFormatter();
        List<Point> wc;
        List<Point> bc;
        List<Point> wq;
        List<Point> bq;
        List<KilledChecker> l;
        using (FileStream inStr = new FileStream(Application.persistentDataPath + @"\Save", FileMode.Open))
        {
            wc = formatter.Deserialize(inStr) as List<Point>;
            bc = formatter.Deserialize(inStr) as List<Point>;
            wq = formatter.Deserialize(inStr) as List<Point>;
            bq = formatter.Deserialize(inStr) as List<Point>;
            l = formatter.Deserialize(inStr) as List<KilledChecker>;
        }
        foreach (var v in l)
        {
            if (v.color)
            {
                if (v.isQueen)
                {
                    WhiteCheckersKilled.Add(Instantiate(WhiteQueen));
                }
                else
                {
                    WhiteCheckersKilled.Add(Instantiate(WhiteFigure));
                }
                WhiteCheckersKilled[WhiteCheckersKilled.Count - 1].transform.position = new Vector3(v.x, v.y, v.z);
            }
            else
            {
                if (v.isQueen)
                {
                   BlackCheckersKilled.Add(Instantiate(BlackQueen));
                }
                else
                {
                    BlackCheckersKilled.Add(Instantiate(BlackFigure));
                }
                BlackCheckersKilled[BlackCheckersKilled.Count - 1].transform.position = new Vector3(v.x, v.y, v.z);
            }
        }

        WhiteCheckers = new List<GameObject>();
        BlackCheckers = new List<GameObject>();
        WhiteQueens = new List<GameObject>();
        BlackQueens = new List<GameObject>();
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
        foreach (Point p in wq)
        {
            obj = Instantiate(WhiteQueen);
            obj.transform.position = new Vector3(p.x, p.y, -2);
            WhiteQueens.Add(obj);
        }
        foreach (Point p in bq)
        {
            obj = Instantiate(BlackQueen);
            obj.transform.position = new Vector3(p.x, p.y, -2);
            BlackQueens.Add(obj);
        }
    }
    void TryToGetQueen(int x, int y)
    {
        if (CheckEmptyCell(x, y) && WhiteMoveExpected)
        {
            if (y == 7)
            {
                GameObject obj = Instantiate(WhiteQueen);
                obj.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -2);
                WhiteQueens.Add(obj);
                print("Maked w queen");
            }
        }
        if (CheckEmptyCell(x, y) && !WhiteMoveExpected)
        {
            if (y == 0)
            {
                GameObject obj = Instantiate(BlackQueen);
                obj.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -2);
                BlackQueens.Add(obj);
                print("Maked b queen");
            }
        }
    }

    bool IsQueen(GameObject obj)
    {

        foreach (var o in WhiteQueens)
        {
            if (ConvertxToIntCoordinate(obj.transform.position.x) == ConvertxToIntCoordinate(o.transform.position.x)
                &&
                ConvertyToIntCoordinate(obj.transform.position.y) == ConvertyToIntCoordinate(o.transform.position.y))
            {
                return true;
            }
        }
        foreach (var o in BlackQueens)
        {
            if (ConvertxToIntCoordinate(obj.transform.position.x) == ConvertxToIntCoordinate(o.transform.position.x)
                &&
                ConvertyToIntCoordinate(obj.transform.position.y) == ConvertyToIntCoordinate(o.transform.position.y))
            {
                return true;
            }
        }
        return false;
    }
    void TransformQueenPos(GameObject checker, Vector3 setTo)
    {
        foreach (var o in WhiteQueens)
        {
            if (ConvertxToIntCoordinate(checker.transform.position.x) == ConvertxToIntCoordinate(o.transform.position.x)
                &&
                ConvertyToIntCoordinate(checker.transform.position.y) == ConvertyToIntCoordinate(o.transform.position.y))
            {
                o.transform.position = setTo;
            }
        }
        foreach (var o in BlackQueens)
        {
            if (ConvertxToIntCoordinate(checker.transform.position.x) == ConvertxToIntCoordinate(o.transform.position.x)
                &&
                ConvertyToIntCoordinate(checker.transform.position.y) == ConvertyToIntCoordinate(o.transform.position.y))
            {
                o.transform.position = setTo;
            }
        }
    }
    //Go to position in the array
    void GoTo(int x, int y)
    {
        HideGreenSignal();

        if (MouseClicked && Array[y, x] == 0)
        {
            if (!IsQueen(ChosenChecker))
            {
                TryToGetQueen(x, y);
            }
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
            if (IsQueen(ChosenChecker))
            {
                TransformQueenPos(ChosenChecker, new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -2));
                ChosenChecker.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -1);

            }
            else
            {
                ChosenChecker.transform.position = new Vector3(ConvertxToFloatCoordinate(x), ConvertyToFloatCoordinate(y), -1);
            }


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
                if (CanHit() || CanHitBack() || QueenCanHit())
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

    void ShowKilledChecker(int bcCount, int wcCount, int bqCount, int wqCount)
    {
        if (bcCount != BlackCheckers.Count)
        {
            if (bqCount == BlackQueens.Count)
            {
                BlackCheckersKilled.Add(Instantiate(BlackFigure));
            }
            else
            {
                BlackCheckersKilled.Add(Instantiate(BlackQueen));
            }
            if (BlackCheckersKilled.Count <= 8)
            {
                BlackCheckersKilled[BlackCheckersKilled.Count - 1].transform.position = new Vector3(ConvertxToFloatCoordinate(BlackCheckersKilled.Count - 1), ConvertyToFloatCoordinate(0) - 2 * DISTANCE, -2);
            }
            else
            {
                BlackCheckersKilled[BlackCheckersKilled.Count - 1].transform.position = new Vector3(ConvertxToFloatCoordinate(BlackCheckersKilled.Count - 9), ConvertyToFloatCoordinate(0) - 3 * DISTANCE, -2);
            }
        }
        else if (wcCount != WhiteCheckers.Count)
        {
            if (wqCount == WhiteQueens.Count)
            {
                WhiteCheckersKilled.Add(Instantiate(WhiteFigure));
            }
            else
            {
                WhiteCheckersKilled.Add(Instantiate(WhiteQueen));
            }
            if (WhiteCheckersKilled.Count <= 8)
            {
                WhiteCheckersKilled[WhiteCheckersKilled.Count - 1].transform.position = new Vector3(ConvertxToFloatCoordinate(WhiteCheckersKilled.Count - 1), ConvertyToFloatCoordinate(7) + 2 * DISTANCE, -2);
            }
            else
            {
                WhiteCheckersKilled[WhiteCheckersKilled.Count - 1].transform.position = new Vector3(ConvertxToFloatCoordinate(WhiteCheckersKilled.Count - 9), ConvertyToFloatCoordinate(7) + 3 * DISTANCE, -2);
            } 
        }
    }

    void Hit(int kill_x, int kill_y)
    {
        int bccount = BlackCheckers.Count;
        int wccount = WhiteCheckers.Count;
        int bqcount = BlackQueens.Count;
        int wqcount = WhiteQueens.Count;

        
        DeleteChecker(kill_x, kill_y, WhiteMoveExpected);
        ShowKilledChecker(bccount,wccount,bqcount,wqcount);
        GoTo(x, y);
        if (CanHit() || CanHitBack() || QueenCanHit())
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
        Save();
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
        if (ChosenQueen())
        {
            int a = ConvertxToIntCoordinate(ChosenChecker.transform.position.x);
            int b = ConvertyToIntCoordinate(ChosenChecker.transform.position.y);
            int qx = a;
            int qy = b;
            // right top move
            for (int i = 1; a + i < 8 && b + i < 8; i++)
            {
                qx = a + i;
                qy = b + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (((WhiteMoveExpected && CheckBlackChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2)))
                        && qx + 2 == ConvertxToIntCoordinate(MousePos.x) && qy + 2 == ConvertyToIntCoordinate(MousePos.y))
                    {
                        this.x = qx + 2;
                        this.y = qy + 2;
                        Hit(qx + 1, qy + 1);
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            // left bottom move
            for (int i = 1; a - i >= 0 && b - i >= 0; i++)
            {
                qx = a - i;
                qy = b - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (((WhiteMoveExpected && CheckBlackChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2)))
                        && qx - 2 == ConvertxToIntCoordinate(MousePos.x) && qy - 2 == ConvertyToIntCoordinate(MousePos.y))
                    {
                        this.x = qx - 2;
                        this.y = qy - 2;
                        Hit(qx - 1, qy - 1);
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            // right bottom move
            for (int i = 1; a + i < 8 && b - i >= 0; i++)
            {
                qx = a + i;
                qy = b - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (((WhiteMoveExpected && CheckBlackChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2)))
                        && qx + 2 == ConvertxToIntCoordinate(MousePos.x) && qy - 2 == ConvertyToIntCoordinate(MousePos.y))
                    {
                        this.x = qx + 2;
                        this.y = qy - 2;
                        Hit(qx + 1, qy - 1);
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            // left top move
            for (int i = 1; a - i >= 0 && b + i < 8; i++)
            {
                qx = a - i;
                qy = b + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if (((WhiteMoveExpected && CheckBlackChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2)))
                        && qx - 2 == ConvertxToIntCoordinate(MousePos.x) && qy + 2 == ConvertyToIntCoordinate(MousePos.y))
                    {
                        this.x = qx - 2;
                        this.y = qy + 2;
                        Hit(qx - 1, qy + 1);
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return false;
    }


    bool QueenCanHit()
    {
        int a = ConvertxToIntCoordinate(ChosenChecker.transform.position.x);
        int b = ConvertyToIntCoordinate(ChosenChecker.transform.position.y);
        int qx = a;
        int qy = b;
        if (IsQueen(ChosenChecker))
        {
            for (int i = 1; a + i < 8 && b + i < 8; i++)
            {
                qx = a + i;
                qy = b + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if ((WhiteMoveExpected && CheckBlackChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy + 1) && CheckEmptyCell(qx + 2, qy + 2)))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; a - i >= 0 && b - i >= 0; i++)
            {
                qx = a - i;
                qy = b - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if ((WhiteMoveExpected && CheckBlackChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy - 1) && CheckEmptyCell(qx - 2, qy - 2)))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; a + i < 8 && b - i >= 0; i++)
            {
                qx = a + i;
                qy = b - i;
                if (CheckEmptyCell(qx, qy))
                {
                    if ((WhiteMoveExpected && CheckBlackChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx + 1, qy - 1) && CheckEmptyCell(qx + 2, qy - 2)))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; a - i >= 0 && b + i < 8; i++)
            {
                qx = a - i;
                qy = b + i;
                if (CheckEmptyCell(qx, qy))
                {
                    if ((WhiteMoveExpected && CheckBlackChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2))
                        || (!WhiteMoveExpected && CheckWhiteChecker(qx - 1, qy + 1) && CheckEmptyCell(qx - 2, qy + 2)))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
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
            if (IsQueen(obj))
            {
                GameObject reserv = ChosenChecker;
                ChosenChecker = obj;
                if (QueenCanHit())
                {
                    IsHaveToHit = true;
                    FiguresHaveToHit.Add(ChosenChecker);
                    ChosenChecker = reserv;
                }
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

    bool ChosenQueen()
    {
        if (WhiteMoveExpected)
        {
            foreach (var o in WhiteQueens)
            {
                if (o.transform.position.x == ChosenChecker.transform.position.x && o.transform.position.y == ChosenChecker.transform.position.y)
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (var o in BlackQueens)
            {
                if (o.transform.position.x == ChosenChecker.transform.position.x && o.transform.position.y == ChosenChecker.transform.position.y)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CanQueenGoTo(int x, int y)
    {
        if (Mathf.Abs(ConvertxToIntCoordinate(ChosenChecker.transform.position.x) - x) == Mathf.Abs(ConvertyToIntCoordinate(ChosenChecker.transform.position.y) - y))
        {
            return true;
        }
        return false;
    }

    bool TryToGoQueen(int x, int y)
    {
        int qx = ConvertxToIntCoordinate(ChosenChecker.transform.position.x);
        int qy = ConvertyToIntCoordinate(ChosenChecker.transform.position.y);
        if (x < qx && y < qy)
        {
            for (int i = qx - 1, j = qy - 1; i >= x && j >= y; i--, j--)
            {
                if (i == x && j == y)
                {
                    GoTo(x, y);
                    WhiteMoveExpectedChange();
                    return true;
                }
                if (!CheckEmptyCell(i, j))
                {
                    break;
                }

            }
        }
        if (x > qx && y < qy)
        {
            for (int i = qx + 1, j = qy - 1; i <= x && j >= y; i++, j--)
            {
                if (i == x && j == y)
                {
                    GoTo(x, y);
                    WhiteMoveExpectedChange();
                    return true;
                }
                if (!CheckEmptyCell(i, j))
                {
                    break;
                }

            }
        }
        if (x < qx && y > qy)
        {
            for (int i = qx - 1, j = qy + 1; i >= x && j <= y; i--, j++)
            {
                if (i == x && j == y)
                {
                    GoTo(x, y);
                    WhiteMoveExpectedChange();
                    return true;
                }
                if (!CheckEmptyCell(i, j))
                {
                    break;
                }

            }
        }
        if (x > qx && y > qy)
        {
            for (int i = qx + 1, j = qy + 1; i <= x && j <= y; i++, j++)
            {
                if (i == x && j == y)
                {
                    GoTo(x, y);
                    WhiteMoveExpectedChange();
                    return true;
                }
                if (!CheckEmptyCell(i, j))
                {
                    break;
                }

            }
        }
        return false;
    }


    // General playing method
    void Playing()
    {
        if (WhiteCheckers.Count == 0 && WhiteQueens.Count == 0)
        {
            ST.Instanse.Winner = 0;
            Save();
        }
        if (BlackCheckers.Count == 0 && BlackQueens.Count == 0)
        {
            ST.Instanse.Winner = 1;
            Save();
        }

        if (WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {
            foreach (var v in BlackCheckers)
            {
                if (ConvertxToIntCoordinate(v.transform.position.x) == ConvertxToIntCoordinate(MousePos.x)
                    && ConvertyToIntCoordinate(v.transform.position.y) == ConvertyToIntCoordinate(MousePos.y))
                {
                    ShowHelpSignals(WhiteMoveExpected);
                    return;
                }
            }

        }
        if (!WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {
            foreach (var v in WhiteCheckers)
            {
                if (ConvertxToIntCoordinate(v.transform.position.x) == ConvertxToIntCoordinate(MousePos.x)
                    && ConvertyToIntCoordinate(v.transform.position.y) == ConvertyToIntCoordinate(MousePos.y))
                {
                    ShowHelpSignals(WhiteMoveExpected);
                    return;
                }
            }

        }
        
        // White checker choose
        if (WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {

            if (!HaveToHit(WhiteCheckers))
            {

                TryToChooseChecker(WhiteCheckers);
                HideRedSignals();
                HideHelpSignals();

            }
            else
            {
                TryToChooseCheckerToHit();
                HideHelpSignals();
            }
            return;
        }

        // Black checker choose
        if (!WhiteMoveExpected && Click() && !MouseClicked && !MultiHit)
        {
 
            if (!HaveToHit(BlackCheckers))
            {
                TryToChooseChecker(BlackCheckers);
                HideRedSignals();
                HideHelpSignals();
            }
            else
            {
                TryToChooseCheckerToHit();
                HideHelpSignals();
            }
            return;
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
                            Save();
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
                    if (ChosenQueen())
                    {
                        if (CanQueenGoTo(x, y))
                        {
                            if (QueenCanHit())
                            {
                                if (TryHit())
                                {
                                    Save();
                                    return;
                                }

                            }
                            else if (!MultiHit && !IsHaveToHit)
                            {
                                TryToGoQueen(x, y);
                                Save();
                                return;
                            }
                        }
                    }
                }

            }

        }
        return;

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
