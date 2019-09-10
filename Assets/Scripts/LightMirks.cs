using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMirks : MonoBehaviour {

    Light l;

    float r;
    float r2;

	// Use this for initialization
	void Start () {
        l = GetComponent<Light>();
        r = Random.Range(1.0f, 2.0f);
        r2 = Random.Range(0.0f, 2.0f);
    }
	
	// Update is called once per frame
	void Update () {
        //l.intensity = Random.Range(0.8f, 1.2f);
        l.intensity = 1.0f + Mathf.Sin(Time.time * r + r2) * 0.2f;
	}
}
