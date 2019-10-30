using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I am acting as a Trigger for: " + other.gameObject);
    }

    private void OnCollisionStay(Collision other) {
        Debug.Log("I will print this message for a long time.");
    }
}
