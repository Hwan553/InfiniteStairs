using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainScene : MonoBehaviour
{


    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnCharacterButton()
    {
        PlayClickSound();
        SceneManager.LoadScene("Select");
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }


}
