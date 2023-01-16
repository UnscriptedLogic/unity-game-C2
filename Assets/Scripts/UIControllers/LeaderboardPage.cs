using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class LeaderboardPage : MonoBehaviour
    {
        [Header("Window")]
        [SerializeField] private Image background;
        [SerializeField] private Image border;
        [SerializeField] private Color bgColor;
        [SerializeField] private Color borderColor;

        [Header("Scores")]
        [SerializeField] private Transform scoreParent;
        [SerializeField] private GameObject scoreCard;

        private void Start()
        {
            StartCoroutine(AWSManager.instance.GetAllLeaderboardScore(new AWSManager.LeaderboardPayload(mode: DifficultyMode.Easy), OnSuccess: res =>
            {
                for (int i = 0; i < res.Count; i++)
                {
                    GameObject score = Instantiate(scoreCard, scoreParent);
                    score.GetComponent<ScoreCard>().Initialize(i + 1, null, res[i]["username"]["S"].ToString());
                }
            }, err =>
            {
                Debug.Log(err.downloadHandler.text);
            }));
        }

        private void OnValidate()
        {
            background.color = bgColor;
            border.color = borderColor;
        }
    }
}