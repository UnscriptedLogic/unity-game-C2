using PlayerManagement;
using System;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeReference] private PlayerController player;

        public PlayerController Player => player;
        public static LevelManager instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UINavigator.Push("LoadingScreen").GetComponent<LoadingScreen>().SetText($"Loading {GameManager.DifficultyMode} mode...");

            string skinName = GameManager.UserPayload != null ? GameManager.UserPayload.s3_skinpointer : "defaultskin";
            StartCoroutine(AWSManager.instance.GetItemWithName(skinName, res =>
            {
                Coroutine objectCoroutine = StartCoroutine(AWSManager.instance.InstantiateObjectFromS3(res[0]["s3_skinpointer"]["S"].ToString(), skinName, player.transform, Vector3.zero, Quaternion.identity, go =>
                {
                    if (go.TryGetComponent(out BoxCollider collider))
                        Destroy(collider);

                    go.transform.localPosition = Vector3.zero;
                    LoadingComplete();
                }));

            }, err => Debug.Log(err.downloadHandler.text)));
        }

        private void LoadingComplete()
        {
            UINavigator.HidePageVert(UINavigator.GetTopPage());
        }

        private void LevelCompleted()
        {

        }
    }
}