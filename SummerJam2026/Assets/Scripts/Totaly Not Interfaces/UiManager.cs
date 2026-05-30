using UnityEngine;

public class UiManager : MonoBehaviour
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