using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.limphus.retro_survival_shooter
{
    public enum CardinalDirection { NORTH, EAST, SOUTH, WEST }

    public class WorldBorder : MonoBehaviour
    {
        [SerializeField] private CardinalDirection direction;

        [SerializeField] private UnityEvent unityEvent;

        [SerializeField] private float travelDelayTime = 3f;

        public static bool IsInBorder = false, IsTravelling = false;

        private Collider playerCollider;

        private void OnTriggerEnter(Collider other)
        {
            //if the player enters this trigger
            if (other.CompareTag("Player"))
            {
                IsInBorder = true;

                playerCollider = other;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other == playerCollider)
            {
                if (Input.GetKey(KeyCode.E) && !IsTravelling)
                {
                    StartTravel();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //if the player enters this trigger
            if (other.CompareTag("Player"))
            {
                IsInBorder = false;

                playerCollider = null;
            } 
        }

        void StartTravel()
        {
            IsTravelling = true;

            Debug.Log("Started World Border Travel");

            Invoke(nameof(Travel), travelDelayTime);
        }

        void Travel()
        {
            //call our event
            unityEvent.Invoke();

            CheckDirection(playerCollider);

            EndTravelling();
        }

        void EndTravelling()
        {
            IsTravelling = false;
        }

        private void CheckDirection(Collider other)
        {
            Vector3 currentPos = other.transform.position;
            Vector3 newPos;

            float heightOffset = 2f;

            //calculate the new position of teh player!
            switch (direction)
            {
                case CardinalDirection.NORTH:

                    newPos = new Vector3(currentPos.x, currentPos.y + heightOffset, 2);

                    MovePlayer(other.transform, newPos);
                    RotatePlayer(other.transform, Quaternion.Euler(0, 0, 0));

                    break;

                case CardinalDirection.EAST:

                    newPos = new Vector3(2, currentPos.y + heightOffset, currentPos.z);

                    MovePlayer(other.transform, newPos);
                    RotatePlayer(other.transform, Quaternion.Euler(0, 90, 0));

                    break;

                case CardinalDirection.SOUTH:

                    newPos = new Vector3(currentPos.x, currentPos.y + heightOffset, 510);

                    MovePlayer(other.transform, newPos);
                    RotatePlayer(other.transform, Quaternion.Euler(0, 180, 0));

                    break;

                case CardinalDirection.WEST:

                    newPos = new Vector3(510, currentPos.y + heightOffset, currentPos.z);

                    MovePlayer(other.transform, newPos);
                    RotatePlayer(other.transform, Quaternion.Euler(0, 270, 0));

                    break;

                default:
                    break;
            }
        }

        private void MovePlayer(Transform other, Vector3 position)
        {
            other.position = position;
        }

        private void RotatePlayer(Transform other, Quaternion rotation)
        {
            other.rotation = rotation;

            Debug.Log("rotating player!");
        }
    }
}