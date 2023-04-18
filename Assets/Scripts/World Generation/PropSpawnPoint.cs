using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct PropSpawnStruct
    {
        public GameObject prop;
        [Range(0f, 100f)] public float chanceToSpawn;
    }

    public class PropSpawnPoint : MonoBehaviour
    {
        [Tooltip("The amount of spawn attempts this will make")]
        [SerializeField] private int spawnAttempts;

        [Space, SerializeField] private PropSpawnStruct[] props;

        [Tooltip("Forces a prop to spawn, via looping through all props until one is spawned")]
        [SerializeField] private bool forceSpawn;

        [Tooltip("Applies a random force to the spawned prop, granted it has a rigidbody")]
        [Space, SerializeField] private bool applyRandomForce;

        private GameObject prop;

        private RenderingHandler renderingHandler;

        private void Start() => SpawnProp();

        private void SpawnProp()
        {
            if (props.Length == 0) return;

            //for loop to go through multiple spawn attempts
            for (int i = 0; i < spawnAttempts; i++)
            {
                if (forceSpawn)
                {
                    //a while loop to force us to keep on attempting to spawn props until we have one spawned...
                    while (prop == null)
                    {
                        int k = Random.Range(0, props.Length);

                        if (props[k].prop == null) continue;

                        float j = Random.Range(0f, 100f);

                        if (j <= props[k].chanceToSpawn) prop = Instantiate(props[k].prop, transform.position, transform.rotation, transform);
                    }
                }

                else
                {
                    int k = Random.Range(0, props.Length);

                    if (props[k].prop == null) return;

                    float j = Random.Range(0f, 100f);

                    if (j <= props[k].chanceToSpawn) prop = Instantiate(props[k].prop, transform.position, transform.rotation, transform);
                }

                if (prop && applyRandomForce) ApplyRandomForce();

                //reset the prop back to null
                prop = null;
            }

            if (!renderingHandler) renderingHandler = gameObject.AddComponent<RenderingHandler>();
        }

        private void ApplyRandomForce()
        {
            //attempt to grab the rigidbody and chuck it in a random direction
            Rigidbody rb = prop.GetComponent<Rigidbody>();

            if (rb) rb.AddExplosionForce(100f, prop.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 2f);
        }
    }
}