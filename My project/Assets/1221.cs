using UnityEngine;

public class SpringBall : MonoBehaviour
{
    public Rigidbody ball; // Шарик
    public Transform anchor; // Точка крепления пружины
    public float springStrength = 10f; // Жесткость пружины
    public float damping = 1f; // Демпфирование
    public bool useRopeMode = false; // Режим обычной верёвки
    private bool isDragging = false;
    private LineRenderer lineRenderer;
    public float maxLength = 5f; // Максимальная длина верёвки

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            useRopeMode = !useRopeMode;
        }
    }

    private void FixedUpdate()
    {
        if (!isDragging)
        {
            Vector3 direction = anchor.position - ball.position;
            float distance = direction.magnitude;
            direction.Normalize();

            if (!useRopeMode)
            {
                // Режим пружины (Hooke's Law: F = -k * x)
                Vector3 springForce = direction * (springStrength * distance);
                Vector3 dampingForce = -ball.linearVelocity * damping;
                ball.AddForce(springForce + dampingForce, ForceMode.Force);
            }
            else
            {
                // Режим верёвки — ограничиваем длину, но не гасим скорость
                if (distance > maxLength)
                {
                    ball.position = anchor.position - direction * maxLength;
                    ball.linearVelocity -= Vector3.Project(ball.linearVelocity, direction);
                }
            }
        }

        // Обновление линии
        lineRenderer.SetPosition(0, anchor.position);
        lineRenderer.SetPosition(1, ball.position);
    }

    private void OnMouseDown()
    {
        isDragging = true;
        ball.isKinematic = true;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(ball.position).z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        ball.position = worldPos;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        ball.isKinematic = false;
    }
}
