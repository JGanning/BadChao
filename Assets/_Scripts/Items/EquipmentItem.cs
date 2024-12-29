using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : MonoBehaviour
{
    [SerializeField] private EquipmentScriptableObject[] equipmentArray;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EquipmentScriptableObject pickedUpEquipment = equipmentArray[Random.Range(0, equipmentArray.Length - 1)];
            other.gameObject.GetComponentInParent<Player>().EquipItem(pickedUpEquipment);
            Destroy(gameObject);
        }
    }
}
