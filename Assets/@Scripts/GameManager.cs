using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Bson;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] Stairs;
    public bool[] isTurn;
    public AudioClip gameoverSound;


    private enum State { Start, Left, Right };
    private State state;
    private Vector3 oldPosition;
    private int kiwiScore = 0;
    private AudioSource audioSource;
    private bool hasPlayedGameOverSound = false;

    public GameObject _gameOver;
    public TMP_Text nowScoreText, bestScoreText, scoreText, kiwiScoreText;
    public int maxScore = 0;
    public int nowScore = 0;
    

    public UnityEngine.UI.Image _gauge;
    public bool _gaugeStart = false;
    float gaugeRedcutionRate = 0.0015f;


    void Awake()
    {
        Instance = this;
        Init();
        InitStairs();
        StartCoroutine("CheckGauge");
        GaugeReduce();
        LoadKiwiScore();
        audioSource = GetComponent<AudioSource>();

    }

   
   
    public void Init()
    {
        state = State.Start;
        oldPosition = Vector3.zero;
        isTurn = new bool[ Stairs.Length];
        for (int i = 0; i < Stairs.Length; i++)
        {
            Stairs[i].transform.position = Vector3.zero;
            isTurn[i] = false;
        }
        nowScore = 0;
        scoreText.text = nowScore.ToString();
        _gameOver.SetActive(false);
        _gauge.fillAmount = 1f;
        gaugeRedcutionRate = 0.0015f;

    }
    // 초기 계단 배치 함수
    public void InitStairs()
    {
        // Stairs 배열의 모든 계단 오브젝트를 순회
        for (int i = 0; i < Stairs.Length; i++)
        {
            // 현재 state에 따라 계단 위치를 설정
            switch (state)
            {
                case State.Start:
                    // 시작 상태면 약간 오른쪽(0.75f) 아래(-0.1f로)계단 배치
                    Stairs[i].transform.position = oldPosition + new Vector3(0.75f, -0.1f, 0);
                    // 다음 계단은 오른쪽 방향 기준으로 설정
                    state = State.Right;
                    break;
                case State.Left:
                    // 왼쪽 방향일 경우, 왼쪽(-0.75f) 위로(0.5f) 계단 배치
                    Stairs[i].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                    // 해당 계단은 방향 전환 계단임을 표시
                    isTurn[i] = true;
                    break;

                case State.Right:
                    // 오른쪽 방향일 경우, 오른쪽(0.75f) 위로(0.5f) 계단 배치
                    Stairs[i].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                    // 방향 전환계단이 아님을 표시
                    isTurn[i] = false;
                    break;
            }
            // 현재 계단의 위치를 다음 계단 배치 기준점(oldPosition)으로 저장
            oldPosition = Stairs[i].transform.position;

            // 첫 계단이 아니라면
            if (i != 0)
            {
                // 0 ~ 4 사이 랜덤 값 생성
                int ran = Random.Range(0, 5);

                // 0 ~ 1이면 방향 전환(확률 40%)
                if (ran < 2 && i < Stairs.Length - 1)
                {
                    // 현재 방향과 반대 방향으로 전환
                    state = state == State.Left ? State.Right : State.Left;
                }
            }
        }
    }

    // 특정 인덱스에 계단을 생성하는 함수
    public void SpawnStair(int count)
    {
        // 0 ~ 4 사이 랜덤 값 생성
        int ran = Random.Range(0, 5);

        // 0 ~ 1이면 방향 전환(확률 40%)
        if (ran < 2)
        {
            state = state == State.Left ? State.Right : State.Left;

        }

        // 현재 방향 상태에 따라 계단 위치 설정
        switch (state)
        {

            case State.Left:
                // 왼쪽 방향으로 이동하여 계단 배치
                Stairs[count].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                isTurn[count] = true;
                break;

            case State.Right:
                // 오른쪽 방향으로 이동하여 계단 배치
                Stairs[count].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                isTurn[count] = false;
                break;
        }
        // 배치한 계단의 위치를 새로운 기준점으로 저장
        oldPosition = Stairs[count].transform.position;
    }


    public void AddScore()
    {
        nowScore++;
        scoreText.text = nowScore.ToString();

    }

    // 게이지를 증가시키는 함수
    public void GaugeIncrease()
    {
        // 게이지 fillAmount를 0.05(5%)만큼 증가
        _gauge.fillAmount += 0.05f;
        // 만약 게이지가 1(100%)를 넘으면 1로 고정(오버플로 방지)
        if (_gauge.fillAmount > 1f) _gauge.fillAmount = 1f;
    }


    // 게이지를 서서히 감소시키는 함수
    public void GaugeReduce()
    {
        // 게이지 감소가 시작된 경우에만 실행
        if (_gaugeStart)
        {
            // 현재 점수(nowScore)에 따라 게이지 감소 속도를 조정
            if (nowScore > 30) gaugeRedcutionRate = 0.0033f;
            if (nowScore > 60) gaugeRedcutionRate = 0.0037f;
            if (nowScore > 100) gaugeRedcutionRate = 0.0043f;
            if (nowScore > 150) gaugeRedcutionRate = 0.005f;
            if (nowScore > 200) gaugeRedcutionRate = 0.005f;
            if (nowScore > 300) gaugeRedcutionRate = 0.0065f;
            if (nowScore > 400) gaugeRedcutionRate = 0.0075f;

            // 현재 감소율만큼 게이지 줄이기
            _gauge.fillAmount -= gaugeRedcutionRate;

            // 만약 게이지가 0 이하로 떨어지면
            if (_gauge.fillAmount <= 0)
            {
                // 0으로 고정
                _gauge.fillAmount = 0;
                // 게임오버 처리
                GameOver();
            }
        }
        // 0.02초 후 GaugeReduce()를 다시 호출(반복적으로 게이지 감소)
        Invoke("GaugeReduce", 0.02f);
    }

    // 게이지를 감시하는 코루틴
    IEnumerator CheckGauge()
    {
        // 게이지가 0이 아닌 동안 반복
        while (_gauge.fillAmount != 0)
        {
            // 0.4초 동안 대기
            yield return new WaitForSeconds(0.4f);
        }
        // 게이지가 0이 되면 게임오버 처리
        GameOver();
    }

    public void GameOver()
    {
        CancelInvoke();
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        PlayGameOverSound();

        yield return new WaitForSeconds(1f);
        _gameOver.SetActive(true);

        if (nowScore > maxScore)
        {
            maxScore = nowScore;
        }

        bestScoreText.text = maxScore.ToString();
        nowScoreText.text = nowScore.ToString();

    }

    public void AddKiwiScore(int amount)
    {
        kiwiScore += amount;
        PlayerPrefs.SetInt("KiwiScore", kiwiScore);
        PlayerPrefs.Save();
        UpdateKiwiScoreUI();
    }

    private void UpdateKiwiScoreUI()
    {
        kiwiScoreText.text = $":{kiwiScore}";
    }

    private void LoadKiwiScore()
    {
        kiwiScore = PlayerPrefs.GetInt("KiwiScore", 0);
        UpdateKiwiScoreUI();
    }

    private void PlayGameOverSound()
    {
        if (!hasPlayedGameOverSound && gameoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameoverSound);
            hasPlayedGameOverSound = true;
        }
    }




}
