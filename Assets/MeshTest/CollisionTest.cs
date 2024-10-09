using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    void Start()
    {
        /**
        Vector3 pin = new Vector3(2.3f, -2.7f, 1.6f);
        Vector3 pout = new Vector3(5, 7, -3.2f);
        Vector3 block = new Vector3(2, -3, 1.5f);

        BlockCollider bc = CollisionLibrary.blockColliders[44];
        
        bool isIn = bc.IsIn(pin, block);
        
        Vector3 intersection = bc.GetIntersection(pin, pout, block);
        /**
        Vector3 direction = bc.GetDirection(pin, pout, block, intersection);
        float distance = bc.GetDistance(pin, pout, block, intersection);
        
        Debug.Log(intersection + " " + isIn + " " + direction + " " + distance);
        */
    }
}
