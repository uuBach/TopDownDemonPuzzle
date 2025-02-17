using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class SleepTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float remainingTime;
    private PlayerController playerController;
    public bool startTimer = false;
    public bool startTimerWasSet = false;
    private SleepSpellTextFiller sleepSpellTextFiller;
    private EnemyAI enemyAI;
    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        sleepSpellTextFiller = FindObjectOfType<SleepSpellTextFiller>();
        enemyAI = GetComponentInParent<EnemyAI>();
    }
    private void Update()
    {
        if (enemyAI.isSleeping && !enemyAI.isConfused)
        {
            if (!startTimerWasSet)
            {
                startTimer = true;
                startTimerWasSet = true;
                StartCoroutine(BecomeInvisible());
            }
            StartTimer();
        }
        else
        {
            startTimerWasSet = false;
        }
        if (startTimer)
        {
            remainingTime = enemyAI.sleepTime;
            startTimer = false;
        }
    }
    private void StartTimer()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.CeilToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    private IEnumerator BecomeInvisible()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(enemyAI.sleepTime);
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeIn()
    {
        float startAlpha = timerText.color.a;
        float endAlpha = 1f; // Конечное значение прозрачности (25%)
        float elapsedTime = 0f;

        while (elapsedTime < playerController.fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Используем Lerp для постепенного уменьшения альфа-канала
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / playerController.fadeDuration);

            // Применяем новый альфа-канал к цвету спрайта
            Color color = timerText.color;
            color.a = newAlpha;
            timerText.color = color;

            yield return null; // Ждём следующий кадр
        }

        // Устанавливаем конечное значение альфа-канала точно
        Color finalColor = timerText.color;
        finalColor.a = endAlpha;
        timerText.color = finalColor;
    }
    public IEnumerator FadeOut()
    {
        float startAlpha = timerText.color.a;
        float endAlpha = 0f; 
        float elapsedTime = 0f;
        while (elapsedTime < playerController.fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / playerController.fadeDuration);

            // Применяем новый альфа-канал к цвету спрайта
            Color color = timerText.color;
            color.a = newAlpha;
            timerText.color = color;

            yield return null; // Ждём следующий кадр
        }
        Color finalColor = timerText.color;
        finalColor.a = endAlpha;
        timerText.color = finalColor;
    }
}
