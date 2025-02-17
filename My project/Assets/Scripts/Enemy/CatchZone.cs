using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.gameObject.transform.CompareTag("Player") && transform.GetComponentInParent<EnemyAI>().sawThePlayer && !transform.GetComponentInParent<EnemyAI>().isSleeping) || (collision.gameObject.transform.CompareTag("PlayerKnight") && transform.GetComponentInParent<EnemyAI>().isChasingTheKnightPlayer))
        {
            FindObjectOfType<UIManager>().GameOver();
        }
    }
}
