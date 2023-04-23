using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureRemover : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;

    private Transform[] positions;

    // Start is called before the first frame update
    private void Start()
    {
        positions = gameObject.GetComponentsInChildren<Transform>();

        CheckTerrainCollision();
    }

    private void CheckTerrainCollision()
    {
        bool hitTerrain = false;

        for (int i = 0; i < positions.Length; i++)
        {
            if (Physics.Raycast(positions[i].position, Vector3.down, 0.1f, terrainLayer))
            {
                hitTerrain = true;
            }

            else if (Physics.Raycast(positions[i].position, Vector3.up, 0.1f, terrainLayer))
            {
                hitTerrain = true;
            }

            else hitTerrain = false;
        }

        //if we dont hit the terrain, destroy teh parent!
        if (!hitTerrain) Destroy(gameObject.transform.parent.gameObject);
    }
}
