using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnSphere : MonoBehaviour
{
    public GameObject points;
    GameObject[,,] tablePoint = new GameObject[20, 20, 20];
    Vector3 sizeOfSpace;
    
    // Start is called before the first frame update
    void Start()
    {
        sizeOfSpace = new Vector3(20f,20f,20f);
        for(int i = 0; i < 20; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                for(int k = 0; k < 20; k++)
                {
                    float randCol = Random.Range(0f, 1f);
                    Color newCol = new Color(randCol, randCol, randCol);
                    Vector3 spawnPosition = new Vector3(i*1f,j*1f,k*1f);
                    tablePoint[i, j, k] = Instantiate(points, spawnPosition, Quaternion.identity);
                    tablePoint[i, j, k].GetComponent<Renderer>().material.color = newCol;
                }
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
