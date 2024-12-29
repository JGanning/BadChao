using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Item/Pickup")]
public class PickupScriptableObject : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private float speedBoost = 0;
    [SerializeField] private float turningBoost = 0;

    public void StatBoost(Player player)
    {
        Debug.Log("Boosting stats with " + itemName);
        player.accelerationSpeed += (player.assignedAcceleration * speedBoost);
        player.turnStrength += (player.assignedTurning * turningBoost);
    }
}
