using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class GravityPlayer : MonoBehaviour
{
    Gravity planet;
    Rigidbody rigidbody1;

    public float speed = 10;

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

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal")*-1, Input.GetAxisRaw("Jump"), Input.GetAxisRaw("Vertical")*-1);
        Vector3 direction = input.normalized;
        Vector3 velocity = transform.rotation * direction * speed;
        Vector3 moveAmount = velocity * Time.deltaTime;

        transform.position += moveAmount;
    }
}
