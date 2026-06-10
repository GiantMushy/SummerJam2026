using UnityEngine;

public class EclipseController : MonoBehaviour
{
    [SerializeField] private float moveSpeed         = 5f;
    [SerializeField] private float speedIncreaseRate = 0.05f; // units/s added per second
    [SerializeField] private float maxSpeed          = 20f;
    [SerializeField] private float moveDirection     = -1f;   // -1 = left, 1 = right

    private float currentSpeed;

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void FixedUpdate()
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.fixedDeltaTime, maxSpeed);
        transform.Translate(Vector2.right * moveDirection * currentSpeed * Time.fixedDeltaTime);
    }
}
