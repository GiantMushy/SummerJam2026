using UnityEngine;

public class EclipseController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDirection = -1f; // -1 for left, 1 for right
    // Update is called once per frame
    void FixedUpdate()
    {
        // Move the eclipse horizontally
        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);
    }
}
