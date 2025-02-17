using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAbility : MonoBehaviour
{
    [SerializeField] private bool objectIsInTheZone;
    [SerializeField] private List<EnemyAI> allObjects = new List<EnemyAI>();
    [SerializeField] private EnemyAI closestObject = null;
    [SerializeField] private float flickeringDuration = 0.2f;
    [SerializeField] private bool objectFlickeringCoroutineStarted = false;
    [SerializeField] private EnemyAI rememberedObject = null;
    private PlayerInput inputActions;
    public float sleepTime;
    [SerializeField] private GameObject ZZZsignPrefab;
    private SleepSpellTextFiller sleepSpellTextFiller;
    [SerializeField] private GameObject smokeRingPrefab;
    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Abilities.Sleep.performed += _ => PutToSleep();
        sleepSpellTextFiller = FindObjectOfType<SleepSpellTextFiller>();
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
        if (collision.gameObject.TryGetComponent(out EnemyAI enemy))
        {
            allObjects.Add(enemy);
            objectIsInTheZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyAI enemy))
        {
            allObjects.Remove(enemy);
            objectIsInTheZone = false;
        }
    }
    private void Update()
    {
        if (objectIsInTheZone)
        {
            FindTheClosestEnemyBoy();
        }
        else
        {
            closestObject = null;
        }

        if (closestObject != null && !objectFlickeringCoroutineStarted && closestObject.onTheBase)
        {
            InvokeRepeating("StartFlickeringCoroutine", 0, 0.5f);
            objectFlickeringCoroutineStarted = true;
            rememberedObject = closestObject;
        }
        if (rememberedObject != closestObject)
        {
            CancelInvoke("StartFlickeringCoroutine");
            objectFlickeringCoroutineStarted = false;
        }
    }
    void FindTheClosestEnemyBoy()
    {
        float closestEnemyDistance = Mathf.Infinity;
        foreach (EnemyAI currentEnemy in allObjects)
        {
            float enemyDistance = (currentEnemy.transform.position - transform.parent.transform.position).sqrMagnitude;
            if (enemyDistance < closestEnemyDistance)
            {
                closestEnemyDistance = enemyDistance;
                closestObject = currentEnemy;
            }
        }
        Debug.DrawLine(transform.parent.transform.position, closestObject.transform.position);
    }
    private void StartFlickeringCoroutine()
    {
        StartCoroutine(Flickering());
    }
    private IEnumerator Flickering()
    {
        SpriteRenderer enemySpriteRenderer = closestObject.gameObject.GetComponentInChildren<SpriteRenderer>();
        float startAlpha = enemySpriteRenderer.color.a;
        float endAlpha = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < flickeringDuration)
        {
            elapsedTime += Time.deltaTime;

            // Используем Lerp для постепенного уменьшения альфа-канала
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / flickeringDuration);

            // Применяем новый альфа-канал к цвету спрайта
            Color color = enemySpriteRenderer.color;
            color.a = newAlpha;
            enemySpriteRenderer.color = color;

            yield return null; // Ждём следующий кадр
        }

        // Устанавливаем конечное значение альфа-канала точно
        Color finalColor = enemySpriteRenderer.color;
        finalColor.a = endAlpha;
        enemySpriteRenderer.color = finalColor;

        elapsedTime = 0f;
        while (elapsedTime < flickeringDuration)
        {
            elapsedTime += Time.deltaTime;

            // Используем Lerp для постепенного уменьшения альфа-канала
            float newAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / flickeringDuration);

            // Применяем новый альфа-канал к цвету спрайта
            Color color = enemySpriteRenderer.color;
            color.a = newAlpha;
            enemySpriteRenderer.color = color;

            yield return null; // Ждём следующий кадр
        }
        finalColor = enemySpriteRenderer.color;
        finalColor.a = startAlpha;
        enemySpriteRenderer.color = finalColor;
    }
    private void PutToSleep()
    {
        if (closestObject && !sleepSpellTextFiller.isRefillingSleepSpell && !closestObject.isSleeping)
        {
            if (!closestObject.sawThePlayer)
            {
                Debug.Log("started");
                GameObject smokeRing = Instantiate(smokeRingPrefab, transform.position, Quaternion.identity);
                smokeRing.GetComponent<SmokeRing>().MoveTowardsTheKnight(closestObject.transform);

                // Вычисляем направление к ближайшему врагу
                Vector3 direction = closestObject.transform.position - smokeRing.transform.position;

                // Устанавливаем поворот только по оси Z
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                smokeRing.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                Debug.Log("ended");
                StartCoroutine(PutToSleepCoroutine(closestObject));
                sleepSpellTextFiller.SetFillerToZero();
                StartCoroutine(sleepSpellTextFiller.RefillText());
            }
        }
    }
    private IEnumerator PutToSleepCoroutine(EnemyAI enemySelected)
    {
        yield return new WaitForSeconds(0.5f);
        enemySelected.isSleeping = true;
        StartCoroutine(ZZZmarks(enemySelected));
        yield return new WaitForSeconds(sleepTime);
        enemySelected.isSleeping = false;
    }
    private IEnumerator ZZZmarks(EnemyAI enemySelected)
    {
        GameObject ZZZsign = Instantiate(ZZZsignPrefab, enemySelected.transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
        ZZZsign.transform.parent = enemySelected.transform;
        yield return new WaitForSeconds(sleepTime);
        Destroy(ZZZsign);
    }
}
