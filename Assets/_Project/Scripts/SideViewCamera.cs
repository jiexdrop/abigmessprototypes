using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideViewCamera : MonoBehaviour
{
    [System.Serializable]
    public class PositionSettings
    {
        // distance from our target
        // how far in the sky the camera needs to be ?
        // bools for zooming ad smoothfollowing
        // min and max zoom settings
        public float distanceFromTarget = -10;
        public bool allowZoom = true;
        public float zoomSmooth = 100;
        public float zoomStep = 5;
        public float maxZoom = -5;
        public float minZoom = -15;
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -10;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        // holding our current x and y rotation for our camera
        // bool for allowing orbit
        public float xRotation = -65;
        public float yRotation = -180;
        public bool allowOrbit = true;
        public float yOrbitSmooth = 0.5f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string MOUSE_ORBIT = "MouseOrbit";
        public string ZOOM = "Mouse ScrollWheel";
    }

    public PositionSettings positionSettings = new PositionSettings();
    public OrbitSettings orbitSettings = new OrbitSettings();
    public InputSettings inputSettings = new InputSettings();

    Vector3 destination = Vector3.zero;
    Vector3 camVelocity = Vector3.zero;
    Vector3 currentMousePosition = Vector3.zero;
    Vector3 previousMousePosition = Vector3.zero;
    float mouseOrbitInput, zoomInput;

    public Transform target;

    void Start()
    {
        //setting camera target
        SetCameraTarget(target);

        if (target)
        {
            MoveToTarget();
        }
    }

    public void SetCameraTarget(Transform t)
    {
        // if we want to set a new target at runtime
        target = t;

        if(target == null)
        {
            Debug.LogError("Camera needs target");
        }

    }

    void GetInput()
    {
        // filling the values for our input variables
        mouseOrbitInput = Input.GetAxisRaw(inputSettings.MOUSE_ORBIT);
        zoomInput = Input.GetAxisRaw(inputSettings.ZOOM);
    }

    void Update()
    {
        // getting input
        // zooming
        GetInput();
        if (positionSettings.allowZoom)
        {
            ZoomInOnTarget();
        }
    }

    void FixedUpdate()
    {
        // movetotarget
        // lookattarget
        // orbit
        if (target)
        {
            MoveToTarget();
            LookAtTarget();
            MouseOrbitTarget();
        }
    }

    void MoveToTarget()
    {
        // hadling getting our camera to its destination position
        destination = target.position;
        destination += Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation, 0) * -Vector3.forward * positionSettings.distanceFromTarget;

        if (positionSettings.smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVelocity, positionSettings.smooth);
        } else
        {
            transform.position = destination;
        }
    }

    void LookAtTarget()
    {
        // handling getting our camera to look at the target at all times
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = targetRotation;

    }

    void MouseOrbitTarget()
    {
        // geting the camera t orbit around our character
        previousMousePosition = currentMousePosition;
        currentMousePosition = Input.mousePosition;

        if(mouseOrbitInput > 0)
        {
            orbitSettings.yRotation += (currentMousePosition.x - previousMousePosition.x) * orbitSettings.yOrbitSmooth;
        }
    }

    void ZoomInOnTarget()
    {
        // modifying the distancefromtarget to be closer or further away from our target
        positionSettings.newDistance += positionSettings.zoomStep * zoomInput;

        positionSettings.distanceFromTarget = Mathf.Lerp(positionSettings.distanceFromTarget, positionSettings.newDistance, positionSettings.zoomSmooth * Time.deltaTime);

        if(positionSettings.distanceFromTarget > positionSettings.maxZoom)
        {
            positionSettings.distanceFromTarget = positionSettings.maxZoom;
        }

        if (positionSettings.distanceFromTarget < positionSettings.minZoom)
        {
            positionSettings.distanceFromTarget = positionSettings.minZoom;
        }
    }
}
