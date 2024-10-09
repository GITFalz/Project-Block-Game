using UnityEngine;

public class RigidbodyManager : MonoBehaviour
{
    public Transform player;
    
    public LayerMask groundLayer;

    public Vector3 player_Collider_Center;
    public Vector3 player_Half_Collider_Size;

    public GroundCollider groundCollider;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FreeMove(Vector3 direction, float speed)
    {
        FreeMove(rb, direction, speed);
    }

    public void FreeMove(Rigidbody rb, Vector3 direction, float speed)
    {
        Vector3 force = direction * speed - rb.velocity;
        Move(rb, force);
    }


    public void FlatMove(Vector3 direction, float speed)
    {
        FlatMove(rb, direction, speed);
    }

    public void FlatMove(Rigidbody rb, Vector3 direction, float speed)
    {       
        Vector3 force = direction * speed - GetHorizontalVelocity(rb);
        Move(rb, force);
    }


    public void FloatingCapsule(float pressure)
    {
        FloatingCapsule(rb, player, pressure);
    }

    public void FloatingCapsule(Rigidbody rb, Transform transform, float pressure)
    {
        float distance = GetGroundDistance(transform.position, 1.5f, groundLayer);
        if (distance != 0f)
        {
            float force = distance * pressure - rb.velocity.y;
            Move(rb, Vector3.up * force);
        }
    }

    public void Set(Vector3 direction)
    {
        Vector3 velocity = Vector3.Min(direction, rb.velocity);
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }


    public void Gravity(float gravity)
    {
        Gravity(rb, player, gravity);
    }

    public void Gravity(Rigidbody rb, Transform transform, float gravity)
    {
        Vector3 force = 9.81f * Time.deltaTime * gravity * -transform.transform.up;
        Move(rb, force);
    }


    public bool isGrounded(float distance)
    {
        return groundCollider.isGrounded;
    }

    public bool isGrounded(Transform transform, float distance)
    {
        return CheckGrounded(transform, distance) || rb.velocity.y == 0;

        //return GetRayDistance(new Ray(transform.position, Vector3.down), distance, manager.groundLayer) != 0f;
    }

    public bool isFalling(float distance)
    {
        return isFalling(player, distance);
    }

    public bool isFalling(Transform transform, float distance)
    {
        return !groundCollider.isGrounded || rb.velocity.y < -.1f;
    }

    public bool isRayGrounded(Transform transform, float distance)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(RayData.box_Collider_Ray_Positions[i] + transform.position, Vector3.down, out RaycastHit hit, distance, groundLayer))
            {
                if (hit.normal.Equals(Vector3.up))
                {
                    return true;
                }
            }
        }
        return false;
    }



    //Private

    //Active
    private void Move(Rigidbody rb, Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }


    //Getters
    private Vector3 GetHorizontalVelocity(Rigidbody rb)
    {
        return new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    private bool CheckGrounded(Transform transform, float distance)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + player_Collider_Center, player_Half_Collider_Size, Quaternion.identity, groundLayer);

        return colliders.Length > 0 && isRayGrounded(transform, distance);
    }

    private float GetGroundDistance(Vector3 position, float range, LayerMask layer)
    {
        Ray ray = new Ray(position, Vector3.down);
        float distance = GetRayDistance(ray, range, layer);
        if (distance != 0f)
        {
            return -distance + 1f;
        }
        return 0f;
    }

    private float GetRayDistance(Ray ray, float range, LayerMask layer)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, range, layer))
        {
            return hit.distance;
        }
        return 0f;
    }
}
