using Pathfinding.Examples;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    private KeyPointScript keyPoint;
    private ItemsInput itemsInput;
    private bool theKeyIsInTheZone = false;
    private GameObject selectedDoor;
    [SerializeField] private int keyCode;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip doorOpen;
    private void Awake()
    {
        itemsInput = new ItemsInput();
        itemsInput.Key.DoorOpen.performed += _ => DoorOpen();
    }
    private void OnEnable()
    {
        itemsInput.Enable();
    }
    private void OnDisable()
    {
        itemsInput.Disable();
    }
    private void Start()
    {
        keyPoint = FindObjectOfType<KeyPointScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController player))
        {
            SoundManager.instance.PlaySound(pickUpSound);
            transform.parent = keyPoint.transform;
            transform.localPosition = new Vector3(0.8f, 0.07f, 0);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            player.hasKey = true;
        }
        if (collision.TryGetComponent<DoorScript>(out DoorScript door))
        {
            theKeyIsInTheZone = true;
            selectedDoor = door.transform.parent.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DoorScript>(out DoorScript door))
        {
            theKeyIsInTheZone = false;
            selectedDoor = null;
        }
    }
    private void DoorOpen()
    {
        if (theKeyIsInTheZone)
        {
            if(selectedDoor)
            {
                if (selectedDoor.GetComponentInChildren<DoorScript>().doorCode == keyCode)
                {
                    SoundManager.instance.PlaySound(doorOpen);
                    selectedDoor.GetComponent<Animator>().SetBool("open", true);
                    selectedDoor.transform.GetChild(0).gameObject.SetActive(false);
                    selectedDoor.transform.GetChild(1).gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }
    }
}
