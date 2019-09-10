using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public GameObject listGO;

    public Transform baraban;

    public Text listText;
    public InputField inputField;

    public Transform prefabText;

    public bool isRolled = false;

    public float randVelocity;
    public float bVelocity;
    public float decVelocity;

    public Transform debugPanel;
    public Text debugText;

    public Transform helpPanel;

    public string userDocumentsPath;

    string fullPath = "";
    string pathToList = "";
    string helpOff = "";

    public AudioSource audioPlayer;

    // Use this for initialization
    void Start () {

        userDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        Debug.Log(userDocumentsPath);
        fullPath = userDocumentsPath + "\\Randomizer";
        pathToList = fullPath + "\\games.txt";
        helpOff = fullPath + "\\help_off.txt";

        // Создаём директорию в документах пользователя, как все приличные приложения
        if (!System.IO.Directory.Exists(fullPath))
        {
            System.IO.Directory.CreateDirectory(fullPath);
        }

        if (System.IO.File.Exists(helpOff))
        {
            helpPanel.gameObject.SetActive(false);
        }
        else
        {
            System.IO.File.WriteAllText(helpOff, "help off");
        }

        // Сохраняем или загружаем список игр, если он уже есть
        UpdateFileWithGames();

        listGO.SetActive(false);
        debugPanel.gameObject.SetActive(false);
	}

    void UpdateFileWithGames()
    {
        if (!System.IO.File.Exists(pathToList))
        {
            System.IO.File.WriteAllText(pathToList, listText.text);
        }
        else
        {
            //listText.text = System.IO.File.ReadAllText(pathToList);
            //Debug.Log(listText.text);
            inputField.text = System.IO.File.ReadAllText(pathToList);
            Debug.Log(inputField.text);
        }

        OnListChanged();
    }

    void SaveGames()
    {
        System.IO.File.WriteAllText(pathToList, listText.text);
    }

	// Update is called once per frame
	void Update () {

        UpdateDebug(baraban.rotation.eulerAngles.y, bVelocity, randVelocity, decVelocity);

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("F2 pressed");

            // Выключается список игр
            if (listGO.activeSelf)
            {
                listGO.SetActive(false);
                OnListChanged();
            }
            // Включается список игр
            else
            {
                listGO.SetActive(true);
                inputField.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Выключается список игр
            if (debugPanel.gameObject.activeSelf)
            {
                debugPanel.gameObject.SetActive(false);
                
            }
            // Включается список игр
            else
            {
                debugPanel.gameObject.SetActive(true);
                
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (audioPlayer.isPlaying)
            {
                audioPlayer.Stop();
            }
            else
            {
                audioPlayer.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Выключается список игр
            if (helpPanel.gameObject.activeSelf)
            {
                helpPanel.gameObject.SetActive(false);

            }
            // Включается список игр
            else
            {
                helpPanel.gameObject.SetActive(true);

            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SaveGames();
        }



        if (Input.GetKeyDown(KeyCode.Space) && isRolled == false && !listGO.activeSelf)
        {
            RollBaraban();
        }

        if (isRolled)
        {
            baraban.Rotate(Vector3.up * Time.deltaTime * bVelocity);
            bVelocity -= decVelocity * Time.deltaTime;
        }

        if (bVelocity < 1.0f && isRolled)
        {
            isRolled = false;

            // Здесь нужно дать понять, какой вариант выиграл
            // Для этого текущий угол барабана делим на 360.

            CalculateWinner();

            //float winner = baraban.rotation.eulerAngles.y / 360.0f;
            //Debug.Log("Winner : " + winner);
        }
	}

    void CalculateWinner()
    {
        // Z-позиция выигрывшей таблички ближе всего к 0.0, при этом X-позиция положительна

        List<GameObject> winners = new List<GameObject>();

        GameObject[] goList = GameObject.FindGameObjectsWithTag("Game");

        // Отсеиваем тех, кто левее центра барабана
        for (int i = 0; i < goList.Length; i++)
        {
            if (goList[i].transform.position.x > 0)
            {
                winners.Add(goList[i]);
            }
        }

        List<float> distances = new List<float>();

        for (int i = 0; i < winners.Count; i++)
        {
            distances.Add(Mathf.Abs(winners[i].transform.position.z));
        }

        float minValue = Mathf.Infinity;

        for (int i = 0; i < winners.Count; i++)
        {
            if (distances[i] < minValue)
            {
                minValue = distances[i];
            }
        }

        // Вычисляем, у кого минималка
        for (int i = 0; i < winners.Count; i++)
        {
            if (minValue == distances[i])
            {
                // номер i наш победитель
                winners[i].AddComponent<Blinking>();
                Debug.Log("winner is " + winners[i].GetComponent<TextMesh>().text + " : minValue = " + minValue);

                for (int j = 0; j < winners.Count; j++)
                {
                    Debug.Log("winners[" + j + "] " + winners[j].GetComponent<TextMesh>().text + " : minValue = " + distances[j]);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveGames();
    }

    void OnListChanged()
    {
        // Удаление предыдущих объектов
        ClearGames();

        baraban.localRotation = Quaternion.identity;

        // Парсится список игр
        string[] strText;
        strText = listText.text.Split('\n');

        List<string> fltrGames = new List<string>();

        for (int i = 0; i < strText.Length; i++)
        {
            if (strText[i].Length < 3) continue;
            fltrGames.Add(strText[i]);
        }

        for (int i = 0; i < fltrGames.Count; i++)
        {
            Debug.Log(i + " : " + fltrGames[i]);
            
            GameObject go = CreateGame(fltrGames[i], i, fltrGames.Count);
        }
    }

    void ClearGames()
    {
        GameObject[] goList = GameObject.FindGameObjectsWithTag("Game");
        for (int i = 0; i < goList.Length; i++)
        {
            GameObject.Destroy(goList[i]);
        }
    }

    GameObject CreateGame(string strGame, int n, int all)
    {
        Transform go = (Transform)Instantiate(prefabText);
        TextMesh tm = go.GetComponent<TextMesh>();
        tm.text = strGame;

        float a = 360.0f / all * n;
        float r = 1.4f;
        float x = r * Mathf.Cos(a * Mathf.Deg2Rad);
        float z = r * Mathf.Sin(a * Mathf.Deg2Rad);

        go.SetParent(baraban);

        go.localPosition = new Vector3(x, 2.01f + n * 0.001f, z);
        //go.localPosition = Vector3.zero + Vector3.up * 2 + Vector3.right * 1.68f;
        //go.Translate(Vector3.up * 2);
        //go.Translate(Vector3.right * 0.68f);
        go.Rotate(Vector3.up, 360 - a, Space.World);
        
        

        return go.gameObject;
    }

    void RollBaraban()
    {
        randVelocity = Random.Range(70.0f, 100.0f);
        bVelocity = randVelocity;
        isRolled = true;
        decVelocity = Random.Range(1.0f, 2.0f);
    }

    void UpdateDebug(float bAngle, float bVelocity, float startVelocity, float decVelocity)
    {
        debugText.text = "Baraban Angle : " + bAngle + "\n" + "Baraban Velocity : " + bVelocity + "\n" + "Start Velocity : " + startVelocity + "\n" + 
            "Decrease Velocity : " + decVelocity + "\n" +
            "Frames per second: " + (1.0f / Time.deltaTime).ToString();
    }
}
