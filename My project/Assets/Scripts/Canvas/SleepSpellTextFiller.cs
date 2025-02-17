using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSpellTextFiller : MonoBehaviour
{
    [SerializeField] private float refillTime;
    private RectTransform rectTransform;
    public bool isRefillingSleepSpell = false;
    private PlayerInput inputActions;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        inputActions = new PlayerInput();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    public void SetFillerToZero()
    {
        rectTransform.sizeDelta = new Vector2(160, rectTransform.sizeDelta.y);
    }
    public IEnumerator RefillText()
    {
        isRefillingSleepSpell = true;
        float startWidth = rectTransform.sizeDelta.x;
        float endWidth = 400f;
        float elapsedTime = 0;
        while (elapsedTime < refillTime)
        {
            elapsedTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, endWidth, elapsedTime / refillTime);
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
            yield return null;
        }
        rectTransform.sizeDelta = new Vector2(endWidth, rectTransform.sizeDelta.y);
        isRefillingSleepSpell = false;
    }
    public void ResetAndRefillTheText()
    {
        if (FindObjectOfType<InteractableZone>().enemyWasPutToSleep)
        {
            SetFillerToZero();
            StartCoroutine(RefillText());
            FindObjectOfType<InteractableZone>().enemyWasPutToSleep = false;
        }
    }
}
