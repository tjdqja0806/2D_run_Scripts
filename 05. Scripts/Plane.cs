using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    //로직에 사용되는 변수
    public Transform target; //해당 오브젝트의 활성 위치를 설정하기 위한 변수
    public Player player; //게임을 플레이 하는 Player 오브젝트 변수(스크립트의 변수를 사용하기 위해 변수화)
    public Enemy enemy; //게임에 등장하는 enemy 오브젝트 변수(스크립트의 변수를 사용하기 위해 변수화)
    public Item item; //게임에 등장하는 Item 오브젝트 변수(Item의 type을 알기 위하여 변수화)
    public float maxShotDelay; //주기적으로 총알을 쏘는 것에 사용되는 딜레이 변수(max값)
    public float curShotDelay; //주기적으로 총알을 쏘는 것에 사용되는 딜레이 변수(cur값)

    public GameObject CenterBulletA; //능력 발동 시 생성되는 총알 오브젝트 변수
    public GameObject UpBulletA; //능력 발동 시 생성되는 총알 오브젝트 변수

    void Awake()
    {
        //로직에 사용되는 Component 초기화
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        item = GetComponent<Item>();
    }

    void Update()
    {
        //지속적으로 실행되어야 하는 함수를 Update함수에서 실행
        Follow();
        Fire();
        Reload();
    }
    void Fire() //비행기의 가운데, 위 , 아래 총 3개의 총알을 생성 및 발사하는 로직
    {
        if (curShotDelay >= maxShotDelay)//총알의 무한 생성을 막기 위해 curShotDelay값이 maxShotDelay 보다 크거나 같아 졌을때에만 실행
        {
            //Instantiate 함수를 사용하여 CenterBulletA 변수에 저장되어있는 오브젝트를 지정된 위치에 생성하고 그 것을 변수 Centerbullet 저장
            GameObject Centerbullet = Instantiate(CenterBulletA, transform.position, transform.rotation);
            Rigidbody2D rigid = Centerbullet.GetComponent<Rigidbody2D>();//rollingSword의 Rigidbody2D 컴포넌트를 rigid 변수에 가져온다.
            rigid.velocity = new Vector2(18, rigid.velocity.y); //Rigidbody2D 변수의 속도의 x값에 18를 대입

            //Instantiate 함수를 사용하여 UpBulletA 변수에 저장되어있는 오브젝트를 지정된 위치에 생성하고 그 것을 변수 Upbullet 저장
            GameObject Upbullet = Instantiate(UpBulletA, transform.position + Vector3.up*0.8f, transform.rotation);
            Rigidbody2D Uprigid = Upbullet.GetComponent<Rigidbody2D>();//rollingSword의 Rigidbody2D 컴포넌트를 Uprigid 변수에 가져온다.
            Uprigid.velocity = new Vector2(18, rigid.velocity.y);//Rigidbody2D 변수의 속도의 x값에 18를 대입

            //Instantiate 함수를 사용하여 UpBulletA 변수에 저장되어있는 오브젝트를 지정된 위치에 생성하고 그 것을 변수 Downbullet 저장
            GameObject Downbullet = Instantiate(UpBulletA, transform.position + Vector3.down * 0.8f, transform.rotation);
            Rigidbody2D Downrigid = Downbullet.GetComponent<Rigidbody2D>();//rollingSword의 Rigidbody2D 컴포넌트를 Downrigid 변수에 가져온다.
            Downrigid.velocity = new Vector2(18, rigid.velocity.y);//Rigidbody2D 변수의 속도의 x값에 18를 대입

            curShotDelay = 0; //총알의 무한 생성을 막기 위해 총알 생성 후 딜레이의 cur값을 0으로 설정
        }
        else
             return;

    }
    void Reload() //총알을 주기적으로 생성하기 위해 curShotDelay 설정하는 함수 로직
    {
        curShotDelay += Time.deltaTime; //curShotDelay 값에 Time.deltaTime값을 더한다.
    }
    void Follow() //해당 오브젝트가 target 변수를 따라다니게 하는 함수 로직(Update문에서 함수를 실행하여 계속해서 position을 설정해준다.)
    {
        //해당 오브젝트의 position에 target의 position에서 Vector3값을 뺀 값을 대입하여 position 설정
        gameObject.transform.position = target.position - new Vector3(2.5f, -0.7f, 0);
    }
}
