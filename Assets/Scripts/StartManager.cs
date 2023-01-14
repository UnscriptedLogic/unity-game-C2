using System;
using System.Collections;
using TMPro;
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

        private void Start()
        {
            UINavigator.Push("MainScreen");
            UINavigator.Push("LoadingScreen");

            string existingUsername = PlayerPrefs.GetString("cc2_username");
            if (!string.IsNullOrEmpty(existingUsername))
            {
                //Try to sign in instead
                LogIn(existingUsername, PlayerPrefs.GetString("cc2_password"), () =>
                {
                    
                    return;
                }, () =>
                {
                    UINavigator.Push("NewUserLogin");
                    UINavigator.ShowPageVert(UINavigator.Push("SignUp"), 3000f);
                });
            }
            else
            {
                UINavigator.Push("NewUserLogin");
                UINavigator.ShowPageVert(UINavigator.Push("SignUp"), 3000f);
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
                    UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                    UINavigator.ShowPageHori(UINavigator.Push("MainScreen"), -1920f);
                });
            }

            //Start menu buttons
            playButton.onClick.AddListener(() =>
            {
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                UINavigator.ShowPageHori(UINavigator.Push("DifficultyScreen"), -1920f);
            });

            leaderboardButton.onClick.AddListener(() =>
            {
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), -1920f);
                UINavigator.ShowPageHori(UINavigator.Push("LeaderboardPage"), 6000);
            });

            //Account buttons
            accountButton.onClick.AddListener(() =>
            {
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), -3000f);
                UINavigator.ShowPageHori(UINavigator.Push("AccountPage"), 6000);
            });


            //Play menu buttons
            startButton.onClick.AddListener(() =>
            {
                UINavigator.ShowPageVert(UINavigator.Push("LoadingScreen"), 3000, () =>
                {
                    SceneManager.LoadSceneAsync(1);
                });
            });

            switchToLogin.onClick.AddListener(() =>
            {
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), -4000f);
                UINavigator.ShowPageHori(UINavigator.Push("LogIn"), 4000f);
            });

            switchToSignUp.onClick.AddListener(() =>
            {
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), 3000f);
                UINavigator.ShowPageHori(UINavigator.Push("SignUp"), -3000f);
            });
        }

        private void InitSubmitForms()
        {
            submitSignUp.onClick.AddListener(() =>
            {
                #region Input Validation
                string username = signupUsername.text;
                string password = signupPassword.text;
                string repeatPassword = signupRepeatPassword.text;

                if (string.IsNullOrEmpty(username))
                {
                    StartCoroutine(ShowFeedback("Please input a username"));
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    StartCoroutine(ShowFeedback("Please input a password"));
                    return;
                }

                if (string.IsNullOrEmpty(repeatPassword))
                {
                    StartCoroutine(ShowFeedback("Please input the repeat password"));
                    return;
                }

                if (password != repeatPassword)
                {
                    StartCoroutine(ShowFeedback("Passwords do not match!"));
                    return;
                } 
                #endregion

                StartCoroutine(ShowFeedback("Signing Up..."));

                UserPayload userPayload = new UserPayload(username, password);
                StartCoroutine(AWSManager.instance.CreateUser(checkForExisting: true, userPayload: userPayload, OnSuccess: res =>
                {
                    //UI Related
                    StartCoroutine(ShowFeedback("Success"));
                    UINavigator.PopUntil("LoadingScreen");

                    //Inits static reference to the user
                    GameManager.InitUser(userPayload);
                    GameManager.LogIn();

                    //Generates a GUID for the computer so that it may recognize subsequent log ins
                    PlayerPrefs.SetString("cc2_username", userPayload.username);
                    PlayerPrefs.SetString("cc2_password", userPayload.password);

                    //Pops loading page
                    UINavigator.HidePageVert(UINavigator.instance.Navigator.Peek());
                    return;
                }, OnFailure: err =>
                {
                    StartCoroutine(ShowFeedback($"Error: {err}"));
                }));
            });

            submitLogIn.onClick.AddListener(() =>
            {
                #region Input Validation

                string username = loginUsername.text;
                string password = loginPassword.text;

                if (string.IsNullOrEmpty(username))
                {
                    StartCoroutine(ShowFeedback("Please input a username"));
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    StartCoroutine(ShowFeedback("Please input a password"));
                    return;
                }

                StartCoroutine(ShowFeedback("Logging In..."));

                LogIn(username, password);

                #endregion
            });
        }

        private void LogIn(string username, string password, Action OnSuccess = null, Action OnError = null)
        {
            StartCoroutine(AWSManager.instance.GetUserByName(username, res =>
            {
                Debug.Log(res);
                if (password == res[0]["password"].ToString())
                {
                    StartCoroutine(ShowFeedback("Welcome back! Logging you in..."));

                    UserPayload userPayload = new UserPayload(
                        username: res[0]["username"].ToString(),
                        password: res[0]["password"].ToString(),
                        permission: res[0]["permission"].ToString(),
                        s3_skinpointer: res[0]["s3_skinpointer"].ToString(),
                        easymodeFastest: res[0]["easymodeFastest"].ToString(),
                        medmodeFastest: res[0]["medmodeFastest"].ToString(),
                        hardmodeFastest: res[0]["hardmodeFastest"].ToString()
                        );

                    GameManager.InitUser(userPayload);
                    GameManager.LogIn();

                    UINavigator.PopUntil("LoadingScreen");
                    UINavigator.HidePageVert(UINavigator.instance.Navigator.Peek(), 3000, () =>
                    {
                        UINavigator.PopUntil("MainScreen");
                    });

                    OnSuccess?.Invoke();
                }
                else
                {
                    StartCoroutine(ShowFeedback($"Error: Incorrect username or password"));
                    OnError?.Invoke();
                }
            }, err =>
            {
                StartCoroutine(ShowFeedback($"Error: Incorrect username or password"));
                OnError?.Invoke();
            }));
        }

        private IEnumerator ShowFeedback(string errorText)
        {
            StopAllCoroutines();

            errorTMP.gameObject.SetActive(true);
            errorTMP.text = errorText;

            yield return new WaitForSeconds(3);
            errorTMP.gameObject.SetActive(false);
        }
    }
}