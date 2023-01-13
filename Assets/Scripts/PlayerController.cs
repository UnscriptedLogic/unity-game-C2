using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement
{
    public class PlayerController : MonoBehaviour
    {
        private InputManager inputManager;

        private float distance;
        private bool isDragging;
        private Vector2 startDrag;
        private Vector2 currMousePos;
        private Vector3 endPoint;

        [Header("Attributes")]
        [SerializeField] private float distanceClamp = 5f;
        [SerializeField] private float maxPower = 200f;

        [Header("Components")]
        [SerializeField] private Rigidbody rb;  
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Camera cam;

        private void Start()
        {
            inputManager = InputManager.instance;

            inputManager.isDragging += OnDragging;
            inputManager.mousePos += InputManager_mousePos;

            string skinName = GameManager.UserPayload != null ? GameManager.UserPayload.s3_skinpointer : "defaultskin";
            StartCoroutine(AWSManager.instance.GetItemWithName(skinName, res =>
            {
                StartCoroutine(AWSManager.instance.InstantiateObjectFromS3(res[0]["s3_skinpointer"]["S"].ToString(), skinName, transform, Vector3.zero, Quaternion.identity, go =>
                {
                    Destroy(go.GetComponent<BoxCollider>());
                    go.transform.localPosition = Vector3.zero;
                }));

            }, err => Debug.Log(err.downloadHandler.text)));
        }

        private void Update()
        {
            if (isDragging)
            {
                //We're setting the start position of the line here just in case the cube moves while dragging
                lineRenderer.SetPosition(0, transform.position);

                //Logic used for calculating the directional offset relative to the transform of the cube.
                //i.e. the direction of the line and the length of the line in relation to
                //the direction of the mouse to the cube and the distance of the starting drag position to the current mouse position
                Vector3 correctedPoint = GetWorldPoint(currMousePos);
                Vector3 difference = (GetWorldPoint(startDrag) - correctedPoint).normalized;
                distance = Mathf.Clamp(Vector3.Distance(GetWorldPoint(startDrag), correctedPoint), -distanceClamp, distanceClamp);
                endPoint = transform.position + (difference * distance);
                lineRenderer.SetPosition(1, endPoint);
            }
        }

        //Value of the current mouse from the input manager class
        private void InputManager_mousePos(Vector2 obj) => currMousePos = obj;

        //Logic used for triggering the states of the controller
        private void OnDragging(bool isDragging)
        {
            if (isDragging)
            {
                startDrag = currMousePos;
                lineRenderer.positionCount = 2;
            } 
            else
            {
                lineRenderer.positionCount = 0;

                Vector3 direction = endPoint - transform.position; 
                rb.AddForce(direction * (maxPower / distanceClamp * distance));
            }
            
            this.isDragging = isDragging;
        }

        //Logic for calculating world points from the screen
        private Vector3 GetWorldPoint(Vector2 screenPos)
        {
            Plane plane = new Plane(Vector3.back, transform.position);
            Ray ray = cam.ScreenPointToRay(screenPos);

            if (plane.Raycast(ray, out float length))
            {
                return ray.GetPoint(length);
            }

            return Vector3.zero;
        }
    }
}