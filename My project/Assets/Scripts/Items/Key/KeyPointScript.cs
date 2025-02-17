using UnityEngine;

public class KeyPointScript : MonoBehaviour
{
    public Transform player; // ������ �� ������
    private void Start()
    {
        player = GetComponentInParent<Transform>();
    }

    void Update()
    {
        // �������� ������� ���� � ����������� � ������� ����������
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // �������� Z ����������, ����� �� ���� ������� � ��������

        // ��������� ����������� �� ������ � ����
        Vector3 direction = mousePos - player.position;

        // ��������� ���� � �������� � ��������� ��� � �������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ������������� ���� ��� KeyPoint �� ��� Z
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
