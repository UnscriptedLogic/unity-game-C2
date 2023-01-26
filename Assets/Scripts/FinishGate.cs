using Amazon.S3.Model;
using LevelManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGate : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelManager.instance.LevelCompleted();
        }
    }
}
