using UnityEngine;

public class PossessionCloudWithTrick : MonoBehaviour
{
    private void StartSeeingThePossessed()
    {
        FindObjectOfType<CameraSwitcher>().isPossessingAndCanBeSeen = true;
    }
    private void StopSeeingThePossessed()
    {
        FindObjectOfType<CameraSwitcher>().isPossessingAndCanBeSeen = false;
    }
}
