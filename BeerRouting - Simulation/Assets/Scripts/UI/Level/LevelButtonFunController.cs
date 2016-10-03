﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButtonFunController : MonoBehaviour
{
    public bool debugging = false;

    public GameObject buttonOpen;
    public GameObject buttonClosed;
    public int levelId;

    private Image[] stars;
    private GameState gameState;
    private List<LevelState> levelStates;
    private LevelState levelState;
    private Button button;

    // Use this for initialization
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        gameState = FindObjectOfType<GameState>();
        // gameState.UnlockAllLevelsCompletely();
        levelState = gameState.GetLevelStateByLevelId(levelId);
        levelStates = gameState.GetLevelStates();

        if (debugging)
            Debug.Log("LevelID=" + levelId + " levelState=" + levelState);

        Image[] images = buttonOpen.GetComponentsInChildren<Image>();
        stars = new Image[3];
        stars[0] = images[0];
        stars[1] = images[1];
        stars[2] = images[2];

        SetButtonState();
    }

    private void SetButtonState()
    {
        // Check if each level was passed with 3 stars.
        bool allUnlocked = true;
        for (int i = 0; i < levelStates.Count && i <= 23; i++)
        {
            if (levelStates[i].NumberOfStars != 3)
            {
                allUnlocked = false;
                break;
            }
        }
        if(levelStates.Count == 0)
        {
            allUnlocked = false;
        }

        // Check if level is closed.
        bool isOpen = false;
        if (levelId == 0)
        {
            // First tutorial level is always open.
            isOpen = true;
        }
        else
        {
            // If all levels are passed with 3 stars, unlock fun level.
            if (allUnlocked)
            {
                isOpen = true;
            }
        }

        if (isOpen)
        {
            SetOpen();
        }
        else
        {
            SetClosed();
        }
    }

    private void SetOpen()
    {
        // Set button to open state.
        buttonOpen.SetActive(true);
        buttonClosed.SetActive(false);
        button.enabled = true;

        // Deactivate all stars.
        for (int i = 0; i < 3; i++)
        {
            stars[i].gameObject.SetActive(false);
        }


        if (levelState != null)
        {
            // Activate correct numer of stars.
            switch (levelState.NumberOfStars)
            {
                case 3:
                    stars[2].gameObject.SetActive(true);
                    goto case 2;
                case 2:
                    stars[1].gameObject.SetActive(true);
                    goto case 1;
                case 1:
                    stars[0].gameObject.SetActive(true);
                    break;
            }
        }
    }

    private void SetClosed()
    {
        // Set button to closed state.
        buttonOpen.SetActive(false);
        buttonClosed.SetActive(true);
        button.enabled = false;
    }
}