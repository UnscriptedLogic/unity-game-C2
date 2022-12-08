using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    public static string[] topicsToLog;

    public static void Print(string message, string topic, string source = "")
    {
        if (source != null)
        {
            message += $" | {source}";
        }

        if (topicsToLog == null)
        {
            return;
        }

        foreach (var topicToLog in topicsToLog)
        {
            if (topicToLog == topic)
            {
                Debug.Log(message);
                return;
            }
        }
    }
}
