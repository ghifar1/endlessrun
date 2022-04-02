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
    public AudioClip gameOverBGM;

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

    float LastAction;

    public void Tap()
    {
        if(Time.time - LastAction <= 0.2f) return;

        Debug.Log("tap");
        myChar.Jump();
        LastAction = Time.time;
    }

    bool isDragging = false;
    Vector2 beginDragPosition;

    public void BeginDrag(BaseEventData bed)
    {
        if(Time.time - LastAction <= 0.2f) return;

        Debug.Log("BeginDrag");
        PointerEventData ped = bed as PointerEventData;
        beginDragPosition = ped.position;
        LastAction = Time.time;
        isDragging = true;
    }

    public void EndDrag(BaseEventData bed)
    {
        if(isDragging == false) return;

        Debug.Log("EndDrag");
        PointerEventData ped = bed as PointerEventData;
        Vector2 endDragPosition = ped.position;

        if(beginDragPosition.x > endDragPosition.x)
        {
            Debug.Log("Drag ke kri");
            myChar.Turn(-1);
        }
        else if(beginDragPosition.x < endDragPosition.x)
        {
            Debug.Log("Drag ke kanan");
            myChar.Turn(1);
        }
    }

    public void StartCountdown()
    {
        currCountdown = countdownCount;
        countdownText.text = currCountdown.ToString();
        countdownText.gameObject.SetActive(true);
        Invoke("Countingdown", 1);
    }

    public void GameOver()
    {
        GameoverPanel.SetActive(true);
        ChangeBGM(gameOverBGM, false);
        SaveScore();
    }

    public void SaveScore()
    {
        if(score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
    }

    void PopulateLifeImage()
    {
        for(int i = 1; i < myChar.life; i++)
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
}
