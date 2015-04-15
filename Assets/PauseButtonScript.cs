using UnityEngine;
using System.Collections;

public class PauseButtonScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
    Vector3 MousePos;
	// Update is called once per frame
	void Update () {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Mathf.Sqrt(Mathf.Pow(MousePos.x - this.transform.position.x, 2) + Mathf.Pow(MousePos.y - this.transform.position.y, 2)) < 0.7)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ST.Instanse.Continue = true;
                Camera.main.transform.position = new Vector3(0f, 0f, Camera.main.transform.position.z);
            }
        }

	}
}
