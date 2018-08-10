using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public BossShip enemyShip;
    public PlayerShip player;

    public int points = 0;
    public int numberOfLives = 6;

    public bool gameStarting;
    public bool gameOn;

    public GameObject MainMenuUI;
    public GameObject GameplayUI;

    public GameObject InputNewLeaderboardScoreUI;
    public GameObject LeaderboardUI;
    public GameObject transitionUI;

    public Text LeaderboardPoints1;
    public Text LeaderboardName1;
    public Text LeaderboardPoints2;
    public Text LeaderboardName2;
    public Text LeaderboardPoints3;
    public Text LeaderboardName3;
    public Text LeaderboardPoints4;
    public Text LeaderboardName4;
    public Text LeaderboardPoints5;
    public Text LeaderboardName5;
    private int scoreIndexToAffect = 0;

    private bool showingLeaderboard;
    public float hideLeaderboardTime = 5f;
    private float hideLeaderboardRuntime;

    public float lostALifeWaitTime = 3f;
    private float lostALifeWaitTimeRuntime;

    public GameObject[] livesUI;

    public Text pointsUI;
    public Text creditAmountUI;

    public int[] polybiusScores = new int[5];
    public string[] polybiusNames = new string[5];

    public new AudioSource audio;
    public AudioClip CreditInputSound;
    public AudioClip TransitionSound;
    public AudioClip ExplosionSound;
    public AudioClip Music;
    private bool transitioning;
    private int numberOfCredits = 0;
    private float startingHealthValue = 200;
    private int roundNum = 1;
    private bool BeingUsed;

    public void Use()
    {
        if (!BeingUsed)
        {
            player.Use();
            BeingUsed = true;
        }
        else
        {
            player.Use();
            BeingUsed = false;
        }
    }

    private void OnEnable()
    {
        MainMenuUI.SetActive(true);
        roundNum = 1;
        numberOfCredits = 0;
        creditAmountUI.text = "Credits "+numberOfCredits;
        GameplayUI.SetActive(false);
        LeaderboardUI.SetActive(false);
    }

    private void Start()
    {

        if (!PlayerPrefs.HasKey("PolybiusScore1"))
        {
            PlayerPrefs.SetInt("PolybiusScore1", 00125375);
            PlayerPrefs.SetString("PolybiusScore1Name", "JANITORR");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore2"))
        {
            PlayerPrefs.SetInt("PolybiusScore2", 00045075);
            PlayerPrefs.SetString("PolybiusScore2Name", "JANITORA");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore3"))
        {
            PlayerPrefs.SetInt("PolybiusScore3", 00025050);
            PlayerPrefs.SetString("PolybiusScore3Name", "JANITORE");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore4"))
        {
            PlayerPrefs.SetInt("PolybiusScore4", 00001025);
            PlayerPrefs.SetString("PolybiusScore4Name", "JANITORT");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore5"))
        {
            PlayerPrefs.SetInt("PolybiusScore5", 00000025);
            PlayerPrefs.SetString("PolybiusScore5Name", "JANITORC");
            PlayerPrefs.Save();
        }

        GetLeaderboardData();
    }
  
    private void Update()
    {
        if (!gameOn)
        {
            if (!gameStarting)
            {
                if (BeingUsed && Input.GetKeyDown(KeyCode.Return) && MainMenuUI.gameObject.activeSelf)
                {
                    PlaySound(CreditInputSound,0.75f);
                    transitioning = true;
                    numberOfCredits++;
                    creditAmountUI.text = "Credits "+ numberOfCredits;
                    gameStarting = true;
                }
                else if (BeingUsed && (MainMenuUI.gameObject.activeSelf
                    || LeaderboardUI.gameObject.activeSelf) 
                    && (Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.RightArrow)
                    || Input.GetKeyDown(KeyCode.UpArrow)
                    || Input.GetKeyDown(KeyCode.DownArrow)))
                {
                    //Toggle Leaderboard
                    if (!showingLeaderboard && MainMenuUI.gameObject.activeSelf)
                    {
                        ShowLeaderBoard();
                    }
                    else
                    {
                        HideLeaderBoard();
                    }
                }
                else
                {
                    if (showingLeaderboard)
                    {
                        if (hideLeaderboardRuntime <= 0)
                        {
                            HideLeaderBoard();
                        }
                        else
                        {
                            hideLeaderboardRuntime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (!audio.isPlaying &&
                    (!player.animation.isPlaying && !enemyShip.animation.isPlaying))
                {
                    MainMenuUI.SetActive(false);

                    if (!transitioning)
                    {
                        PlaySound(Music, 1f, true);
                        GameplayUI.SetActive(true);

                        EnableLivesUI();

                        lostALifeWaitTimeRuntime = lostALifeWaitTime;
                        enemyShip.gameObject.SetActive(true);
                        enemyShip.SetHealth(startingHealthValue*roundNum);
                        player.gameObject.SetActive(true);

                        gameOn = true;
                        gameStarting = false;
                    }
                    else
                    {
                        if (!transitionUI.gameObject.activeSelf)
                        {
                            GameplayUI.SetActive(false);

                            if (player.gameObject.activeSelf)
                            {
                                player.ResetObject();
                            }

                            enemyShip.DisableExplosion();
                            enemyShip.gameObject.SetActive(false);
                            player.gameObject.SetActive(false);
                            transitionUI.gameObject.SetActive(true);
                            PlaySound(TransitionSound, 0.75f);
                        }
                        else
                        {
                            transitionUI.gameObject.SetActive(false);
                            transitioning = false;
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return) && MainMenuUI.gameObject.activeSelf)
                {
                    PlaySound(CreditInputSound, 0.75f);
                    numberOfCredits++;
                    creditAmountUI.text = "Credits " + numberOfCredits;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            if (!player.gameObject.activeSelf)
            {

                //Wait a number of seconds
                if (lostALifeWaitTimeRuntime <= 0)
                {
                    //If player dead, reset player, subtract a live
                    LostALife();
                    lostALifeWaitTimeRuntime = lostALifeWaitTime;
                }
                else
                {

                    lostALifeWaitTimeRuntime -= Time.deltaTime;
                }
            }
            else
            {
                if (enemyShip.dead)
                {
                    AddPoints(2575);
                    gameOn = false;
                    gameStarting = true;
                    transitioning = true;
                    PlaySound(ExplosionSound, 0.75f);
                }
            }
        }
    }

    private void SetLeaderBoardActive(bool condition)
    {
        if(condition)
        {
            LeaderboardUI.SetActive(condition);

            LeaderboardName1.text = polybiusNames[0];
            FormatPointsUI(LeaderboardPoints1, polybiusScores[0]);

            LeaderboardName2.text = polybiusNames[1];
            FormatPointsUI(LeaderboardPoints2, polybiusScores[1]);

            LeaderboardName3.text = polybiusNames[2];
            FormatPointsUI(LeaderboardPoints3, polybiusScores[2]);

            LeaderboardName4.text = polybiusNames[3];
            FormatPointsUI(LeaderboardPoints4, polybiusScores[3]);

            LeaderboardName5.text = polybiusNames[4];
            FormatPointsUI(LeaderboardPoints5, polybiusScores[4]);
            
            MainMenuUI.SetActive(false);
            GameplayUI.SetActive(false);
        }
        else
        {
            LeaderboardUI.SetActive(condition);
            MainMenuUI.SetActive(true);
            creditAmountUI.text = "Credits 0";
            GameplayUI.SetActive(false);
        }
    }

    public void NextRound()
    {
        roundNum++;
    }

    private void GetLeaderboardData()
    {
        polybiusScores[0] = PlayerPrefs.GetInt("PolybiusScore1");
        polybiusNames[0] = PlayerPrefs.GetString("PolybiusScore1Name");
        polybiusScores[1] = PlayerPrefs.GetInt("PolybiusScore2");
        polybiusNames[1] = PlayerPrefs.GetString("PolybiusScore2Name");
        polybiusScores[2] = PlayerPrefs.GetInt("PolybiusScore3");
        polybiusNames[2] = PlayerPrefs.GetString("PolybiusScore3Name");
        polybiusScores[3] = PlayerPrefs.GetInt("PolybiusScore4");
        polybiusNames[3] = PlayerPrefs.GetString("PolybiusScore4Name");
        polybiusScores[4] = PlayerPrefs.GetInt("PolybiusScore5");
        polybiusNames[4] = PlayerPrefs.GetString("PolybiusScore5Name");
    }

    private void SaveLeaderBoardData()
    {
        PlayerPrefs.SetInt("PolybiusScore1", polybiusScores[0]);
        PlayerPrefs.SetString("PolybiusScore1Name", polybiusNames[0]);
        PlayerPrefs.SetInt("PolybiusScore2", polybiusScores[1]);
        PlayerPrefs.SetString("PolybiusScore2Name", polybiusNames[1]);
        PlayerPrefs.SetInt("PolybiusScore3", polybiusScores[2]);
        PlayerPrefs.SetString("PolybiusScore3Name", polybiusNames[2]);
        PlayerPrefs.SetInt("PolybiusScore4", polybiusScores[3]);
        PlayerPrefs.SetString("PolybiusScore4Name", polybiusNames[3]);
        PlayerPrefs.SetInt("PolybiusScore5", polybiusScores[4]);
        PlayerPrefs.SetString("PolybiusScore5Name", polybiusNames[4]);
        PlayerPrefs.Save();
    }
    
    private void EnableLivesUI()
    {
        for(int i = 0; i < numberOfLives; i++)
        {
            livesUI[i].SetActive(true);
        }
    }

    private void GameOver()
    {
        audio.Stop();
        gameOn = false;
        roundNum = 1;
        numberOfCredits = 0;
        enemyShip.DisableActiveShapes();
        enemyShip.gameObject.SetActive(gameOn);
        numberOfLives = 6;

        bool newScore = false;

        for(int i = 4; i >= 0;)
        {
            if(polybiusScores[i] < points)
            {
                newScore = true;
                i--;
            }
            else
            {
                scoreIndexToAffect = i;
                break;
            }
        }

        if (newScore)
        {
            InputNewLeaderboardScoreUI.SetActive(true);
        }
        else
        {
            ShowLeaderBoard();
        }
    }

    private void FormatPointsUI(Text UIObject, int pointsValue)
    {
        int length = pointsValue.ToString().Length;

        StringBuilder newScoreText = new StringBuilder();
        
        newScoreText.Append(pointsValue);

        for (int i = length; i < 8; i++)
        {
            newScoreText.Insert(0,"0");
        }

        UIObject.text = newScoreText.ToString();
    }

    private void LostALife()
    {
        if (numberOfLives == 0)
        {
            GameOver();
        }
        else
        {
            livesUI[--numberOfLives].SetActive(false);
            player.gameObject.SetActive(true);
        }
    }

    private void HideLeaderBoard()
    {
        showingLeaderboard = false;
        SetLeaderBoardActive(showingLeaderboard);
        hideLeaderboardRuntime = hideLeaderboardTime;
    }

    public void ShowLeaderBoard()
    {
        showingLeaderboard = true;
        SetLeaderBoardActive(showingLeaderboard);
        hideLeaderboardRuntime = hideLeaderboardTime;
    }

    public void ApplyPointsToLeaderboard()
    {
        polybiusScores[scoreIndexToAffect] = points;
        SaveLeaderBoardData();
    }

    public void PassFinalName(string text)
    {
        polybiusNames[scoreIndexToAffect] = text;
    }

    public void AddPoints(int amount)
    {
        points += amount;

        FormatPointsUI(pointsUI, points);
    }

    public void StopTransitioning()
    {
        transitioning = false;
    }

    public void PlaySound(AudioClip clip, float newVolume, bool repeat = false)
    {
        if(repeat)
        {
            audio.loop = true;
        }
        else
        {
            audio.loop = false;
        }

        audio.volume = newVolume;
        audio.clip = clip;
        audio.Play();
    }
}
