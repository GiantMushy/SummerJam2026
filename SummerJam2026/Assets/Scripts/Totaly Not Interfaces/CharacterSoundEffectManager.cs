using UnityEngine;

public class CharacterSoundEffectManager : MonoBehaviour
{
    [HideInInspector] public CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        
    }
}