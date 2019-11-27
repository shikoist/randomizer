using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public GameObject listGO;

    public Transform baraban;

    // Здесь хранится список игр строкой с переносами
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
    string optionsFile = "";
    string pathToWinnersList = "";

    public AudioSource audioPlayer;

    public Transform floor;
    public Color colorFloor;

    public float startMinVelocity = 90;
    public float startMaxVelocity = 120;

    public float decMinVelocity = 4;
    public float decMaxVelocity = 7;

    public float blinkingTime = 5;

    // Дополнительная строка, содержащая только победителей
    string winnersList = "";

    // Use this for initialization
    void Start () {

        userDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        Debug.Log(userDocumentsPath);
        fullPath = userDocumentsPath + "\\Randomizer";
        pathToList = fullPath + "\\games.txt";
        //helpOff = fullPath + "\\help_off.txt";
        optionsFile = fullPath + "\\options.ini";
        pathToWinnersList = fullPath + "\\winners.txt";

        // Создаём директорию в документах пользователя, как все приличные приложения
        if (!System.IO.Directory.Exists(fullPath))
        {
            System.IO.Directory.CreateDirectory(fullPath);
        }

        if (System.IO.File.Exists(optionsFile))
        {
            helpPanel.gameObject.SetActive(false);
        }
        
        // Сохраняем или загружаем список игр, если он уже есть
        CreateOrReadFileGames();

        // Сохраняем или загружаем настройки
        CreateOrReadFileOptions();

        // Применяем цвет пола
        Renderer rr = floor.GetComponent<Renderer>();
        Debug.Log(ToHex(rr.material.color));
        rr.material.color = colorFloor;

        listGO.SetActive(false);
        debugPanel.gameObject.SetActive(false);
	}

    void CreateOrReadFileGames()
    {
        if (!System.IO.File.Exists(pathToList))
        {
            System.IO.File.WriteAllText(pathToList, inputField.text);
        }
        else
        {
            inputField.text = System.IO.File.ReadAllText(pathToList);
            Debug.Log(inputField.text);
        }

        OnListChanged();
    }

    string CurrentOptions()
    {
        string optionsStr = "colorFloor " + ToHex(colorFloor) + "\n" +
            "startMinVelocity " + startMinVelocity + "\n" +
            "startMMaxVelocity " + startMaxVelocity + "\n" + 
            "decMinVelocity " + decMinVelocity + "\n" +
            "decMaxVelocity " + decMaxVelocity + "\n" +
            "blinkingTime " + blinkingTime + "\n";
        return optionsStr;
    }

    void CreateOrReadFileOptions()
    {
        if (!System.IO.File.Exists(optionsFile))
        {
            System.IO.File.WriteAllText(optionsFile, CurrentOptions());
        }
        else
        {
            string inputText = System.IO.File.ReadAllText(optionsFile);
            Debug.Log("Options file : " + inputText);

            // Парсится и применяется список настроек
            string[] strText;
            // Разбиваем на строки
            strText = inputText.Split('\n');

            // Первая настройка это цвет пола
            // Разбиваем на подстроки с переменными
            string[] strText2 = strText[0].Split(' ');

            Debug.Log("Read color : " + strText2[1]);
            colorFloor = FromHex(strText2[1]);

            // Чтение минимальной скорости
            // Вторая величина это число, первое название
            strText2 = strText[1].Split(' ');
            startMinVelocity = int.Parse(strText2[1]);

            strText2 = strText[2].Split(' ');
            startMaxVelocity = int.Parse(strText2[1]);

            strText2 = strText[3].Split(' ');
            decMinVelocity = int.Parse(strText2[1]);

            strText2 = strText[4].Split(' ');
            decMaxVelocity = int.Parse(strText2[1]);

            strText2 = strText[5].Split(' ');
            blinkingTime = int.Parse(strText2[1]);
        }
    }

    private string ToHex(Color clr)
    {
        string str = "";

        str += "#";
        str += (Mathf.CeilToInt(clr.r * 255.0f)).ToString("X2") +
            (Mathf.CeilToInt(clr.g * 255.0f)).ToString("X2") +
            (Mathf.CeilToInt(clr.b * 255.0f)).ToString("X2");

        return str;
    }
    private Color FromHex(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length != 6)
        {
            Debug.Log("Color not valid " + hex.Length);
            return Color.white;
        }

        int r, g, b;
        r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        Debug.Log("FromHex : " + r + " " + g + " " + b);

        Color rtn = new Color(
             r / 255.0f,
             g / 255.0f,
             b / 255.0f);

        Debug.Log(rtn);

        return rtn;
    }

    void AddStar(string winner)
    {
        // Здесь нужно удалить из текста inputField выигравшую игру
        MainScript ms = GameObject.FindObjectOfType<MainScript>();

        int findWinnerInList = ms.inputField.text.IndexOf(winner);
        ms.inputField.text = ms.inputField.text.Insert(findWinnerInList, "*");
    }

    public void SaveAll()
    {
        System.IO.File.WriteAllText(pathToList, inputField.text);
        System.IO.File.WriteAllText(optionsFile, CurrentOptions());
    }

	// Update is called once per frame
	void Update () {

        UpdateDebug(baraban.rotation.eulerAngles.y, bVelocity, randVelocity, decVelocity, QualitySettings.vSyncCount);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {

            // Редактор списка игр
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Gamelist opened");

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

            // Окно дебага
            if (Input.GetKeyDown(KeyCode.D))
            {
                // Выключается
                if (debugPanel.gameObject.activeSelf)
                {
                    debugPanel.gameObject.SetActive(false);

                }
                // Включается
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

            // Справочная панель
            if (Input.GetKeyDown(KeyCode.H))
            {
                // Выключается справочная панель
                if (helpPanel.gameObject.activeSelf)
                {
                    helpPanel.gameObject.SetActive(false);

                }
                // Включается справочная панель
                else
                {
                    helpPanel.gameObject.SetActive(true);

                }
            }

            // Сохранить всё
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveAll();
            }

            // Вертикальная синхронизация
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (QualitySettings.vSyncCount == 4)
                    QualitySettings.vSyncCount = 0;
                else
                    QualitySettings.vSyncCount++;
            }
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

            CalculateWinner();
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

        // Список расстояний до стрелки карточек, оставшихся справа
        List<float> distances = new List<float>();

        for (int i = 0; i < winners.Count; i++)
        {
            distances.Add(Mathf.Abs(winners[i].transform.position.z));
        }

        // Последовательно уменьшаем расстояние, чтобы узнать самое маленькое
        float minValue = Mathf.Infinity;
        for (int i = 0; i < winners.Count; i++)
        {
            if (distances[i] < minValue)
            {
                minValue = distances[i];
            }
        }

        // Вычисляем, у кого именно минимальное расстояние до стрелки
        for (int i = 0; i < winners.Count; i++)
        {
            // Номер i наш победитель
            if (minValue == distances[i])
            {
                // Добавляем мигание
                winners[i].AddComponent<Blinking>();
                winners[i].GetComponent<Blinking>().endTime = Time.time + blinkingTime;

                // Добавляем в список победителей
                winnersList += winners[i].GetComponent<TextMesh>().text + " at time " + System.DateTime.Now + "\n";

                // Записать в файл победителей
                System.IO.File.WriteAllText(pathToWinnersList, winnersList);

                // Вставляем звёздочку перед именем выигравшей карточки
                AddStar(winners[i].GetComponent<TextMesh>().text);

                // Выводим значения в лог
                Debug.Log("winner is " + winners[i].GetComponent<TextMesh>().text + " : minValue = " + minValue);
                for (int j = 0; j < winners.Count; j++)
                {
                    Debug.Log("winners[" + j + "] " + winners[j].GetComponent<TextMesh>().text + " : minValue = " + distances[j]);
                }

                // Выходим из цикла, потому что победитель найден
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveAll();
    }

    public void OnListChanged()
    {
        if (!isRolled)
        {
            // Удаление предыдущих объектов
            ClearGames();

            baraban.localRotation = Quaternion.identity;

            // Парсится список игр
            string[] strText;

            // Разбиваем на строки
            strText = inputField.text.Split('\n');

            // Создаём более удобный список игр в виде объектов листа, 
            // чтобы точно знать общее количество
            List<string> fltrGames = new List<string>();

            for (int i = 0; i < strText.Length; i++)
            {
                // Убираем игры со звёздочкой
                if (strText[i].Length < 3 || strText[i][0] == '*') continue;
                fltrGames.Add(strText[i]);
            }

            // Создаём карточки с играми на барабане
            for (int i = 0; i < fltrGames.Count; i++)
            {
                Debug.Log(i + " : " + fltrGames[i]);
                GameObject go = CreateGame(fltrGames[i], i, fltrGames.Count);
            }

            // Задаём случайное вращение барабану
            baraban.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0.0f);
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

    // Создаём карточку с игрой
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
        go.Rotate(Vector3.up, 360 - a, Space.World);

        return go.gameObject;
    }

    void RollBaraban()
    {
        randVelocity = Random.Range(startMinVelocity, startMaxVelocity);
        bVelocity = randVelocity;
        isRolled = true;
        decVelocity = Random.Range(decMinVelocity, decMaxVelocity);
    }

    void UpdateDebug(float bAngle, float bVelocity, float startVelocity, float decVelocity, int vsync)
    {
        debugText.text = "Baraban Angle : " + bAngle + "\n" + "Baraban Velocity : " + bVelocity + "\n" + "Start Velocity : " + startVelocity + "\n" + 
            "Decrease Velocity : " + decVelocity + "\n" +
            "Frames per second: " + (1.0f / Time.deltaTime).ToString() + "\n" +
            "VSync mode (0 = off): " + vsync;
    }
}
