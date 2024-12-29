using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Item/Equipment")]
public class EquipmentScriptableObject : ScriptableObject
{
    [SerializeField] private string equipmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private float speedBoost;
    [SerializeField] private float equipmentDuration;
    [SerializeField] private float equipmentCooldown;

    public void UseEquipment(Player player)
    {
        player.accelerationSpeed += (player.assignedAcceleration * speedBoost);

        player.equipmentDuration = equipmentDuration;
        player.equipmentCooldown = equipmentCooldown;
    }

    public void EquipmentUsed(Player player)
    {
        player.accelerationSpeed -= (player.assignedAcceleration * speedBoost);
    }
}
