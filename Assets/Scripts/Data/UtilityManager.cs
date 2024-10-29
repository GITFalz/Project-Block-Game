using UnityEngine;
using UnityEngine.Windows;

public class UtilityManager : MonoBehaviour
{
    public static UtilityManager Instance;

    public Transform player;
    public Rigidbody rb;
    public CapsuleCollider cc;

    public LayerMask groundLayer;

    public Vector3 player_Half_Collider_Size = new Vector3(.4f, 1.5f, .4f);

    public Vector3 player_Collider_Center = new Vector3(0, -1, 0);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        cc = player.GetComponent<CapsuleCollider>();
    }



    public void SetDrag(float value)
    {
        rb.drag = value;
    }   
}

public static class RayData
{
    public static readonly Vector3[] box_Collider_Ray_Positions = new Vector3[25]
    {
        new Vector3(  1f,  0f,  1f),
        new Vector3( .5f,  0f,  1f),
        new Vector3(  0f,  0f,  1f),
        new Vector3(-.5f,  0f,  1f),
        new Vector3( -1f,  0f,  1f),
        new Vector3(  1f,  0f, .5f),
        new Vector3( .5f,  0f, .5f),
        new Vector3(  0f,  0f, .5f),
        new Vector3(-.5f,  0f, .5f),
        new Vector3( -1f,  0f, .5f),
        new Vector3(  1f,  0f,  0f),
        new Vector3( .5f,  0f,  0f),
        new Vector3(  0f,  0f,  0f),
        new Vector3(-.5f,  0f,  0f),
        new Vector3( -1f,  0f,  0f),
        new Vector3(  1f,  0f,-.5f),
        new Vector3( .5f,  0f,-.5f),
        new Vector3(  0f,  0f,-.5f),
        new Vector3(-.5f,  0f,-.5f),
        new Vector3( -1f,  0f,-.5f),
        new Vector3(  1f,  0f, -1f),
        new Vector3( .5f,  0f, -1f),
        new Vector3(  0f,  0f, -1f),
        new Vector3(-.5f,  0f, -1f),
        new Vector3( -1f,  0f, -1f),
    };
}
