using UnityEngine;

public class KnightZone : MonoBehaviour
{
    public GameObject checkerEnemy;
    public bool hasBeenChecked = false;
    private EnemyAI enemyAI;
    public bool friendIsInTheZone;
    private bool knightPlayerChaseWasInvoked = false;
    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }
    private void Start()
    {
        checkerEnemy = transform.parent.GetComponent<EnemyAI>().friendKnight.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend"))
        {
            friendIsInTheZone = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend"))
        {
            friendIsInTheZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend"))
        {
            friendIsInTheZone = false;
        }
    }
    private void Update()
    {
        if (!checkerEnemy.GetComponent<EnemyAI>().wentToCheckTheTorchSimple && !enemyAI.wentToCheckTheTorchSimple)
        {
            if (enemyAI.isSleeping && !hasBeenChecked && !enemyAI.isConfused)
            {
                checkerEnemy.GetComponent<EnemyAI>().CheckTheFriend();
                hasBeenChecked = true;
            }
            if (checkerEnemy.GetComponent<EnemyAI>().reachedTheTorch && !checkerEnemy.GetComponent<EnemyAI>().onTheBase)
            {
                enemyAI.isSleeping = false;
                SoundManager.instance.StopSound(enemyAI.snoring);
                hasBeenChecked = false;
                enemyAI.ZZZSign.SetActive(false);
                StartCoroutine(enemyAI.GetComponentInChildren<SleepTimer>().FadeOut());
            }
            if(FindObjectOfType<CameraSwitcher>().isPossessingAndCanBeSeen && !knightPlayerChaseWasInvoked)
            {
                enemyAI.GetComponent<EnemyAI>().ChaseTheKnightPlayer();
                knightPlayerChaseWasInvoked = true;
            }    
        }
    }
}
