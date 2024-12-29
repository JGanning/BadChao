using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    // Rotating variables
    public Vector3 oscillateFrom;
    public Vector3 oscillateTo;
    public float speed = 1.0f;
    public float rotationsPerMinute = 10.0f;

    private void Awake()
    {
        oscillateFrom = new Vector3(0f, transform.position.y - 0.2f, 0);
        oscillateTo = new Vector3(0f, transform.position.y + 0.2f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
        Vector3 giftPos = Vector3.Lerp(oscillateFrom, oscillateTo, t);
        transform.position = new Vector3(transform.position.x, giftPos.y, transform.position.z);
        transform.Rotate(0, 6 * rotationsPerMinute * Time.deltaTime, 0);
    }
}
