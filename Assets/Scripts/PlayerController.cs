using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerManagement
{
    public class PlayerController : MonoBehaviour
    {
        private InputManager inputManager;

        private float distance;
        private bool isDragging;
        private bool registeredFirst;
        private Vector2 startDrag;
        private Vector2 currMousePos;
        private Vector3 endPoint;
        private Vector3 lightPos;

        [Header("Attributes")]
        [SerializeField] private float distanceClamp = 5f;
        [SerializeField] private float maxPower = 200f;

        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Camera cam;
        [SerializeField] private Transform spotlightTransform;
        [SerializeField] private Light spotlight;
        [SerializeField] private Light playerlight;

        public Rigidbody Rb => rb;
        public BoxCollider BoxCollider => boxCollider;

        public Action<Collision> CollisionEnter;
        public Action<Collision> CollisionStay;
        public Action<Collision> CollisionExit;

        public Action<Collider> TriggerEnter;
        public Action<Collider> TriggerStay;
        public Action<Collider> TriggerExit;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI strengthTMP;
        [SerializeField] private TextMeshProUGUI speedTMP;

        private void OnEnable()
        {
            inputManager = InputManager.instance;

            inputManager.isDragging += OnDragging;
            inputManager.mousePos += InputManager_mousePos;

            lightPos = playerlight.transform.position - transform.position;
        }

        private void OnDisable()
        {
            inputManager.isDragging -= OnDragging;
            inputManager.mousePos -= InputManager_mousePos;
        }

        private void Update()
        {
            playerlight.transform.position = transform.position + lightPos;

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

                spotlightTransform.forward = (endPoint - transform.position).normalized;

                strengthTMP.text = "Strength:\n" + (maxPower / distanceClamp * distance).ToString("0.00");
            }

            speedTMP.text = "Speed:\n" + rb.velocity.magnitude.ToString("0.00");
        }

        //Value of the current mouse from the input manager class
        private void InputManager_mousePos(Vector2 obj) => currMousePos = obj;

        //Logic used for triggering the states of the controller
        private void OnDragging(bool isDragging)
        {
            if (rb.velocity.magnitude > 0)
                return;

            if (isDragging)
            {
                startDrag = currMousePos;
                lineRenderer.positionCount = 2;
                registeredFirst = true;
            } 
            else
            {
                if (!registeredFirst) return;

                lineRenderer.positionCount = 0;

                Vector3 direction = endPoint - transform.position;
                float strength = maxPower / distanceClamp * distance;
                rb.AddForce(direction.normalized * strength, ForceMode.VelocityChange);
                registeredFirst = false;
            }
            
            spotlight.enabled = isDragging;
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

        private void OnTriggerEnter(Collider other) => TriggerEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => TriggerStay?.Invoke(other);
        private void OnTriggerExit(Collider other) => TriggerExit?.Invoke(other);

        private void OnCollisionEnter(Collision collision) => CollisionEnter?.Invoke(collision);
        private void OnCollisionStay(Collision collision) => CollisionStay?.Invoke(collision);
        private void OnCollisionExit(Collision collision) => CollisionExit?.Invoke(collision);
    }
}