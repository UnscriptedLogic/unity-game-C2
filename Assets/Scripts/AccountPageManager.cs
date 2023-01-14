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

            ConfirmDelete.onClick.AddListener(() =>
            {
                GameManager.DeleteAccount();
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
                for (int i = 0; i < res.Count; i++)
                {
                    string skinName = res[i]["name"]["S"].ToString();   
                    GameObject skinButton = Instantiate(skinButtonPrefab, inventoryGridParent);
                    skinButton.GetComponentInChildren<TextMeshProUGUI>().text = skinName;

                    //StartCoroutine(AWSManager.instance.GetIconFromS3Pointer(res[i]["s3_skinpointer"]["S"].ToString(), sprite =>
                    //{
                    //    skinButton.transform.GetChild(2).GetComponent<Image>().sprite = sprite;
                    //}, err =>
                    //{

                    //}));

                    skinButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GameManager.SetSkin(skinName);
                        Debug.Log(skinName);
                    });

                }
            }, err =>
            {

            }));
        }
    }
}