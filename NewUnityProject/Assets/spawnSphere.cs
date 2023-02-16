using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace name1
{
    public class spawnSphere : MonoBehaviour
    {
        struct Triangle {
            public float3 a, b, c;
        };

        AppendStructuredBuffer<Triangle> triangles;
        public GameObject points;
        public const int MYSIZE = 5;
        public const int NBCUBES = MYSIZE - 1;
        public float spawnVal = 0f;
        float oldSpawnVal;
        float[,,] randCol = new float[MYSIZE, MYSIZE, MYSIZE];
        float[,] cubes = new float[NBCUBES,8];
        Vector3[,,] spawnPosition = new Vector3[MYSIZE, MYSIZE, MYSIZE];
        Color[,,] newCol = new Color[MYSIZE, MYSIZE, MYSIZE];
        GameObject[,,] tablePoint = new GameObject[MYSIZE, MYSIZE, MYSIZE];
        //Vector3 sizeOfSpace;

        float3[] cornerOffsets = new float3[8]{
            float3(0, 0, 1), // v0
            float3(1, 0, 1), // v1
            float3(1, 0, 0), // v2
            float3(0, 0, 0), // v3
            float3(0, 1, 1), // v4
            float3(1, 1, 1), // v5
            float3(1, 1, 0), // v6
            float3(0, 1, 0)  // v7
        };
        
        // Start is called before the first frame update
        void Start()
        {
            oldSpawnVal = spawnVal;
            //sizeOfSpace = new Vector3(20f,20f,20f);
            int i = 0, j = 0, k = 0;
            for( i = 0; i < MYSIZE; i++)
            {
                for( j = 0; j < MYSIZE; j++)
                {
                    for( k = 0; k < MYSIZE; k++)
                    {
                        randCol[i,j,k] = UnityEngine.Random.Range(0f, 1f);
                        newCol[i,j,k] = new Color(randCol[i,j,k], randCol[i,j,k], randCol[i,j,k]);
                        spawnPosition[i,j,k] = new Vector3(i*1f,j*1f,k*1f);
                        Spawn(tablePoint, spawnPosition[i,j,k], i, j, k, newCol[i,j,k]);
                        
                    }
                }
            }
            for( i = 0; i < MYSIZE; i++)
            {
                for( j = 0; j < MYSIZE; j++)
                {
                    for( k = 0; k < MYSIZE; k++)
                    {
                        for(int c = 0; c < NBCUBES; c++)
                        {
                            cubes[c,0] = randCol[i,j,k];
                            cubes[c,1] = randCol[i+1,j,k];
                            cubes[c,2] = randCol[i,j+1,k];
                            cubes[c,3] = randCol[i,j,k+1];
                            cubes[c,4] = randCol[i+1,j+1,k];
                            cubes[c,5] = randCol[i+1,j,k+1];
                            cubes[c,6] = randCol[i,j+1,k+1];
                            cubes[c,7] = randCol[i+1,j+1,k+1];
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(spawnVal < oldSpawnVal)
            {
                for(int i = 0; i < MYSIZE; i++)
                {
                    for(int j = 0; j < MYSIZE; j++)
                    {
                        for(int k = 0; k < MYSIZE; k++)
                        {
                            if(randCol[i,j,k] >= spawnVal)
                            {
                                tablePoint[i, j, k].GetComponent<Renderer>().enabled = true;
                            }
                        }
                    }
                }
            }
            else if(spawnVal > oldSpawnVal)
            {
                for(int i = 0; i < MYSIZE; i++)
                {
                    for(int j = 0; j < MYSIZE; j++)
                    {
                        for(int k = 0; k < MYSIZE; k++)
                        {
                            if(randCol[i,j,k] < spawnVal)
                            {
                                tablePoint[i, j, k].GetComponent<Renderer>().enabled = false;
                            }
                        }
                    }
                }
            }

            int cubeIndex = 0;
            if(cubes[0,0] < spawnVal) cubeIndex |= 1;
            if(cubes[0,1] < spawnVal) cubeIndex |= 2;
            if(cubes[0,2] < spawnVal) cubeIndex |= 4;
            if(cubes[0,3] < spawnVal) cubeIndex |= 8;
            if(cubes[0,4] < spawnVal) cubeIndex |= 16;
            if(cubes[0,5] < spawnVal) cubeIndex |= 32;
            if(cubes[0,6] < spawnVal) cubeIndex |= 64;
            if(cubes[0,7] < spawnVal) cubeIndex |= 128;
            
            int[] edges = new int[16];
            for(int i = 0; i < 16; i++)
            {
                edges[i] = MarchingConfig.TriTable[cubeIndex,i];
            }

            for(int i = 0; edges[i] != -1; i +=3)
            {
                int edge00 = MarchingConfig.EdgeCon[edges[i],0];
                int edge01 = MarchingConfig.EdgeCon[edges[i],1];

                int edge10 = MarchingConfig.EdgeCon[edges[i + 1],0];
                int edge11 = MarchingConfig.EdgeCon[edges[i + 1],1];

                int edge20 = MarchingConfig.EdgeCon[edges[i + 2],0];
                int edge21 = MarchingConfig.EdgeCon[edges[i + 2],1];

                Triangle tri;
                tri.a = interp(cornerOffsets[edge00],cubes[0,edge00],cornerOffsets[edge01],cubes[0,edge01]);
                tri.b = interp(cornerOffsets[edge10],cubes[0,edge10],cornerOffsets[edge11],cubes[0,edge11]);
                tri.c = interp(cornerOffsets[edge20],cubes[0,edge20],cornerOffsets[edge21],cubes[0,edge21]);
                triangles.Append(tri);
            }
            oldSpawnVal = spawnVal;

            
        }

        void Spawn(GameObject[,,] tablePoint, Vector3 spawnPosition, int i,int j,int k,Color newCol )
        {
            tablePoint[i, j, k] = Instantiate(points, spawnPosition, Quaternion.identity);
            tablePoint[i, j, k].GetComponent<Renderer>().material.color = newCol;
        }

        float3 interp(float3 edgeVertex1, float valueAtVertex1, float3 edgeVertex2, float valueAtVertex2)
        {
            return (edgeVertex1 + (spawnVal - valueAtVertex1) * (edgeVertex2 - edgeVertex1)  / (valueAtVertex2 - valueAtVertex1));
        }
    }
}