using PlayerManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTrigger : MonoBehaviour
{
    private PlayerController controller;

    [SerializeField] private bool isRandom;
    [SerializeField] private GameObject[] particles;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();

        controller.CollisionEnter += CollisionEnter;   
    }

    private void CollisionEnter(Collision obj)
    {
        ContactPoint[] contactPoints = obj.contacts;

        int index = 0;

        if (isRandom)
            index = UnityEngine.Random.Range(0, particles.Length);

        Destroy(Instantiate(particles[index], contactPoints[0].point, Quaternion.identity), 10f);
    }
}
