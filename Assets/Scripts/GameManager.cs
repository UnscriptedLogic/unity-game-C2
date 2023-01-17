using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameManager
{
    public static UserPayload UserPayload { get; private set; }
    public static bool isLoggedIn = true;
    public const string GUID_NAME = "cc2_guid";

    public static DifficultyMode DifficultyMode;

    public static Action<bool> OnLogInStatusChanged;
    
    public static void LogIn()
    {
        isLoggedIn = true;
        OnLogInStatusChanged?.Invoke(isLoggedIn);

        PlayerPrefs.SetString("cc2_username", UserPayload.username);
        PlayerPrefs.SetString("cc2_password", UserPayload.password);
    }

    public static void InitUser(UserPayload userPayload)
    {
        UserPayload = userPayload;
    }

    public static void SetSkin(string skinName) => UserPayload.s3_skinpointer = skinName;

    public static void SignOut()
    {
        PlayerPrefs.SetString("cc2_username", "");
        PlayerPrefs.SetString("cc2_password", "");
    }

    public static void UpdateScore(float timing)
    {
        switch (DifficultyMode)
        {
            case DifficultyMode.Easy:

                UserPayload.easymodeFastest = Mathf.Max(float.Parse(UserPayload.easymodeFastest), timing).ToString();
                break;
            
            case DifficultyMode.Medium:
                UserPayload.medmodeFastest = Mathf.Max(float.Parse(UserPayload.medmodeFastest), timing).ToString();
                break;
         
            case DifficultyMode.Hard:
                UserPayload.hardmodeFastest = Mathf.Max(float.Parse(UserPayload.hardmodeFastest), timing).ToString();
                break;
            
            default:
                break;
        }
    }
}
