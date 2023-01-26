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
        [SerializeField] private TextMeshProUGUI timingTMP;

        public void Initialize(int rank, Sprite icon, string username, string timing)
        {
            this.rank.text = $"#{rank}";
            this.icon.sprite = icon;
            this.username.text = username;
            timingTMP.text = timing;
        }
    }
}