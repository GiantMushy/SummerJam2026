using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        
    }
}