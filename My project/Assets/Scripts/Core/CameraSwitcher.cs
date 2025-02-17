using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineStateDrivenCamera cam1; // перва€ камера
    public CinemachineStateDrivenCamera cam2; // втора€ камера
    public float blendTime = 1f; // врем€ дл€ плавного перехода

    private CinemachineBrain cinemachineBrain;

    [SerializeField] private GameObject knightPlayer;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject invisibilityText;
    [SerializeField] private GameObject possessionCloudPrefab;
    [SerializeField] private TextMeshProUGUI invisibityTimerText;
    public bool isPossessingAndCanBeSeen = false;

    [SerializeField] private GameObject knightWhoChecksTheTorch;
    [SerializeField] private GameObject knightWhoNotChecksTheTorch;
    [SerializeField] private AudioClip possoutSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private GameObject SignText2;
    [SerializeField] private GameObject SignText3;
    [SerializeField] private GameObject SignText4;
    private bool yellowKeytextWasSet;
    private bool greenKeytextWasSet;
    private bool redKeytextWasSet;
    void Start()
    {
        Cursor.visible = false;
        // Ќаходим Cinemachine Brain на основной камере
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        // ”станавливаем начальную активную камеру
        cam1.Priority = 10;
        cam2.Priority = 5;
        SoundManager.instance.PlayBackgroundMusic(backgroundMusic);
    }
    public void SwitchCamera()
    {
        if (cam1.Priority > cam2.Priority)
        {
            cam1.Priority = 5;
            cam2.Priority = 10;
        }
        else
        {
            cam1.Priority = 10;
            cam2.Priority = 5;
        }

        // ƒобавл€ем плавный переход
        cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, blendTime);
    }
    private void Update()
    {
        if (player.transform.GetChild(2).transform.Find("YellowKey") && !yellowKeytextWasSet)
        {
            SignText2.gameObject.SetActive(true);
            yellowKeytextWasSet = true;
        }
        else if(player.transform.GetChild(2).transform.Find("GreenKey") && !greenKeytextWasSet)
        {
            SignText3.gameObject.SetActive(true);
            greenKeytextWasSet = true;
        }
        else if (player.transform.GetChild(2).transform.Find("RedKey") && !redKeytextWasSet)
        {
            SignText4.gameObject.SetActive(true);
            redKeytextWasSet = true;
        }
    }
    public void DemonOut()
    {
        SoundManager.instance.PlaySound(possoutSound);
        knightPlayer.GetComponent<KnightPlayerController>().isPossessed = false;
        player.SetActive(true);
        player.transform.position = knightPlayer.transform.position;
        knightPlayer.SetActive(false);
        FindObjectOfType<CameraSwitcher>().SwitchCamera();
        GameObject possessionCloud = Instantiate(possessionCloudPrefab, knightPlayer.transform.position, Quaternion.identity);
        invisibilityText.SetActive(true);
        player.GetComponentInChildren<InteractableZone>().possessedKnight.SetActive(true);
        player.GetComponentInChildren<InteractableZone>().possessedKnight.transform.position = player.transform.position;
        FindObjectOfType<InvisibleTimer>().remainingTime = 0;
        player.GetComponent<PlayerController>().isInvisible = false;
        player.GetComponent<PlayerController>().hasKey = false;
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        player.GetComponent<SpriteRenderer>().color = color;
        Color color1 = invisibityTimerText.color;
        color1.a = 0f;
        invisibityTimerText.color = color1;
        FindObjectOfType<InvisibilityTextFiller>().SetFillerToZero();
        StartCoroutine(FindObjectOfType<InvisibilityTextFiller>().RefillText());
        StartCoroutine(FindObjectOfType<PossessionTextFiller>().RefillText());
        Color colorKnightTimer = player.GetComponentInChildren<InteractableZone>().possessedKnight.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color;
        colorKnightTimer.a = 1f;
        player.GetComponentInChildren<InteractableZone>().possessedKnight.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = colorKnightTimer;
        StartCoroutine(player.GetComponentInChildren<InteractableZone>().possessedKnight.GetComponent<EnemyAI>().JustPutToSleep());
        player.GetComponentInChildren<InteractableZone>().possessedKnight.GetComponent<EnemyAI>().onTheBase = false;
        player.GetComponentInChildren<InteractableZone>().possessedKnight.transform.GetChild(0).transform.GetComponent<Animator>().SetBool("sleep", true);
        Color color2 = player.GetComponentInChildren<InteractableZone>().possessedKnight.GetComponentInChildren<SpriteRenderer>().color;
        color2.a = 1f;
        player.GetComponentInChildren<InteractableZone>().possessedKnight.GetComponentInChildren<SpriteRenderer>().color = color2;
    }


}
