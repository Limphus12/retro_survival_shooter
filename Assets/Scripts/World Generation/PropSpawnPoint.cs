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
        [SerializeField] private float spawnDistance = 16f;

        [Space, SerializeField] private PropSpawnStruct[] props;

        [Tooltip("Forces a prop to spawn, via looping through all props until one is spawned")]
        [SerializeField] private bool forceSpawn;

        [Tooltip("Applies a random force to the spawned prop, granted it has a rigidbody")]
        [Space, SerializeField] private bool applyRandomForce;
        [SerializeField] private float forceMulti = 1f;

        [Space, SerializeField] private bool randomRotation;

        private GameObject prop;

        private RenderingHandler renderingHandler;

        private bool spawnedProps = false;

        private void Update()
        {
            float distance = Vector3.Distance(GameManager.Player.transform.position, transform.position);

            if (distance <= spawnDistance && !spawnedProps)
            {
                spawnedProps = true;
                SpawnProps();
            }
        }

        private void SpawnProps()
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

                        Quaternion targetRotation;

                        if (randomRotation) targetRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        else targetRotation = transform.rotation;

                        if (j <= props[k].chanceToSpawn) prop = Instantiate(props[k].prop, transform.position, targetRotation, transform);
                    }
                }

                else
                {
                    int k = Random.Range(0, props.Length);

                    if (props[k].prop == null) return;

                    float j = Random.Range(0f, 100f);

                    Quaternion targetRotation;

                    if (randomRotation) targetRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    else targetRotation = transform.rotation;

                    if (j <= props[k].chanceToSpawn) prop = Instantiate(props[k].prop, transform.position, targetRotation, transform);
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

            if (rb) rb.AddExplosionForce(100f * forceMulti, prop.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 2f);
        }
    }
}