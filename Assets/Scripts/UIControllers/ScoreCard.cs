using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class ScoreCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rank;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI username;

        public void Initialize(int rank, Sprite icon, string username)
        {
            this.rank.text = $"#{rank}";
            this.icon.sprite = icon;
            this.username.text = username;
        }
    }
}