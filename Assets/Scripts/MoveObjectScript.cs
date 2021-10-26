using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : MonoBehaviour
{
    public Vector3 moveSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
