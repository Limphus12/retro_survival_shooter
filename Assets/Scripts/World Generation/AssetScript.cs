using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AssetScript : MonoBehaviour
    {
        [SerializeField] private Vector3 boxExtents = Vector3.one;

        // Start is called before the first frame update
        void Start()
        {
            Collider[] collisions = Physics.OverlapBox(transform.position, boxExtents);

            if (collisions.Length != 0)
            {
                int i = 0;
                foreach (Collider col in collisions)
                {
                    if (col.CompareTag("EnvironmentalAsset"))
                    {
                        Destroy(col.gameObject);

                        i++;
                    }
                }

                Debug.Log("Removed " + i + " Environmental Assets");
            }
        }
    }
}