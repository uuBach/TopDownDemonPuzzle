using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private Animator animator;
    private bool pressed = false;
    [SerializeField] private GameObject bigDoor;
    private bool theButtonIsReallyPressed;
    [SerializeField] AudioClip buttonUp;
    [SerializeField] AudioClip buttonDown;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (theButtonIsReallyPressed)
        {
            bigDoor.transform.GetComponent<Animator>().SetBool("open", true);
            bigDoor.transform.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            bigDoor.transform.GetComponent<Animator>().SetBool("open", false);
            bigDoor.transform.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerKnight"))
        {
            animator.SetBool("pressed", true);
            theButtonIsReallyPressed = true;
            SoundManager.instance.PlaySound(buttonDown);
            if (collision.gameObject.CompareTag("KnightWithFriend"))
            {
                pressed = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerKnight"))
        {
            animator.SetBool("pressed", true);
            theButtonIsReallyPressed = true;
            if (collision.gameObject.CompareTag("KnightWithFriend"))
            {
                pressed = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KnightWithFriend") || collision.gameObject.CompareTag("PlayerKnight"))
        {
            animator.SetBool("pressed", false);
            pressed = false;
            theButtonIsReallyPressed = false;
            SoundManager.instance.PlaySound(buttonUp);
        }
        if (pressed && collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("pressed", true);
            theButtonIsReallyPressed = true;
        }
        else if(!pressed && collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("pressed", false);
            theButtonIsReallyPressed = false;
            SoundManager.instance.PlaySound(buttonUp);
        }
    }
}
