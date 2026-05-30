using UnityEngine;

public class EquipmentManager : MonoBehaviour
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