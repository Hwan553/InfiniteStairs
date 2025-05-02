using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{

    public static CharacterManager Instance { get; private set; }
    public CharacterData[] _characters = new CharacterData[4];
    private int playerCoins;
    private int selectedCharacterIndex = 0;
    private GameObject spawnedCharacter; // 생성된 캐릭터를 저장하는 변수 추가

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("KiwiScore", 600);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacterData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 저장된 캐릭터 데이터와 코인 정보를 불러오는 함수
    void LoadCharacterData()
    {
        // 저장된 코인(KiwiScore)을 불러오고 없으면 기본값 600 세팅
        playerCoins = PlayerPrefs.GetInt("KiwiScore", 600);

        // 모든 캐릭터를 순회
        for (int i = 0; i < _characters.Length; i++)
        {
            // PlayerPrefs에 캐릭터가 구매된 상태(1)로 저장되어 있으면 구매된 것으로 표시
            if (PlayerPrefs.GetInt(_characters[i].characterName, 0) == 1)
            {
                _characters[i].isPurchased = true;
            }
        }

        // 첫 번째 캐릭터(기본 캐릭터)는 항상 구매된 상태로 보장
        if (!_characters[0].isPurchased)
        {
            _characters[0].isPurchased = true;
            PlayerPrefs.SetInt(_characters[0].characterName, 1); 
        }
    }

    // 캐릭터를 구매하는 함수
    public bool BuyCharacter(int index)
    {
        // 인덱스 0(기본 캐릭터)은 구매 불가
        if (index == 0) return false;

        // 구매하지 않은 캐릭터이고 플레이어 코인이 가격 이상인 경우
        if (!_characters[index].isPurchased && playerCoins >= _characters[index].price)
        {
            // 코인을 차감하고
            playerCoins -= _characters[index].price;

            // 변경된 코인 수 저장
            PlayerPrefs.SetInt("KiwiScore", playerCoins);

            // 캐릭터 구매 상태 저장
            PlayerPrefs.SetInt(_characters[index].characterName, 1);

            // 변경사항 저장
            PlayerPrefs.Save();

            // 데이터 다시 로드해서 적용
            LoadCharacterData();
            return true;// 구매 성공
        }
        return false; // 구매 실패
    }

    // 선택된 캐릭터 데이터를 반환하는 함수
    public CharacterData GetSelectedCharacter()
    {
        // 선택된 캐릭터 인덱스 가져오기, 기본값은 0
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);

        // 인덱스 범위 체크
        if (selectedIndex < 0 || selectedIndex >= _characters.Length)
            return null;

        // 선택된 캐릭터가 구매된 상태인지 다시 확인하여 반영
        _characters[selectedIndex].isPurchased = PlayerPrefs.GetInt(_characters[selectedIndex].characterName, 0) == 1;

        // 선택된 캐릭터 반환
        return _characters[selectedIndex];
    }

    // 선택된 캐릭터 프리팹을 반환하는 함수
    public GameObject GetSelectedCharacterPrefab()
    {
        // 선택된 캐릭터 인덱스 가져오기
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);

        // 인덱스 유효성 체크
        if (selectedIndex < 0 || selectedIndex >= _characters.Length)
        {
            return null;
        }
        // 선택된 캐릭터의 프리팹(GameObject)반환
        return _characters[selectedIndex]._characterPrefab;
    }

    // 생성된 캐릭터를 저장하는 함수
    public void SetSpawnedCharacter(GameObject character)
    {
        spawnedCharacter = character;
    }

    // 플레이어 이동 요청 함수
    public void MovePlayer()
    {
        // 생성된 캐릭터가 존재할 때만 실행
        if (spawnedCharacter != null)
        {
            // 생성된 캐릭터에서 PlayerController 컴포넌트를 가져와 PlayerMove() 실행
            spawnedCharacter.GetComponent<PlayerController>().PlayerMove(); 
        }

    }

    // 플레이어 방향전환 요청 함수
    public void TurnPlayer()
    {
        // 생성된 캐릭터가 존재할 때만 실행
        if (spawnedCharacter != null)
        {
            // 생성된 캐릭터에서 PlayerController 컴포넌트를 가져와 PlayerMove() 실행
            spawnedCharacter.GetComponent<PlayerController>().PlayerTurn(); 
        }

    }

    public void ReStartPlayer()
    {
        if (spawnedCharacter != null)
        {
            spawnedCharacter.GetComponent<PlayerController>().RestartButton();
        }
    }

    // 플레이어의 코인 상태를 반환하는 함수
    public int GetPlayerCoins()
    {
        return playerCoins;
    }

    // 캐릭터 선택 함수
    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= _characters.Length) return;

        selectedCharacterIndex = index;
        PlayerPrefs.SetInt("SelectedCharacterIndex", index);
        PlayerPrefs.Save();
    }

}


