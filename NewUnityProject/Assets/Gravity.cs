using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float g = -9.8f;


    public void GravityPlanet(Rigidbody player)
    {
        Vector3 p = (player.position - transform.position).normalized;
        Vector3 local = player.transform.up;

        player.AddForce(p * g);
        player.rotation = Quaternion.FromToRotation(local, p) * player.rotation;
    }
}
