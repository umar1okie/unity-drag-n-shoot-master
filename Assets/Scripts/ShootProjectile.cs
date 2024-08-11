using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private float speed;
    public void FireProjectile()
    {

        // Instantiate the projectile at the cannon hole's position
        GameObject projectile = Instantiate(point, null);
        projectile.transform.position = this.transform.position;

        // Apply force to launch the projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddRelativeForce(-this.transform.right * speed);
    }
}
