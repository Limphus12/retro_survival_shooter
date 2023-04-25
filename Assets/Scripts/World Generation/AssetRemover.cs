using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AssetRemover : MonoBehaviour
    {
        [SerializeField] private Vector3 boxExtents = Vector3.one;
        [SerializeField] private Vector3 offset = Vector3.zero;

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
            Collider[] collisions = Physics.OverlapBox(transform.position + offset, boxExtents, transform.rotation);

            if (collisions.Length != 0)
            {
                int i = 0;
                foreach (Collider col in collisions)
                {
                    //if the collider matches the asset tag and it's not our parent
                    if (col.CompareTag(AssetTag) && transform.parent != col.transform)
                    {
                        Destroy(col.gameObject);

                        i++;
                    }
                }
            }

            Destroy(gameObject);
        }

        public Vector3 GetBoxExtents() => boxExtents;
        public Vector3 GetOffset() => offset;

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position + offset, boxExtents);
            }
        }
    }
}