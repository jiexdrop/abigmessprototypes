﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideViewController : MonoBehaviour
{

    float speed = 10f;
    float rotationSpeed = 10f;

    public Transform cubePostion;
    public GameObject holdCube;
    public Transform saveCubeTransformParent;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    enum Direction
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        if (translation != 0)
        {
            // Move translation along the object's z-axis
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + translation);
        }
        if (rotation != 0)
        {
            // Rotate around our y-axis
            transform.position = new Vector3(rotation + transform.position.x, transform.position.y, transform.position.z);
        }

        if (translation > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0,0);
        } else if (translation < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (rotation > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (rotation < 0)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }

        Debug.DrawRay(transform.position, transform.forward, Color.black);

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("isJumping");
        }

        if(translation != 0 || rotation != 0)
        {
            animator.SetBool("isRunning", true);
        } else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, layerMask))
            {
                saveCubeTransformParent = hit.collider.gameObject.transform.parent;
                hit.collider.gameObject.transform.parent = cubePostion;
                holdCube = hit.collider.gameObject;
                holdCube.GetComponent<Rigidbody>().isKinematic = true;
            }

        }

        if (holdCube != null)
        {
            holdCube.transform.position = Vector3.Lerp(holdCube.transform.position, cubePostion.position, Time.deltaTime * speed);
            holdCube.transform.rotation = Quaternion.Slerp(holdCube.transform.rotation, cubePostion.rotation, Time.deltaTime * speed);
        }

        if (holdCube != null && Input.GetMouseButtonUp(0))
        {
            holdCube.transform.parent = saveCubeTransformParent;
            holdCube.GetComponent<Rigidbody>().isKinematic = false;
            holdCube = null;
            
        }
    }


}
