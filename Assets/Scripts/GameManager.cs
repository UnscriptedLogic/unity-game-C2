using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameManager
{
    public static bool isLoggedIn = true;

    public static Action<bool> OnLogInStatusChanged;
    
    public static void LogIn()
    {
        isLoggedIn = true;
        OnLogInStatusChanged?.Invoke(isLoggedIn);
    }
}
