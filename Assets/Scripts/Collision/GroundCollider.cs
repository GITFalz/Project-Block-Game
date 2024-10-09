using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    public bool isGrounded = false;
    public Vector3 lastNormal;
    
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > .9f)
            {
                isGrounded = true;
                lastNormal = contact.normal;
                break;
            }
        }
    }

    private void OnCollisionStay(Collision collision) 
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > .9f)
            {
                isGrounded = true;
                lastNormal = contact.normal;
                break;
            }
            else
            {
                isGrounded = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (Vector3.Dot(lastNormal, Vector3.up) > .9f)
            isGrounded = false;
    }
}
