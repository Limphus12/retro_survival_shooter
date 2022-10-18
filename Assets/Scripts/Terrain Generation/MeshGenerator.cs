using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class MeshGenerator : MonoBehaviour
    {
        Mesh mesh; //the mesh component

        Vector3[] vertices; //array of vertices
        int[] triangles; //array of triangles
        Vector2[] uvs; //array of uvs

        [Header("Terrain Size")]
        [SerializeField] private Vector2Int size; //how big we want our grid of vertices
        [SerializeField] private int gridMultiplier; //how far apart we want our vertices

        //[Header("Terrain Height")]
        //[Tooltip("A lower value will create smaller hills and features, a higher value will create larger hills and peaks")] 
        //[SerializeField] private float heightMultiplier = 2.0f;

        //[Tooltip("A lower value will create smoother terrain, a higher value will create more rough terrain")]
        //[Range(0.0f, 1.0f)]
        //[SerializeField] private float noiseMultiplier = 0.3f;


        [Header("Perlin Noise")]
        [SerializeField] private float noiseScale = 1.0f;
        [SerializeField] private int seed, octaves = 4;
        [SerializeField] [Range(0f, 1f)] private float persistance = 0.5f;
        [SerializeField] private float lacunarity = 2.0f, heightMultiplier = 2.0f;

        // Start is called before the first frame update
        private void Start()
        {
            GenerateMesh();

            //now we grab the asset generator from our child and tell it to place assets!
            AssetGenerator ag = GetComponentInChildren<AssetGenerator>();

            if (ag) ag.GenerateAssets();

            /*
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            
            CreateShape();
            UpdateMesh();
            */
        }

        public void GenerateMesh()
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh
            GetComponent<MeshFilter>().sharedMesh = mesh;

            //ensure we have a clean mesh
            mesh.Clear();

            //generate the vertices and triangles
            mesh.vertices = CreateVertices();
            mesh.triangles = CreateTriangles();
            mesh.uv = GenerateUVs();

            //recalculate normals on the mesh
            mesh.RecalculateNormals();

            // optionally, add a mesh collider (As suggested by Franku Kek via Youtube comments).
            // To use this, your MeshGenerator GameObject needs to have a mesh collider
            // component added to it.  Then, just re-enable the code below.
            mesh.RecalculateBounds();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private Vector3[] CreateVertices()
        {
            //generate a grid of vertices, vertex count = (xSize + 1) * (zSize + 1)
            Vector3[] vertices = new Vector3[(size.x + 1) * (size.y + 1)];

            //calculate a height map, passing in our size variables, seed etc.
            float[,] heightMap = Noise.ComplexNoiseMap(size.x + 1, size.y + 1, seed, noiseScale, octaves, persistance, lacunarity);
            //float[,] heightMap = Noise.SimpleNoiseMap(size.x + 1, size.y + 1, noiseScale);

            //using a nested for loop to generate all our vertices
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    //grabbing the perlin noise value, and multiplying it by the heightmultiplier
                    float y = heightMap[x, z] * heightMultiplier;

                    //adding in simple perlin noise to offset our points on the y axis
                    //float y = Mathf.PerlinNoise(x * noiseMultiplier, z * noiseMultiplier) * heightMultiplier;

                    //adding a new vertice to our array
                    vertices[i] = new Vector3(x * gridMultiplier, y, z * gridMultiplier);
                    i++;
                }
            }

            //need to do this so that the uvs can be calculated
            this.vertices = vertices;

            return vertices;
        }

        private int[] CreateTriangles()
        {
            //using another nested for loop to generate our triangles
            int[] triangles = new int[size.x * size.y * 6]; //our triangle array, 6 points in a quad multiplied by our xSize and zSize

            int vert = 0, tris = 0; //used to keep track of the vertices and triangles we are on

            for (int z = 0; z < size.y; z++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    //generating a triangle
                    //we add vert to each of the triangles to offset them correctly
                    //we add tris to the index so that we don't update the same 6 points

                    //first triangle
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + size.x + 1;
                    triangles[tris + 2] = vert + 1;

                    //second triangle
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + size.x + 1;
                    triangles[tris + 5] = vert + size.x + 2;

                    vert++;
                    tris += 6;
                }

                vert++; //we do this to elimiate weird behaviour with our grid
                //i.e weird lighting effects and connections between certain vertices
            }

            return triangles;
        }

        private Vector2[] GenerateUVs()
        {
            //create a new vec2 array
            Vector2[] uvs = new Vector2[vertices.Length];

            //nested for loop to generate uv data
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    uvs[i] = new Vector2((float)x / size.x, (float)z / size.y);
                    i++;
                }
            }

            return uvs;
        }

        #region OLD CODE
        private void CreateShape()
        {
            //using the xSize and zSize, generate a grid of vertices
            //vertex count = (xSize + 1) * (zSize + 1)
            vertices = new Vector3[(size.x + 1) * (size.y + 1)];

            //using a nested for loop to generate all our vertices
            //turns out you can add additional variables inside for loops?
            //now we've got i = 0 and z = 0... pretty interesting.
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    //adding in simple perlin noise to offset our points on the y axis
                    //float y = Mathf.PerlinNoise(x * noiseMultiplier, z * noiseMultiplier) * heightMultiplier;

                    //adding a new vertice to our array
                    //vertices[i] = new Vector3(x, y, z);
                    i++;
                }
            }

            //using another nested for loop to generate our triangles
            triangles = new int[size.x * size.y * 6]; //our triangle array, 6 points in a quad multiplied by our xSize and zSize

            int vert = 0, tris = 0; //used to keep track of the vertices and triangles we are on

            for (int z = 0; z < size.y; z++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    //generating a triangle
                    //we add vert to each of the triangles to offset them correctly
                    //we add tris to the index so that we don't update the same 6 points

                    //first triangle
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + size.x + 1;
                    triangles[tris + 2] = vert + 1;

                    //second triangle
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + size.x + 1;
                    triangles[tris + 5] = vert + size.x + 2;

                    vert++;
                    tris += 6;
                }

                vert++; //we do this to elimiate weird behaviour with our grid
                //i.e weird lighting effects and connections between certain vertices
            }

            uvs = new Vector2[vertices.Length];

            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    uvs[i] = new Vector2((float)x / size.x, (float)z / size.y);
                    i++;
                }
            }
        }

        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

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

        #endregion

        private void OnValidate()
        {
            if (size.x < 1) size.x = 1;
            if (size.y < 1) size.y = 1;
            if (lacunarity < 1) lacunarity = 1;
            if (octaves < 0) octaves = 0;

            GenerateMesh();
        }
    }
}