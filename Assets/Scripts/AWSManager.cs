using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AWSManager : MonoBehaviour
{
    public string URL;
    public string assetName;
    
    private void Start()
    {
        StartCoroutine(LoadAsset(URL, assetName));
    }

    private IEnumerator LoadAsset(string url, string assetName)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url))
        {
            yield return webRequest.SendWebRequest();
            AssetBundle remoteBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            if (remoteBundle == null)
            {
                Log.Print("Failed to load AssetBundle!", name);
                yield break;
            }

            Instantiate(remoteBundle.LoadAsset(assetName));
        }
    }
}