using JetBrains.Annotations;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private GameManager _gameManager;  
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool _isTurn = false;
    private bool _isDie = false;
    private AudioSource audioSource;


    private int moveCount = 0;
    private int turnCount = 0;
    private int spawnCount = 0;
   
    

    public Transform _restartPosition;
    public AudioClip clickSound;
    public AudioClip moveSound;
    public AudioClip turnSound;
    public AudioClip kiwiSound;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = GameManager.Instance;
        audioSource = GetComponent<AudioSource>();

        if (_restartPosition == null)
        {
            GameObject restartPoint = new GameObject("RestartPoint");
            restartPoint.transform.position = transform.position;
            _restartPosition = restartPoint.transform;
        }

        startPosition = transform.position;
        Init();

      
    }

    private void Init()
    {
        if (_animator != null)
        {
            _animator.SetBool("Die", false);
        }

        startPosition = _restartPosition.position;
        endPosition = startPosition;
        transform.position = startPosition;
        moveCount = 0;
        spawnCount = 0;
        turnCount = 0;
        _isTurn = false;
        _spriteRenderer.flipX = _isTurn;
        _isDie = false;
    }

    public void PlayerTurn()
    {
        if (IsOut()) return;

        
        _isTurn = !_isTurn;
        _spriteRenderer.flipX = _isTurn; 

      
    }

    public void PlayerMove()
    {
       
        if (_isDie || IsOut())
            return;
       
        moveCount++;
        MoveDirection();

        if (IsFailTurn())
        {
            PlayerDie();
            return;
        }

        if (moveCount > 5)
        {
            ReSpawnStairs();
        }

        _gameManager.AddScore();
        _gameManager.GaugeIncrease();
        _gameManager._gaugeStart = true;
    }

    private bool IsOut()
    {
        float minX = -9.0f;
        float maxX = 9.0f;
        float minY = 0.0f;
        if (transform.position.x < minX || transform.position.x > maxX || transform.position.y < minY)
        {
            return true;
        }
        return false;
    }

    private void MoveDirection()
    {
        if (_isTurn)
        {
            endPosition += new Vector3(-0.75f, 0.5f, 0);
        }
        else
        {
            endPosition += new Vector3(0.75f, 0.5f, 0);
        }

        transform.position = endPosition;

        if (_animator != null)
        {
            _animator.SetTrigger("IsMove");
        }
    }

    private bool IsFailTurn()
    {
        bool result = _gameManager.isTurn[turnCount] != _isTurn;

        turnCount++;
        if (turnCount > _gameManager.Stairs.Length - 1)
        {
            turnCount = 0;
        }

        return result;
    }

    private void ReSpawnStairs()
    {
        _gameManager.SpawnStair(spawnCount);
        spawnCount = (spawnCount + 1) % _gameManager.Stairs.Length;
    }

    public void RestartButton()
    {
        PlayClickSound();
        Init();
        _gameManager.Init();
        _gameManager.InitStairs();
        _gameManager._gaugeStart = true;
        _gameManager.GaugeReduce();

        transform.position = _restartPosition.position;
        endPosition = _restartPosition.position;


    }

    private void PlayerDie()
    {
        _gameManager.GameOver();
        _animator.SetBool("Die", true);
        _isDie = true;
    }

   

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kiwi"))
        {
            collision.gameObject.SetActive(false);
            _gameManager.AddKiwiScore(2);
           
        }
    }

    public void OnMainButton()
    {
        PlayClickSound();
        SceneManager.LoadScene("Main");
    }
    public void OnMoveButtonPressed()
    {
        PlayMoveSound();
        CharacterManager.Instance.MovePlayer();
    }
    public void OnTurnButtonPressed()
    {
        PlayturnSound();
        CharacterManager.Instance.TurnPlayer();
    }

    public void OnReStartButton()
    {
        PlayClickSound();
        CharacterManager.Instance.ReStartPlayer();
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void PlayMoveSound()
    {
        if (moveSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(moveSound);
        }
    }

    private void PlayturnSound()
    {
        if (turnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(turnSound);
        }
    }

    
}
