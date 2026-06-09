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
        float angle = transform.eulerAngles.z + 90f; // 90 because 90 = left in this setup

        if (angle > 180f) angle -= 360f;

        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);

        // Snap to -1, 0, 1
        x = Mathf.RoundToInt(x);
        y = Mathf.RoundToInt(y);

        animator.SetFloat("Horizontal", x);
        animator.SetFloat("Vertical", y);
    }

}