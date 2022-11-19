using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Load(string path)
    {
        StartCoroutine(Load2(path));
    }

    IEnumerator Load2(string path)
    {
        Debug.Log("Try to loading: " + path);

        // Start downloading
        using (var download = new WWW(path))
        {
            // Wait for download to finish
            yield return download;
            // Create ogg vorbis file
            var clip = download.GetAudioClip();
            // Play it
            if (clip != null)
            {
                GetComponent<AudioSource>().clip = clip;
                GetComponent<AudioSource>().Play();
                Debug.Log("Ogg vorbis loaded, starting to play");
            }
            else     // Handle error
            {
                Debug.Log("Ogg vorbis download failed. (Incorrect link?)");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
