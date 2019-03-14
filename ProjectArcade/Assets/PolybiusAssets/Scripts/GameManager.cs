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
    private int numberOfCredits = 0;
    private int roundNum = 1;
    private int startNumberOfLives;
    private bool BeingUsed;
    private bool transitioning;
    private bool NextScreenTriggered;
    private readonly float startingHealthValue = 200;

    public bool IsBeingUsed()
    {
        return BeingUsed;
    }

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
        startNumberOfLives = numberOfLives;

        if (!PlayerPrefs.HasKey("PolybiusScore1"))
        {
            PlayerPrefs.SetInt("PolybiusScore1", 00125375);
            PlayerPrefs.SetString("PolybiusScore1Name", "JANITOR ");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore2"))
        {
            PlayerPrefs.SetInt("PolybiusScore2", 00045075);
            PlayerPrefs.SetString("PolybiusScore2Name", "JOE     ");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore3"))
        {
            PlayerPrefs.SetInt("PolybiusScore3", 00025050);
            PlayerPrefs.SetString("PolybiusScore3Name", "GABE    ");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore4"))
        {
            PlayerPrefs.SetInt("PolybiusScore4", 00001025);
            PlayerPrefs.SetString("PolybiusScore4Name", "MIKE    ");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("PolybiusScore5"))
        {
            PlayerPrefs.SetInt("PolybiusScore5", 00000025);
            PlayerPrefs.SetString("PolybiusScore5Name", "S O R A ");
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
                if (BeingUsed && Input.GetButtonDown("Start") && numberOfCredits == 0 && MainMenuUI.gameObject.activeSelf)
                {
                    PlaySound(CreditInputSound, 0.75f);
                    transitioning = true;
                    numberOfCredits++;
                    creditAmountUI.text = "Credits " + numberOfCredits;
                    gameStarting = true;
                }
                else if(NextScreenTriggered && (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0))
                {
                    NextScreenTriggered = false;
                }
                else if (BeingUsed && (MainMenuUI.gameObject.activeSelf
                    || LeaderboardUI.gameObject.activeSelf) && 
                    ((Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")) || 
                    (!NextScreenTriggered && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))))
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

                    NextScreenTriggered = true;
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
                        enemyShip.SetHealth(startingHealthValue * roundNum);
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
                            PlaySound(TransitionSound, 1f);
                        }
                        else
                        {
                            transitionUI.gameObject.SetActive(false);
                            transitioning = false;
                        }
                    }
                }
                //else if (Input.GetButtonDown("Start") && numberOfCredits == 0 && MainMenuUI.gameObject.activeSelf)
                //{
                //    Debug.Log("Start = "+Input.GetButtonDown("Start"));
                //    Debug.Log("Called");
                //    PlaySound(CreditInputSound, 0.75f);
                //    numberOfCredits++;
                //    creditAmountUI.text = "Credits " + numberOfCredits;
                //}
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

	//Returns true if a profane word was found in the string
    public bool ProfanityCheck(string text)
    {
        string input = text.ToLower();

        string []profaneWords = {"latino","mexican","blak","blac","whit","white", "shit", "poop", "puss","pussie","pussy",
                                 "fuc","fuk","phuck","phuc","phuk","black","suc","dik" ,"tity","titties","kike","kite"
								 ,"priest","suk","suck","sux","sex","hole","spic","dix","nut","tit","titie", "moolies",
								 "niger","nig","mother","fuck","fart","ass","dic","nazi","hitler", "stalin",
								 "ashole","bitch","cuck","cock","kek","kuk","cuc","nerd","lonely","bad"};
        
        //Remove spaces
        for(int i = 0; i < input.Length; i++)
        {
            if(input[i] == ' ')
            {
                input = input.Remove(i,1);
				i--;
            }
        }
		
		char currentTrackedChar=' ';
		string compactInput = "";
		string compactByTwo = "";
		
		//Take out any reoccuring text
        for(int i = 0; i < input.Length; i++)
        {
            if(input[i] != currentTrackedChar)
            {
				compactInput += input[i];
                currentTrackedChar = input[i];
            }
        }
		
		//Take out any reoccuring text (but allows 2 characters to occur)
        for(int i = 0, secondOccured=0; i < input.Length; i++)
        {
            if(input[i] != currentTrackedChar)
            {
				compactByTwo += input[i];
                currentTrackedChar = input[i];
				secondOccured = 0;
            }
			else
			{
				if(secondOccured == 0)
				{
					secondOccured = 1;
					compactByTwo += input[i];
				}
			}
        }
		
		input = compactInput;
		
		//Console.WriteLine("Parsed = "+input);

		//For each profane word
        for (int profaneWordIndex = 0; profaneWordIndex < profaneWords.Length; profaneWordIndex++)
        {
			//Run that word throughout the name to see if any of them are there
            for (int left = 0, right = profaneWords[profaneWordIndex].Length; left < input.Length; left++)
            {
				//if(input.Length-(left+right) >= 0)
				//Console.WriteLine(profaneWords[profaneWordIndex]+" against "+input.Substring(left,right));
				   
                if(input.Length-(left+right) >= 0 && profaneWords[profaneWordIndex] == input.Substring(left,right))
                {
                    return true;
                }
            }
        }
		
		input = compactByTwo;
		
		//Console.WriteLine("Parsed = "+input);

		//For each profane word
        for (int profaneWordIndex = 0; profaneWordIndex < profaneWords.Length; profaneWordIndex++)
        {
			//Run that word throughout the name to see if any of them are there
            for (int left = 0, right = profaneWords[profaneWordIndex].Length; left < input.Length; left++)
            {
				//if(input.Length-(left+right) >= 0)
				//Console.WriteLine(profaneWords[profaneWordIndex]+" against "+input.Substring(left,right));
				   
                if(input.Length-(left+right) >= 0 && profaneWords[profaneWordIndex] == input.Substring(left,right))
                {
                    return true;
                }
            }
        }

        return false;
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
        for(int i = 0; i < livesUI.Length; i++)
        {
            if (i < numberOfLives)
            {
                livesUI[i].SetActive(true);
            }
            else
            {
                livesUI[i].SetActive(false);
            }
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
        numberOfLives = startNumberOfLives;

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
