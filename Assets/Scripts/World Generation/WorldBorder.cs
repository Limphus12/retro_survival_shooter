using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public enum CardinalDirection { NORTH, EAST, SOUTH, WEST }

    public class WorldBorder : MonoBehaviour
    {
        [SerializeField] private CardinalDirection direction;

        [SerializeField] private float travelDelayTime = 3f;

        [SerializeField] private WorldGenerator worldGenerator;

        [SerializeField] private LayerMask layerMask;

        public static bool IsInBorder = false, IsTravelling = false;

        public static event EventHandler<Events.OnStringChangedEventArgs> OnBorderChanged;

        private void Start()
        {
            MovePlayer(GameManager.Player.transform, new Vector3(192, 1000, 192));
        }

        private void OnTriggerEnter(Collider other)
        {
            //if the player enters this trigger
            if (other.gameObject == GameManager.Player)
            {
                IsInBorder = true;

                if (!IsTravelling) StartTravel();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //if the player enters this trigger
            if (other.gameObject == GameManager.Player)
            {
                IsInBorder = false;
            }
        }

        void StartTravel()
        {
            IsTravelling = true;

            OnBorderChanged?.Invoke(this, new Events.OnStringChangedEventArgs { i = "Exiting - " + worldGenerator.CurrentWorldData.name });

            Invoke(nameof(Travel), travelDelayTime);
        }

        void Travel()
        {
            //call our event
            worldGenerator.Travel(direction);

            CheckDirection();

            EndTravelling();
        }

        void EndTravelling()
        {
            OnBorderChanged?.Invoke(this, new Events.OnStringChangedEventArgs { i = "Entering - " + worldGenerator.CurrentWorldData.name });

            IsTravelling = false;
        }

        private void CheckDirection()
        {
            Vector3 currentPos = GameManager.Player.transform.position;
            Vector3 newPos;

            float heightOffset = 1000f;

            //calculate the new position of teh player!
            switch (direction)
            {
                case CardinalDirection.NORTH:

                    newPos = new Vector3(currentPos.x, heightOffset, 132);

                    MovePlayer(GameManager.Player.transform, newPos);
                    RotatePlayer(GameManager.Player.transform, Quaternion.Euler(0, 0, 0));

                    break;

                case CardinalDirection.EAST:

                    newPos = new Vector3(132, heightOffset, currentPos.z);

                    MovePlayer(GameManager.Player.transform, newPos);
                    RotatePlayer(GameManager.Player.transform, Quaternion.Euler(0, 90, 0));

                    break;

                case CardinalDirection.SOUTH:

                    newPos = new Vector3(currentPos.x, heightOffset, 252);

                    MovePlayer(GameManager.Player.transform, newPos);
                    RotatePlayer(GameManager.Player.transform, Quaternion.Euler(0, 180, 0));

                    break;

                case CardinalDirection.WEST:

                    newPos = new Vector3(252, heightOffset, currentPos.z);

                    MovePlayer(GameManager.Player.transform, newPos);
                    RotatePlayer(GameManager.Player.transform, Quaternion.Euler(0, 270, 0));

                    break;

                default:
                    break;
            }
        }

        private void MovePlayer(Transform player, Vector3 position)
        {
            //do a raycast to find where we should place the player
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                //calculate the position here
                Vector3 placementPoint = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);

                player.position = placementPoint;
            }
        }

        private void RotatePlayer(Transform player, Quaternion rotation) => player.rotation = rotation;
    }
}