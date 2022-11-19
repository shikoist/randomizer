using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainScript : MonoBehaviour {

    int currentCamPos = 0;

    public GameObject listGO;

    public Transform baraban;

    // Здесь хранится список игр строкой с переносами
    public TMP_InputField inputField;

    public Transform prefabText;

    public bool isRolled = false;
    public bool isBlinking = false;

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
    string musicFile = "";

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
        //Debug.Log(userDocumentsPath);
        fullPath = userDocumentsPath + "\\Randomizer\\";
        pathToList = fullPath + "games.txt";
        //helpOff = fullPath + "help_off.txt";
        optionsFile = fullPath + "options.ini";
        pathToWinnersList = fullPath + "winners.txt";
        musicFile = fullPath + "music.ogg";

        Debug.Log(fullPath);

        

        // Создаём директорию в документах пользователя, как все приличные приложения
        if (!System.IO.Directory.Exists(fullPath))
        {
            System.IO.Directory.CreateDirectory(fullPath);
        }

        /*if (System.IO.File.Exists(optionsFile))
        {
            helpPanel.gameObject.SetActive(false);
        }*/
        
        // Сохраняем или загружаем список игр, если он уже есть
        CreateOrReadFileGames();

        // Сохраняем или загружаем настройки
        CreateOrReadFileOptions();

        // Применяем цвет пола
        Renderer rr = floor.GetComponent<Renderer>();
        //Debug.Log(ToHex(rr.material.color));
        rr.material.color = colorFloor;

        // Почему-то не сохраняется опция wrapping в компоненте Text,
        // который хранит игры
        TMP_Text tt = listGO.GetComponentsInChildren<TMP_Text>()[1];
        tt.enableWordWrapping = false;

        listGO.SetActive(false);
        debugPanel.gameObject.SetActive(false);

        Music music = FindObjectOfType<Music>();
        if (music == null)
        {
            Debug.Log("Object Music not found");
        }
        else
        {
            music.Load(musicFile);
        }
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
            //Debug.Log(inputField.text);
        }

        ResetBaraban();
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
            //Debug.Log("Options file : " + inputText);

            // Парсится и применяется список настроек
            string[] strText;
            // Разбиваем на строки
            strText = inputText.Split('\n');

            // Первая настройка это цвет пола
            // Разбиваем на подстроки с переменными
            string[] strText2 = strText[0].Split(' ');

            //Debug.Log("Read color : " + strText2[1]);
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
            //Debug.Log("Color not valid " + hex.Length);
            return Color.white;
        }

        int r, g, b;
        r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        //Debug.Log("FromHex : " + r + " " + g + " " + b);

        Color rtn = new Color(
             r / 255.0f,
             g / 255.0f,
             b / 255.0f);

        //Debug.Log(rtn);

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

        //Music ON OFF
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

        //Поменять положение камеры, чтобы осмотреть весь барабан, например
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3[] camPos = {
                new Vector3(1.5f, 4, 0),
                new Vector3(0, 6.8f, 0)
            };
            currentCamPos++;

            if (currentCamPos >= camPos.Length)
            {
                currentCamPos = 0;
            }

            Camera cam = FindObjectOfType<Camera>();
            cam.transform.position = camPos[currentCamPos];
        }

        // Справочная панель
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F1))
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



        // Вертикальная синхронизация
        /*if (Input.GetKeyDown(KeyCode.V))
        {
            if (QualitySettings.vSyncCount == 4)
                QualitySettings.vSyncCount = 0;
            else
                QualitySettings.vSyncCount++;
        }*/

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Сохранить всё
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveAll();
            }

            // Был баг, когда при выключении списка игр буква l добавлялась случайным образом
            // Это потому что открытый список ловил эту кнопку, когда выключался
            // Редактор списка игр
            if (Input.GetKeyDown(KeyCode.L))
            {
                //Debug.Log("Gamelist opened");

                // Выключается список игр
                if (listGO.activeSelf)
                {
                    listGO.SetActive(false);
                    ResetBaraban();
                }
                // Включается список игр
                else
                {
                    listGO.SetActive(true);
                    inputField.Select();
                    ResetBaraban();
                    bVelocity = 0;
                    isRolled = false;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Space) && isRolled == false && isBlinking == false && !listGO.activeSelf)
        {
            RollBaraban();
        }

        if (isRolled)
        {
            baraban.Rotate(Vector3.up * Time.deltaTime * bVelocity);
            bVelocity -= decVelocity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isBlinking)
        {
            GameObject blinkingGame = GameObject.FindObjectOfType<Blinking>().gameObject;
            Blinking b = blinkingGame.GetComponent<Blinking>();
            b.ExitBlinking();
            isBlinking = false;
            ResetBaraban();
        }

        if (bVelocity < 1.0f && isRolled)
        {
            isRolled = false;
            Debug.Log("Baraban stopped rolling. calculate winner");
            CalculateWinner();
        }
	}

    void CalculateWinner()
    {
        // Z-позиция выигрывшей таблички ближе всего к 0.0, при этом X-позиция положительна

        GameObject[] goList = GameObject.FindGameObjectsWithTag("Game");
        List<GameObject> winners = new List<GameObject>();

        // Отсеиваем тех, кто левее центра барабана
        for (int i = 0; i < goList.Length; i++)
        {
        //    if (goList[i].transform.position.x > 0)
        //    {
                winners.Add(goList[i]);
                //Debug.Log(goList[i]);
        //    }
        }

        // Список расстояний до стрелки карточек, оставшихся справа
        //List<float> distances = new List<float>();

        //Список углов карточек
        List<float> angles = new List<float>();

        for (int i = 0; i < winners.Count; i++)
        {
            //distances.Add(Mathf.Abs(winners[i].transform.position.z));
            float a = winners[i].transform.rotation.eulerAngles.y;

            // Поскольку углы у карточек считаются от 0 до 360,
            // то при этом карточки, находящиеся ближе к 360,
            // визуально находятся чуть выше стрелки. Их тоже надо учитывать,
            // если они ближе к стрелке, чем нижние.
            // Карточки, имеющие угол около 180, визуально лежат на другой
            // стороне от барабана.
            // Поэтому отняв от 360 градусов угол карточки,
            // мы как бы перекладываем их вниз, и они теперь тоже
            // учитываются.
            if (a > 180) a = 360 - a;

            angles.Add(a);
            //Debug.Log(winners[i].name + " : angle " + a.ToString());
        }

        // Последовательно уменьшаем расстояние, чтобы узнать самое маленькое
        float minValue = Mathf.Infinity;
        for (int i = 0; i < winners.Count; i++)
        {
            //if (distances[i] < minValue)
            if (angles[i] < minValue)
            {
                minValue = Mathf.Abs(angles[i]);
            }
        }

        // Вычисляем, у кого именно минимальное расстояние до стрелки
        for (int i = 0; i < winners.Count; i++)
        {
            // Номер i наш победитель!
            if (minValue == angles[i])
            {
                // Добавляем мигание
                winners[i].AddComponent<Blinking>();
                isBlinking = true;
                //winners[i].GetComponent<Blinking>().endTime = Time.time + blinkingTime;

                //Показать карточку прямо перед стрелкой
                winners[i].transform.SetParent(null);
                winners[i].transform.position = new Vector3(0.45f, 2.05f, 0.1f);
                winners[i].transform.rotation = Quaternion.identity;

                // Добавляем в список победителей
                winnersList += winners[i].name + " at time " + System.DateTime.Now + "\n";

                // Вставляем звёздочку перед именем выигравшей карточки
                AddStar(winners[i].name);

                // Записать в файл победителей
                System.IO.File.WriteAllText(pathToWinnersList, winnersList);

                // Выводим значения в лог
                Debug.Log("winner is " + winners[i].name + " : minValue = " + minValue);

                for (int j = 0; j < winners.Count; j++)
                {
                    Debug.Log(winners[j].name + " : angle = " + angles[j]);
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

    // В этой функции мы раскладываем карточки, 
    // имея список игр в строке inputField
    public void ResetBaraban()
    {
        if (!isRolled)
        {
            // Удаление предыдущих объектов
            ClearGames();

            // Сбрасываем поворот модели барабана в ноль
            baraban.localRotation = Quaternion.identity;

            // Парсится список игр
            string[] strText;

            // Разбиваем на строки
            strText = inputField.text.Split('\n');

            // Создаём более удобный список игр в виде объектов листа, 
            // чтобы точно знать общее количество
            List<string> fltrGames = new List<string>();

            // Убираем игры со звёздочкой, переносим их в новый список
            for (int i = 0; i < strText.Length; i++)
            {
                if (strText[i].Length < 3 || strText[i][0] == '*') continue;
                fltrGames.Add(strText[i]);
            }

            // Перемешиваем карточки
            for (int i = 0; i < fltrGames.Count; i++)
            {
                string tmp = "";
                int rnd = Random.Range(0, fltrGames.Count);

                tmp = fltrGames[i];
                fltrGames[i] = fltrGames[rnd];
                fltrGames[rnd] = tmp;
            }

            // Создаём карточки с играми на барабане
            for (int i = 0; i < fltrGames.Count; i++)
            {
                //Debug.Log(i + " : " + fltrGames[i]);
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

    // Создаём карточку с игрой, n - номер игры, all - всего игр
    GameObject CreateGame(string strGame, int n, int all)
    {
        Transform go = (Transform)Instantiate(prefabText);

        // Меняем текст в карточке на название игры из параметра функции
        Text text = go.GetComponentInChildren<Text>();
        text.text = strGame;

        // Переименовываем сам объект для удобства
        go.name = strGame;

        //0.4 расстояние от центра
        //2 расстояние от пола
        //0.12 расстояние от горизонтали, потому что сама карточка 0,24 м в высоту
        go.position = new Vector3(0.4f, 2.01f, 0.12f);

        go.SetParent(baraban);

        // Угол, на который мы поворачиваем барабан перед прикреплением
        // следующей карточки, равен 360, поделённому на количество игр
        float a = 360.0f / all;
        baraban.Rotate(Vector3.up, a, Space.World);
        
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
