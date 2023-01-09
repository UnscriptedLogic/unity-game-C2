using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Profiling;
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

        [Header("Form Fields(Sign Up)")]
        [SerializeField] private TMP_InputField signupUsername;
        [SerializeField] private TMP_InputField signupPassword;
        [SerializeField] private TMP_InputField signupRepeatPassword;
        [SerializeField] private Button switchToLogin;
        [SerializeField] private Button submitSignUp;
        
        [Header("Form Fields(Log In)")]
        [SerializeField] private TMP_InputField loginUsername;
        [SerializeField] private TMP_InputField loginPassword;
        [SerializeField] private Button switchToSignUp;
        [SerializeField] private Button submitLogIn;

        [SerializeField] private TextMeshProUGUI errorTMP;

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
                ShowPageVert(UINavigator.Push("NewUserLogin"), 3000f);
                UINavigator.Push("SignUp");
            }

            InitButtons();
            InitSubmitForms();
        }

        private void InitButtons()
        {
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
                ShowPageHori(UINavigator.Push("LeaderboardPage"));
            });

            startButton.onClick.AddListener(() =>
            {
                ShowPageVert(UINavigator.Push("LoadingScreen"), 3000, () =>
                {
                    SceneManager.LoadSceneAsync(1);
                });
            });

            switchToLogin.onClick.AddListener(() =>
            {
                HidePageHori(UINavigator.instance.Navigator.Peek(), -4000f);
                ShowPageHori(UINavigator.Push("LogIn"), 4000f);
            });

            switchToSignUp.onClick.AddListener(() =>
            {
                HidePageHori(UINavigator.instance.Navigator.Peek(), 3000f);
                ShowPageHori(UINavigator.Push("SignUp"), -3000f);
            });
        }

        private void InitSubmitForms()
        {
            submitLogIn.onClick.AddListener(() =>
            {
                string username = signupUsername.text;
                string password = signupPassword.text;
                string repeatPassword = signupRepeatPassword.text;

                if (string.IsNullOrEmpty(username))
                {
                    ShowError("Please input a username");
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    ShowError("Please input a password");
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    ShowError("Please input the repeat password");
                    return;
                }

                if (password == repeatPassword)
                {
                    ShowError("Passwords do not match!");
                    return;
                }

                AWSManager.UserPayload userPayload = new AWSManager.UserPayload(username, password);

                AWSManager.instance.CreateUser(userPayload, res =>
                {
                    HidePageVert(UINavigator.instance.Navigator.Peek());
                    return;
                }, err =>
                {
                    errorTMP.text = err;
                });
            });

            submitSignUp.onClick.AddListener(() =>
            {

            });
        }

        private void ShowError(string errorText)
        {
            errorTMP.gameObject.SetActive(true);
            errorTMP.text = errorText;
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

        private void HidePageVert(GameObject page, float to = 3000f)
        {
            float prevPos = page.transform.position.y;
            LeanTween.moveY(page, to, transitionSpeed).setEase(tweenType).setOnComplete(() =>
            {
                page.SetActive(false);
                page.transform.position = new Vector3(page.transform.position.x, prevPos, page.transform.position.z);
            });
        }

        private void ShowPageHori(GameObject page, float from = 3000f)
        {
            float prevPos = page.transform.position.x;
            page.transform.position = new Vector3(from, page.transform.position.y, page.transform.position.z);
            LeanTween.moveX(page, prevPos, transitionSpeed).setEase(tweenType);
        }

        private void HidePageHori(GameObject page, float to = 3000f)
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