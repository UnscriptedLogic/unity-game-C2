using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private Button updateAccount;
        [SerializeField] private Button deleteAccount;

        [SerializeField] private GameObject skinButtonPrefab;
        [SerializeField] private Transform inventoryGridParent;

        private void Awake()
        {
            GameManager.OnLogInStatusChanged += OnLoggedIn;
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