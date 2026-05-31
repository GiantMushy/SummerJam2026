using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    [SerializeField] private bool useSmoothing = true;

    [Header("Smoothing Options")]
    [SerializeField] private float smoothSpeed = 4f;   // SmoothDamp: higher = snappier
    [SerializeField] private float lerpSpeed = 12f;    // Lerp: higher = snappier

    private Vector3 _velocity = Vector3.zero;
    private float _z;

    private void Awake()
    {
        _z = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new(target.position.x, target.position.y, _z);

        if (useSmoothing)
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, 1f / smoothSpeed);
        else
            transform.position = Vector3.Lerp(transform.position, desired, lerpSpeed * Time.deltaTime);
    }
}
