using UnityEngine.UI;
using UnityEngine;
using System;

public class LeaderboardInput : MonoBehaviour {

    enum NameCharacter { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, _};

    public Text ScoreTextUI;
    public Text inputTextUI;
    public Text FinalNameTextUI;

    public GameManager gameManager;

    private NameCharacter selectedCharIndex;

    //Text that will change to whatever character the user selected
    private char[] inputText = {' ' ,' ' ,' ' ,' ' , ' ', ' ', ' ', ' '};

    //Represents the 'background text', I.E. the character the user confirmed 
    private char[] finalNameText = { '-', '-', '-', '-', '-', '-', '-', '-'};

    private int confirmedCharIndex = 0;

    private bool NextCharTriggered;

    private void OnEnable()
    {
        confirmedCharIndex = 0;

        selectedCharIndex = new NameCharacter();
        
        inputTextUI.text = new String(inputText);

        FinalNameTextUI.text = new String(finalNameText);

        ScoreTextUI.text = gameManager.pointsUI.text;

        UpdateText(0);
    }

    private void UpdateText(int index,bool letterConfirmed = false, bool ignoreIndexes = false)
    {
        if (!ignoreIndexes)
        {
            //The player confirmed the letter
            if (letterConfirmed)
            {
                inputText[index] = ' ';
                finalNameText[index] = selectedCharIndex.ToString()[0];
            }
            //The player is selecting text
            else
            {
                inputText[index] = selectedCharIndex.ToString()[0];
                finalNameText[index] = ' ';
            }
        }
        
        inputTextUI.text = new String(inputText);
        FinalNameTextUI.text = new String(finalNameText);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!gameManager.IsBeingUsed())
        {
            return;
        }
        else if (!NextCharTriggered && ((Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0) || Input.GetAxis("Vertical") > 0))
        {
            if ((int)selectedCharIndex >= 26)
            {
                selectedCharIndex = NameCharacter.A;
            }
            else
            {
                selectedCharIndex++;
            }

            UpdateText(confirmedCharIndex);

            NextCharTriggered = true;
        }
        else if (!NextCharTriggered && ((Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0) || Input.GetAxis("Vertical") < 0))
        {

            if (selectedCharIndex <= 0)
            {
                selectedCharIndex = NameCharacter._;
            }
            else
            {
                selectedCharIndex--;
            }

            UpdateText(confirmedCharIndex);

            NextCharTriggered = true;
        }
        else if (NextCharTriggered && Input.GetAxis("Vertical") == 0)
        {
            NextCharTriggered = false;
        }
        else if (Input.GetButtonDown("Back") && confirmedCharIndex > 0)
        {
            finalNameText[confirmedCharIndex] = '-';
            inputText[confirmedCharIndex] = ' ';
            confirmedCharIndex--;
            finalNameText[confirmedCharIndex] = ' ';
            inputText[confirmedCharIndex] = selectedCharIndex.ToString()[0];
            UpdateText(0,false,true);
        }
        else if (Input.GetButtonDown("Fire") || Input.GetButtonDown("Start"))
        {
            if (selectedCharIndex == NameCharacter._)
            {
                inputText[confirmedCharIndex] = selectedCharIndex.ToString()[0];
                finalNameText[confirmedCharIndex] = ' ';
                UpdateText(0, false, true);
            }
            else
            {
                UpdateText(confirmedCharIndex, true);
            }

            confirmedCharIndex++;

            //if we've hit the end disable this gameobject and enable leaderboard
            if (confirmedCharIndex == 8)
            {
                //Reset input text
                for (int i = 0; i < finalNameText.Length; i++)
                {
                    finalNameText[i] = '-';
                    inputText[i] = ' ';
                }

                //CHECK FOR PROFANITY HERE
                if(gameManager.ProfanityCheck(FinalNameTextUI.text))
                {
                    confirmedCharIndex = 0;
					inputText[confirmedCharIndex] = selectedCharIndex.ToString()[0];
                    UpdateText(0, false, true);
                }
                else
                {

                gameManager.PassFinalName(FinalNameTextUI.text);
                gameManager.ApplyPointsToLeaderboard();
                gameManager.ShowLeaderBoard();
                gameObject.SetActive(false);

                }
            }
            else
            {
                UpdateText(confirmedCharIndex);
            }
        }
    }
}
