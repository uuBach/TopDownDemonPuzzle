using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject signsScreen;
    [SerializeField] private AudioClip signSound;
    public void TurnOff()
    {
        signsScreen.transform.gameObject.SetActive(true);
        SoundManager.instance.PlaySound(signSound);
        Time.timeScale = 0;
        GetComponentInChildren<Animator>().SetBool("read", true);
    }
    private void Update()
    {
        if (signsScreen.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                signsScreen.transform.gameObject.SetActive(false);
                SoundManager.instance.PlaySound(signSound);
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
