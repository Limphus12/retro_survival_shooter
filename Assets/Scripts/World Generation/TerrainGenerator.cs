using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct TerrainDataStruct
    {
        [Space]
        public float noiseScale;
        public int octaves;
        [Range(0f, 1f)] public float persistance;
        public float lacunarity, heightMultiplier;
        public Noise.NormalizeMode normalizeMode;

        [Space]
        public Material terrainMaterial;

        [Space]
        [Tooltip("The chances of choosing Red, Green, Blue or Black for the vertex color. For instance, [25, 50, 75] gives a 25% chance for each color.")]
        public Vector3Int colorChance;
    }

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
        [SerializeField] private int size = 128; //how big we want our grid of vertices
        [SerializeField] private int gridMultiplier = 4; //how far apart we want our vertices

        [Space]
        [SerializeField] private Vector2Int offset;

        public TerrainDataStruct TerrainData { private get; set; }

        public void SetOffset(Vector2Int offset) => this.offset = offset;

        //just generates the mesh!
        public void GenerateTerrain()
        {
            //make sure to clear the current terrain first!
            ClearTerrain(); GenerateMesh(); //then start generating the mesh!
        }

        public void ClearTerrain()
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh and name
            GetComponent<MeshFilter>().sharedMesh = mesh; mesh.name = "Terrain";

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

            //set the mesh filter's mesh and name
            GetComponent<MeshFilter>().sharedMesh = mesh; mesh.name = "Terrain";

            //ensure we have a clean mesh
            mesh.Clear();

            //generate the vertices, triangles, UVs and colours
            mesh.vertices = GenerateVertices();
            mesh.triangles = GenerateTriangles();
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

            //finally, we can change the material for different biomes!
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material = TerrainData.terrainMaterial;
        }

        //when we want to generate a mesh with a pre-determined vertice array
        private void GenerateMesh(Vector3[] vertices)
        {
            //create a new mesh
            mesh = new Mesh();

            //set the mesh filter's mesh and name
            GetComponent<MeshFilter>().sharedMesh = mesh; mesh.name = "Terrain";

            //ensure we have a clean mesh
            mesh.Clear();

            //generate the vertices, triangles, UVs and colours
            mesh.vertices = vertices; //we dont have to regenerate the vertices, just assign them
            mesh.triangles = GenerateTriangles();
            mesh.uv = GenerateUVs();
            mesh.colors = colors; //don't need to regenerate the colors

            //recalculate normals on the mesh
            mesh.RecalculateNormals();

            // optionally, add a mesh collider (As suggested by Franku Kek via Youtube comments).
            // To use this, your MeshGenerator GameObject needs to have a mesh collider
            // component added to it.  Then, just re-enable the code below.
            mesh.RecalculateBounds();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private Vector3[] GenerateVertices()
        {
            //generate a grid of vertices, vertex count = (xSize + 1) * (zSize + 1)
            Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];

            //calculate a height map, passing in our size variables, seed etc.
            float[,] heightMap = Noise.ComplexNoiseMap(size + 1, size + 1, TerrainData.noiseScale, TerrainData.octaves, TerrainData.persistance, TerrainData.lacunarity, offset, TerrainData.normalizeMode);

            //using a nested for loop to generate all our vertices
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    //grabbing the perlin noise value, and multiplying it by the heightmultiplier
                    float y = heightMap[x, z] * TerrainData.heightMultiplier;

                    //adding a new vertice to our array
                    vertices[i] = new Vector3(x * gridMultiplier, y, z * gridMultiplier);
                    i++;
                }
            }

            //setting our vertices to these generated ones
            this.vertices = vertices;

            return vertices;
        }

        private int[] GenerateTriangles()
        {
            //using another nested for loop to generate our triangles
            int[] triangles = new int[size * size * 6]; //our triangle array, 6 points in a quad multiplied by our xSize and zSize

            int vert = 0, tris = 0; //used to keep track of the vertices and triangles we are on

            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    //generating a triangle
                    //we add vert to each of the triangles to offset them correctly
                    //we add tris to the index so that we don't update the same 6 points

                    //first triangle
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + size + 1;
                    triangles[tris + 2] = vert + 1;

                    //second triangle
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + size + 1;
                    triangles[tris + 5] = vert + size + 2;

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
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    uvs[i] = new Vector2((float)x / size, (float)z / size);
                    i++;
                }
            }

            //setting our uvs to these generated ones
            this.uvs = uvs;

            return uvs;
        }

        private Vector2[] GenerateUVs(Vector3[] vertices)
        {
            //create a new vec2 array
            Vector2[] uvs = new Vector2[vertices.Length];

            //nested for loop to generate uv data
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    uvs[i] = new Vector2((float)x / size, (float)z / size);
                    i++;
                }
            }

            return uvs;
        }

        private Color[] GenerateColors()
        {
            //create a new color array
            Color[] colors = new Color[vertices.Length];

            //nested for loop to generate color data
            //this implementation picks a random color from R, G, B or Black, based on the color chance.
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    int j = Random.Range(0, 101);

                    if (j > 0 && j <= TerrainData.colorChance.x) colors[i] = Color.red;
                    else if (j > TerrainData.colorChance.x && j <= TerrainData.colorChance.y) colors[i] = Color.green;
                    else if (j > TerrainData.colorChance.y && j <= TerrainData.colorChance.z) colors[i] = Color.blue;
                    else colors[i] = Color.black;

                    i++;
                }
            }

            //setting our colors to these generated ones
            this.colors = colors;

            return colors;
        }
        public void ModifyVertices(StructureAreaStruct sas)
        {
            //for loop to run through each vertice in our vertices array
            for (int i = 0; i < vertices.Length; i++)
            {
                //loop through each area and see if the vertex lands in that area
                for (int j = 0; j < sas.structureAreas.Count; j++)
                {
                    //create a min position vector
                    Vector3 minPos = new Vector3(
                        sas.structurePositions[j].x - sas.structureAreas[j].x,
                        sas.structurePositions[j].y - sas.structureAreas[j].y,
                        sas.structurePositions[j].z - sas.structureAreas[j].z);

                    //create a max position vector
                    Vector3 maxPos = new Vector3(
                        sas.structurePositions[j].x + sas.structureAreas[j].x,
                        sas.structurePositions[j].y + sas.structureAreas[j].y,
                        sas.structurePositions[j].z + sas.structureAreas[j].z);

                    //check if the vertex is not within the min/max positions
                    if (vertices[i].x < minPos.x || vertices[i].x > maxPos.x ||
                        vertices[i].y < minPos.y || vertices[i].y > maxPos.y ||
                        vertices[i].z < minPos.z || vertices[i].z > maxPos.z)
                    {
                        //do nothing
                    }

                    else
                    {
                        float yPos = sas.structurePositions[j].y;

                        Debug.Log("vertex " + i + " is within a structure area; moving it vertically to y - " + yPos);

                        vertices[i].y = yPos;
                    }
                }
            }

            //regen the mesh, based on these new vertices
            GenerateMesh(vertices);
        }
    }
}