using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingTMP;

    public void ResetText() => loadingTMP.text = "Loading...";
    public void SetText(string text) => loadingTMP.text = text;
}
