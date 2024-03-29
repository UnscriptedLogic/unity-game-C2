﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    public static string[] topicsToLog;

    public const string AWS_TOPIC = "aws";
    public const string AWS_TOPIC_SUCCESS = "aws/success";
    public const string AWS_TOPIC_ERRORS = "aws/errors";
    public const string PlayerControls = "playercontrols";

    public static void Print(string message, string topic, string source = "", Object context = null)
    {
        if (source != "")
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
                Debug.Log(message, context);
                return;
            }
        }
    }
}
