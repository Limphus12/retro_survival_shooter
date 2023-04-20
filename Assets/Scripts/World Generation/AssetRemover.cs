using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AssetRemover : MonoBehaviour
    {
        [SerializeField] private Vector3 boxExtents = Vector3.one;

        [SerializeField] private string assetTag;
        public string AssetTag { get; private set; }

        [SerializeField] private bool debug;

        private void Awake()
        {
            AssetTag = assetTag;
        }

        // Start is called before the first frame update
        void Start()
        {
            Collider[] collisions = Physics.OverlapBox(transform.position, boxExtents);

            if (collisions.Length != 0)
            {
                int i = 0;
                foreach (Collider col in collisions)
                {
                    if (col.CompareTag(AssetTag))
                    {
                        Destroy(col.gameObject);

                        i++;
                    }
                }

                //Debug.Log("Removed " + i + " " + assetTag + "s");
            }

            //destroy this object after removing shit
            Destroy(gameObject);
        }

        public Vector3 GetBoxExtents() => boxExtents;

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