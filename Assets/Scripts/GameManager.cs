using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameManager
{
    public static UserPayload UserPayload { get; private set; }
    public static bool isLoggedIn = true;
    public const string GUID_NAME = "cc2_guid";

    public static Action<bool> OnLogInStatusChanged;
    
    public static void LogIn()
    {
        isLoggedIn = true;
        OnLogInStatusChanged?.Invoke(isLoggedIn);
    }

    public static void InitUser(UserPayload userPayload)
    {
        UserPayload = userPayload;
    }
}
