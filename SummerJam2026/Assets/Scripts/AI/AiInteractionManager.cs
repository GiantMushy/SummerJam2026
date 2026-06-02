using System;
using UnityEngine;

public class AiInteractionManager : InteractionManager
{
    public event Action TriggerChase;
    public event Action TriggerAttached;
    public event Action TriggerLeaveRange;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            // Replace with TriggerAttach when that actually does something
            character.Die();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player Trigger Radius"))
        {
            TriggerChase?.Invoke();
        }
        else if (other.CompareTag("Eclipse"))
        {
            character.Die();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player Trigger Exit Radius"))
            TriggerLeaveRange?.Invoke();
    }
}
