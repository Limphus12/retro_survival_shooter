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

        private void OnTriggerEnter(Collider other)
        {
            //if the player enters this trigger
            if (other.CompareTag("Player"))
            {
                //call our event
                unityEvent.Invoke();

                CheckDirection(other);
            }
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

                    break;

                case CardinalDirection.EAST:

                    newPos = new Vector3(2, currentPos.y + heightOffset, currentPos.z);

                    MovePlayer(other.transform, newPos);

                    break;

                case CardinalDirection.SOUTH:

                    newPos = new Vector3(currentPos.x, currentPos.y + heightOffset, 510);

                    MovePlayer(other.transform, newPos);

                    break;

                case CardinalDirection.WEST:

                    newPos = new Vector3(510, currentPos.y + heightOffset, currentPos.z);

                    MovePlayer(other.transform, newPos);

                    break;

                default:
                    break;
            }
        }

        private void MovePlayer(Transform other, Vector3 position)
        {
            other.position = position;
        }
    }
}