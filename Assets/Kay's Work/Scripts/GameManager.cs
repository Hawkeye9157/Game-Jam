using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public CarController CarController;
    public CheckpointMain checkpointMain;

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

    [Header("Countdown Settings")]
    public TMP_Text countdownText;
    public float countdownDuration = 3f;

    [Header("Audio")]
    public AudioSource backgroundAudio;
    public AudioSource countdownAudio;
    public AudioSource loseAudio;
    public AudioSource winAudio;
    public AudioSource mainMenuAudio;

    [Header("Particles")]
    public ParticleSystem confettiParticles;

    [Header ("Lap")]
    private float currentLapTime;
    private int lapCount;
    private bool isRacing;

    public bool isCountdownActive;
    private bool isWinner;


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

        //countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        SetGameState("MainMenu");

    }

    private void Update()
    {
        float speed = CarController.CurrentCarSpeed;
        if (isRacing)
        {
            currentLapTime += Time.deltaTime;
            speedCounter.text = $"{speed:0} km/h";
            UpdateLapTimeDisplay();

            Debug.Log("getting player position");
            int position = checkpointMain.GetPlayerPosition();
            Debug.Log("Player position: " + position);
            positionText.text = GetPositionSuffix(position);
            //get player position
            if (checkpointMain != null && CarController != null)
            {
                
            }

        }
    }

    private string GetPositionSuffix(int position)
    {
        Debug.Log("position: " + position);
        switch (position)
        {
            case 1: return position + "st";
            case 2: return position + "nd";
            case 3: return position + "rd";
            default: return position + "th";
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

        //countdown 
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Stop all audio first
        if (mainMenuAudio != null && mainMenuAudio.isPlaying)
            mainMenuAudio.Stop();
        if (backgroundAudio != null && backgroundAudio.isPlaying)
            backgroundAudio.Stop();

        switch (state)
        {
            case "MainMenu":
                mainMenuUI.SetActive(true);
                Time.timeScale = 0;

                if (mainMenuAudio != null && !mainMenuAudio.isPlaying)
                    mainMenuAudio.Play();
                break;

            case "InGame":
                inGameUI.SetActive(true);
                Time.timeScale = 1;

                StartCoroutine(StartCountdown());

                if (backgroundAudio != null && !backgroundAudio.isPlaying)
                    backgroundAudio.Play();

                break;

            case "GameEnd":
                gameEndUI.SetActive(true);
                Time.timeScale = 0;
                finalLapTimeText.text = FormatTime(currentLapTime);

                break;
        }
    }


    private IEnumerator StartCountdown() 
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            isCountdownActive = true;
        }

        //ai movement
        SetAICarsEnabled(false);

        // Disable car control
        if (CarController != null)
            CarController.SetControlEnabled(false);

        // Countdown sequence
        float timer = countdownDuration;
        int lastIntegerValue = Mathf.CeilToInt(timer) + 1;
        while (timer > 0)
        {
            int currentIntegerValue = Mathf.CeilToInt(timer);

            if (currentIntegerValue != lastIntegerValue)
            {
                if (countdownAudio != null)
                    countdownAudio.Play();

                lastIntegerValue = currentIntegerValue;
            }

            countdownText.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        // GO! message
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        // Hide countdown
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        isCountdownActive = false;
        if (countdownAudio != null && countdownAudio.isPlaying)
            countdownAudio.Stop();
        SetAICarsEnabled(true);

        // Enable car control and start race
        if (CarController != null)
            CarController.SetControlEnabled(true);

        StartNewRace(); 
    }

    private void SetAICarsEnabled(bool enabled) 
    {
        CarDriverAI[] aiCars = FindObjectsOfType<CarDriverAI>();
        foreach (CarDriverAI ai in aiCars)
        {
            ai.allowedToMove = enabled;
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

        // Play confetti
        if (confettiParticles != null)
            confettiParticles.Play();

      /*  int finalPosition = checkpointMain.GetPlayerPosition();
        isWinner = (finalPosition == 1);

        if (isWinner)
            WinGame();
        else
            LoseGame();
*/

        SetGameState("GameEnd");
    }

    private void WinGame()
    {
        // Play confetti only on win
        if (confettiParticles != null)
            confettiParticles.Play();

        winLoseText.text = "YOU WIN!";
        if (winAudio != null)
            winAudio.Play();
    }

    private void LoseGame()
    {
        winLoseText.text = "YOU LOSE";
        if (loseAudio != null)
            loseAudio.Play();
    }




    private void UpdateLapTimeDisplay()
    {
        lapTimeText.text = FormatTime(currentLapTime);
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        float seconds = time % 60;
        return $"{minutes:00}:{seconds:00.00}";
    }

    public void StartGame() => SetGameState("InGame");
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void QuitGame()
    {
        Debug.Log("is quitting?");
        Application.Quit();

    }

}