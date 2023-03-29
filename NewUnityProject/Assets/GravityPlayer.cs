using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class GravityPlayer : MonoBehaviour
{
    Gravity planet;
    Rigidbody rigidbody1;

    void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Gravity>();
        rigidbody1 = GetComponent<Rigidbody>();

        rigidbody1.useGravity = false;
        rigidbody1.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        planet.GravityPlanet(rigidbody1);
    }
}
