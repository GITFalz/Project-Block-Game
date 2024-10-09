using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log("hello");
            if (contact.normal.Equals(Vector3.up) || contact.normal.Equals(Vector3.down))
                Physics.IgnoreCollision(collision.collider, GetComponent<BoxCollider>());
        }
    }
}
