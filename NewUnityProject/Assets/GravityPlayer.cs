using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class GravityPlayer : MonoBehaviour
{
    Gravity planet;
    Rigidbody rigidbody;

    void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Gravity>();
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        planet.GravityPlanet(rigidbody);
    }
}
