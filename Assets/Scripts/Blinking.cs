using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour {

    float startTime;
    float rateTime = 0.25f;
    int n = 90;

    bool blink;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (startTime < Time.time)
        {
            startTime = startTime + rateTime;

            TextMesh tm = this.GetComponent<TextMesh>();

            Renderer rr = this.transform.GetChild(0).GetComponent<Renderer>();
            

            if (blink)
            {
                tm.color = Color.red;
                //rr.materials[0].color = Color.black;
                blink = false;
            }
            else {
                tm.color = Color.black;
                //rr.materials[0].color = Color.red;
                blink = true;
            }

            n--;

            if (n <= 0)
            {
                tm.color = Color.red;
                //rr.materials[0].color = Color.black;

                // Здесь нужно удалить из текста inputField выигравшую игру
                MainScript ms = GameObject.FindObjectOfType<MainScript>();

                TextMesh tem = this.GetComponent<TextMesh>();

                int findWinnerInList = ms.inputField.text.IndexOf(tem.text);
                //ms.inputField.text = ms.inputField.text.Remove(findWinnerInList, tem.text.Length + 1);
                ms.inputField.text = ms.inputField.text.Insert(findWinnerInList, "*");

                ms.OnListChanged();
                ms.SaveAll();

                GameObject.Destroy(this.gameObject);
            }
        }
	}
}
