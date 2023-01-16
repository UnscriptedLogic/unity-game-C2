using Amazon.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIManagement
{
    public class AccountPageManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI usernameTP;
        [SerializeField] private TextMeshProUGUI statsTMP;
        [SerializeField] private Image skinIcon;
        [SerializeField] private Button signOutAccount;
        [SerializeField] private Button updateAccount;
        [SerializeField] private Button deleteAccount;

        [SerializeField] private Button ConfirmSignOut;
        [SerializeField] private Button ConfirmDelete;

        [SerializeField] private Button[] cancelButtons;

        [SerializeField] private GameObject skinButtonPrefab;
        [SerializeField] private Transform inventoryGridParent;

        [Header("Update Account")]
        [SerializeField] private GameObject inputGaurd;
        [SerializeField] private TextMeshProUGUI usernameTMP;
        [SerializeField] private TMP_InputField oldPasswordTMP;
        [SerializeField] private TMP_InputField newPasswordTMP;
        [SerializeField] private TMP_InputField repeatNewPasswordTMP;
        [SerializeField] private Button updateButton;
        [SerializeField] private TextMeshProUGUI feedbackTMP;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color successColor;
        [SerializeField] private Color errorColor;

        private void Awake()
        {
            GameManager.OnLogInStatusChanged += OnLoggedIn;
        }

        private void Start()
        {
            signOutAccount.onClick.AddListener(() =>
            {
                UINavigator.ShowPageVert(UINavigator.Push("ConfirmSignOut"), 3000f);
            });

            updateAccount.onClick.AddListener(() =>
            {
                feedbackTMP.gameObject.SetActive(false);
                usernameTMP.text = $"{GameManager.UserPayload.username} - cannot be changed";
                UINavigator.ShowPageVert(UINavigator.Push("UpdateAccount"), 3000f);
            });

            deleteAccount.onClick.AddListener(() =>
            {
                UINavigator.ShowPageVert(UINavigator.Push("ConfirmDeleteAccount"), 3000f);
            });

            ConfirmSignOut.onClick.AddListener(() =>
            {
                GameManager.SignOut();
                UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), 3000f, () =>
                {
                    UINavigator.PopAll();
                    UINavigator.Push("MainScreen");
                    UINavigator.ShowPageVert(UINavigator.Push("LoadingScreen"), 3000f, () =>
                    {
                        UINavigator.Push("NewUserLogin");
                        UINavigator.ShowPageVert(UINavigator.Push("SignUp"), 3000f);
                    });
                });
            });

            updateButton.onClick.AddListener(() =>
            {
                //check that all fields are not empty
                if (string.IsNullOrEmpty(oldPasswordTMP.text))
                {
                    StartCoroutine(ShowFeedback("Please input your old password", errorColor));
                    return;
                }
                else if (string.IsNullOrEmpty(newPasswordTMP.text))
                {
                    StartCoroutine(ShowFeedback("Please input your new password", errorColor));
                    return;
                }
                else if (string.IsNullOrEmpty(repeatNewPasswordTMP.text))
                {
                    StartCoroutine(ShowFeedback("Please repeat your new password", errorColor));
                    return;
                }

                //new password matches with repeat new password
                string newpassword = newPasswordTMP.text;
                if (newpassword != repeatNewPasswordTMP.text)
                {
                    StartCoroutine(ShowFeedback("New password fields do not match", errorColor));
                    return;
                }

                //old password matches current password
                StartCoroutine(AWSManager.instance.GetUserByName(GameManager.UserPayload.username, res =>
                {
                    if (res[0]["password"].ToString() != oldPasswordTMP.text)
                    {
                        StartCoroutine(ShowFeedback("Inputted old password does not match with existing old password", errorColor));
                        return;
                    }

                    //call aws method to update password
                    StartCoroutine(ShowFeedback("All fields valid | Updating Account...", defaultColor));

                    inputGaurd.SetActive(true);

                    UserPayload updatedUserPayload = GameManager.UserPayload;
                    updatedUserPayload.password = newpassword;
                    StartCoroutine(AWSManager.instance.UpdateUserByUsername(updatedUserPayload, res =>
                    {
                        StartCoroutine(ShowFeedback("Account Updated", successColor, 1f, () => 
                        {
                            StopAllCoroutines();
                            UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), 3000f, () => inputGaurd.SetActive(false));
                        }));

                        GameManager.InitUser(updatedUserPayload);
                        GameManager.LogIn();

                    }, err =>
                    {
                        StartCoroutine(ShowFeedback(err, errorColor));
                        inputGaurd.SetActive(false);
                    }));
                }, err =>
                {
                    StartCoroutine(ShowFeedback("Old password is incorrect", errorColor));
                    return;
                }));
            });

            ConfirmDelete.onClick.AddListener(() =>
            {
                StartCoroutine(AWSManager.instance.DeleteUserByUsername(GameManager.UserPayload.username, () =>
                {
                    GameManager.SignOut();
                    UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek(), 3000f, () =>
                    {
                        UINavigator.PopAll();
                        UINavigator.Push("MainScreen");
                        UINavigator.ShowPageVert(UINavigator.Push("LoadingScreen"), 3000f, () =>
                        {
                            UINavigator.Push("NewUserLogin");
                            UINavigator.ShowPageVert(UINavigator.Push("SignUp"), 3000f);
                        });
                    });
                }));
            });

            for (int i = 0; i < cancelButtons.Length; i++)
            {
                cancelButtons[i].onClick.AddListener(() =>  
                {
                    UINavigator.HidePageHori(UINavigator.instance.Navigator.Peek());
                });
            }
        }

        private void OnLoggedIn(bool loggedIn)
        {
            if (loggedIn)
            {
                usernameTP.text = GameManager.UserPayload.username;
                statsTMP.text = $"{GameManager.UserPayload.easymodeFastest}\n{GameManager.UserPayload.medmodeFastest}\n{GameManager.UserPayload.hardmodeFastest}";
            }

            StartCoroutine(AWSManager.instance.GetAllItems(res =>
            {
                for (int i = 0; i < inventoryGridParent.childCount; i++)
                {
                    Destroy(inventoryGridParent.GetChild(i).gameObject);
                }

                for (int i = 0; i < res.Count; i++)
                {
                    string skinName = res[i]["name"]["S"].ToString();   
                    GameObject skinButton = Instantiate(skinButtonPrefab, inventoryGridParent);
                    skinButton.GetComponentInChildren<TextMeshProUGUI>().text = skinName;

                    //TODO: Figure out how to download sprites across the net

                    skinButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GameManager.SetSkin(skinName);
                    });
                }
            }, err =>
            {

            }));
        }

        private IEnumerator ShowFeedback(string feedback, Color color, float displayTime = 3f, Action OnCompleted = null)
        {
            feedbackTMP.gameObject.SetActive(true);
            feedbackTMP.text = feedback;
            feedbackTMP.color = color;

            yield return new WaitForSeconds(displayTime);
            feedbackTMP.gameObject.SetActive(false);

            OnCompleted?.Invoke();
        }
    }
}