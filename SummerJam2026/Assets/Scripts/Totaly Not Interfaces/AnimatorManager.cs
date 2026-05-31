using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [HideInInspector] public CharacterManager character;
    [HideInInspector] public Animator animator;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        
    }
}