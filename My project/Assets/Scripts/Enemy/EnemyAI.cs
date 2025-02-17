using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;
public class EnemyAI : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 200f;
    [SerializeField] private float nextWayPointDistance = 2f;
    Path path;
    int currentWayPoint = 0;
    [SerializeField] private bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float invokeTime;
    private Animator animator;
    public bool sawThePlayer = false;
    public bool isSeeingThePlayer = false;
    [SerializeField] private LayerMask knightLayer;  // Слой knight
    [SerializeField] private LayerMask cameraLayer;  // Слой camera
    [SerializeField] private LayerMask ignoreLayer;

    [SerializeField] private GameObject basePoint;

    [SerializeField] public bool basePathWasInvoked = false;
    [SerializeField] private bool playerPathWasInvoked = false;
    public bool onTheBase;

    [SerializeField] GameObject QuestionMarksPrefabs;

    [SerializeField] private bool hasSeenThePlayer = false;

    public bool isSleeping = false;

    public bool isConfused = false;

    [SerializeField] private GameObject ZZZsignPrefab;
    private SleepSpellTextFiller sleepSpellTextFiller;
    [SerializeField] private GameObject smokeRingPrefab;
    public float sleepTime;
    public float shortSleepTime;
    public Vector3 torchPosition = Vector3.zero;
    [SerializeField] public Transform friendKnight = null;
    [SerializeField] private bool wentToCheck;
    [SerializeField] private float checkTorchTime;
    [SerializeField] private float checkFriendTime;
    [SerializeField] public bool reachedTheTorch;
    private KnightZone knightZone;
    public Coroutine sleepCoroutine;

    [SerializeField] public GameObject ZZZSign;

    [SerializeField] GameObject Torch = null;

    public bool wentToCheckTheTorch = false;
    public bool wentToCheckTheFriend = false;

    public bool wentToCheckTheTorchSimple = false;
    [SerializeField] private KnightPlayerController knightPlayerController;
    public bool isChasingTheKnightPlayer;

    [SerializeField] AudioClip[] audioClipsWat = new AudioClip[] { };
    [SerializeField] AudioClip[] audioClipsHey = new AudioClip[] { };
    [SerializeField] public AudioClip snoring;
    private bool stepsSoundPlayed;
    private AudioSource localAudioSource;
    [SerializeField] AudioClip footStepsSound;
    private static bool isFootstepsPlaying = false;
    private void Awake()
    {
        sleepSpellTextFiller = FindObjectOfType<SleepSpellTextFiller>();

        if(friendKnight)
            knightZone = GetComponentInChildren<KnightZone>();
    }
    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        localAudioSource = gameObject.AddComponent<AudioSource>();
        localAudioSource.loop = true;
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }
    void GetPathToBase()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, basePoint.transform.position, OnPathComplete);
        }
    }
    void GetPathToTorch()
    {
        if (seeker.IsDone() && torchPosition != Vector3.zero)
        {
            seeker.StartPath(rb.position, torchPosition + new Vector3(0.5f, 0, 0), OnPathComplete);
        }
    }
    void GetPathToFriend()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, friendKnight.transform.position, OnPathComplete);
        }
    }
    void GetPathToKnightPlayer()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, knightPlayerController.gameObject.transform.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    private void Update()
    {
        if (IsAnimationPlaying("Run") && !stepsSoundPlayed && !isFootstepsPlaying)
        {
            localAudioSource.clip = footStepsSound;
            localAudioSource.Play();
            stepsSoundPlayed = true;
            isFootstepsPlaying = true; // Устанавливаем флаг, чтобы другие объекты не включали звук
        }
        else if (!IsAnimationPlaying("Run") && stepsSoundPlayed || isSleeping || FindObjectOfType<UIManager>().gameOver)
        {
            localAudioSource.Stop();
            stepsSoundPlayed = false;
            isFootstepsPlaying = false; // Сбрасываем флаг, чтобы другие объекты могли включить звук
        }
        
        


        if (Torch)
        {
            if (!Torch.GetComponent<Torch>().isLit && basePathWasInvoked && !wentToCheckTheFriend)
            {
                wentToCheckTheTorch = true;
            }
            else if(onTheBase)
            {
                wentToCheckTheTorch = false;
            }
            else if(sawThePlayer)
            {
                wentToCheckTheTorch = false;
            }
        }
        if (!isSleeping)
        {
            animator.SetBool("sleep", false);
            AdjustEnemyDirection();

            if (!onTheBase && !reachedTheTorch)
            {
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
            }

            if (player && player.isInvisible)
            {
                sawThePlayer = false;
                isSeeingThePlayer = false;
            }

            if (!onTheBase && !basePathWasInvoked && !isSeeingThePlayer && !sawThePlayer)
            {
                CancelInvoke("UpdatePath");
                CancelInvoke("GetPathToFriend");
                CancelInvoke("GetPathToTorch");
                InvokeRepeating("GetPathToBase", 0, invokeTime);
                basePathWasInvoked = true;
                StartCoroutine(QuestionMarks());
            }
            else if (sawThePlayer && !playerPathWasInvoked)
            {
                CancelInvoke("GetPathToBase");
                CancelInvoke("GetPathToFriend");
                CancelInvoke("GetPathToTorch");
                InvokeRepeating("UpdatePath", 0, invokeTime);
                playerPathWasInvoked = true;
                onTheBase = false;
            }
            else if (onTheBase && !sawThePlayer)
            {
                CancelInvoke("GetPathToBase");
                CancelInvoke("GetPathToFriend");
                CancelInvoke("GetPathToTorch");
                basePathWasInvoked = false;
                playerPathWasInvoked = false;
            }
        }
        else
        {
            if (!isConfused)
            {
                animator.SetBool("sleep", true);
            }
            isSeeingThePlayer = false;
            sawThePlayer = false;
        }
        if (isChasingTheKnightPlayer)
        {
            onTheBase = false;
            CancelInvoke("GetPathToBase");
            CancelInvoke("GetPathToFriend");
            CancelInvoke("GetPathToTorch");
            CancelInvoke("UpdatePath");
            if (knightPlayerController.gameObject.transform.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
                animator.SetBool("run", true);
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        if (transform.gameObject.CompareTag("KnightWithFriend"))
        {
            if (!knightPlayerController.gameObject.activeInHierarchy)
            {
                isChasingTheKnightPlayer = false;
                CancelInvoke("GetPathToKnightPlayer");
            }
        }

        if(!isSleeping && !sawThePlayer && !wentToCheckTheFriend && !wentToCheckTheTorch && !wentToCheckTheTorchSimple && onTheBase && !isConfused && Vector3.Distance(transform.position, basePoint.transform.position) > 1)
        {
            onTheBase = false;
        }

    }
    private void FixedUpdate()
    {
        if (!isSleeping)
        {
            CheckThePlayer();
            CheckIfIsSeeingThePlayer();
            MoveToTheTarget();
        }
        else if (!wentToCheck)
        {
            CheckThePlayer();
            CheckIfIsSeeingThePlayer();
        }

    }
    private void CheckIfTheyAreOneTheBase()
    {
        if (Vector3.Distance(transform.position, basePoint.transform.position) > 1)
        {
            onTheBase = false;
        }
    }
    private void MoveToTheTarget()
    {
        if (path == null)
            return;

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }

        if (currentWayPoint >= path.vectorPath.Count && basePathWasInvoked)
        {
            if (!wentToCheck)
            {
                basePathWasInvoked = false;
                CancelInvoke("GetPathToBase");
                onTheBase = true;
            }
            else
            {
                CancelInvoke("GetPathToTorch");
                CancelInvoke("GetPathToFriend");
                reachedTheTorch = true;
            }
        }

    }
    private void AdjustEnemyDirection()
    {
        if (sawThePlayer)
        {
            if (player.gameObject.transform.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        if (!sawThePlayer && !onTheBase && !wentToCheck)
        {
            if (basePoint.gameObject.transform.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }
    private void CheckThePlayer()
    {
        if (player)
        {
            if (!basePathWasInvoked)
            {
                if (!sawThePlayer && !player.isInvisible)
                {
                    int ignoreLayers = knightLayer | cameraLayer | ignoreLayer;
                    RaycastHit2D ray = Physics2D.Raycast(transform.position, player.gameObject.transform.position - transform.position, Mathf.Infinity, ~ignoreLayers);
                    if (ray.collider != null)
                    {
                        sawThePlayer = ray.collider.CompareTag("Player");
                        hasSeenThePlayer = ray.collider.CompareTag("Player");
                        if (sawThePlayer)
                        {
                            Debug.DrawRay(transform.position, player.gameObject.transform.position - transform.position, Color.green);
                            if (!isSleeping)
                            {
                                SoundManager.instance.PlaySound(audioClipsHey[Random.Range(0, audioClipsHey.Length)]);
                            }
                        }
                        else
                        {
                            Debug.DrawRay(transform.position, player.gameObject.transform.position - transform.position, Color.red);
                        }
                    }
                }
            }
        }
    }
    private void CheckIfIsSeeingThePlayer()
    {
        if (player)
        {
            int ignoreLayers = knightLayer | cameraLayer | ignoreLayer;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, player.gameObject.transform.position - transform.position, Mathf.Infinity, ~ignoreLayers);
            if (ray.collider != null)
            {
                isSeeingThePlayer = ray.collider.CompareTag("Player");
            }
        }
    }
    private IEnumerator QuestionMarks()
    {
        if (hasSeenThePlayer)
        {
            SoundManager.instance.PlaySound(audioClipsWat[Random.Range(0, audioClipsWat.Length)]);
            GameObject QuestionMarks = Instantiate(QuestionMarksPrefabs, transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
            QuestionMarks.transform.parent = transform;
            isSleeping = true;
            animator.SetBool("run", false);
            isConfused = true;
            yield return new WaitForSeconds(2);
            isConfused = false;
            isSleeping = false;
            Destroy(QuestionMarks);
        }
    }
    public void TurnOff()
    {
        sleepCoroutine = StartCoroutine(PutToSleep());
    }
    public IEnumerator PutToSleep()
    {
        if (!sleepSpellTextFiller.isRefillingSleepSpell && !isSleeping)
        {
            if (!sawThePlayer)
            {
                SoundManager.instance.PlaySound(snoring);
                ZZZSign.SetActive(true);
                GameObject smokeRing = null;
                if (player.gameObject.activeInHierarchy)
                {
                    smokeRing = Instantiate(smokeRingPrefab, player.transform.position, Quaternion.identity);
                }
                else
                {
                    smokeRing = Instantiate(smokeRingPrefab, FindObjectOfType<KnightPlayerController>().transform.position, Quaternion.identity);
                }
                smokeRing.GetComponent<SmokeRing>().MoveTowardsTheKnight(transform);
                Vector3 direction = transform.position - smokeRing.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                smokeRing.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                yield return new WaitForSeconds(0.5f);
                isSleeping = true;
                yield return new WaitForSeconds(sleepTime);
                isSleeping = false;
                ZZZSign.SetActive(false);

                if (friendKnight)
                    knightZone.hasBeenChecked = false;
            }
        }
    }
    public IEnumerator JustPutToSleep()
    {
        SoundManager.instance.PlaySound(snoring);
        ZZZSign.SetActive(true);
        isSleeping = true;
        yield return new WaitForSeconds(sleepTime);
        isSleeping = false;
        StartCoroutine(transform.GetComponentInChildren<SleepTimer>().FadeOut());
        ZZZSign.SetActive(false);
    }
    public void CheckTheTorch()
    {
        StartCoroutine(CheckTheTorchCoroutine());
    }
    private IEnumerator CheckTheTorchCoroutine()
    {
        if (!isSleeping)
        {
            wentToCheckTheTorchSimple = true;
            wentToCheck = true;
            onTheBase = false;
            basePathWasInvoked = true;
            CancelInvoke("GetPathToBase");
            CancelInvoke("UpdatePath");
            InvokeRepeating("GetPathToTorch", 0, invokeTime);

            if (torchPosition.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            GameObject QuestionMarks = Instantiate(QuestionMarksPrefabs, transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
            QuestionMarks.transform.parent = transform;
            SoundManager.instance.PlaySound(audioClipsWat[Random.Range(0, audioClipsWat.Length)]);
            yield return new WaitForSeconds(checkTorchTime);
            Destroy(QuestionMarks);
            wentToCheck = false;
            reachedTheTorch = false;
            basePathWasInvoked = false;
            wentToCheckTheTorchSimple = false;
        }
    }
    public void CheckTheFriend()
    {
        if (!isSleeping && !sawThePlayer)
        {
            StartCoroutine(CheckTheFriendCoroutine());
        }
    }
    public void ChaseTheKnightPlayer()
    {
        if (knightPlayerController.gameObject.activeInHierarchy)
        {
            onTheBase = false;
            SoundManager.instance.PlaySound(audioClipsHey[Random.Range(0, audioClipsHey.Length)]);
            InvokeRepeating("GetPathToKnightPlayer", 0, invokeTime);
            CancelInvoke("UpdatePath");
            CancelInvoke("GetPathToFriend");
            CancelInvoke("GetPathToTorch");
            CancelInvoke("GetPathToBase");
            isChasingTheKnightPlayer = true;
        }
    }
    private IEnumerator CheckTheFriendCoroutine()
    {
        if (!isSleeping)
        {
            wentToCheckTheFriend = true;
            wentToCheck = true;
            onTheBase = false;
            basePathWasInvoked = true;
            CancelInvoke("GetPathToBase");
            CancelInvoke("UpdatePath");
            InvokeRepeating("GetPathToFriend", 0, invokeTime);

            if (friendKnight.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            GameObject QuestionMarks = Instantiate(QuestionMarksPrefabs, transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
            QuestionMarks.transform.parent = transform;
            SoundManager.instance.PlaySound(audioClipsWat[Random.Range(0, audioClipsWat.Length)]);
            yield return new WaitForSeconds(checkFriendTime);
            Destroy(QuestionMarks);
            wentToCheck = false;
            reachedTheTorch = false;
            basePathWasInvoked = false;
            wentToCheckTheFriend = false;
        }
    }
    private bool IsAnimationPlaying(string animationName)
    {
        // Получаем информацию о текущем состоянии в слое 0
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Проверяем, совпадает ли имя текущей анимации с заданным именем
        return stateInfo.IsName(animationName);
    }
}
