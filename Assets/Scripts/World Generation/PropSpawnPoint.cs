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
        [SerializeField] private PropSpawnStruct[] props;

        private void Start() => SpawnProp();

        private void SpawnProp()
        {
            if (props.Length == 0) return;

            int i = Random.Range(0, props.Length);

            if (props[i].prop == null) return;

            float j = Random.Range(0f, 100f);

            if (j <= props[i].chanceToSpawn) Instantiate(props[i].prop, transform.position, transform.rotation, transform);
        }
    }
}