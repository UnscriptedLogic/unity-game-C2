using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static AWSManager;

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

    public static bool HasNewHighScore(float time)
    {
        switch (DifficultyMode)
        {
            case DifficultyMode.Easy:

                return float.Parse(UserPayload.easymodeFastest) > time;

            case DifficultyMode.Medium:
                return float.Parse(UserPayload.medmodeFastest) > time;

            case DifficultyMode.Hard:
                return float.Parse(UserPayload.hardmodeFastest) > time;

            default:
                return false;
        }
    }

    public static LeaderboardPayload ParseScore(float timing)
    {
        LeaderboardPayload payload = new LeaderboardPayload();
        payload.username = UserPayload.username;
        payload.timing = timing;
        payload.s3_skinpointer = UserPayload.s3_skinpointer;

        switch (DifficultyMode)
        {
            case DifficultyMode.Easy:
                UserPayload.easymodeFastest = Mathf.Min(float.Parse(UserPayload.easymodeFastest), timing).ToString();
                payload.mode = "leaderboard-easy";
                break;
            
            case DifficultyMode.Medium:
                UserPayload.medmodeFastest = Mathf.Min(float.Parse(UserPayload.medmodeFastest), timing).ToString();
                payload.mode = "leaderboard-medium";
                break;
         
            case DifficultyMode.Hard:
                UserPayload.hardmodeFastest = Mathf.Min(float.Parse(UserPayload.hardmodeFastest), timing).ToString();
                payload.mode = "leaderboard-hard";
                break;
            
            default:
                break;
        }

        return payload;
    }
}
