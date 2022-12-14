using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AWSManager : MonoBehaviour
{
    private class UserPayload
    {
        public string username;
        public string password;
        public string permission;
        public string s3_skinpointer;
        public string easymodeFastest;
        public string medmodeFastest;
        public string hardmodeFastest;

        public UserPayload(string username, string password, string permission, string s3_skinpointer, string easymodeFastest, string medmodeFastest, string hardmodeFastest)
        {
            this.username = username;
            this.password = password;
            this.permission = permission;
            this.s3_skinpointer = s3_skinpointer;
            this.easymodeFastest = easymodeFastest;
            this.medmodeFastest = medmodeFastest;
            this.hardmodeFastest = hardmodeFastest;
        }
    }

    private class PayLoad
    {
        public string operation;
        public UserPayload userPayload;

        public PayLoad(string operation, UserPayload userPayload)
        {
            this.operation = operation;
            this.userPayload = userPayload;
        }
    }

    private string rootURL = "https://bnnpywvfa5.execute-api.us-east-1.amazonaws.com";

    private IEnumerator SendAWSWebRequest(string route, WWWForm form, Action<UnityWebRequest> OnSuccess = null, Action<UnityWebRequest> OnFailure = null, Action<UnityWebRequest> OnAny = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Post($"{rootURL}/{route}", form))
        {
            request.SetRequestHeader("Content-Type", "application/data");
            request.SetRequestHeader("Accept", "*/*");
            request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");

            yield return request.SendWebRequest();

            if (OnAny != null)
                OnAny(request);
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                if (OnFailure != null)
                    OnFailure(request);
            }
            else
            {
                if (OnSuccess != null)
                    OnSuccess(request);
            }
        }
    }

    #region User Methods
    private IEnumerator DeleteUserByUsername(string username)
    {
        bool userExists = false;

        WWWForm getUserForm = new WWWForm();
        getUserForm.AddField("operation", "getOne");
        getUserForm.AddField("username", username);

        yield return StartCoroutine(GetUserByName(username: username, OnSuccess: res => userExists = true));

        if (!userExists)
        {
            Log.Print("User does not exist!", Log.AWS_TOPIC_ERRORS);
            yield break;
        }

        WWWForm deleteForm = new WWWForm();
        deleteForm.AddField("operation", "deleteOne");
        deleteForm.AddField("username", username);

        yield return StartCoroutine(SendAWSWebRequest("users", deleteForm, res =>
        {
            Log.Print(res.downloadHandler.text, Log.AWS_TOPIC_SUCCESS);
        }));
    }

    private IEnumerator GetUserByName(string username, Action<string> OnSuccess = null, Action<string> OnFailure = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "getOne");
        form.AddField("username", username);

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res => OnSuccess(res.downloadHandler.ToString()), OnFailure: res => OnFailure(res.downloadHandler.ToString())));
    }

    private IEnumerator GetAllUsers(Action<string> OnSuccess = null, Action<string> OnFailure = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "get");

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res => OnSuccess(res.downloadHandler.ToString()), OnFailure: res => OnFailure(res.downloadHandler.ToString())));
    }

    private IEnumerator CreateUser(UserPayload userPayload, Action<string> OnSuccess = null, Action<string> OnFailure = null)
    {
        bool userExists = false;

        WWWForm getUserForm = new WWWForm();
        getUserForm.AddField("operation", "getOne");
        getUserForm.AddField("username", userPayload.username);

        yield return StartCoroutine(GetUserByName(username: userPayload.username, OnSuccess: res => userExists = true));

        if (userExists)
        {
            Log.Print("User with that username already exists!", Log.AWS_TOPIC_ERRORS, name);
            yield break;
        }

        string dynamoItems = JsonConvert.SerializeObject(userPayload);

        WWWForm form = new WWWForm();
        form.AddField("operation", "create");
        form.AddField("userpayload", dynamoItems);

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res => OnSuccess(res.downloadHandler.ToString()), OnFailure: res => OnFailure(res.downloadHandler.ToString())));
    } 
    #endregion
}