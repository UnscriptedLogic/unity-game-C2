using Amazon.Runtime.Internal;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

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

    private void Start()
    {
        //StartCoroutine(Upload());
        StartCoroutine(GetAllUsers());
    }

    private IEnumerator GetAllUsers()
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "get");

        using (UnityWebRequest request = UnityWebRequest.Post($"{rootURL}/users", form))
        {
            request.SetRequestHeader("Content-Type", "application/data");
            request.SetRequestHeader("Accept", "*/*");
            request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");

            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.Print(request.error, Log.AWS_TOPIC, name);
            }
            else
            {
                Log.Print(request.downloadHandler.text, Log.AWS_TOPIC, name);
            }
        }
    }

    private IEnumerator Upload()
    {
        string dynamoItems = JsonConvert.SerializeObject(new UserPayload("HelloFromUnity", "password", "admin", "somelink", "0", "0", "0"));

        WWWForm form = new WWWForm();
        form.AddField("operation", "create");
        form.AddField("userpayload", dynamoItems);

        using (UnityWebRequest request = UnityWebRequest.Post($"{rootURL}/users", form))
        {
            request.SetRequestHeader("Content-Type", "application/data");
            request.SetRequestHeader("Accept", "*/*");
            request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");

            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.Print(request.error, Log.AWS_TOPIC, name);
            }
            else
            {
                Log.Print(request.downloadHandler.text, Log.AWS_TOPIC, name);
            }
        }
    }

    private void DebugHeaders(Dictionary<string, string> headers)
    {
        string responseHeaders = "Response Headers: \n";
        foreach (KeyValuePair<string, string> header in headers)
        {
            responseHeaders += $"{header.Key} : {header.Value}\n";
        }
        Log.Print(responseHeaders, Log.AWS_TOPIC);
    }


    //private IEnumerator LoadAsset(string url, string assetName)
    //{
    //    using (UnityWebRequest webRequest = new UnityWebRequest(url))
    //    {
    //        yield return webRequest.SendWebRequest();
    //        AssetBundle remoteBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
    //        if (remoteBundle == null)
    //        {
    //            Log.Print("Failed to load AssetBundle!", name);
    //            yield break;
    //        }

    //        Instantiate(remoteBundle.LoadAsset(assetName));
    //    }
    //}
}