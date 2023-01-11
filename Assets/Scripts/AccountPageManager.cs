using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        }
    }
}