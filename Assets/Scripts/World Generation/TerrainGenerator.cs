using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainGenerator : MonoBehaviour
    {
        Mesh mesh; //the mesh component

        Vector3[] vertices; //array of vertices
        int[] triangles; //array of triangles
        Vector2[] uvs; //array of uvs
        Color[] colors; //array of colors

        [Header("Terrain Size")]
        [SerializeField] private Vector2Int size; //how big we want our grid of vertices
        [SerializeField] private int gridMultiplier; //how far apart we want our vertices

        [Header("Perlin Noise")]
        [SerializeField] private float noiseScale = 1.0f;
        [SerializeField] private int octaves = 4;
        [SerializeField] [Range(0f, 1f)] private float persistance = 0.5f;
        [SerializeField] private float lacunarity = 2.0f, heightMultiplier = 2.0f;

        [Space]
        [SerializeField] private Vector2Int offset;

        [Header("Vertex Colors")]
        [Tooltip("The chances of choosing Red, Green, Blue or Black for the vertex color. For instance, [25, 50, 75] gives a 25% chance for each color.")]
        [SerializeField] private Vector3Int colorChance = new Vector3Int(25, 50, 75);

        private int seed;

        //sets our seed and generates the mesh
        public void GenerateTerrain(int seed, Vector2Int offset)
        {
            this.seed = seed; this.offset = new Vector2Int(Random.Range(-32000, 32000), Random.Range(-32000, 32000));

            //make sure to clear the current terrain first!
            ClearTerrain();

            //then start generating the mesh!
            GenerateMesh();
        }

        //just generates the mesh!
        public void GenerateTerrain()
        {
            //make sure to clear the current terrain first!
            ClearTerrain();

            //then start generating the mesh!
            GenerateMesh();
        }

        public void ClearTerrain()
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh
            GetComponent<MeshFilter>().sharedMesh = mesh;

            //ensure we have a clean mesh
            mesh.Clear();

            //recalculate normals on the mesh
            mesh.RecalculateNormals();

            // optionally, add a mesh collider (As suggested by Franku Kek via Youtube comments).
            // To use this, your MeshGenerator GameObject needs to have a mesh collider
            // component added to it.  Then, just re-enable the code below.
            mesh.RecalculateBounds();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private void GenerateMesh()
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh
            GetComponent<MeshFilter>().sharedMesh = mesh;

            //ensure we have a clean mesh
            mesh.Clear();

            //generate the vertices, triangles, UVs and colours
            mesh.vertices = CreateVertices();
            mesh.triangles = CreateTriangles();
            mesh.uv = GenerateUVs();
            mesh.colors = GenerateColors();

            //recalculate normals on the mesh
            mesh.RecalculateNormals();

            // optionally, add a mesh collider (As suggested by Franku Kek via Youtube comments).
            // To use this, your MeshGenerator GameObject needs to have a mesh collider
            // component added to it.  Then, just re-enable the code below.
            mesh.RecalculateBounds();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        //when we want to generate a mesh based on MeshData
        public void GenerateMesh(MeshData meshData)
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh
            GetComponent<MeshFilter>().sharedMesh = mesh;

            //ensure we have a clean mesh
            mesh.Clear();

            //set the vertices, triangles, UVs and colours
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.uv = meshData.uvs;
            mesh.colors = meshData.colors;

            //recalculate normals on the mesh
            mesh.RecalculateNormals();

            // optionally, add a mesh collider (As suggested by Franku Kek via Youtube comments).
            // To use this, your MeshGenerator GameObject needs to have a mesh collider
            // component added to it.  Then, just re-enable the code below.
            mesh.RecalculateBounds();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        public MeshData GetMeshData()
        {
            MeshData meshData = new MeshData
            {
                vertices = vertices,
                triangles = triangles,
                uvs = uvs,
                colors = colors
            };

            return meshData;
        }

        private Vector3[] CreateVertices()
        {
            //generate a grid of vertices, vertex count = (xSize + 1) * (zSize + 1)
            Vector3[] vertices = new Vector3[(size.x + 1) * (size.y + 1)];

            //calculate a height map, passing in our size variables, seed etc.
            float[,] heightMap = Noise.ComplexNoiseMap(size.x + 1, size.y + 1, seed, noiseScale, octaves, persistance, lacunarity, offset);

            //using a nested for loop to generate all our vertices
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    //grabbing the perlin noise value, and multiplying it by the heightmultiplier
                    float y = heightMap[x, z] * heightMultiplier;

                    //adding a new vertice to our array
                    vertices[i] = new Vector3(x * gridMultiplier, y, z * gridMultiplier);
                    i++;
                }
            }

            //setting our vertices to these generated ones
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

            //setting our triangles to these generated ones
            this.triangles = triangles;

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

            //setting our uvs to these generated ones
            this.uvs = uvs;

            return uvs;
        }

        private Color[] GenerateColors()
        {
            //create a new color array
            Color[] colors = new Color[vertices.Length];

            //nested for loop to generate color data
            //this implementation picks a random color from R, G, B or Black, based on the color chance.
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++)
                {
                    int j = Random.Range(0, 101);

                    if (j > 0 && j <= colorChance.x) colors[i] = Color.red;
                    else if (j > colorChance.x && j <= colorChance.y) colors[i] = Color.green;
                    else if (j > colorChance.y && j <= colorChance.z) colors[i] = Color.blue;
                    else if (j > colorChance.z && j <= 100) colors[i] = Color.black;
                    
                    i++;
                }
            }

            //setting our colors to these generated ones
            this.colors = colors;

            return colors;
        }
    }

    [System.Serializable]
    public struct MeshData
    {
        public Vector3[] vertices; //array of vertices
        public int[] triangles; //array of triangles
        public Vector2[] uvs; //array of uvs
        public Color[] colors; //array of colors
    }

}