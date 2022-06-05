using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    //Call class from another class
    // public static CharacterControl character;
    
    #region Event
    public Action<int> Score;
    public Action<int> CountLife;
    public Action<int> StartCountDown;
    public Action<bool> Running;
    public Action<string> highscore; 
    #endregion
    
    #region Shared Script
    [SerializeField] private CharacterControl character;
    [SerializeField] private EnvironmentControl envControl;
    [SerializeField] private MenuControl menu;
    #endregion

    #region Const Variable
    private const string PLAYERHIGHSCORE = "highscore";
    private const string deadAnimation = "dead";
    private const string fallAnimation = "fall";
    #endregion

    #region Variable

    public int score;

    [Header("User Interface")]
    public GameObject pausePanel;
    public GameObject GameoverPanel;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip gameBGM;
    public AudioClip gameOverBGM;

    private int life;
    private float LastAction;
    private int countdownCount = 4;

    private bool isDragging = false;
    private Vector2 beginDragPosition;
    
    #endregion
    
    private void Awake()
    {
        character.Hit += OnHitObstacle;
        character.Score += AddScore;
        character.Countdown += OnStartCountdown;
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CountingDown());
        PopulateLifeImage();
        ChangeBGM(gameBGM, true);
    }

    
    public void Tap()
    {
        if (Time.time - LastAction <= 0.2f) return;

        Debug.Log("tap");
        character.Jump();
        LastAction = Time.time;
    }

    public void BeginDrag(BaseEventData bed)
    {
        if (Time.time - LastAction <= 0.2f) return;

        Debug.Log("BeginDrag");
        PointerEventData ped = bed as PointerEventData;
        beginDragPosition = ped.position;
        LastAction = Time.time;
        isDragging = true;
    }

    public void EndDrag(BaseEventData bed)
    {
        if (isDragging == false) return;

        Debug.Log("EndDrag");
        PointerEventData ped = bed as PointerEventData;
        Vector2 endDragPosition = ped.position;

        if (beginDragPosition.x > endDragPosition.x)
        {
            Debug.Log("Drag ke kri");
            character.Turn(-1);
        }
        else if (beginDragPosition.x < endDragPosition.x)
        {
            Debug.Log("Drag ke kanan");
            character.Turn(1);
        }
    }
    
    private void OnStartCountdown(bool obj)
    {
        character.isStartCountdown = false;
        StartCoroutine(CountingDown());
    }

    public void GameOver()
    {
        GameoverPanel.SetActive(true);
        ChangeBGM(gameOverBGM, false);
        SaveScore();
    }

    private IEnumerator CountingDown()
    {
        while(countdownCount > 0)
		{
            if (countdownCount == 0) break;
            yield return new WaitForSeconds(1);
            countdownCount--;
            StartCountDown?.Invoke(countdownCount);
        }
        character.StartRun();
    }
    

    public void SaveScore()
    {
        if (score > PlayerPrefs.GetInt(PLAYERHIGHSCORE))
        {
            PlayerPrefs.SetInt(PLAYERHIGHSCORE, score);
            highscore?.Invoke(PLAYERHIGHSCORE);
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

    private void PopulateLifeImage()
    {
        for (int i = 1; i < life; i++)
        {
            GameObject life = Instantiate(menu.lifePanel.transform.GetChild(0).gameObject, menu.lifePanel.transform);
            life.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 120, 0);
        }
    }

    private void OnHitObstacle(bool value)
    {
        character.anim.SetTrigger(fallAnimation);
        menu.lifePanel.GetChild(life).gameObject.SetActive(false);
        
        if (life == 0)
        {
            character.anim.SetBool(deadAnimation, true);
            GameOver();
        }
    }

    private void OnDestroy()
    {
        character.Hit -= OnHitObstacle;
        character.Score -= AddScore;
        character.Countdown -= OnStartCountdown;
    }

    public void ChangeBGM(AudioClip newBGM, bool isLooping)
    {
        bgmSource.clip = newBGM;
        bgmSource.loop = isLooping;
        bgmSource.Play();
    }
    
    public void AddScore()
    {
        score++;
        Score?.Invoke(score);
    }

}
