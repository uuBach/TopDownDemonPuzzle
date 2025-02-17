using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private PlayerInput inputActions;
    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] public float fadeDuration = 2f;
    public bool isInvisible = false;
    [SerializeField] public float invisibilityTime = 5f;
    private InvisibilityTextFiller invisibilityTextFiller;
    private List<EnemyAI> enemies = new List<EnemyAI>();
    [SerializeField] public bool isSeenByOneOfTheEnemies;
    [SerializeField] public bool wasSeenByOneOfTheEnemies;
    [SerializeField] private GameObject cloudPrefab;
    public bool hasKey = false;
    [SerializeField] AudioClip footSteps;
    private bool stepsSoundPlayed;
    [SerializeField] private AudioClip puffSound;
    private void Awake()
    {
        inputActions = new PlayerInput();
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputActions.Abilities.Invisible.performed += _ => StartCoroutine(BecomeInvisible());
        invisibilityTextFiller = FindObjectOfType<InvisibilityTextFiller>();

        EnemyAI[] enemyArray = FindObjectsOfType<EnemyAI>();

        // ��������� �� � ������
        enemies.AddRange(enemyArray);

    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void Update()
    {
        GetMovementValues();
        if(movement == Vector2.zero)
        {
            animator.SetBool("run", false);
        }
        AdjustPlayerFacingDirection();
        CheckIfIsSeenByEnemies();
        CheckIfWasSeenByEnemies();
        if(movement != Vector2.zero && !stepsSoundPlayed && !isInvisible)
        {
            SoundManager.instance.PlayLoopedAudio(footSteps);
            stepsSoundPlayed = true;
        }
        else if(movement == Vector2.zero || isInvisible)
        {
            SoundManager.instance.StopLoopedAudio(footSteps);
            stepsSoundPlayed = false;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnDisable()
    {
        inputActions.Disable();
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
        if(mousePos.x < playerPos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    private IEnumerator BecomeInvisible()
    {
        if (!invisibilityTextFiller.isRefillingInvisibility && !isSeenByOneOfTheEnemies)
        {
            if (!isInvisible)
            {
                SoundManager.instance.PlaySound(puffSound);
                isInvisible = true;
                GameObject cloud = Instantiate(cloudPrefab, transform.position + new Vector3(0.13f, 0.81f, 0), Quaternion.identity);
                float startAlpha = spriteRenderer.color.a;
                float endAlpha = 0.25f; // �������� �������� ������������ (25%)
                float elapsedTime = 0f;

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;

                    // ���������� Lerp ��� ������������ ���������� �����-������
                    float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

                    // ��������� ����� �����-����� � ����� �������
                    Color color = spriteRenderer.color;
                    color.a = newAlpha;
                    spriteRenderer.color = color;

                    yield return null; // ��� ��������� ����
                }

                // ������������� �������� �������� �����-������ �����
                Color finalColor = spriteRenderer.color;
                finalColor.a = endAlpha;
                spriteRenderer.color = finalColor;
                yield return new WaitForSeconds(invisibilityTime);

                elapsedTime = 0f;
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;

                    // ���������� Lerp ��� ������������ ���������� �����-������
                    float newAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / fadeDuration);

                    // ��������� ����� �����-����� � ����� �������
                    Color color = spriteRenderer.color;
                    color.a = newAlpha;
                    spriteRenderer.color = color;

                    yield return null; // ��� ��������� ����
                }
                finalColor = spriteRenderer.color;
                finalColor.a = startAlpha;
                spriteRenderer.color = finalColor;
                isInvisible = false;
                StartCoroutine(invisibilityTextFiller.RefillText());
            }
        }
    }
    private void CheckIfIsSeenByEnemies()
    {
        isSeenByOneOfTheEnemies = false; // ��������� ��������

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy.isSeeingThePlayer)
            {
                isSeenByOneOfTheEnemies = true;
                break; // ��������� ����, ��� ��� ������ �� ����� ��������� ��������� ������
            }
        }
    }

    private void CheckIfWasSeenByEnemies()
    {
        wasSeenByOneOfTheEnemies = false; // ��������� ��������

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy.sawThePlayer)
            {
                wasSeenByOneOfTheEnemies = true;
                break; // ��������� ����, ��� ��� ������ �� ����� ��������� ��������� ������
            }
        }
    }
}
