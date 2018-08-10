using UnityEngine.UI;
using UnityEngine;
using System;

public class LeaderboardInput : MonoBehaviour {

    enum NameCharacter { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, _ };

    public Text ScoreTextUI;
    public Text inputTextUI;
    public Text FinalNameTextUI;

    public GameManager gameManager;

    private NameCharacter selectedChar;

    private char[] inputText = {' ' ,' ' ,' ' ,' ' , ' ', ' ', ' ', ' '};
    private char[] finalNameText = { '_', '_', '_', '_', '_', '_', '_', '_'};

    private int letterIndex = 0;

    private void OnEnable()
    {
        letterIndex = 0;

        selectedChar = new NameCharacter();
        
        inputTextUI.text = new String(inputText);

        FinalNameTextUI.text = new String(finalNameText);

        ScoreTextUI.text = gameManager.pointsUI.text;

        UpdateText(0);
    }

    private void UpdateText(int index,bool final = false)
    {
        if(final)
        {
            inputText[index] = ' ';
            finalNameText[index] = selectedChar.ToString()[0];
        }
        else
        {
            inputText[index] = selectedChar.ToString()[0];
            finalNameText[index] = ' ';
        }
        
        inputTextUI.text = new String(inputText);
        FinalNameTextUI.text = new String(finalNameText);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if((int) selectedChar >= 26)
            {
                selectedChar = NameCharacter.A;
            }
            else
            {
                selectedChar++;
            }

            UpdateText(letterIndex);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            if (selectedChar <= 0)
            {
                selectedChar = NameCharacter._;
            }
            else
            {
                selectedChar--;
            }

            UpdateText(letterIndex);
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            UpdateText(letterIndex,true);
            letterIndex++;

            //if we've hit the end disable this gameobject and enable leaderboard
            if(letterIndex == 8)
            {
                for(int i = 0; i < finalNameText.Length; i++)
                {
                    finalNameText[i] = '_';
                }

                gameManager.PassFinalName(FinalNameTextUI.text);
                gameManager.ApplyPointsToLeaderboard();
                gameManager.ShowLeaderBoard();
                gameObject.SetActive(false);
            }
            else
            {
                UpdateText(letterIndex);
            }
        }
        else 
        {
            return;
        }
    }
}
