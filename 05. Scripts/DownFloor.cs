using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownFloor : MonoBehaviour
{
    Rigidbody2D rigid;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            rigid.gravityScale = 2;
            rigid.velocity = new Vector2(rigid.velocity.x, -10);
        }
        else if(collision.gameObject.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
