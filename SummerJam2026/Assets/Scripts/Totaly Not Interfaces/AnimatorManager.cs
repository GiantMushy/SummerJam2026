using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public CharacterManager character;
    public Animator animator;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        
    }
}