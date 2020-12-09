using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //로직에 필요한 변수 
    public GameManager gameManager; //점수 및 게임의 여러가지를 관리하는 게임메니져 오브젝트 변수(스크립트를 사용하기 위하여 변수화)
    public GameObject playerCollider; //플레이어의 충돌 범위 변수
    public GameObject eBorder; //아이템 사용시 플레이어가 밑으로 떨어지는 것을 방지하기 위한 오브젝트 변수
    public float playerSpeed; //플레이어의 이동 스피스 변수
    public float jumpPower; //플레이어의 점프 파워 변수
    public int curJumpCount; //플레이어가 현재 점프를 몇번 했는지를 확인하기 위한 변수
    public int maxJumpCount; //플레이어의 최대 점프 횟수를 설정하는 변수
    public int score; //게임의 점수 변수
    Color unHit; //Hit되지 않았을 때의 색 변수
    Color onHit; //Hit되었을 때의 색 변수

    public GameObject isTriggerslide; //Slide할 때 활성화 되는 Hit가 가능한 Collider 변수
    public GameObject unTriggerSlide; //Slide할 때 활성화 되는 Hit가 불가능한 Collider 변수

    public Slider healthBar; //플레이어의 체력바

    public CapsuleCollider2D hitErea; //Slide이외의 모든 상태의 Hit가 가능한 Collider 변수
    public GameObject unTriggerHit; //Slide이외의 모든 상태에서 Hit가 불가능한 Collider 변수

    Rigidbody2D rigid; //Rigidbody2D 컴포넌트의 변수
    Animator anim; //Animator 컴포넌트의 변수
    SpriteRenderer spriteRenderer; //SpriterRenderer 컴포넌트의 변수

    public bool isHit; //Hit되었을 때 사용되는 bool 변수
    public bool isSlide; //Slide할 때 사용되는 bool변수
    public bool isFall; //맵 밑으로 떨어질 때 사용되는 bool 변수
    public bool isSpeedUp; //SpeedUp아이템을 먹었을 때 사용되는 bool 변수
    public bool isBig; //Big아이템을 먹었을 때 사용되는 bool 변수

    void Awake()
    {
        //로직에 사용되는 Component 초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitErea = GetComponent<CapsuleCollider2D>();
        

        //변수에서 설정했던 Color 변수 설정
        unHit = new Color(1, 1, 1, 0.6f);
        onHit = new Color(1, 1, 1, 1);

    }
    void Update()
    {
        //움직임(점프, 슬라이드), 착지 등 지속적으로 실행되어야 하는 함수
        //빠른 입력 반응이 필요하기 때문에 Update 함수에서 실행
        Jump();
        Ray();
        Slide();
    }
    void FixedUpdate()
    {
        //지속적으로 실행되어야 하는 함수이지만 빠른 입력이 필요하지 않은 함수들은 FixedUpdate 함수에서 실행
        Move();
        HealthBar();
    }
    void Move() //플레이어를 움직이게 하는 로직
    {
        //플레이어의 Rigidbody2D 변수의 속도의 x값에 playerSpeed변수를 대입하여 플레이어를 움직인다.
        rigid.velocity = new Vector2(playerSpeed, rigid.velocity.y);
    }
    void Ray()//플레이어의 무한점프 방지 및 낙하 이벤트 로직
    {
        /*Landing Platform
        플레이어의 아래쪽으로 Ray를 발사하여 Layer가 Platform인 값을 rayHit변수에 저장하고 
        rayHit가 null이 아니고 rayHit의 태그가 Floor이면 바닥에 닿았다는 것임으로 curJumpCount를 0으로 설정한다.*/
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.7f, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.collider.tag == "Floor")
                {
                    curJumpCount = 0;
                }
            }
        }
        /*Pass Platform
        플레이어의 오른쪽으로 Ray를 발사하여 Layer가 Platform인 값을 rayHitB변수에 저장하고
        rayHitB가 null이 아니고 rayHitB의 태그가 Floor이면 Coroutine을 시작한다.*/
        Debug.DrawRay(rigid.position, Vector3.right, new Color(0, 1, 0));
        RaycastHit2D rayHitB = Physics2D.Raycast(rigid.position, Vector3.right, 0.3f, LayerMask.GetMask("Platform"));

        if(rayHitB.collider != null)
        {
            if (rayHitB.collider.tag == "Floor")
            {
                StartCoroutine(Fall());
            }
            
        }
    }
    void Jump()//플레이어의 점프 로직
    {
        if (Input.GetButtonDown("Jump") && curJumpCount < maxJumpCount) //curJumpCount가 maxJumpCount보다 작으면서 Space Bar를 눌렀을 때
        {
            //플레이어의 Rigidbody2D 변수 속도의 Vector2 값에 위쪽으로 jumpPower만큼 곱한다. -> curJumpCount를 하나 올린다. -> Animator의 doJump 트리거를 Set한다.
            rigid.velocity = Vector2.up * jumpPower;
            curJumpCount++;
            anim.SetTrigger("doJump");
        }
    }
    void Slide()//플레이어의 Slide 로직
    {
        if (Input.GetButtonDown("Vertical")) //키보드의 W, S키 또는 위, 아래 방향키가 눌렸을 때
        {
            /*Animator의 불값을 셋팅하여 애니매이션 실행 -> 변수 isSlide를 true로 설정
              -> Slide 상태에 사용되지 않는 Collider는 비활성화 -> Slide 상태에서 사용되는 Collider 활성화*/
            anim.SetBool("isSlide", true);
            isSlide = true;
            hitErea.enabled = false;
            isTriggerslide.SetActive(true);
            unTriggerSlide.SetActive(true);
            unTriggerHit.SetActive(false);
        }

        if (Input.GetButtonUp("Vertical"))//키보드의 W, S키 또는 위, 아래 방향키를 땠을 때
        {
            /*Animator의 불값을 셋팅하여 애니매이션 종료 -> 변수 isSlide를 false로 설정
              -> Slide 상태에서 사용했던  Collider는 비활성화 -> 평소 상태에서 사용되는 Collider 재활성화*/
            anim.SetBool("isSlide", false);
            isSlide = false;
            hitErea.enabled = true;
            isTriggerslide.SetActive(false);
            unTriggerSlide.SetActive(false);
            unTriggerHit.SetActive(true);
        }
    }
    void HealthBar()//체력바 관리 로직(FixedUpdate에서 주기적으로 실행)
    {
        if(healthBar.value == 0) //Slide인 healthBar의 value값이 0이 되면 실행
        {
            gameManager.GameOver();//gameManager의 GameOver 함수 실행
        }
        else 
        {
            healthBar.value -= 0.0003f;//healthBar의 value값에서 0.0002씩 빠지는 로직
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Hurdle")//충돌 대상의 태그가 Enemy이거나 Hurdle 이면 실행되는 로직
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();//충돌이 발생한 오브젝트의 Enemy 스트립트를 가져오는 로직(Enemy  스크립트의 변수 및 함수를 사용하기 위해)
            if (!isSpeedUp && !isBig)
            {
                if (isHit) //이중 데미지를 방지하기 위한 조건 추가
                    return;
                isHit = true;//적과 충돌시 bool값 isHit를 true로 하여 바로 다시 충돌하였을때 데미지를 연속으로 입는것을 방지하기 위한 bool값
                switch (enemy.type)//enemy의 타입별 healthBar의 감소량을 다르게 하기 위하여 switch ~ case 문 사용
                {
                    case "Monster":
                        healthBar.value -= 0.04f;
                        break;
                    case "Frog":
                        healthBar.value -= 0.04f;
                        break;
                    case "Eagle":
                        healthBar.value -= 0.04f;
                        break;
                    case "Hurdle":
                        healthBar.value -= 0.02f;
                        break;
                    case "Swing":
                        healthBar.value -= 0.03f;
                        break;
                }
                spriteRenderer.color = new Color(1, 0, 0, 0.6f); //플레이어의 spriteRenderer의 색을 변화
                if (isSlide) //Slide 중에 Hit 되었을 때에는 Slide중에  Hit가 가능한 Collider 변수 isTriggerslide를 비활성화
                    isTriggerslide.SetActive(false);
                else //Slide 중이 아닌경우에는 충돌 범위 변수인 HitErea를 비활성화
                {
                    hitErea.enabled = false;
                }
                Invoke("ReturnColor", 2f); //2초 뒤에 ReturnColor 함수를 실행
            }
            else if(isSpeedUp || isBig) 
            {
                score += enemy.enemyScore; //플레이어의 score변수에 enemy 스크립트의 enemyScore변수를 더한다.
                enemy.StartCoroutine("EnemyDie"); // enemy 스크립트의 EnemyDie Coroutine을 실행
            }
        }
        else if (collision.gameObject.tag == "Item") //충돌 대상의 태그가 Item이면 실행되는 로직
        {
            Item item = collision.gameObject.GetComponent<Item>();//충돌이 발생한 오브젝트의 Item 스트립트를 가져오는 로직(Item  스크립트의 type 변수를 사용하기 위해)
            switch (item.type)//item의 타입별 score 증가량을 다르게 하기 위하여 switch ~ case 문 사용
            {
                case "Bronze":
                    score += 1000;
                    break;
                case "Silver":
                    score += 2000;
                    break;
                case "Gold":
                    score += 3000;
                    break;
                case "Jam":
                    score += 5000;
                    break;
                case "Heal":
                    if (healthBar.value == 1) //Heal 아이템을 먹었지만 플레이어의 healthBar가 가득 차있을 경우에는 score를 더한다.
                        score += 5000;
                    else 
                        healthBar.value += 0.03f;
                    break;
                case "BigHeal":
                    if (healthBar.value == 1)
                        score += 10000;
                    else
                        healthBar.value += 0.08f;
                    break;
                case "Big":
                    StartCoroutine(BigItem()); //BigItem Coroutine을 실행하는 로직
                    break;
                case "SpeedUp":
                    StartCoroutine(SpeedUpItem()); //SpeedUp Coroutine을 실행하는 로직
                    break;
            }
            Destroy(collision.gameObject); //충돌한 오브젝트를 파괴하는 로직
        }
        else if (collision.gameObject.tag == "Border")//충돌 대상의 태그가 Border이면 실행되는 로직
        {
            healthBar.value -= 0.04f;
            StartCoroutine(Climb());
        }
        else if (collision.gameObject.tag == "Sword")//충돌 대상의 태그가 Sword이면 실행되는 로직
        {
            healthBar.value -= 0.03f;
            spriteRenderer.color = new Color(1, 0, 0, 0.6f);
            if (isSlide)
                isTriggerslide.SetActive(false);
            else
            {
                hitErea.enabled = false;
            }
            Invoke("ReturnColor", 2f);
        }
    }
    void ReturnColor() //Hit된후 spriteRenderer의 색을 원래대로 되돌리기 위한 함수(Invoke 함수를 이용하여 Hit 된 후 일정 시간 후에 함수 실행)
    {
        spriteRenderer.color = onHit;
        isHit = false;
        if (isSlide)
            isTriggerslide.SetActive(true);
        else
            hitErea.enabled = true;
    }

    IEnumerator BigItem() //플레이어의 크기가 커지는 아이템을 먹었을때 실행되는 Coroutine
    {
        //Coroutine 실행 과정
        /* bool값 isBig을 true로 설정 -> 플레이어의 localScale을 4배로 설정 ->  플레이어 position의 y값을 새로 설정(localScale이 증가하면 플레이어의 중심을 기준으로 커지기 때문)
         -> 낙하하는 것을 방지하기 위해 eBorder 활성화 -> 3초 뒤에 플레이어의 색을 불투명하게 설정 -> 플레이어의 localScale을 다시 1배로 설정
         -> 플레이어의 position의 y값을 원래대로 설정 -> 피격 범위 변수 hitErea 비활성화 -> curJumpCount를 0으로 설정 -> 1초뒤 bool값 isBig을 false로 설정 
         -> 플레이어의 색을 다시 하얀색으로 설정 -> 피격 범위 변수 hitErea 재활성화 -> 낙하 방지를 위해 활성화 했던 eBorder를 비활성화 */
        yield return new WaitForSeconds(0.01f);
        isBig = true;
        gameObject.transform.localScale = new Vector3(4, 4, 4);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2.2f, gameObject.transform.position.z);
        eBorder.SetActive(true);
        yield return new WaitForSeconds(3f);
        spriteRenderer.color = new Color(1, 1, 1, 0.6f);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.7f, gameObject.transform.position.z);
        hitErea.isTrigger = false;
        curJumpCount = 0;
        yield return new WaitForSeconds(1f);
        isBig = false;
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        hitErea.isTrigger = true;
        eBorder.SetActive(false);
        yield return new WaitForSeconds(0.01f);
    }
    IEnumerator SpeedUpItem() //플레이어의 속도가 빨라지는 아이템을 먹었을때 실행되는 Coroutine
    {
        //Coroutine 실행 과정
        /* playerSpeed 변수를 8로 설정(평소 설정 = 4) -> 낙하하는 것을 방지하기 위해 eBorder 활성화 -> bool값 isSpeedUp을 true로 설정 
         -> 4초 뒤 플레이어의 색을 불투명하게 설정 -> playerSpeed변수를 4로 설정 -> 피격 범위 변수 hitErea 비활성화 -> curJumpCount를 0으로 설정
         -> 1초 뒤 bool값 isSpeedUp을 false로 설정 -> 피격 범위 변수 hitErea 재활성화 -> 플레이어의 색을 다시 하얀색으로 설정 -> 낙하 방지를 위해 활성화 했던 eBorder를 비활성화 */
        yield return new WaitForSeconds(0.01f);
        playerSpeed = 8;
        eBorder.SetActive(true);
        isSpeedUp = true;
        yield return new WaitForSeconds(4f);
        spriteRenderer.color = new Color(1, 1, 1, 0.6f);
        playerSpeed = 4;
        hitErea.isTrigger = false;
        curJumpCount = 0;
        yield return new WaitForSeconds(1f);
        isSpeedUp = false;
        hitErea.isTrigger = true;
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        eBorder.SetActive(false);
        yield return new WaitForSeconds(0.01f);

    }
    IEnumerator Fall() //플레이어가 앞쪽으로 맵과 충돌했을 때 실행되는 Coroutine
    {
        //플레이어의 오른쪽 방향으로 Ray 발사하여 충돌한 대상의 Layer가 Platform이고 tag가 Floor이면 플레이어가 0.5초간 맵을 통과할수 있도록 해주는 로직
        yield return new WaitForSeconds(0.001f);
        Debug.DrawRay(rigid.position, Vector3.right, new Color(0, 1, 0));
        RaycastHit2D rayHitB = Physics2D.Raycast(rigid.position, Vector3.right, 0.3f, LayerMask.GetMask("Platform"));
        CapsuleCollider2D playerCap;
        playerCap = playerCollider.GetComponent<CapsuleCollider2D>();
        if (rayHitB.collider != null)
        {
            if (rayHitB.collider.tag == "Floor")
            {
                playerCap.isTrigger = true;
                yield return new WaitForSeconds(0.5f);
                playerCap.isTrigger = false;
            }
        }
    }
    IEnumerator Climb() //플레이어가 맵의 바닥 및으로 떨어졌을 때 실행되는 Coroutine
    {
        //Coroutine 실행 과정
        /* 1초 뒤 플레이어의 position의 y좌표를 5로 이동 -> 0.5초 뒤 플레이어의 색을 unHit 변수에 저장된 색으로 바꾼다 ->  낙하하는 것을 방지하기 위해 eBorder 활성화
          -> 2초 뒤 플레이어의 색을 onHit 변수에 저장된 색으로 바꾼다. -> 낙하 방지를 위해 활성화 했던 eBorder를 비활성화  */
        yield return new WaitForSeconds(1f);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5f, gameObject.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = unHit;
        yield return new WaitForSeconds(0.001f);
        eBorder.SetActive(true);
        yield return new WaitForSeconds(2f);
        spriteRenderer.color = onHit;
        yield return new WaitForSeconds(1f);
        eBorder.SetActive(false);
        yield break;

    }
    void HitEreaReturn()//플레이어의 충돌을 담당하는 Collider를 활성화 시키는 함수 로직
    {
        hitErea.enabled = true;
    }
}
