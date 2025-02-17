using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour, IInteractable
{
    public bool isLit = true;
    private Animator animator;
    private InteractableZone zone;
    [SerializeField] private AudioClip turnOffSound;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        zone = FindFirstObjectByType<InteractableZone>();
    }
    public void TurnOff()
    {
        SoundManager.instance.PlaySound(turnOffSound);
        animator.SetBool("putOut", true);
        Instantiate(zone.lilSmoke, transform.position + new Vector3(0.05f, 0.75f, 0), Quaternion.identity, transform);
        transform.GetChild(1).transform.gameObject.SetActive(false);
        isLit = false;
    }
}
