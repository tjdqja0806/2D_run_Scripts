using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Vector3 offSet;
    public Transform target;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        offSet = new Vector3(target.position.x + 4f, 2f, -10);
        gameObject.transform.position = offSet;
    }
}
