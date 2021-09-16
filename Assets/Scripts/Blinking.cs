using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinking : MonoBehaviour {

    float startTime;
    //public float endTime;
    float rateTime = 0.25f;
    //TextMesh tm;
    Text t;
    //Renderer rr;
    bool blink;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        //tm = this.GetComponent<TextMesh>();
        t = this.GetComponentInChildren<Text>();
        //rr = this.transform.GetChild(0).GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (startTime < Time.time)
        {
            startTime = startTime + rateTime;

            if (blink)
            {
                t.color = Color.red;
                blink = false;
            }
            else {
                t.color = Color.black;
                blink = true;
            }
        }

        //if (endTime < Time.time)
        //{
        //    t.color = Color.red;

            // Обновляем список и игры
            //MainScript ms = GameObject.FindObjectOfType<MainScript>();
            //ms.OnListChanged();

            //Destroy(this);
        //}
	}

    public void ExitBlinking()
    {
        t.color = Color.black;
        Destroy(this);
    }
}
