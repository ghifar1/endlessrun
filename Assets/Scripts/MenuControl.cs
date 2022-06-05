using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private GameControl manager;
    [SerializeField] private Text countDown;

    public Text highScoreText;
    public Text scoreCountTxt;
    public Transform lifePanel;



    private void Awake()
    {
        manager.Score += OnScoreChange;
        manager.highscore += OnHighScoreChange;
        manager.StartCountDown += OnCountDownChange;
    }

	private void OnDestroy()
	{
        manager.Score -= OnScoreChange;
        manager.highscore -= OnHighScoreChange;
        manager.StartCountDown -= OnCountDownChange;
    }

	// Update is called once per frame
	public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void OnScoreChange(int newScore)
    {
        scoreCountTxt.text = newScore.ToString();
    }

    private void OnHighScoreChange(string id)
    {
       highScoreText.text = PlayerPrefs.GetInt(id).ToString();
    }

    private void OnCountDownChange(int value)
    {
        if (value == 0)
        {
            countDown.gameObject.SetActive(false);
        }
        countDown.gameObject.SetActive(true);
        countDown.text = value.ToString();

    }
}
