using UnityEngine;

public class EnemyGFX : MonoBehaviour
{
    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if(player.gameObject.transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
