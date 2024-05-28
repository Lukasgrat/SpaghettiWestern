using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

public float moveSpeed = 10;
    public float jumpHeight = 10;
    public float gravity = 9.81f;
    public float airControl = 10;
    public GameObject weaponPrefab;

    CharacterController controller;
    Vector3 input;
    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        weaponPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;

        input *= moveSpeed;

        if(controller.isGrounded)
        {
            moveDirection = input;

            //we can jump
            if(Input.GetButton("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
            }
            else 
            {
                moveDirection.y = 0.0f;
            }
        }
        else 
        {
            ///we are midair
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        //toggles and untoggles gun
        if(Input.GetKeyDown(KeyCode.Q) && weaponPrefab.activeSelf)
        {
            weaponPrefab.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            weaponPrefab.SetActive(true);
        }
    }
    
}
