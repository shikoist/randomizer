using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBallsCreate : MonoBehaviour {

    public Transform baraban;
    public Transform ballPrefab;

    public int n = 32;

    // стартовая позиция x 2.6, y 2.2

	// Use this for initialization
	void Start () {
        float a = 360.0f / n;
        float r = 2.6f;
        float x, z, a1;
        
        for (int i = 0; i < n; i++)
        {
            a1 = a * i;
            x = r * Mathf.Cos(a1 * Mathf.Deg2Rad);
            z = r * Mathf.Sin(a1 * Mathf.Deg2Rad);
            
            Transform t = (Transform)Instantiate(ballPrefab);
            t.position = new Vector3(x, 2.2f, z);
            t.SetParent(baraban);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
