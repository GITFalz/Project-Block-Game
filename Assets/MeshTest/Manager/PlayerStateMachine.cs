using System;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerBaseState currentState;
    
    public PlayerFallingState playerFallingState;
    
    public World worldScript;

    public LayerMask groundLayer;

    [Range(0.1f, 4f)]
    public float gravityMultiplier = 2f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        playerFallingState = new();
        
        currentState = playerFallingState;
        
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(PlayerBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void ApplyGravity()
    {
        velocity.y -= gravityMultiplier * Time.deltaTime;

        bool doY = false;
        float yDistance = -1f;

        foreach (Vector3 pos in bottomCollisionPoints)
        {
            Vector3 futurePosition = transform.position + velocity + pos;
            
            Vector3Int blockPos = Mathp.floor(futurePosition);

            Vector3Int chunkPosition = new Vector3Int(
                blockPos.x >= 0 ? blockPos.x & ~31 : -((Mathp.abs(blockPos.x) + 32) & ~31),
                blockPos.y >= 0 ? blockPos.y & ~31 : -((Mathp.abs(blockPos.y) + 32) & ~31),
                blockPos.z >= 0 ? blockPos.z & ~31 : -((Mathp.abs(blockPos.z) + 32) & ~31)
            );

            ChunkData chunkData = worldScript.GetChunk(chunkPosition);

            if (chunkData != null)
            {
                int index = blockPos.x + blockPos.y * 32 + blockPos.z * 1024;

                if (chunkData.blocks[index] != null)
                {
                    BlockCollider bs = CollisionLibrary.blockColliders[44];
            
                    /**
                    if (bs.IsIn(futurePosition, blockPos))
                    {
                        if (Physics.Raycast(transform.position + pos, Vector3.up * velocity.y, out RaycastHit hit, groundLayer))
                        {
                            if (!doY)
                            {
                                yDistance = hit.distance;
                                doY = true;
                            }
                            else if (yDistance > hit.distance)
                            {
                                yDistance = hit.distance;
                            }
                        }
                    }
                    */
                }
            }
        }
        
        Debug.Log(velocity);

        if (doY)
        {
            velocity.y += yDistance;
        }

        transform.position += velocity;
    }

    public static Vector3[] bottomCollisionPoints = new Vector3[]
    {
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, 0.5f),
    };
}