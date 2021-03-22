using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTarget : MonoBehaviour
{

    public Vector3[] positions;
    public int positionIndex;

    // Start is called before the first frame update
    void Start()
    {
        positions = new Vector3[4] { 
            new Vector3(3.21f, 1.88f, -1.18f), 
            new Vector3(3.21f, 1.18f, 2f), 
            new Vector3(-3.21f, 1.18f, -1.18f), 
            new Vector3(-3.21f, 1.18f, 2f) 
        };
        positionIndex = 0;
        gameObject.transform.position = positions[positionIndex];
        InvokeRepeating("UpdatePosition", 0, 1);
    }

    void UpdatePosition()
    {
        print("in update");
        if (positionIndex == 3) positionIndex = 0;
        else positionIndex++;
        gameObject.transform.position = positions[positionIndex];
    }
}
