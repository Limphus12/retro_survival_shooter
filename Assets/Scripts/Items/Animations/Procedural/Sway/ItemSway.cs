using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ItemSway : MonoBehaviour
    {
        [Header("Attributes - Item Default Settings")]
        [SerializeField] protected PlayerController playerController;

        [Space]
        [SerializeField] protected Vector3 defaultPosition;
        [SerializeField] protected Quaternion defaultRotation;

        [Space]
        [SerializeField] protected float defaultSwaySmooth = 8.0f;
        [SerializeField] protected float defaultSwayAmount = 2.0f, defaultSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float defaultTiltSmooth = 8.0f;
        [SerializeField] protected float defaultTiltAmount = 2.0f, defaultTiltMaximum = 2.0f;


        [Header("Attributes - Item Running Settings")]
        [SerializeField] protected Vector3 runningPosition;
        [SerializeField] protected Quaternion runningRotation;

        [Space]
        [SerializeField] protected float runningSwaySmooth = 8.0f;
        [SerializeField] protected float runningSwayAmount = 2.0f, runningSwayMaximum = 2.0f;

        [Space]
        [SerializeField] protected float runningTiltSmooth = 8.0f;
        [SerializeField] protected float runningTiltAmount = 2.0f, runningTiltMaximum = 2.0f;

        protected Quaternion initialRotation;

        protected virtual void Awake()
        {
            initialRotation = defaultRotation;

            if (!playerController) playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        protected virtual void Update() 
        {
            CheckSway();
            CheckTilt();
        }

        protected virtual void CheckSway()
        {
            if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
            {
                Sway(runningSwayAmount, runningSwayMaximum, runningSwaySmooth, runningPosition);
            }

            else Sway(defaultSwayAmount, defaultSwayMaximum, defaultSwaySmooth, defaultPosition);
        }

        protected virtual void CheckTilt()
        {
            if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
            {
                Tilt(runningTiltAmount, runningTiltMaximum, runningTiltSmooth, runningRotation);
            }

            else Tilt(defaultTiltAmount, defaultTiltMaximum, defaultTiltSmooth, defaultRotation);
        }

        protected void Sway(float amount, float maximum, float smooth, Vector3 position)
        {
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate sway positions
            float swayPositionX = Mathf.Clamp(inputs.x * amount, -maximum, maximum) / 100;
            float swayPositionY = Mathf.Clamp(inputs.y * amount, -maximum, maximum) / 100;

            //calculate final target position
            Vector3 targetPosition = new Vector3(swayPositionX, swayPositionY);

            //apply target position
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + position, smooth * Time.deltaTime);
        }

        protected void Tilt(float amount, float maximum, float smooth, Quaternion rotation)
        {
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate tilt rotations
            float tiltRotationY = Mathf.Clamp(inputs.x * amount, -maximum, maximum);
            float tiltRotationX = Mathf.Clamp(inputs.y * amount, -maximum, maximum);
            float tiltRotationZ = Mathf.Clamp(Input.GetAxis("Horizontal") * amount, -maximum, maximum);

            //calculate target rotation
            Quaternion tiltRotation = Quaternion.Euler(new Vector3(tiltRotationX, -tiltRotationY, -tiltRotationZ));

            Quaternion targetRotation = rotation * tiltRotation;

            //apply target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation * initialRotation, smooth * Time.deltaTime);
        }

        private Vector2 Inputs()
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }
}