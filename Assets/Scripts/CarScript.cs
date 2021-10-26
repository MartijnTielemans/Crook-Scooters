using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public Vector3 moveSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Car Hit obstacle");
            moveSpeed = new Vector3(0, 0, 0);
        }
    }
}
