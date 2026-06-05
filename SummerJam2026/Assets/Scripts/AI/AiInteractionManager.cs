using System;
using UnityEngine;

public class AiInteractionManager : InteractionManager
{
    public event Action TriggerChase;
    public event Action TriggerAttached;
    public event Action TriggerLeaveRange;
    public event Action TriggerEnterAttackRange;
    public event Action TriggerExitAttackRange;
    public event Action TriggerEclipse;

    private bool hasAttached = false;

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
        if (hasAttached) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasAttached = true;
            TriggerAttached?.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player Trigger Radius"))
            TriggerChase?.Invoke();
        else if (other.CompareTag("Player Attack Radius"))
            TriggerEnterAttackRange?.Invoke();
        else if (other.CompareTag("Eclipse"))
            TriggerEclipse?.Invoke();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player Trigger Exit Radius"))
            TriggerLeaveRange?.Invoke();
        else if (other.CompareTag("Player Attack Radius"))
            TriggerExitAttackRange?.Invoke();
    }
}
