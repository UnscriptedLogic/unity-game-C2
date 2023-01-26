using Amazon.Runtime.Internal.Util;
using PlayerManagement;
using System;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelManagement
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private TextMeshProUGUI timerTMP;

        [Header("Difficulty Mode Physics Materials")]
        [SerializeField] private PhysicMaterial easymodeMat;
        [SerializeField] private PhysicMaterial medmodeMat;
        [SerializeField] private PhysicMaterial hardmodeMat;

        [Header("Pause")]
        [SerializeField] private Button pauseBtn;
        [SerializeField] private Button resumeBtn;
        [SerializeField] private Button mainmenuBtn;
        [SerializeField] private Button quitBtn;

        public PlayerController Player => player;
        public static LevelManager instance;

        private RigidbodyConstraints constraints;
        private bool completed;
        private bool loaded;
        private float time;
        private AssetBundle loadedBundle;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UINavigator.Push("LoadingScreen").GetComponent<LoadingScreen>().SetText($"Loading {GameManager.DifficultyMode} mode...");

            #region Pause Buttons
            pauseBtn.onClick.AddListener(() =>
            {
                loaded = false;
                UINavigator.ShowPageHori(pagename: "PauseScreen", OnCompleted: () =>
                {
                    constraints = player.Rb.constraints;
                    player.Rb.constraints = RigidbodyConstraints.FreezeAll;
                });
            });

            resumeBtn.onClick.AddListener(() =>
            {
                loaded = true;
                player.Rb.constraints = constraints;
                UINavigator.HidePageHori();
            });

            mainmenuBtn.onClick.AddListener(() =>
            {
                UINavigator.ShowPageVert("LoadingScreen", 3000f, () =>
                {
                    SceneManager.LoadSceneAsync(0);
                    Time.timeScale = 1f;
                });
            });

            quitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
                Time.timeScale = 1f;
            }); 
            #endregion

            //S3 to Unity Skin Instantiating
            string skinName = GameManager.UserPayload != null ? GameManager.UserPayload.s3_skinpointer : "defaultskin";
            StartCoroutine(AWSManager.instance.GetItemWithName(skinName, res =>
            {
                Coroutine objectCoroutine = StartCoroutine(AWSManager.instance.InstantiateObjectFromS3(res[0]["s3_skinpointer"]["S"].ToString(), skinName, player.transform, Vector3.zero, Quaternion.identity, (go, bundle) =>
                {
                    if (go.TryGetComponent(out BoxCollider collider))
                        Destroy(collider);

                    go.transform.localPosition = Vector3.zero;
                    LoadingComplete();
                }));

            }, err => Debug.Log(err.downloadHandler.text)));
        }

        private void Update()
        {
            //Timer related
            if (completed || !loaded)
                return;

            time += Time.deltaTime;
            timerTMP.text= time.ToString("0.0");
        }

        //Once Level is loaded with player skins ready
        private void LoadingComplete()
        {
            switch (GameManager.DifficultyMode)
            {
                case DifficultyMode.Easy:
                    player.BoxCollider.material = easymodeMat;
                    break;
                case DifficultyMode.Medium:
                    player.BoxCollider.material = medmodeMat;
                    break;
                case DifficultyMode.Hard:
                    player.BoxCollider.material = hardmodeMat;
                    break;
                default:
                    break;
            }

            UINavigator.HidePageVert();
            loaded = true;
        }

        //When the player hits the end goal
        public void LevelCompleted()
        {
            UINavigator.ShowPageHori("SendingDataScreen");
            
            completed = true;
            player.Rb.constraints = RigidbodyConstraints.FreezeAll;

            if (GameManager.HasNewHighScore(time))
            {
                StartCoroutine(AWSManager.instance.UpdateUserScore(GameManager.ParseScore(time), OnSuccess: () =>
                {
                    StartCoroutine(AWSManager.instance.UpdateUserByUsername(GameManager.UserPayload, OnSuccess: res =>
                    {
                        ReturnHome();
                    }));
                }));
            } else
            {
                ReturnHome();
            }
        }

        private void ReturnHome()
        {
            UINavigator.ShowPageVert("LoadingScreen", 3000f, () =>
            {
                SceneManager.LoadSceneAsync(0);
                StopAllCoroutines();
            });
        }
    }
}