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

    void UpdateUI()
    {
        CharacterData currentCharacter = CharacterManager.Instance._characters[currentIndex];

       
        int playerCoins = PlayerPrefs.GetInt("KiwiScore", 0);

        if (currentCharacter._characterPrefab != null)
        {
            _characterImage.sprite = currentCharacter._characterPrefab.GetComponent<SpriteRenderer>().sprite;
        }

        _characterNameText.text = currentCharacter.characterName;
        _characterPriceText.text = currentCharacter.isPurchased ? "Owned" : $"Price: {currentCharacter.price}";
        _coinText.text = $"Coins: {playerCoins}";

        if (currentIndex == 0)
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(!currentCharacter.isPurchased);
        }

        selectButton.gameObject.SetActive(currentCharacter.isPurchased);
    }

    public void MoveLeft()
    {
        PlayClickSound();
        currentIndex = (currentIndex - 1 + CharacterManager.Instance._characters.Length) % CharacterManager.Instance._characters.Length;
        UpdateUI();
    }

    public void MoveRight()
    {
        PlayClickSound();
        currentIndex = (currentIndex + 1) % CharacterManager.Instance._characters.Length;
        UpdateUI();
    }

    public void BuyCharacter()
    {
        if (CharacterManager.Instance.BuyCharacter(currentIndex))
        {
            PlayBuySound();
    
                UpdateUI();
            _coinText.color = Color.white;
        }
        else
        {
            Debug.Log("Insufficient coins");
            _coinText.color = Color.red;
        }
    }

    public void SelectCharacter()
    {
        PlayClickSound();
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);
        PlayerPrefs.Save();

    }

    public void StartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("Game");
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


