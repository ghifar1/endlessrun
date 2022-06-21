using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl current;

    //Call class from another class
    public CharacterControl myChar;

    [Header("Countdown Properties")]

    public Text countdownText;
    public int countdownCount;
    int currCountdown;

    [Header("User Interface")]

    public GameObject pausePanel;
    public GameObject GameoverPanel;
    public Transform lifePanel;
    public Text scoreCountTxt;
    public int score;

    [Header("Audio")]

    public AudioSource bgmSource;
    public AudioClip gameBGM;
    public AudioClip dessertBGM;
    public AudioClip gameOverBGM;

    [Header("Move Speed By Time")]

    public float maxSpeed;
    public float nextSpeedAdd;
    public float nextTimeAdd;
    public float speedTimeStepper;

    private float timeStamp = 0;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        Time.timeScale = 1;
        StartCountdown();
        PopulateLifeImage();
        ChangeBGM(gameBGM, true);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    int UniqueRandom()
    {
        int rand = Random.Range(0, EnvironmentControl.current.seasonNames.Length);
        if (rand == EnvironmentControl.current.lastActiveSeasonindex)
        {
            return this.UniqueRandom();
        } else
        {
            return rand;
        }
    }

    private void FixedUpdate()
    {
        timeStamp += Time.deltaTime;
        //Debug.Log(Mathf.Ceil(timeStamp));
        if (EnvironmentControl.current.moveSpeed <= maxSpeed && timeStamp > speedTimeStepper)
        {
            EnvironmentControl.current.moveSpeed += nextSpeedAdd;
            speedTimeStepper += nextTimeAdd;
        }

        if (Mathf.Ceil(timeStamp) % 60 == 0)
        {
            int rand = UniqueRandom();
            Debug.Log("Change Season" + rand);
            EnvironmentControl.current.activeSeasonIndex = rand;
            return;
        }

        if(EnvironmentControl.current.lastActiveSeasonindex != EnvironmentControl.current.activeSeasonIndex)
        {
            int activeSeason = EnvironmentControl.current.activeSeasonIndex;
            EnvironmentControl.current.lastActiveSeasonindex = activeSeason;
            if(activeSeason == 0)
            {
                ChangeBGM(gameBGM, true);
            } else
            {
                ChangeBGM(dessertBGM, true);
            }
        }
    }

    float LastAction;

    public void Tap()
    {
        if (Time.time - LastAction <= 0.2f) return;

        //Debug.Log("tap");
        myChar.Jump();
        LastAction = Time.time;
    }

    bool isDragging = false;
    Vector2 beginDragPosition;

    public void BeginDrag(BaseEventData bed)
    {
        if (Time.time - LastAction <= 0.2f) return;

        //Debug.Log("BeginDrag");
        PointerEventData ped = bed as PointerEventData;
        beginDragPosition = ped.position;
        LastAction = Time.time;
        isDragging = true;
    }

    public void EndDrag(BaseEventData bed)
    {
        if (isDragging == false) return;

        //Debug.Log("EndDrag");
        PointerEventData ped = bed as PointerEventData;
        Vector2 endDragPosition = ped.position;

        if (beginDragPosition.x > endDragPosition.x)
        {
            //Debug.Log("Drag ke kri");
            myChar.Turn(-1);
        }
        else if (beginDragPosition.x < endDragPosition.x)
        {
            //Debug.Log("Drag ke kanan");
            myChar.Turn(1);
        }
    }

    public void StartCountdown()
    {
        currCountdown = countdownCount;
        countdownText.text = currCountdown.ToString();
        countdownText.gameObject.SetActive(true);
        Invoke("CountingDown", 1);
    }

    public void GameOver()
    {
        GameoverPanel.SetActive(true);
        ChangeBGM(gameOverBGM, false);
        SaveScore();
    }

    void CountingDown()
    {
        currCountdown--;
        countdownText.text = currCountdown.ToString();
        if(currCountdown > 0)
        {
            Invoke("CountingDown", 1);
        }
        else
        {
            countdownText.gameObject.SetActive(false);
            myChar.StartRun();
        }
    }
    

    public void SaveScore()
    {
        if (score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
    }

    public void OnApplicationPause(bool pause)
    {
        pausePanel.SetActive(pause);
        if(pause == true)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }

    public void Retry()
    {
        SaveScore();
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
        SaveScore();
        SceneManager.LoadScene("Menu");
    }

    void PopulateLifeImage()
    {
        for (int i = 1; i < myChar.life; i++)
        {
            GameObject life = Instantiate(lifePanel.transform.GetChild(0).gameObject, lifePanel.transform);
            life.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 120, 0);
        }
    }

    public void ChangeBGM(AudioClip newBGM, bool isLooping)
    {
        bgmSource.clip = newBGM;
        bgmSource.loop = isLooping;
        bgmSource.Play();
    }

    public void AddScore(int newScore)
    {
        score += newScore;
        scoreCountTxt.text = score.ToString();
    }
}
