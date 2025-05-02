using Unity.VisualScripting;
using UnityEngine;

public class PlayerLoad : MonoBehaviour
{
    private void Start()
    {

        if (CharacterManager.Instance != null)
        {
            GameObject selectedCharacterPrefab = CharacterManager.Instance.GetSelectedCharacterPrefab();
            if (selectedCharacterPrefab != null)
            {
                GameObject character = Instantiate(selectedCharacterPrefab, Vector3.zero, Quaternion.identity);

                CharacterManager.Instance.SetSpawnedCharacter(character);
            }
        }
    }

}




