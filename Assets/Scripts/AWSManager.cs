using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using UnityEngine.Windows;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class UserPayload
{
    public string username;
    public string password;
    public string permission;
    public string s3_skinpointer;
    public string coins;
    public string easymodeFastest;
    public string medmodeFastest;
    public string hardmodeFastest;

    public UserPayload(string username, string password, string permission = "user", string s3_skinpointer = "defaultskin", string coins = "0", string easymodeFastest = "0", string medmodeFastest = "0", string hardmodeFastest = "0")
    {
        this.username = username;
        this.password = password;
        this.permission = permission;
        this.s3_skinpointer = s3_skinpointer;
        this.coins = coins;
        this.easymodeFastest = easymodeFastest;
        this.medmodeFastest = medmodeFastest;
        this.hardmodeFastest = hardmodeFastest;
    }
}

public enum DifficultyMode
{
    Easy,
    Medium,
    Hard
}

public class AWSManager : MonoBehaviour
{
    public struct LeaderboardPayload
    {
        public string mode;
        public string username;
        public float timing;
        public string s3_skinpointer;

        public LeaderboardPayload(DifficultyMode mode, string username = "", float timing = 0, string s3_skinpointer = "defaultskin")
        {
            switch (mode)
            {
                case DifficultyMode.Easy:
                    this.mode = "leaderboard-easy";
                    break;
                case DifficultyMode.Medium:
                    this.mode = "leaderboard-medium";
                    break;
                case DifficultyMode.Hard:
                    this.mode = "leaderboard-hard";
                    break;
                default:
                    this.mode = "leaderboard-easy";
                    break;
            }

            this.username = username;
            this.timing = timing;
            this.s3_skinpointer = s3_skinpointer;
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

    public static AWSManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Users
        //StartCoroutine(UpdateUserByUsername(new UserPayload(username, password, permission, s3_skinpointer, easy, med, hard)));
        //StartCoroutine(DeleteUserByUsername(username));

        //Leaderboards
        //StartCoroutine(GetAllLeaderboardScore(new LeaderboardPayload(mode: DifficultyMode.Medium)));
        //StartCoroutine(UpdateUserScore(new LeaderboardPayload(mode: DifficultyMode.Easy, username, int.Parse(easy), s3_skinpointer)));
        //StartCoroutine(DeleteUserScore(new LeaderboardPayload(mode: DifficultyMode.Easy, username, int.Parse(easy))));
        
        //Items
        //StartCoroutine(GetAllItems(res => Debug.Log(res), err => Debug.Log(err.downloadHandler.text)));
    }

    public IEnumerator InstantiateObjectFromS3(string objectLink, string objectName, Transform parent, Vector3 position, Quaternion rotation, Action<GameObject, AssetBundle> OnSuccess)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(objectLink);
        www.downloadHandler = new DownloadHandlerAssetBundle(www.url, 0);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);
        if (assetBundle != null)
        {
            Debug.Log(assetBundle);
            GameObject prefab = assetBundle.LoadAsset<GameObject>(objectName);
            if (prefab != null)
            {
                OnSuccess?.Invoke(Instantiate(prefab, position, rotation, parent), assetBundle);
                assetBundle.Unload(false);
                
            } else
            {
                Debug.Log(prefab);
            }
        } else
        {
            Debug.Log(assetBundle);
        }
    }

    public IEnumerator GetIconFromS3Pointer(string url, Action<Sprite> OnSuccess, Action<string> OnError = null)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.error);
            OnError?.Invoke(request.error);
            yield break;
        }

        Debug.Log(request);
        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        OnSuccess?.Invoke(sprite);
    }

    public IEnumerator LoadImageFromURL(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.error);
            Debug.LogError(www.downloadedBytes);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            
        }
    }

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

    private IEnumerator SendAWSWebRequest(string route, string items, Action<UnityWebRequest> OnSuccess = null, Action<UnityWebRequest> OnFailure = null, Action<UnityWebRequest> OnAny = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Post($"{rootURL}/{route}", items))
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
    public IEnumerator DeleteUserByUsername(string username, Action OnSuccess = null, Action<UnityWebRequest> OnError = null)
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
            OnSuccess?.Invoke();
        }, err =>
        {
            OnError?.Invoke(err);
        }));
    }

    public IEnumerator UpdateUserByUsername(UserPayload userPayload, Action<string> OnSuccess = null, Action<string> OnFailure = null)
    {
        bool userExists = false;

        WWWForm getUserForm = new WWWForm();
        getUserForm.AddField("operation", "getOne");
        getUserForm.AddField("username", userPayload.username);

        yield return StartCoroutine(GetUserByName(username: userPayload.username, OnSuccess: res => userExists = true));

        if (!userExists)
        {
            Log.Print("User with that username does not exist!", Log.AWS_TOPIC_ERRORS, name);
            yield break;
        } else
        {
            Log.Print("User found! Proceeding with update...", Log.AWS_TOPIC_SUCCESS, name);
        }

        yield return StartCoroutine(CreateUser(userPayload, OnSuccess, OnFailure));
    }

    public IEnumerator GetUserByName(string username, Action<JArray> OnSuccess = null, Action<string> OnFailure = null)
    {
        //Debug.Log($"Getting user by username: {username}...");

        WWWForm form = new WWWForm();
        form.AddField("operation", "getOne");
        form.AddField("username", username);

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res =>
        {
            if (res.downloadHandler.text == "")
            {
                OnFailure?.Invoke("User does not exist!");
            }
            else
            {
                string result = $"[{res.downloadHandler.text}]";

                JArray jsonArray = JArray.Parse(result);
                OnSuccess?.Invoke(jsonArray);
            }
        }));
    }

    private IEnumerator GetAllUsers(Action<string> OnSuccess = null, Action<string> OnFailure = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "get");

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res => OnSuccess(res.downloadHandler.text.ToString()), OnFailure: res => OnFailure(res.downloadHandler.text.ToString())));
    }

    public IEnumerator CreateUser(UserPayload userPayload, Action<string> OnSuccess = null, Action<string> OnFailure = null, bool checkForExisting = false)
    {
        if (checkForExisting)
        {
            bool userExists = false;

            WWWForm getUserForm = new WWWForm();
            getUserForm.AddField("operation", "getOne");
            getUserForm.AddField("username", userPayload.username);

            yield return StartCoroutine(GetUserByName(username: userPayload.username, OnSuccess: res => userExists = true));

            if (userExists)
            {
                Log.Print("User with that username already exists!", Log.AWS_TOPIC_ERRORS, name);
                OnFailure("User already exist");
                yield break;
            }
        }

        string dynamoItems = JsonConvert.SerializeObject(userPayload);

        WWWForm form = new WWWForm();
        form.AddField("operation", "create");
        form.AddField("username", userPayload.username);
        form.AddField("password", userPayload.password);
        form.AddField("permission", userPayload.permission);
        form.AddField("s3_skinpointer", userPayload.s3_skinpointer);
        form.AddField("coins", userPayload.coins);
        form.AddField("easymodeFastest", userPayload.easymodeFastest);
        form.AddField("medmodeFastest", userPayload.medmodeFastest);
        form.AddField("hardmodeFastest", userPayload.hardmodeFastest);

        yield return StartCoroutine(SendAWSWebRequest("users", form, OnSuccess: res => OnSuccess?.Invoke(res.downloadHandler.text.ToString()), OnFailure: res => OnFailure?.Invoke(res.downloadHandler.text.ToString())));
    }

    #endregion

    #region Leaderboard Methods
    private IEnumerator GetUserScore(LeaderboardPayload leaderboardPayload, Action OnSuccess = null, Action OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "get");
        form.AddField("mode", leaderboardPayload.mode);
        form.AddField("username", leaderboardPayload.username);
        form.AddField("timing", (int)leaderboardPayload.timing);

        yield return StartCoroutine(SendAWSWebRequest("leaderboard-easy", form, res =>
        {
            Debug.Log(res.downloadHandler.text);

            if (OnSuccess != null)
                OnSuccess?.Invoke();

        }, err =>
        {
            Debug.Log(err.downloadHandler.text);

            if (OnError != null)
                OnError?.Invoke();
        }));
    }

    public IEnumerator GetAllLeaderboardScore(LeaderboardPayload leaderboardPayload, Action<JArray> OnSuccess = null, Action<UnityWebRequest> OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "getAll");
        form.AddField("mode", leaderboardPayload.mode);

        yield return StartCoroutine(SendAWSWebRequest("leaderboard-easy", form, res =>
        {
            if (OnSuccess != null)
            {
                string result = res.downloadHandler.text.Substring(1, res.downloadHandler.text.Length - 2);

                JArray jsonArray = JArray.Parse(res.downloadHandler.text);
                OnSuccess?.Invoke(jsonArray);
            }

        }, err =>
        {
            if (OnError != null)
                OnError?.Invoke(err);
        }));
    }

    public IEnumerator UpdateUserScore(LeaderboardPayload leaderboardPayload, Action OnSuccess = null, Action OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "update");
        form.AddField("mode", leaderboardPayload.mode);
        form.AddField("username", leaderboardPayload.username);
        form.AddField("timing", (int)leaderboardPayload.timing);
        form.AddField("s3_skinpointer", leaderboardPayload.s3_skinpointer);

        yield return StartCoroutine(SendAWSWebRequest("leaderboard-easy", form, res =>
        {
            Debug.Log(res.downloadHandler.text);

            if (OnSuccess != null)
                OnSuccess?.Invoke();
      
        }, err =>
        {
            Debug.Log(err.downloadHandler.text);

            if (OnError != null)
                OnError?.Invoke();
        }));
    }

    private IEnumerator DeleteUserScore(LeaderboardPayload leaderboardPayload, Action OnSuccess = null, Action OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "delete");
        form.AddField("username", leaderboardPayload.username);
        form.AddField("timing", (int)leaderboardPayload.timing);

        yield return StartCoroutine(SendAWSWebRequest("leaderboard-easy", form, res =>
        {
            Debug.Log(res.downloadHandler.text);

            if (OnSuccess != null)
                OnSuccess?.Invoke();

        }, err =>
        {
            Debug.Log(err.downloadHandler.text);

            if (OnError != null)
                OnError?.Invoke();
        }));
    }
    #endregion

    #region Items

    public IEnumerator GetItemWithName(string name, Action<JArray> OnSuccess = null, Action<UnityWebRequest> OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "get");
        form.AddField("name", name);

        yield return StartCoroutine(SendAWSWebRequest("items", form, res =>
        {
            if (OnSuccess != null)
            {
                string result = $"[{res.downloadHandler.text}]";
                OnSuccess?.Invoke(JArray.Parse(result));
            }

        }, err =>
        {
            if (OnError != null)
                OnError?.Invoke(err);
        }));
    }

    public IEnumerator GetAllItems(Action<JArray> OnSuccess = null, Action<UnityWebRequest> OnError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "getAll");

        yield return StartCoroutine(SendAWSWebRequest("items", form, res =>
        {
            if (OnSuccess != null)
            {
                string result = res.downloadHandler.text.Substring(1, res.downloadHandler.text.Length - 2);

                JArray jsonArray = JArray.Parse(res.downloadHandler.text);
                OnSuccess?.Invoke(jsonArray);
            }

        }, err =>
        {
            if (OnError != null)
                OnError?.Invoke(err);
        }));
    }

    #endregion
}