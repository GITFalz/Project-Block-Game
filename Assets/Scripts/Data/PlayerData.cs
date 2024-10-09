using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField]
    public List<Data> data = new List<Data>();

    [SerializeField]
    public int index;

    [System.Serializable]
    public class Data
    {
        [SerializeField]
        public Movement movement;

        [SerializeField]
        public Animation animation;

        [System.Serializable]
        public class Movement
        {
            public float walkSpeed;
            public float runSpeed;
            public float sprintSpeed;
            public float dashSpeed;
            public float jumpHeight;
        }

        [System.Serializable]
        public class Animation
        {
            public List<Animation> animations;
        }
    }

    public float GetWalkSpeed()
    {
        return data.ElementAt(index).movement.walkSpeed;
    }

    public float GetRunSpeed()
    {
        return data.ElementAt(index).movement.runSpeed;
    }

    public float GetSprintSpeed()
    {
        return data.ElementAt(index).movement.sprintSpeed;
    }

    public float GetDashSpeed()
    {
        return data.ElementAt(index).movement.dashSpeed;
    }

    public float GetJumpHeight()
    {
        return data.ElementAt(index).movement.jumpHeight;
    }
}
