using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableZone : MonoBehaviour
{
    private List<IInteractable> nearbyInteractableObjects = new List<IInteractable>();
    private IInteractable closestObject = null;
    [SerializeField] private bool theObjectIsInTheZone = false;
    [SerializeField] private float flickeringDuration;
    [SerializeField] private bool objectFlickeringCoroutineStarted = false;
    private bool objectFlickeringCoroutineEnded = false;
    [SerializeField] private IInteractable rememberedObject = null;
    private PlayerInput inputActions;
    [SerializeField] public GameObject lilSmoke;
    [SerializeField] private GameObject knightPlayer;
    [SerializeField] private GameObject invisibilityGameObject;
    [SerializeField] private GameObject possessionCloudPrefab;
    [SerializeField] private GameObject possessionCloudWithTrickPrefab;
    public bool enemyWasPutToSleep;
    public GameObject possessedKnight;
    [SerializeField] private AudioClip possessionSound;
    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Abilities.Sleep.performed += _ => Interact();
        inputActions.Abilities.Possession.performed += _ => Possession();
    }
    private void OnEnable()
    {
        inputActions.Abilities.Enable();
    }
    private void OnDisable()
    {
        inputActions.Abilities.Disable();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IInteractable>(out IInteractable interactableObject))
        {
            nearbyInteractableObjects.Add(interactableObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IInteractable>(out IInteractable interactableObject))
        {
            nearbyInteractableObjects.Remove(interactableObject);
        }
    }
    private void Update()
    {
        theObjectIsInTheZone = !(nearbyInteractableObjects.Count == 0);
        if (theObjectIsInTheZone)
        {
            FindTheClosestObject();
        }
        else
        {
            closestObject = null;
        }

        if (transform.gameObject.transform.parent.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if (playerController.wasSeenByOneOfTheEnemies)
            {
                closestObject = null;
            }
        }

        if(closestObject is EnemyAI enemyRemove && !enemyRemove.onTheBase)
        {
            nearbyInteractableObjects.Remove(enemyRemove);
        }
        else if(closestObject is Torch torch && !torch.isLit)
        {
            nearbyInteractableObjects.Remove(torch);
        }

        if (transform.gameObject.transform.parent.TryGetComponent<PlayerController>(out PlayerController playerController2))
        {
            if (closestObject != null && !objectFlickeringCoroutineStarted && !playerController2.wasSeenByOneOfTheEnemies)
            {
                if (closestObject is EnemyAI enemy && !enemy.onTheBase)
                {
                    return;
                }
                InvokeRepeating("StartFlickeringCoroutine", 0, 0.5f);
                objectFlickeringCoroutineStarted = true;
                rememberedObject = closestObject;
                objectFlickeringCoroutineEnded = false;
            }
            else if ((rememberedObject != closestObject && !objectFlickeringCoroutineEnded) || (closestObject is EnemyAI enemy && !enemy.onTheBase))
            {
                CancelInvoke("StartFlickeringCoroutine");
                objectFlickeringCoroutineStarted = false;
                objectFlickeringCoroutineEnded = true;
            }
        }
        else
        {
            if (closestObject != null && !objectFlickeringCoroutineStarted)
            {
                if (closestObject is EnemyAI enemy && !enemy.onTheBase)
                {
                    return;
                }
                InvokeRepeating("StartFlickeringCoroutine", 0, 0.5f);
                objectFlickeringCoroutineStarted = true;
                rememberedObject = closestObject;
                objectFlickeringCoroutineEnded = false;
            }
            else if ((rememberedObject != closestObject && !objectFlickeringCoroutineEnded) || (closestObject is EnemyAI enemy && !enemy.onTheBase))
            {
                CancelInvoke("StartFlickeringCoroutine");
                objectFlickeringCoroutineStarted = false;
                objectFlickeringCoroutineEnded = true;
            }
        }


    }
    private void StartFlickeringCoroutine()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Flickering());
        }
    }
    private void FindTheClosestObject()
    {
        float closestObjectDistance = Mathf.Infinity;
        foreach (IInteractable currentObject in nearbyInteractableObjects)
        {
            Transform objectTransform = ((MonoBehaviour)currentObject).transform;
            float objectDistance = (objectTransform.position - transform.parent.transform.position).sqrMagnitude;
            if (objectDistance < closestObjectDistance)
            {
                closestObjectDistance = objectDistance;
                closestObject = currentObject;
            }
        }
    }
    private IEnumerator Flickering()
    {
        SpriteRenderer objectSpriteRenderer = ((MonoBehaviour)closestObject).transform.GetComponentInChildren<SpriteRenderer>();
        float startAlpha = 1;
        float endAlpha = 0.25f;
        float elapsedTime = 0f;
        while (elapsedTime < flickeringDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / flickeringDuration);
            Color color = objectSpriteRenderer.color;
            color.a = newAlpha;
            objectSpriteRenderer.color = color;
            yield return null;
        }

        Color finalColor = objectSpriteRenderer.color;
        finalColor.a = endAlpha;
        objectSpriteRenderer.color = finalColor;
        elapsedTime = 0f;
        while (elapsedTime < flickeringDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / flickeringDuration);
            Color color = objectSpriteRenderer.color;
            color.a = newAlpha;
            objectSpriteRenderer.color = color;
            yield return null;
        }
        finalColor = objectSpriteRenderer.color;
        finalColor.a = startAlpha;
        objectSpriteRenderer.color = finalColor;
    }
    private void Interact()
    {
        IInteractable interactableObject = closestObject as IInteractable;

        if (interactableObject != null)
        {
            if (interactableObject is EnemyAI enemy1)
            {
                if (!enemy1.isSleeping || !enemy1.onTheBase)
                interactableObject.TurnOff();
            }
            else
            {
                interactableObject.TurnOff();
            }
        }

        if(interactableObject is EnemyAI enemy && !FindObjectOfType<SleepSpellTextFiller>().isRefillingSleepSpell && !enemy.isSleeping)
        {
            enemyWasPutToSleep = true;
            FindObjectOfType<SleepSpellTextFiller>().ResetAndRefillTheText();
        }
    }
    private void Possession()
    {
        if (!FindObjectOfType<PossessionTextFiller>().isRefillingPossession)
        {
            if (transform.parent.TryGetComponent<PlayerController>(out PlayerController player))
            {
                IInteractable interactableObject = closestObject as IInteractable;
                if (interactableObject != null && interactableObject is EnemyAI enemy && enemy.isSleeping)
                {
                    SoundManager.instance.PlaySound(possessionSound);
                    SoundManager.instance.StopSound(enemy.snoring);
                    possessedKnight = enemy.gameObject;
                    transform.parent.gameObject.SetActive(false);
                    knightPlayer.SetActive(true);
                    FindObjectOfType<InvisibilityTextFiller>().SetFillerToZero();
                    knightPlayer.transform.position = enemy.transform.position;
                    enemy.gameObject.SetActive(false);
                    FindObjectOfType<CameraSwitcher>().SwitchCamera();
                    if (!enemy.gameObject.CompareTag("KnightWithFriend"))
                    {
                        GameObject possessionCloud = Instantiate(possessionCloudPrefab, knightPlayer.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        GameObject possessionCloudWithTrick = Instantiate(possessionCloudWithTrickPrefab, knightPlayer.transform.position, Quaternion.identity);
                    }
                    knightPlayer.GetComponent<KnightPlayerController>().isPossessed = true;
                }
            }
        }
    }
}
