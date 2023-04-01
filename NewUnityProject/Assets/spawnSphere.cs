using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace name1
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class spawnSphere : MonoBehaviour
    {
        struct Triangle {
            public Vector3 a, b, c;
        };

        struct Point {
            public float randCol;
            public Vector3 spawnPosition;
            public Color newCol;
            public GameObject pointObject;
        };

        public GameObject points;
        public const int MYSIZE = 10;
        public const int NBCUBES = (MYSIZE - 1)*(MYSIZE - 1)*(MYSIZE - 1);

        public float spawnVal = 0f;
        float oldSpawnVal;

        Vector3[] cornerOffsets = {
            new Vector3(0, 0, 1), // v0
            new Vector3(1, 0, 1), // v1
            new Vector3(1, 0, 0), // v2
            new Vector3(0, 0, 0), // v3
            new Vector3(0, 1, 1), // v4
            new Vector3(1, 1, 1), // v5
            new Vector3(1, 1, 0), // v6
            new Vector3(0, 1, 0)  // v7
        };

        Point[,,] ptn = new Point[MYSIZE, MYSIZE, MYSIZE];
        Point[,] cubesCorners = new Point[NBCUBES,8];
        
        //list of vertices
        List<Vector3> middlePoints = new List<Vector3>();

        List<int> triangles = new List<int>();

        public MeshFilter MeshFilter;
        Mesh mesh;
        
        // Start is called before the first frame update
        float RandCol(Vector3 center, Vector3 point, float space, float centerCord)
        {
            float rayon = 9*space;
            float randCol;
            //float dist = Vector3.Distance(point, center)*1f/1000;
            float dist = Vector3.Distance(point, new Vector3(0,0,0));
            if(Math.Abs(dist) > Math.Abs(rayon-centerCord))
            {
                randCol = 0f;
            }
            else
            {
                randCol = 1f;
            }
            //float randCol = Random.Range(dist, 1f);
            if(point.x == 0-centerCord || point.x == (MYSIZE-1)*space-centerCord || point.y == 0-centerCord || point.y == (MYSIZE-1)*space-centerCord || point.z == 0-centerCord || point.z == (MYSIZE-1)*space-centerCord)
            {
                randCol = 0f;
            }
            return randCol;
        }
        void Start()
        {
            mesh = new Mesh();

            float space = 0.5f;
            Vector3 center = new Vector3((MYSIZE-1)*space/2,(MYSIZE-1)*space/2,(MYSIZE-1)*space/2);
            float centerCord = (MYSIZE-1)*space/2;
            oldSpawnVal = spawnVal;

            int i = 0, j = 0, k = 0;
            for( i = 0; i < MYSIZE; i++)
            {
                for( j = 0; j < MYSIZE; j++)
                {
                    for( k = 0; k < MYSIZE; k++)
                    {
                        ptn[i,j,k].spawnPosition = new Vector3(i*space-centerCord,j*space-centerCord,k*space-centerCord);
                        ptn[i,j,k].randCol = RandCol(center, ptn[i,j,k].spawnPosition, space, centerCord);
                        ptn[i,j,k].newCol = new Color(ptn[i,j,k].randCol, ptn[i,j,k].randCol, ptn[i,j,k].randCol);
                    }
                }
            }
            
            //Ajoute les points à des carrés c
            int nc = 0;
            for( i = 0; i < MYSIZE-1; i++)
            {
                for( j = 0; j < MYSIZE-1; j++)
                {
                    for( k = 0; k < MYSIZE-1; k++)
                    {
                        cubesCorners[nc,0] = ptn[i,j,k+1];     //0 0 1
                        cubesCorners[nc,1] = ptn[i+1,j,k+1];   //1 0 1
                        cubesCorners[nc,2] = ptn[i+1,j,k];     //1 0 0
                        cubesCorners[nc,3] = ptn[i,j,k];       //0 0 0
                        cubesCorners[nc,4] = ptn[i,j+1,k+1];   //0 1 1
                        cubesCorners[nc,5] = ptn[i+1,j+1,k+1]; //1 1 1
                        cubesCorners[nc,6] = ptn[i+1,j+1,k];   //1 1 0
                        cubesCorners[nc,7] = ptn[i,j+1,k];     //0 1 0
                        nc+=1;
                    }
                }
            }

            for(int c = 0; c < NBCUBES; c++)
            {
                MarchingCubes(c);
            }
        }

        // Update is called once per frame
        //void Update()
        //{
        //    
        //    
        //}

        void MarchingCubes(int c)
        {

            //find index for triangulation
            int cubeIndex = 0;
            if(cubesCorners[c,0].randCol < spawnVal) cubeIndex |= 1;
            if(cubesCorners[c,1].randCol < spawnVal) cubeIndex |= 2;
            if(cubesCorners[c,2].randCol < spawnVal) cubeIndex |= 4;
            if(cubesCorners[c,3].randCol < spawnVal) cubeIndex |= 8;
            if(cubesCorners[c,4].randCol < spawnVal) cubeIndex |= 16;
            if(cubesCorners[c,5].randCol < spawnVal) cubeIndex |= 32;
            if(cubesCorners[c,6].randCol < spawnVal) cubeIndex |= 64;
            if(cubesCorners[c,7].randCol < spawnVal) cubeIndex |= 128;

            List<int> triangulation = new List<int>();
            for(int i = 0; MarchingConfig.TriTable[cubeIndex,i] != -1; i++)
            {
                triangulation.Add(MarchingConfig.TriTable[cubeIndex,i]);
            }

            
            //find center of segments of cubes where points are différents
            for(int edgeIndex = 0; edgeIndex < triangulation.Count ;edgeIndex++)
            {
                int indexA = MarchingConfig.EdgeCon[triangulation[edgeIndex],0];
                int indexB = MarchingConfig.EdgeCon[triangulation[edgeIndex],1];

                Vector3 middlePos = (cubesCorners[c,indexA].spawnPosition + cubesCorners[c,indexB].spawnPosition) / 2;
                middlePoints.Add(middlePos);
                
            }
            Vector3[] middlePointToTable = new Vector3[middlePoints.Count];
            
            for(int i = 0; i < middlePointToTable.GetLength(0); i++)
            {
                middlePointToTable[i] = middlePoints[i];
                triangles.Add(i);
            }

            ////add these two triangles to the mesh

            int[] trianglesToTable = new int[triangles.Count];

            for(int i = 0; i < trianglesToTable.GetLength(0); i++)
            {
                trianglesToTable[i] = triangles[i];
            }

            mesh.SetVertices(middlePointToTable);
            mesh.SetTriangles(trianglesToTable, 0);
            mesh.RecalculateNormals();
            MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            meshc.sharedMesh = mesh;
            MeshFilter.mesh = mesh;
        }
    }
}