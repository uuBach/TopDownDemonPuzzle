using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PossessionTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    private float remainingTime;
    private bool startTimer = false;
    [SerializeField] private bool startTimerWasSet = false;
    private PossessionTextFiller possessionTextFiller;
    private KnightPlayerController knightPlayer;
    private PlayerController playerController;
    private InteractableZone interactableZone;
    private void Awake()
    {
        possessionTextFiller = FindObjectOfType<PossessionTextFiller>();
        knightPlayer = GetComponentInParent<KnightPlayerController>();
        playerController = FindObjectOfType<PlayerController>();
        interactableZone = FindObjectOfType<InteractableZone>();
    }
    private void Update()
    {
        if (knightPlayer.isPossessed)
        {
            if (!startTimerWasSet)
            {
                startTimer = true;
                startTimerWasSet = true;
                possessionTextFiller.SetFillerToZero();
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
            remainingTime = knightPlayer.possessionTime;
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
        yield return new WaitForSeconds(knightPlayer.possessionTime);
        StartCoroutine(FadeOut());
        FindObjectOfType<CameraSwitcher>().DemonOut();
        knightPlayer.isPossessed = false;
        startTimerWasSet = false;
        startTimer = true;
    }
    private IEnumerator FadeIn()
    {
        float startAlpha = timerText.color.a;
        float endAlpha = 1f; // Конечное значение прозрачности (25%)
        float elapsedTime = 0f;

        while (elapsedTime < 2)
        {
            elapsedTime += Time.deltaTime;

            // Используем Lerp для постепенного уменьшения альфа-канала
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / 2);

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
        while (elapsedTime < 2)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / 2);

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
