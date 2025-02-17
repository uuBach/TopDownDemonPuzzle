using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class InvisibleTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float remainingTime;
    private PlayerController playerController;
    private bool startTimer = false;
    private bool startTimerWasSet = false;
    private InvisibilityTextFiller invisibilityTextFiller;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        invisibilityTextFiller = FindObjectOfType<InvisibilityTextFiller>();
    }
    private void Update()
    {
        if(playerController.isInvisible)
        {
            if (!startTimerWasSet)
            {
                startTimer = true;
                startTimerWasSet = true;
                invisibilityTextFiller.SetFillerToZero();
                StartCoroutine(BecomeInvisible());
            }
            StartTimer();
        }
        else
        {
            startTimerWasSet = false;
        }
        if(startTimer)
        {
            remainingTime = playerController.invisibilityTime;
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
        remainingTime = Mathf.Clamp(remainingTime, 0, playerController.invisibilityTime);
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.CeilToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    private IEnumerator BecomeInvisible()
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
        yield return new WaitForSeconds(remainingTime);

        elapsedTime = 0f;
        while (elapsedTime < playerController.fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Используем Lerp для постепенного уменьшения альфа-канала
            float newAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / playerController.fadeDuration);

            // Применяем новый альфа-канал к цвету спрайта
            Color color = timerText.color;
            color.a = newAlpha;
            timerText.color = color;

            yield return null; // Ждём следующий кадр
        }
        finalColor = timerText.color;
        finalColor.a = startAlpha;
        timerText.color = finalColor;
    }
}
