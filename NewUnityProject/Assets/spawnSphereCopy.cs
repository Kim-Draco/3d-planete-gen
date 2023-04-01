using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace name1
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class spawnSphereCopy : MonoBehaviour
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
        //float[,,] randCol = new float[MYSIZE, MYSIZE, MYSIZE];
        //float[,] cubes = new float[NBCUBES,8];
        //Vector3[,,] spawnPosition = new Vector3[MYSIZE, MYSIZE, MYSIZE];
        //Color[,,] newCol = new Color[MYSIZE, MYSIZE, MYSIZE];
        //GameObject[,,] tablePoint = new GameObject[MYSIZE, MYSIZE, MYSIZE];
        //Vector3 sizeOfSpace;

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
        //MeshRenderer Matrenderer;

        //public Material mat;

        //void Awake(){
        //    mesh = GetComponent<MeshFilter>().mesh;
        //    
        //}
        
        // Start is called before the first frame update
        float RandCol(Vector3 center, Vector3 point, float space)
        {
            float dist = Vector3.Distance(point, center)*1f/1000;
            float randCol = Random.Range(dist, 1f);
            if(point.x == 0 || point.x == (MYSIZE-1)*space || point.y == 0 || point.y == (MYSIZE-1)*space || point.z == 0 || point.z == (MYSIZE-1)*space)
            {
                randCol = 0f;
            }
            return randCol;
        }
        void Start()
        {
            mesh = new Mesh();
            //Matrenderer = GetComponent<MeshRenderer>();
            //Matrenderer.material = mat;
            float space = 1f;
            Vector3 center = new Vector3((MYSIZE-1)*space/2,(MYSIZE-1)*space/2,(MYSIZE-1)*space/2);
            oldSpawnVal = spawnVal;
            //sizeOfSpace = new Vector3(20f,20f,20f);
            int i = 0, j = 0, k = 0;
            for( i = 0; i < MYSIZE; i++)
            {
                for( j = 0; j < MYSIZE; j++)
                {
                    for( k = 0; k < MYSIZE; k++)
                    {
                        ptn[i,j,k].spawnPosition = new Vector3(i*space,j*space,k*space);
                        ptn[i,j,k].randCol = RandCol(center, ptn[i,j,k].spawnPosition, space);
                        //ptn[i,j,k].randCol = Random.Range(0f, 1f);
                        ptn[i,j,k].newCol = new Color(ptn[i,j,k].randCol, ptn[i,j,k].randCol, ptn[i,j,k].randCol);
                        ptn[i, j, k].pointObject = Instantiate(points, ptn[i,j,k].spawnPosition, Quaternion.identity);
                        ptn[i, j, k].pointObject.GetComponent<Renderer>().material.color = ptn[i,j,k].newCol;
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
                        //for(int c = 0; c < NBCUBES; c++)
                        //{
                        //    cubes[c,0] = ptn[i,j,k].randCol;
                        //    cubes[c,1] = ptn[i+1,j,k].randCol;
                        //    cubes[c,2] = ptn[i,j+1,k].randCol;
                        //    cubes[c,3] = ptn[i,j,k+1].randCol;
                        //    cubes[c,4] = ptn[i+1,j+1,k].randCol;
                        //    cubes[c,5] = ptn[i+1,j,k+1].randCol;
                        //    cubes[c,6] = ptn[i,j+1,k+1].randCol;
                        //    cubes[c,7] = ptn[i+1,j+1,k+1].randCol;
                        //}
                    
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
            //MarchingCubes(0);
            for(int c = 0; c < NBCUBES; c++)
            {
                //print("cube " + c + " :\n");
                //for(int c2 = 0; c2< 7; c2++)
                //{
                //    print("- " + cubesCorners[c,c2].spawnPosition + "\n");
                //}
                //print("\n");
                MarchingCubes(c);
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if(spawnVal < oldSpawnVal)
            //{
            //    for(int i = 0; i < MYSIZE; i++)
            //    {
            //        for(int j = 0; j < MYSIZE; j++)
            //        {
            //            for(int k = 0; k < MYSIZE; k++)
            //            {
            //                if(ptn[i,j,k].randCol >= spawnVal)
            //                {
            //                    ptn[i,j,k].pointObject.GetComponent<Renderer>().enabled = true;
            //                }
            //            }
            //        }
            //    }
            //}
            //else if(spawnVal > oldSpawnVal)
            //{
            //    for(int i = 0; i < MYSIZE; i++)
            //    {
            //        for(int j = 0; j < MYSIZE; j++)
            //        {
            //            for(int k = 0; k < MYSIZE; k++)
            //            {
            //                if(ptn[i,j,k].randCol < spawnVal)
            //                {
            //                    ptn[i,j,k].pointObject.GetComponent<Renderer>().enabled = false;
            //                }
            //            }
            //        }
            //    }
            //}
            
            

            //oldSpawnVal = spawnVal;
            
        }

        Vector3 interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2)
        {
            return (edgeVertex1 + (spawnVal - valueAtVertex1) * (edgeVertex2 - edgeVertex1)  / (valueAtVertex2 - valueAtVertex1));
        }

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
            
            //print("CubeIndex : " + cubeIndex);

            List<int> triangulation = new List<int>();
            for(int i = 0; MarchingConfig.TriTable[cubeIndex,i] != -1; i++)
            {
                triangulation.Add(MarchingConfig.TriTable[cubeIndex,i]);
                //print("triangulation : " + triangulation[i]);
            }

            
            //find center of segments of cubes where points are différents
            for(int edgeIndex = 0; edgeIndex < triangulation.Count ;edgeIndex++)
            {
                int indexA = MarchingConfig.EdgeCon[triangulation[edgeIndex],0];
                int indexB = MarchingConfig.EdgeCon[triangulation[edgeIndex],1];

                Vector3 middlePos = (cubesCorners[c,indexA].spawnPosition + cubesCorners[c,indexB].spawnPosition) / 2;
                //Vector3 middlePos = new Vector3((cubesCorners[indexA].spawnPosition.x + cubesCorners[indexB].spawnPosition.x) / 2.0f,
                //                                (cubesCorners[indexA].spawnPosition.y + cubesCorners[indexB].spawnPosition.y) / 2.0f,
                //                                (cubesCorners[indexA].spawnPosition.z + cubesCorners[indexB].spawnPosition.z) / 2.0f);
                //print("middlePos : " + middlePos + " cubesCorners[indexA] : " + cubesCorners[indexA].spawnPosition + " cubesCorners[indexB] : " + cubesCorners[indexB].spawnPosition);
                middlePoints.Add(middlePos);
                
            }

            //for(int i = 0; triangulation[i] != -1; i +=3)
            //{
            //    int edge00 = MarchingConfig.EdgeCon[triangulation[i],0];
            //    int edge01 = MarchingConfig.EdgeCon[triangulation[i],1];
//
            //    int edge10 = MarchingConfig.EdgeCon[triangulation[i + 1],0];
            //    int edge11 = MarchingConfig.EdgeCon[triangulation[i + 1],1];
//
            //    int edge20 = MarchingConfig.EdgeCon[triangulation[i + 2],0];
            //    int edge21 = MarchingConfig.EdgeCon[triangulation[i + 2],1];
//
            //    Triangle tri;
            //    tri.a = interp(cornerOffsets[edge00],cubesCorners[0,0,0].corners[edge00].randCol,cornerOffsets[edge01],cubesCorners[0,0,0].corners[edge01].randCol);
            //    tri.b = interp(cornerOffsets[edge10],cubesCorners[0,0,0].corners[edge10].randCol,cornerOffsets[edge11],cubesCorners[0,0,0].corners[edge11].randCol);
            //    tri.c = interp(cornerOffsets[edge20],cubesCorners[0,0,0].corners[edge20].randCol,cornerOffsets[edge21],cubesCorners[0,0,0].corners[edge21].randCol);
            //    triangles.Append(tri);
            //}

            //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
            //triangles[0] = 0;
            //triangles[1] = 1;
            //triangles[2] = 2;
//
            Vector3[] middlePointToTable = new Vector3[middlePoints.Count];
            
            for(int i = 0; i < middlePointToTable.GetLength(0); i++)
            {
                middlePointToTable[i] = middlePoints[i];
                //print(middlePoints[i]);
                triangles.Add(i);
                //if(i!= 0 && (i+1) % 3 == 0)
                //{
                //    triangles.Add(i-2);
                //    triangles.Add(i);
                //    triangles.Add(i-1);
                //}
            }
            ////add these two triangles to the mesh
            print("Count : " + triangles.Count);

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