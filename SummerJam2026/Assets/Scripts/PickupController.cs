using UnityEngine;

public class PickupController : MonoBehaviour
{
    private enum PickupType { Health, Fuel }
    [SerializeField] private PickupType pickupType = PickupType.Health;
    [SerializeField] private float pickupAmount = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacterManager player = collision.GetComponent<PlayerCharacterManager>();
        if (player == null) return;

        switch (pickupType)
        {
            case PickupType.Health:
                player.playerCombatManager.Heal(pickupAmount);
                break;
            case PickupType.Fuel:
                player.playerInventoryManager.AddFuel(pickupAmount);
                break;
        }

        Destroy(gameObject);
    }
}
