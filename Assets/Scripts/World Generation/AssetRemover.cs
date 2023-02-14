using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AssetRemover : MonoBehaviour
    {
        [SerializeField] private Vector3 boxExtents = Vector3.one;

        [SerializeField] private string assetTag;

        [SerializeField] private bool debug;

        public Vector3 GetBoxExtents() => boxExtents;

        // Start is called before the first frame update
        void Start()
        {
            Collider[] collisions = Physics.OverlapBox(transform.position, boxExtents, transform.localRotation);

            if (collisions.Length != 0)
            {
                int i = 0;
                foreach (Collider col in collisions)
                {
                    if (col.CompareTag(assetTag))
                    {
                        Destroy(col.gameObject);

                        i++;
                    }
                }

                Debug.Log("Removed " + i + " " + assetTag + "s");
            }
        }

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, boxExtents);
            }
        }
    }
}