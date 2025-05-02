using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI_CharacterSelect : MonoBehaviour
{
    public Image _characterImage;
    public TMP_Text _characterNameText, _characterPriceText, _coinText;
    public Button leftButton, rightButton, buyButton, selectButton, startButton;
    public AudioClip clickSound;
    public AudioClip buySound;

    private int currentIndex = 0;
    private AudioSource audioSource;

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        audioSource = GetComponent<AudioSource>();
        UpdateUI();
    }

    // 현재 인덱스에 해당하는 캐릭터 정보를 UI에 반영
    void UpdateUI()
    {
        // 현재 캐릭터 데이터
        CharacterData currentCharacter = CharacterManager.Instance._characters[currentIndex];

       
        int playerCoins = PlayerPrefs.GetInt("KiwiScore", 0); // 저장된 코인 값 불러오기

        if (currentCharacter._characterPrefab != null) // 캐릭터 프리팹에서 스프라이트 이미지 가져와 표시
        {
            _characterImage.sprite = currentCharacter._characterPrefab.GetComponent<SpriteRenderer>().sprite;
        }

        // 텍스트 UI업데이트
        _characterNameText.text = currentCharacter.characterName;
        _characterPriceText.text = currentCharacter.isPurchased ? "Owned" : $"Price: {currentCharacter.price}";
        _coinText.text = $"Coins: {playerCoins}";

        if (currentIndex == 0) // 0번 캐릭터(기본 캐릭터)는 구매 버튼 숨김
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            // 구매하지 않은 캐릭터에만 구매 버튼 표시
            buyButton.gameObject.SetActive(!currentCharacter.isPurchased);
        }

        // 구매한 캐릭터에만 선택 버튼 표시
        selectButton.gameObject.SetActive(currentCharacter.isPurchased);
    }

    // 왼쪽 화살표 클릭 시 호출
    public void MoveLeft()
    {
        PlayClickSound(); // 효과음 재생
        currentIndex = (currentIndex - 1 + CharacterManager.Instance._characters.Length) % CharacterManager.Instance._characters.Length;
        UpdateUI(); // 인덱스 번경 후 UI 갱신
    }

    // 오른쪽 화살표 클릭 시 호출
    public void MoveRight()
    {
        PlayClickSound();
        currentIndex = (currentIndex + 1) % CharacterManager.Instance._characters.Length;
        UpdateUI();
    }

    // 캐릭터 구매 처리
    public void BuyCharacter()
    {
        if (CharacterManager.Instance.BuyCharacter(currentIndex)) // 코인 충족 시 구매 성공
        {
            PlayBuySound(); // 구매 효과음
    
                UpdateUI(); // UI 갱신
            _coinText.color = Color.white; // 정상 색상
        }
        else
        {
            Debug.Log("Insufficient coins");
            _coinText.color = Color.red; // 코인 부족시 빨간색 표시
        }
    }

    // 선택한 캐릭터 저장
    public void SelectCharacter()
    {
        PlayClickSound();
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex); // 선택 인덱스 저장
        PlayerPrefs.Save(); // 저장 적용

    }

    // 게임 시작 시 호출
    public void StartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("Game"); // Game 씬 로드
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void PlayBuySound()
    {
        if (buySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buySound);
        }
    }
}


