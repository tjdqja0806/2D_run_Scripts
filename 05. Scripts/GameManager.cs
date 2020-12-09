using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //로직에 필요한 변수 
    public Player player; //게임을 플레이 하는 플레이어 오브젝트 변수(스크립트의 변수를 사용하기 위해 변수화)
    public Slider abilityBar; //플레이어의 능력의 게이지를 보여주는 Slider 변수
    public GameObject ability; //플레이어의 능력 변수
    public GameObject bulletBorder; //플레이어의 능력으로 소환되는 총알을 막아주는 Border 변수
    public Text scoreText; //게임의 Score를 텍스트화한 변수

    public GameObject mainPanel; //게임 메인 화면 UI Panel 변수
    public GameObject gamePanel; //게임 화면 UI Panel 변수
    public GameObject overPanel; //게임 오버 화면 UI Panel 변수
    public Text bestScoreText; //게임의 Best 점수를 저장하는 변수
    public Text overScoreText; //게임 오버되면 해당 게임의 점수를 저장하는 변수
    public Text newRecoed; //신기록을 경신하면 나타나는 Text 변수

    public bool isAbility; //능력의 발동 여부를 구분하는 bool값 변수
    void Awake()
    {
        Time.timeScale = 0; //게임 시작시 게임의 시간 0으로 설정
        isAbility = false; //bool변수 초기화
        bestScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("BestScore")); //게임 메인 화면의 Best 점수를 나타내기 위한 로직(000,000형식으로 나타냄)
    }

    void LateUpdate() //LateUpdate = Update함수가 다 실행되고 나면 실행되는 함수
    {
        AbilityBar();
        scoreText.text = string.Format("{0:n0}", player.score); //플레이어 스크립트의 score 변수에 있는 값을 000,000형식으로 scoreText 변수의 Text에 표시하는 로직
    }

    void AbilityBar()
    {
        if(!isAbility)
            abilityBar.value += 0.00009f;//능력이 발동중이 아니라면 abilityBar의 value의 값에 0.0001씩 더한다.
        else 
        {
            abilityBar.value -= 0.00025f;//능력이 발동중이면 abilityBar의 value의 값에서 0.00025씩 뺀다.

            if (abilityBar.value == 0) //abilityBar의 value의 값이 0이 되면 1초 뒤에 AbilityEnd 함수를 실행시키는 로직
            {
                ability.SetActive(false);//능력을 비활성화
                Invoke("AbilityEnd", 1f);
            }
        }

        if (abilityBar.value == 1) //abilityBar의 value의 값이 1이 되면 능력 이벤트를 실행시키는 로직
        {
            ability.SetActive(true); //능력을 활성화
            isAbility = true; //능력이 발동중인것을 구분하기 위하여 bool 값을 true로 설정
            bulletBorder.SetActive(true); //능력에서 발사되는 총알을 맵에서 사라지게 해주는 Border를 활성화
            player.hitErea.enabled = false; //능력중에 Hit가 되지 않도록 플레이어 스크립트의 충돌을 담당하는 Collider를 비활성화 
        }
    }

    void AbilityEnd()//능력이 끝날 때 실행되는 함수 로직
    {
        isAbility = false;//능력의 발동이 끝났으니 bool 값을 false로 설정
        bulletBorder.SetActive(false); //능력이 사라지니 총알이 발사될 일이 없으니 Border를 비활성화
    }
    public void GameStart() //게임 메인 화면의 GameStart 버튼을 누르면 실행되는 함수 로직
    {
        Time.timeScale = 1; //게임의 시간 1로 설정
        mainPanel.SetActive(false); //게임 메인 화면 UI Panel을 비활성화
        gamePanel.SetActive(true); //게임 화면 UI Panel을 활성화
        player.gameObject.SetActive(true); //비활성화 되어있던 플레이어를 활성화
    }
    public void GameOver() //플레이어의 채력이 0이 되어 GameOver가 되면 실행되는 로직
    {
        Time.timeScale = 0; //게임의 시간을 0으로 설정
        gamePanel.SetActive(false); //게임 화면 UI Panel을 비활성화
        overPanel.SetActive(true); //게임 오버 화면 UI Panel을 활성화
        player.gameObject.SetActive(false); //플레이어 오브젝트를 비활성화
        overScoreText.text = string.Format("{0:n0}", player.score); //게임 점수를 overScoreText의 text에 000,000형식으로 저장
        int prfabsScore = PlayerPrefs.GetInt("BestScore"); //PlayerPrefs 안의 BestScore를 prfabsScore 변수에 저장
        if (player.score > prfabsScore) //플레이어 스크립트의 score 변수 값이 prfabsScore 값보다 크면 실행되는 로직
        {
            newRecoed.gameObject.SetActive(true); //신기록 갱신 시 나타나는 오브젝트 활성화
            PlayerPrefs.SetInt("BestScore", player.score); //PlayerPrefs 안의 BestScore에 플레이어 스크립트의 score값을 저장
        }
    }
    public void MainTitle() //게임 오버 화면의 버튼을 누르면 Scene을 새로 Load하여 다시 실행시키는 함수 로직(Unity의 Click기능을 사용하여 버튼에 기능을 이식)
    {
        SceneManager.LoadScene(0); //Scene(0)번을 로드
    }
}
