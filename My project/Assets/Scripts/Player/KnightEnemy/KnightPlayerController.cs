using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightPlayerController : MonoBehaviour
{
    private PlayerInput inputActions;
    private Vector2 movement;
    private Rigidbody2D playerRb;
    [SerializeField] private float speed;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isPossessed;
    [SerializeField] public float possessionTime;

    private bool stepsSoundPlayed;
    private AudioSource localAudioSource;
    [SerializeField] AudioClip footStepsSound;
    private void Awake()
    {
        inputActions = new PlayerInput();
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        localAudioSource = gameObject.AddComponent<AudioSource>();
        localAudioSource.loop = true;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void Update()
    {
        GetMovementValues();
        AdjustPlayerFacingDirection();
        if (movement == Vector2.zero)
        {
            animator.SetBool("run", false);
        }

        if (movement != Vector2.zero && !stepsSoundPlayed)
        {
            SoundManager.instance.PlayLoopedAudio(footStepsSound);
            stepsSoundPlayed = true;
        }
        else if (movement == Vector2.zero)
        {
            SoundManager.instance.StopLoopedAudio(footStepsSound);
            stepsSoundPlayed = false;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void GetMovementValues()
    {
        movement = inputActions.Movement.Move.ReadValue<Vector2>();
    }
    private void Move()
    {
        playerRb.MovePosition(playerRb.position + movement * (speed * Time.fixedDeltaTime));
        animator.SetBool("run", true);
    }
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        if (mousePos.x < playerPos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
