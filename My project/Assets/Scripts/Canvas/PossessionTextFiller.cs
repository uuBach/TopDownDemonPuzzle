using System.Collections;
using UnityEngine;

public class PossessionTextFiller : MonoBehaviour
{
    [SerializeField] private float refillTime;
    private RectTransform rectTransform;
    public bool isRefillingPossession = false;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetFillerToZero()
    {
        rectTransform.sizeDelta = new Vector2(160, rectTransform.sizeDelta.y);
    }
    public IEnumerator RefillText()
    {
        isRefillingPossession = true;
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
        isRefillingPossession = false;
    }
}
