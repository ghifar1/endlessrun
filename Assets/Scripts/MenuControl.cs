using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    public Text highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        Time.timeScale = 1;
    }

    // Update is called once per frame
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
