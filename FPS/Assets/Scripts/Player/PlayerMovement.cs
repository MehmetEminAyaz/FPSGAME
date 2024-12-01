using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Movement : MonoBehaviour
{
    [Header("Player Movement")]
    public float speed = 5f;
    public float jumpForce = 2f;
    private CharacterController characterController;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    private bool isGrounded;
    private Vector3 velocity;

    [Header("Foot Steps")]
    public AudioSource leftFootAudioSource;
    public AudioSource rightFootAudioSource;
    public AudioClip[] footstepSounds;
    public float footStepInterval = 0.5f;
    private float nextFootStepTime;
    private bool isLeftFootStep = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
    }

    void Update()
    {
       isGrounded= Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded&&velocity.y<0 )
        {
            velocity.y = -2;
        }
        HandleMovement();
        HandleGravity();
        //Handle FootSteps
        if(isGrounded && characterController.velocity.magnitude>0.1f && Time.time>= nextFootStepTime)
        {
            PlayerFootStepSound();
            nextFootStepTime = Time.time + footStepInterval;
        }
        //Handle jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        } 
        characterController.Move(velocity*Time.deltaTime);
    }

    
    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
        movement.y = 0;
        characterController.Move(movement*speed*Time.deltaTime);

    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }
    void PlayerFootStepSound()
    {
        AudioClip footStepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        if (isLeftFootStep)
        {
            leftFootAudioSource.PlayOneShot(footStepClip);
        }
        else
        {
            rightFootAudioSource.PlayOneShot(footStepClip);
        }
        isLeftFootStep=!isLeftFootStep;
    }
}
