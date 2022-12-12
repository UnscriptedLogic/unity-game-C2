using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogType
{
    None,
    General,
    Warning,
    Error,
    All
}

[DefaultExecutionOrder(-1)]
public class LogBehaviour : MonoBehaviour
{
    [SerializeField] private LogType typeToLog;
    [SerializeField] private string[] topics;

    private void Awake()
    {
        if (topics == null)
        {
            Debug.Log("Not listening to any logging topics! Zzz");
            return;
        }

        if (typeToLog == LogType.None)
        {
            Debug.Log("Logging has been switched off. Enjoy the empty console! :D");
            return;
        }

        Log.topicsToLog = topics;
    }
}
