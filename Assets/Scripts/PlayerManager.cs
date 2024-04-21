using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerColor
{
    Aqua,
    Green,
    Lime,
    Magenta,
    Purple,
    Red,
    Sky,
    Yellow,
}

public enum PlayerState
{
    Menu,
    Dead,
    Ready,
    Alive,
    Hidden
}

public class PlayerManager : MonoBehaviour
{

    public float gravity = -9.81f;
    public float strength = 5f;
    // public Sprite[] sprites;
    
    public Sprite[] aquaSprites;
    public Sprite[] greenSprites;
    public Sprite[] limeSprites;
    public Sprite[] magentaSprites;
    public Sprite[] purpleSprites;
    public Sprite[] redSprites;
    public Sprite[] skySprites;
    public Sprite[] yellowSprites;

    public Sprite[] currentSprites;
    
    public Camera playerCamera;
    public Text playerScoreText;
    public Text playerNameText;
    public bool isAlive = true;
    
    public PlayerColor color;
    public GameManager.GameStats gameStats;
    public int score = 0;
    public bool circleButtonDown = false;
    public bool triangleButtonDown = false;
    
    private SpriteRenderer _spriteRenderer;
    private Vector3 _direction;
    private int _spriteIndex = 0;
    
    private PlayerState _playerState;
    
    public PlayerState State
    {
        get => _playerState;
        set
        {
            switch (value)
            {
                case PlayerState.Menu:
                    isAlive = false;
                    enabled = false;

                    playerScoreText.gameObject.SetActive(false);
                    playerNameText.gameObject.SetActive(true);

                    playerCamera.enabled = true;

                    _playerState = PlayerState.Menu;
                    break;
                case PlayerState.Dead:
                    isAlive = false;
                    enabled = false;

                    playerScoreText.gameObject.SetActive(true);
                    playerNameText.gameObject.SetActive(true);
                    
                    playerCamera.enabled = false;
                    
                    _playerState = PlayerState.Dead;
                    break;
                case PlayerState.Ready:
                    isAlive = true;
                    enabled = false;
                    
                    score = 0;
                    playerScoreText.text = score.ToString();
                    
                    playerScoreText.gameObject.SetActive(true);
                    playerNameText.gameObject.SetActive(true);
                    
                    playerCamera.enabled = true;
                    
                    Vector3 position = transform.position;
                    position.y = 0f;
                    transform.position = position;
        
                    _direction = Vector3.zero;
                    
                    _playerState = PlayerState.Ready;
                    break;
                case PlayerState.Alive:
                    isAlive = true;
                    enabled = true;
                    
                    playerScoreText.gameObject.SetActive(true);
                    playerNameText.gameObject.SetActive(true);
                    
                    playerCamera.enabled = true;
                    
                    _playerState = PlayerState.Alive;
                    break;
                case PlayerState.Hidden:
                    isAlive = false;
                    enabled = false;
                    
                    playerScoreText.gameObject.SetActive(false);
                    playerNameText.gameObject.SetActive(false);
                    
                    playerCamera.enabled = false;
                    
                    _playerState = PlayerState.Hidden;
                    break;
            }
        }
    }
    
    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentSprites = RandomizeSprites();
    }

    void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    Sprite[] RandomizeSprites()
    {
        // Randomize color
        int randomColor = Random.Range(0, 8);
        switch (randomColor)
        {
            case 0:
                color = PlayerColor.Aqua;
                return aquaSprites;
            case 1:
                color = PlayerColor.Green;
                return greenSprites;
            case 2:
                color = PlayerColor.Lime;
                return limeSprites;
            case 3:
                color = PlayerColor.Magenta;
                return magentaSprites;
            case 4:
                color = PlayerColor.Purple;
                return purpleSprites;
            case 5:
                color = PlayerColor.Red;
                return redSprites;
            case 6:
                color = PlayerColor.Sky;
                return skySprites;
            default:
                color = PlayerColor.Yellow;
                return yellowSprites;
        }
    }


    void Update()
    {
        if (GameManager.Instance.State == GameState.Menu)
        {
            if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1) || circleButtonDown)
            {
                switch (color)
                {
                    case PlayerColor.Aqua:
                        color = PlayerColor.Green;
                        currentSprites = greenSprites;
                        break;
                    case PlayerColor.Green:
                        color = PlayerColor.Lime;
                        currentSprites = limeSprites;
                        break;
                    case PlayerColor.Lime:
                        color = PlayerColor.Magenta;
                        currentSprites = magentaSprites;
                        break;
                    case PlayerColor.Magenta:
                        color = PlayerColor.Purple;
                        currentSprites = purpleSprites;
                        break;
                    case PlayerColor.Purple:
                        color = PlayerColor.Red;
                        currentSprites = redSprites;
                        break;
                    case PlayerColor.Red:
                        color = PlayerColor.Sky;
                        currentSprites = skySprites;
                        break;
                    case PlayerColor.Sky:
                        color = PlayerColor.Yellow;
                        currentSprites = yellowSprites;
                        break;
                    case PlayerColor.Yellow:
                        color = PlayerColor.Aqua;
                        currentSprites = aquaSprites;
                        break;
                }
                _spriteRenderer.sprite = currentSprites[_spriteIndex];
            }
        }
        
        if (!isAlive || GameManager.Instance.State != GameState.Playing) return;
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || triangleButtonDown)
        {
            _direction = Vector3.up * strength;
            // GameManager.Instance.soundEffects.PlayJump();
        }
        
        _direction.y += gravity * Time.deltaTime;
        transform.position += _direction * Time.deltaTime;
    }

    void AnimateSprite()
    {
        _spriteIndex++;

        if (_spriteIndex >= currentSprites.Length)
        {
            _spriteIndex = 0;
        }
        
        _spriteRenderer.sprite = currentSprites[_spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            State = PlayerState.Dead;
        } else if (other.gameObject.tag == "Scoring")
        {
            score++;
            playerScoreText.text = score.ToString();
            GameManager.Instance.soundEffects.PlayCollectCoin();
        }
    }
}
