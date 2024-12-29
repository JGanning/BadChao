using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private PickupScriptableObject[] pickupArray;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            pickupArray[Random.Range(0, pickupArray.Length - 1)].StatBoost(other.GetComponentInParent<Player>());
            Destroy(gameObject);
        }
    }
}
