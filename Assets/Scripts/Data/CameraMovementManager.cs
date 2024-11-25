using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    private CameraBaseState currentState;

    private FirstPerson firstPerson = new();
    private ThirdPerson thirdPerson = new();

    public Airborne airborne;
    public Grounded grounded;

    public Transform player;
    public Transform playerY;

    public Transform camAnchor;
    public Transform cam;

    [Header("Airborne camera settings")]
    public float fly_camDistance = 5f;
    public float fly_camHeight = 2f;
    public float fly_followSpeed = 10f;

    [Header("Grounded camera settings")]
    public float ground_camDistance = 7f;
    public float ground_camHeight = 2f;
    public float ground_followSpeed = 100f;

    public LayerMask layer;

    private Func<bool> cameraSwitchInput;
    private Func<bool> controlInput;
    private Func<bool> c;

    private StateSwitch<PerspectiveBaseState> cameraSwitch;

    private void Start()
    {
        airborne = new Airborne();
        grounded = new Grounded();

        airborne.Init(this);
        grounded.Init(this);

        cameraSwitchInput = PlayerInput.Instance.CameraSwitchInput;
        controlInput = PlayerInput.Instance.ControlInput;
        c = PlayerInput.Instance.CInput;
        cameraSwitch = new StateSwitch<PerspectiveBaseState>(cameraSwitchInput, thirdPerson, firstPerson);

        currentState = grounded;
    }

    void Update()
    {
        if (controlInput() && c())
            return;
        
        cameraSwitch.CanSwitch();
        cameraSwitch.currentState.UpdateState(this);
    } 

    public void SwitchState(CameraBaseState state)
    {
        currentState = state;
    }

    public void AnchorPosition(float distance)
    {
        camAnchor.position = player.position - player.forward * distance;
    }

    public void Position()
    {
        cam.position = player.position;
    }

    public void Position(float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, -player.forward, out hit, distance, layer))
        {
            cam.position = camAnchor.position + camAnchor.forward * (distance - hit.distance + .1f);
            return;
        }
        cam.position = camAnchor.position;
    }


    public abstract class CameraBaseState
    {
        public abstract void Init(CameraMovementManager cam);
        public abstract void Move(CameraMovementManager cam);
    }

    public class Airborne : CameraBaseState
    {
        public Transform player;
        public Transform camera;

        public float camDistance;
        public float camHeight;
        public float followSpeed;

        public override void Init(CameraMovementManager cam)
        {
            player = cam.player;
            camera = cam.camAnchor;

            camDistance = cam.fly_camDistance;
            camHeight = cam.fly_camHeight;
            followSpeed = cam.fly_followSpeed;
        }

        public override void Move(CameraMovementManager cam)
        {
            Vector3 targetPosition = player.position + Vector3.up * camHeight - player.forward * camDistance;
            camera.position = Vector3.Lerp(camera.position, targetPosition, followSpeed * Time.deltaTime);
            camera.rotation = Quaternion.Lerp(camera.rotation, player.rotation, followSpeed * Time.deltaTime);

            cam.AnchorPosition(camDistance);
            cam.Position(camDistance);
        }
    }

    public class Grounded : CameraBaseState
    {
        public Transform player;
        public Transform camera;

        public float camDistance;
        public float camHeight;
        public float followSpeed;

        public override void Init(CameraMovementManager cam)
        {
            player = cam.player;
            camera = cam.camAnchor;

            camDistance = cam.ground_camDistance;
            camHeight = cam.ground_camHeight;
            followSpeed = cam.ground_followSpeed;
        }

        public override void Move(CameraMovementManager cam)
        {
            Vector3 targetPosition = player.position + player.up * camHeight - player.forward * camDistance;
            camera.position = Vector3.Lerp(camera.position, targetPosition, followSpeed * Time.deltaTime);
            camera.rotation = Quaternion.Lerp(camera.rotation, player.rotation, followSpeed * Time.deltaTime);

            cam.AnchorPosition(camDistance);
            cam.Position(camDistance);
        }
    }

    public abstract class PerspectiveBaseState
    {
        public abstract void UpdateState(CameraMovementManager cam);
    }

    public class FirstPerson : PerspectiveBaseState
    {
        public override void UpdateState(CameraMovementManager cam)
        {
            cam.playerY.eulerAngles = new Vector3(0f, cam.player.eulerAngles.y, 0f);
            cam.cam.rotation = cam.player.rotation;
            cam.Position();
        }
    }

    public class ThirdPerson : PerspectiveBaseState
    {
        public override void UpdateState(CameraMovementManager cam)
        {
            cam.playerY.eulerAngles = new Vector3(0f, cam.player.eulerAngles.y, 0f);
            cam.cam.rotation = cam.player.rotation;
            cam.currentState.Move(cam);
        }
    }
}
