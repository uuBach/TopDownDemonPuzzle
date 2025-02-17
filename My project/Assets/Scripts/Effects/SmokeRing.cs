using UnityEngine;

public class SmokeRing : MonoBehaviour
{
    private Transform targetObject;
    [SerializeField] private float smokeSpeed;
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetObject.position, Time.deltaTime * smokeSpeed);
    }
    public void MoveTowardsTheKnight(Transform knightTransform)
    {
        targetObject = knightTransform;
    }
}
