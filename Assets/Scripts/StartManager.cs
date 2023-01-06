using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelManagement
{
    public class StartManager : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button accountButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private float transitionSpeed = 0.5f;
        [SerializeField] private LeanTweenType tweenType;

        private void Start()
        {
            playButton.onClick.AddListener(() =>
            {
                GameObject prevPage = UINavigator.instance.Navigator.Peek();
                LeanTween.moveX(prevPage, -1920f, transitionSpeed).setEase(tweenType).setOnComplete(() =>
                {
                    prevPage.SetActive(false);
                });

                ShowPageHori(UINavigator.Push("DifficultyScreen"), -1920f);
                ShowBackButton(true);
            });

        }

        private void ShowPageHori(GameObject page, float from)
        {
            float prevPos = page.transform.position.x;
            page.transform.position = new Vector3(from, page.transform.position.y, page.transform.position.z);
            LeanTween.moveX(page, prevPos, transitionSpeed).setEase(tweenType);
        }

        private void HidePageHori(GameObject page, float to)
        {
            LeanTween.moveX(page, to, transitionSpeed).setEase(tweenType).setOnComplete(() =>
            {
                page.SetActive(false);
            });
        }

        private void ShowBackButton(bool value)
        {
            if (value)
            {
                ShowPageHori(UINavigator.PushPageWithIndex(0), 1920f);
            }
            else
            {
                HidePageHori(backButton.transform.parent.gameObject, 1920f);
            }
        }
    }
}