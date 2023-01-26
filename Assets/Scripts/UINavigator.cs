using System;
using System.Collections.Generic;
using UnityEngine;

public class UINavigator : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;

    private Stack<GameObject> navigator = new();

    public static UINavigator instance;
    public Stack<GameObject> Navigator => navigator;

    [Header("Animation Settings")]
    [SerializeField] private float transitionSpeed = 0.5f;
    [SerializeField] private LeanTweenType tweenType;

    [Header("Debug")]
    [SerializeField] private bool enableScriptModify;
    [SerializeField] private int pageToShow;

    private void Awake()
    {
        instance = this;
    }

    public static GameObject Push(string pageName)
    {
        if (PageExists(pageName, out int i))
        {
            instance.pages[i].SetActive(true);
            instance.navigator.Push(instance.pages[i]);
            return instance.pages[i];
        }

        Debug.Log($"The page with name {pageName} is not found");
        return null;
    }

    public static GameObject PushPageWithIndex(int index)
    {
        if (index < instance.pages.Count)
        {
            instance.pages[index].SetActive(true);
            instance.navigator.Push(instance.pages[index]);
            return instance.pages[index];
        }

        Debug.Log($"The page with the index {index} is not found");
        return null;
    }

    public static GameObject Pop()
    {
        if (instance.navigator.Count > 0)
        {
            GameObject page = instance.navigator.Peek();
            instance.navigator.Pop().SetActive(false);
            return page;
        }

        return null;
    }

    public static GameObject PopWithoutDisable()
    {
        if (instance.navigator.Count > 0)
        {
            GameObject page = instance.navigator.Peek();
            instance.navigator.Pop();
            return page;
        }

        return null;
    }

    public static void PopUntil(string pageName)
    {
        while (instance.navigator.Count > 0)
        {
            if (instance.navigator.Peek().name == pageName)
            {
                return;
            }

            Pop();
        }

        Debug.Log($"The page with name {pageName} is not found. Navigation stack is empty");
    }

    public static void PopAndPush(string pageName)
    {
        if (PageExists(pageName, out int index))
        {
            Pop();
            PushPageWithIndex(index);
        }
    }

    public static void PopAll()
    {
        while (instance.navigator.Count > 0)
        {
            Pop();
        }
    }

    public static string GetTopPageName()
    {
        if (instance.navigator.Count > 0)
        {
            return instance.navigator.Peek().name;
        }

        return null;
    }

    public static GameObject GetTopPage()
    {
        if (instance.navigator.Count > 0)
        {
            return instance.navigator.Peek();
        }

        return null;
    }

    public static bool PageExists(string pageName, out int index)
    {
        for (int i = 0; i < instance.pages.Count; i++)
        {
            if (instance.pages[i].name == pageName)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public static void ShowPageVert(string pagename, float from, Action OnCompleted = null)
    {
        GameObject page = Push(pagename);
        float prevPos = page.transform.position.y;
        page.transform.position = new Vector3(page.transform.position.x, from, page.transform.position.z);
        LeanTween.moveY(page, prevPos, instance.transitionSpeed).setEase(instance.tweenType).setOnComplete(() =>
        {
            if (OnCompleted != null)
            {
                OnCompleted();
            }
        });
    }

    public static void HidePageVert(float to = 3000f, Action onCompleted = null)
    {
        GameObject page = instance.navigator.Peek();
        float prevPos = page.transform.position.y;
        LeanTween.moveY(page, to, instance.transitionSpeed).setEase(instance.tweenType).setOnComplete(() =>
        {
            page.SetActive(false);
            page.transform.position = new Vector3(page.transform.position.x, prevPos, page.transform.position.z);

            onCompleted?.Invoke();
        });
    }

    public static void ShowPageHori(string pagename, float from = 3000f, Action OnCompleted = null)
    {
        GameObject page = Push(pagename);
        float prevPos = page.transform.position.x;
        page.transform.position = new Vector3(from, page.transform.position.y, page.transform.position.z);
        LeanTween.moveX(page, prevPos, instance.transitionSpeed).setEase(instance.tweenType).setOnComplete(() => OnCompleted?.Invoke());
    }

    public static void HidePageHori(float to = 3000f, Action OnComplete = null)
    {
        GameObject page = instance.navigator.Peek();
        float prevPos = page.transform.position.x;
        PopWithoutDisable();
        LeanTween.moveX(page, to, instance.transitionSpeed).setEase(instance.tweenType).setOnComplete(() =>
        {
            page.transform.position = new Vector3(prevPos, page.transform.position.y, page.transform.position.z);
            page.SetActive(false);

            OnComplete?.Invoke();
        });
    }

    private void OnValidate()
    {
        if (enableScriptModify)
        {
            if (navigator.Count > 0)
            {
                navigator.Pop().SetActive(false);
            }

            if (pageToShow < pages.Count)
            {
                pages[pageToShow].SetActive(true);
                navigator.Push(pages[pageToShow]);
            }
        }
    }
}