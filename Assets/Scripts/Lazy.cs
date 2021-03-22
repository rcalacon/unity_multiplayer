using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazy : MonoBehaviour
{
    public Vector3 newPosition;

    void Start()
    {
        InvokeRepeating("SetRandomPos", 0, 1);
    }

    void SetRandomPos()
    {
        newPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(1f, 3f), Random.Range(-5f, 5f));
        gameObject.transform.position = newPosition;
    }

}
