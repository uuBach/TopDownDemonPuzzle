using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] private GameObject victoryZone;
    [SerializeField] private AudioClip victorySound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerKnight"))
        {
            victoryZone.SetActive(true);
            Time.timeScale = 0;
            SoundManager.instance.StopAllSounds();
            SoundManager.instance.PlaySound(victorySound);
        }
    }
}
