using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelManagement
{
    public class StartManager : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button accountButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private Button startButton;

        [SerializeField] private Button[] backToMainMenuButtons;

        [Header("General Settings")]
        [SerializeField] private float transitionSpeed = 0.5f;
        [SerializeField] private LeanTweenType tweenType;

        private void Start()
        {
            UINavigator.Push("LoadingScreen");

            string guid = PlayerPrefs.GetString("cc2_guid");
            if (string.IsNullOrEmpty(guid))
            {
                ShowPageVert(UINavigator.Push("NewUserLogin"), 2500f);
            }

            //Back buttons to the main screen
            for (int i = 0; i < backToMainMenuButtons.Length; i++)
            {
                backToMainMenuButtons[i].onClick.AddListener(() =>
                {
                    HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                    ShowPageHori(UINavigator.Push("MainScreen"), -1920f);
                });
            }

            playButton.onClick.AddListener(() =>
            {
                HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                ShowPageHori(UINavigator.Push("DifficultyScreen"), -1920f);
            });

            leaderboardButton.onClick.AddListener(() => 
            {
                HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                ShowPageHori(UINavigator.Push("LeaderboardPage"), 3000f);
            });

            startButton.onClick.AddListener(() => 
            {
                ShowPageVert(UINavigator.Push("LoadingScreen"), 3000, () =>
                {
                    SceneManager.LoadSceneAsync(1);
                });
            });
        }

        private void ShowPageVert(GameObject page, float from, Action OnCompleted = null)
        {
            float prevPos = page.transform.position.y;
            page.transform.position = new Vector3(page.transform.position.x, from, page.transform.position.z);
            LeanTween.moveY(page, prevPos, transitionSpeed).setEase(tweenType).setOnComplete(() =>
            {
                if (OnCompleted != null)
                {
                    OnCompleted();
                }
            });
        }

        private void ShowPageHori(GameObject page, float from)
        {
            float prevPos = page.transform.position.x;
            page.transform.position = new Vector3(from, page.transform.position.y, page.transform.position.z);
            LeanTween.moveX(page, prevPos, transitionSpeed).setEase(tweenType);
        }

        private void HidePageHori(GameObject page, float to)
        {
            float prevPos = page.transform.position.x;
            LeanTween.moveX(page, to, transitionSpeed).setEase(tweenType).setOnComplete(() =>
            {
                page.SetActive(false);
                page.transform.position = new Vector3(prevPos, page.transform.position.y, page.transform.position.z);
            });
        }
    }
}