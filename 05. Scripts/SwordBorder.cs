﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBorder : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Sword")
        {
            Destroy(collision.gameObject);
        }
    }
}
