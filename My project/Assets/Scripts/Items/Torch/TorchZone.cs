using UnityEngine;

public class TorchZone : MonoBehaviour
{
    public bool hasChecker;
    [SerializeField] private GameObject checkerEnemy;
    private Torch torch;
    [SerializeField] private bool hasBeenChecked = false;
    private void Awake()
    {
        torch = GetComponentInParent<Torch>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == checkerEnemy)
        {
            hasChecker = true;
            collision.gameObject.GetComponent<EnemyAI>().torchPosition = transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == checkerEnemy)
        {
            hasChecker = false;
            collision.gameObject.GetComponent<EnemyAI>().torchPosition = Vector3.zero;
        }
    }
    private void Update()
    {
        if(!torch.isLit && !hasBeenChecked && hasChecker)
        {
            checkerEnemy.GetComponent<EnemyAI>().CheckTheTorch();
            hasBeenChecked = true;
        }
    }
}
