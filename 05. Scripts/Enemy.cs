using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //로직에 필요한 변수 
    public GameObject player; //게임을 플레이 하는 플레이어 오브젝트 변수(스크립트의 변수를 사용하기 위해 변수화)
    public string type; //enemy의 type을 구분하기 위한 변수
    public int enemyScore; //enemy가 가지고 있는 score 변수
    public int nextMove; //Moster type의 enemy의 다음 움직임을 위해 사용되는 int형 변수
    
    bool isDie; //enemy가 파괴되었음을 구분하기위한 bool값 변수
    public bool isEagle; //enemy의 type을 구분하기 위한 bool값 변수
    bool isBunny; //enemy의 type을 구분하기 위한 bool값 변수

    Rigidbody2D rigid; //Rigidbody2D 컴포넌트의 변수
    SpriteRenderer spriteRenderer; //SpriterRenderer 컴포넌트의 변수
    BoxCollider2D box; //BoxCollider2D 컴포넌트의 변수
    Animator anim; //Animator 컴포넌트의 변수
    void Awake()
    {
        //로직에 사용되는 Component 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        nextMove = 1; 
        if(type == "Eagle") //type이 Eagel이면 실행되는 로직
        {
            rigid.AddForce(Vector2.left * 2f, ForceMode2D.Impulse); //Rigidbody2D Vector2값에 왼쪽으로 2만큼 힘을 가한는 로직
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        EnemyMove();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "PlayerB") //충돌 대상의 태그가 Bullet이거나 PlayerB이면 실행
        { 
            Player playerLogic; //playeLogic 변수 선언
            playerLogic = player.GetComponent<Player>(); //플레이어의 스크립트 컴포넌트를 playerLogic에 저장
            playerLogic.score += enemyScore; //플레이어 스크립트의 socre변수에 enemyScore 변수를 더한다.
            StartCoroutine(EnemyDie()); //EnemyDie Coroutine을 실행
        }
        else if(collision.gameObject.tag == "Border") //충돌 대상의 태그가 Border이면 실행
        {
            Destroy(gameObject); //enemy를 파괴
        }
    }

    public IEnumerator EnemyDie()
    {
        //Coroutine 실행 과정
        /* RigidBody2D의 gravityScale을 1로 설정 (공통 실행)
         -> [enemy의 색을 (1,0,0,1) RGB 값으로 바꾼다. -> Rigidbody2D 값에 오른쪽으로 2만큼 Impulse모드로 힘을 가한다.
         -> Rigidbody2D 값에 위쪽으로 1만큼 Impulse모드로 힘을 가한다. -> enemy의 rotation의 z좌표를 -15로 설정](enemy의 type이 Monster, Frog, Eagle 이면 실행)
         ->[Rigidbody2D 값에 오른쪽으로 15만큼 Impulse모드로 힘을 가한다.](enemy의 type이 Hurdle 일때 실행)
         -> enemy가 죽은것을 구분하기 위하여 bool값 isDie를 true로 설정 -> BoxCollider2D 변수의 isTrigger를 true로 설정 -> 1.5초뒤 오브젝트 비활성화(공통실행) */
        yield return new WaitForSeconds(0.001f);
        rigid.gravityScale = 1;
        if (type == "Monster" || type == "Frog" || type == "Eagle")
        {
            spriteRenderer.color = new Color(1, 0, 0, 1);
            rigid.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
            rigid.AddForce(Vector2.up * 1f, ForceMode2D.Impulse);
            gameObject.transform.rotation = new Quaternion(0, 0, -15f, 0);
        }
        else if(type == "Hurdle")
        {
            rigid.AddForce(Vector2.right * 15f, ForceMode2D.Impulse);
        }
        isDie = true;
        box.isTrigger = true;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
    }
    void EnemyMove()//enemy의 움직임을 구분하는 함수
    {
        if (type == "Monster" && !isDie)//enemy의 type이 Monster이고 isDie가 false 일때 실행
            MonsterMove();
        else if (type == "Frog" && !isDie)//enemy의 type이 Frog이고 isDie가 false 일때 실행
            FrogRay();
        else if (type == "Eagle" && !isDie)//enemy의 type이 Eagle이고 isDie가 false 일때 실행
            EagleRay();
    }    
    
    void MonsterMove()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        if (nextMove == 1)
        {
            spriteRenderer.flipX = true;
            StartCoroutine(NextMove());
        }
        else if(nextMove == -1)
        {
            spriteRenderer.flipX = false;
            StartCoroutine(NextMove());
        }
    }
    IEnumerator NextMove()
    {       
        yield return new WaitForSeconds(0.01f);
        if (nextMove == 1)
        {
            yield return new WaitForSeconds(1.8f);
            nextMove = -1;
            yield break;
        }
        else if (nextMove == -1)
        {
            yield return new WaitForSeconds(1.8f);
            nextMove = 1;
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
    }
    
    IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.01f);
        rigid.velocity = new Vector2(rigid.velocity.x, 7);
        yield return new WaitForSeconds(0.1f);
    }
    void FrogRay()
    {
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit3 = Physics2D.Raycast(rigid.position, Vector3.down, 0.35f, LayerMask.GetMask("Platform"));

        if (rayHit3.collider != null)
        {
            if (rayHit3.collider.tag == "Floor")
            {
                StartCoroutine(Jump());
            }
        }
    }
    IEnumerator RowFly()
    {
        yield return new WaitForSeconds(0.01f);
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        yield return new WaitForSeconds(0.01f);
        rigid.AddForce(Vector2.left * 0.25f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3f);
        rigid.velocity = new Vector2(0, 4);
        rigid.gravityScale = 2;
        yield return new WaitForSeconds(0.01f);
    }
    void EagleRay()
    {
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.85f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null)
        {
            if (rayHit.collider.tag == "Floor")
            {
                StartCoroutine(RowFly());
            }
        }
    }
 }
