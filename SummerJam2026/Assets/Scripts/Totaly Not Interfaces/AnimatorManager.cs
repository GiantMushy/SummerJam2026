using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [HideInInspector] public CharacterManager character;

    int vertical;
    int horizontal;
    [HideInInspector] public GameObject animatorGameObject; // to reset world rotation
    [HideInInspector] public Animator animator;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        animator = GetComponentInChildren<Animator>();
    }


    protected virtual void Start()
    {
        
    }


    public virtual void UpdateAnimatorLocomotionParameters()
    {
        float carAngle    = transform.eulerAngles.z;
        float snappedCar  = Mathf.Round(carAngle / 45f) * 45f;

        // Tilt the sprite by the remainder so it bridges the gap between swaps (max ±22.5°).
        animatorGameObject.transform.rotation = Quaternion.Euler(0f, 0f, carAngle - snappedCar);

        // +90° offset maps Unity's z=0-faces-up to the H/V coordinate system.
        float snapped = snappedCar + 90f;
        float x = Mathf.Round(Mathf.Cos(snapped * Mathf.Deg2Rad));
        float y = Mathf.Round(Mathf.Sin(snapped * Mathf.Deg2Rad));

        animator.SetFloat("Horizontal", x);
        animator.SetFloat("Vertical", y);
    }

}