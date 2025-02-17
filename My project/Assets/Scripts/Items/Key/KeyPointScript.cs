using UnityEngine;

public class KeyPointScript : MonoBehaviour
{
    public Transform player; // ссылка на игрока
    private void Start()
    {
        player = GetComponentInParent<Transform>();
    }

    void Update()
    {
        // ѕолучаем позицию мыши и преобразуем в мировые координаты
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // ќбнул€ем Z координату, чтобы не было проблем с глубиной

        // ¬ычисл€ем направление от игрока к мыши
        Vector3 direction = mousePos - player.position;

        // ¬ычисл€ем угол в радианах и переводим его в градусы
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ”станавливаем угол дл€ KeyPoint по оси Z
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
