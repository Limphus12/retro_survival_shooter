using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        Mesh mesh; //the mesh component

        Vector3[] vertices; //array of vertices
        int[] triangles; //array of triangles

        [Header("Terrain Size")]
        [Range(1, 256)] [SerializeField] private int xSize = 20; //how big we want our grid of vertices on the x axis
        [Range(1, 256)] [SerializeField] private int zSize = 20; //how big we want our grid of vertices on the z axis

        [Header("Terrain Height")]
        [SerializeField] private float heightMultiplier = 2.0f;

        [Tooltip("A lower value will create smoother terrain, a higher value will create more rough terrain")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float noiseMultiplier = 0.3f;

        // Start is called before the first frame update
        private void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
            UpdateMesh();
        }

        private void CreateShape()
        {
            //using the xSize and zSize, generate a grid of vertices
            //vertex count = (xSize + 1) * (zSize + 1)
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];

            //using a nested for loop to generate all our vertices
            //turns out you can add additional variables inside for loops?
            //now we've got i = 0 and z = 0... pretty interesting.
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    //adding in simple perlin noise to offset our points on the y axis
                    float y = Mathf.PerlinNoise(x * noiseMultiplier, z * noiseMultiplier) * heightMultiplier;

                    //adding a new vertice to our array
                    vertices[i] = new Vector3(x, y, z);
                    i++;
                }
            }

            //using another nested for loop to generate our triangles
            triangles = new int[xSize * zSize * 6]; //our triangle array, 6 points in a quad multiplied by our xSize and zSize

            int vert = 0, tris = 0; //used to keep track of the vertices and triangles we are on

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    //generating a triangle
                    //we add vert to each of the triangles to offset them correctly
                    //we add tris to the index so that we don't update the same 6 points

                    //first triangle
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;

                    //second triangle
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;
                }

                vert++; //we do this to elimiate weird behaviour with our grid
                //i.e weird lighting effects and connections between certain vertices
            }
            
            //old mesh generation code - makes a quad
            /*
            vertices = new Vector3[]
            {
                new Vector3(0,0,0),
                new Vector3(0,0,1),
                new Vector3(1,0,0),
                new Vector3(1,0,1)
            };

            triangles = new int[]
            {
                0, 1, 2,
                1, 3, 2
            };
            */
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            if (vertices == null) return;

            //for (int i = 0; i < vertices.Length; i++)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }
}