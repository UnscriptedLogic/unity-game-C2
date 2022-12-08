using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.S3;
using System.Threading.Tasks;
using Amazon.Runtime;
using System.Linq;
using Amazon.S3.Model;
using System.Text;
using System.IO;

public class AWSManager : MonoBehaviour
{
    [SerializeField] private string _accessKey;
    [SerializeField] private string _secretKey;

    private async void Start()
    {
        await GetBucketsAsync();
    }

    private async Task GetBucketsAsync()
    {
        AmazonS3Client client = new AmazonS3Client(new BasicAWSCredentials(_accessKey, _secretKey), Amazon.RegionEndpoint.USEast1);
        var buckets = await client.ListBucketsAsync();
        foreach (var bucket in buckets.Buckets)
        {
            var objects = await client.ListObjectsAsync(bucket.BucketName);
            if (objects != null)
            {
                Debug.Log($"For bucket {bucket.BucketName}, files are: {string.Join(", ", objects.S3Objects.Select(x => x.Key))}");
                foreach (var s3Object in objects.S3Objects)
                {
                    var response = await client.GetObjectAsync(new GetObjectRequest
                    {
                        BucketName = bucket.BucketName,
                        Key = s3Object.Key
                    });

                    var loadedObject = AssetBundle.LoadFromStreamAsync(response.ResponseStream);
                    if (loadedObject == null)
                    {
                        Debug.LogWarning("Retrieved object is null");
                    }

                    var prefab = loadedObject.assetBundle.LoadAsset<GameObject>("RapidTower");
                    Instantiate(prefab);
                }
            }
        }
    }
}