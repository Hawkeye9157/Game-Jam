using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public CarController CarController;


    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject inGameUI;
    public GameObject gameEndUI;

    [Header("In-Game UI")]
    public TMP_Text lapTimeText;
    public TMP_Text lapCountText;
    public TMP_Text speedCounter;
    public TMP_Text positionText;

    [Header("Game End UI")]
    public TMP_Text winLoseText;
    public TMP_Text finalLapTimeText;
    public Button restartButton;
    public Button quitButton;

    private float currentLapTime;
    private int lapCount;
    private bool isRacing;


    private string FormatPosition(int pos)
    {
        switch (pos)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{pos}th";
        }
    }




    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Start()
    {

        mainMenuUI.SetActive(false);
        inGameUI.SetActive(false);
        gameEndUI.SetActive(false);

        SetGameState("MainMenu");

    }

    private void Update()
    {
        float speed = CarController.CurrentCarSpeed;
        if (isRacing)
        {
            currentLapTime += Time.deltaTime;
            speedCounter.text = speed.ToString();
            UpdateLapTimeDisplay();

        }
    }


    public void RegisterLap()
    {
        lapCount++;
        lapCountText.text = $"{lapCount}/4";

        if (lapCount >= 4)
        {
            EndGame();
        }
    }

    public void SetGameState(string state)
    {
        // Hide all UI first
        mainMenuUI.SetActive(false);
        inGameUI.SetActive(false);
        gameEndUI.SetActive(false);

        switch (state)
        {
            case "MainMenu":
                mainMenuUI.SetActive(true);
                Time.timeScale = 0;
                break;

            case "InGame":
                inGameUI.SetActive(true);
                Time.timeScale = 1;
                StartNewRace();
                break;

            case "GameEnd":
                gameEndUI.SetActive(true);
                Time.timeScale = 0;
                finalLapTimeText.text = FormatTime(currentLapTime);
                break;
        }
    }

    private void StartNewRace()
    {
        currentLapTime = 0f;
        lapCount = 0;
        isRacing = true;
        UpdateLapTimeDisplay();
        lapCountText.text = "0/4";
    }

    private void EndGame()
    {
        isRacing = false;
        SetGameState("GameEnd");
    }

    private void UpdateLapTimeDisplay()
    {
        lapTimeText.text = FormatTime(currentLapTime);
    }

    private string FormatTime(float time)
    {
        return $"{time:00.00}s";
    }

    public void StartGame() => SetGameState("InGame");
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void QuitGame()
    {
        Debug.Log("is quitting?");
        Application.Quit();

    }

}