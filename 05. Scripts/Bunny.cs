using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{
    //로직에 사용되는 변수
    public bool isRun; //달리고 있는중인 것을 확인하기 위한 bool값 변수
    public float maxThrowDelay; //검을 던지기 위한 딜레이 변수(최대값)
    public float curThrowDelay; //검을 던지기 위한 딜레이 변수(현재값)

    public GameObject sword; //Bunny가 던지는 검 오브젝트 변수
    Vector2 offSet = new Vector2(1.5f, 0); //position 값을 조정하기 위한 Vector2 변수
    Rigidbody2D rigid; //Rigidbody2D 컴포넌트의 변수

    void Awake ()
    {
        //로직에 사용되는 Component 초기화
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //지속적으로 실행되어야 하는 함수를 Update함수에서 실행
        BunnyRay();
        BunnyMove();
        Reload();
        ThrowSword();
    }
    void BunnyMove() //Bunny 캐릭터가 움직이는 함수 로직
    {
        if (isRun) //bool값 isRun이 true일때
        {
            rigid.gravityScale = 1; //RigidBody2D의 gravityScale을 1로 설정하여 중력의 영향을 받는 로직
            rigid.velocity = new Vector2(4, rigid.velocity.y); //Rigidbody2D 변수의 속도의 x값에 4를 대입
        }
     }
    void BunnyRay()//Bunny 캐릭터가 플레이어를 감지하기 위해 Ray를 사용하는 로직
    {
        Debug.DrawRay(rigid.position - offSet, Vector3.left * 10f, new Color(0, 1, 0)); //Debug를 사용하여 Unity의 Scene 화면에서 Ray를 확인
        //RigidBody2D의 position에서 offSet변수의 Vector2 값을 뺀 위치에서 왼쪽으로 Ray를 발사하고 충동한 대상의 Layer가 Player경우 rayHit에 저장
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position - offSet, Vector3.left, 10f, LayerMask.GetMask("Player")); 

            if (rayHit.collider != null)//rayHit와 충돌한 대상이 null이 아니면 실행(위에서 Layer가 Player인 경우에만 저장함으로 Player만 충돌)
            {
                if (rayHit.collider.tag == "Player") //충돌 대상의 태그가 Player이면 실행
                {
                isRun = true; //bool값 isRun을 true로 설정
                }
            }
     }
    void ThrowSword() //Bunny 캐릭터가 Sword를 생성 및 던지는 함수 로직
    {
        if(isRun && curThrowDelay > maxThrowDelay) //isRun이 true이고 curThrowDelay가 maxThrowDelay보다 크면 실행
        {
            //Instantiate 함수를 사용하여 sword 변수에 저장되어있는 오브젝트를 지정된 위치에 생성하고 그 것을 변수 rollingSword에 저장
            GameObject rollingSword = Instantiate(sword, transform.position + Vector3.up *0.25f + Vector3.left * 1f, transform.rotation);
            Rigidbody2D swordRigid = rollingSword.GetComponent<Rigidbody2D>(); //rollingSword의 Rigidbody2D 컴포넌트를 swordRigid 변수에 가져온다.
            swordRigid.velocity = new Vector2(-5, rigid.velocity.y); //Rigidbody2D 변수의 속도의 x값에 -5를 대입(왼쪽으로 이동키시기 위하여 x에 음수를 대입)

            curThrowDelay = 0; //칼이 계속 생성되는 것을 막기 위하여 curThrowDelay 0으로 설정
        }
    }
   void Reload()//칼을 주기적으로 생성하기 위해 curThrowDelay를 설정하는 함수 로직
    {
        if(isRun)//Bunny 캐릭터가 달리는 중일때
        curThrowDelay += Time.deltaTime;//curThrowDelay에 Time.deltaTime값을 더한다.
    } 
 }

