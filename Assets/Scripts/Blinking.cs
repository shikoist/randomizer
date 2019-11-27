using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour {

    float startTime;
    public float endTime;
    float rateTime = 0.25f;
    TextMesh tm;
    Renderer rr;
    bool blink;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        tm = this.GetComponent<TextMesh>();
        rr = this.transform.GetChild(0).GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (startTime < Time.time)
        {
            startTime = startTime + rateTime;

            if (blink)
            {
                tm.color = Color.red;
                blink = false;
            }
            else {
                tm.color = Color.black;
                blink = true;
            }
        }

        if (endTime < Time.time)
        {
            tm.color = Color.red;

            // Обновляем список и игры
            MainScript ms = GameObject.FindObjectOfType<MainScript>();
            ms.OnListChanged();

            Destroy(this);
        }
	}
}
