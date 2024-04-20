using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    Alive,
    Hidden
}

public class PlayerManager : MonoBehaviour
{

    public float gravity = -9.81f;
    public float strength = 5f;
    public Sprite[] sprites;
    
    public Camera playerCamera;
    public Text playerScoreText;
    public Text playerNameText;
    public GameObject getReady;
    public GameObject gameOver;
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

                    getReady.SetActive(true);
                    gameOver.SetActive(false); 
                    playerScoreText.gameObject.SetActive(false);
                    playerNameText.gameObject.SetActive(true);

                    _playerState = PlayerState.Menu;
                    break;
                case PlayerState.Dead:
                    isAlive = false;
                    enabled = false;

                    getReady.SetActive(false);
                    gameOver.SetActive(true);
                    playerScoreText.gameObject.SetActive(true);
                    playerNameText.gameObject.SetActive(true);
                    
                    _playerState = PlayerState.Dead;
                    break;
                case PlayerState.Alive:
                    isAlive = true;
                    enabled = true;
                    
                    getReady.SetActive(false);
                    gameOver.SetActive(false);
                    playerScoreText.gameObject.SetActive(true);
                    playerNameText.gameObject.SetActive(true);
                    
                    _playerState = PlayerState.Alive;
                    break;
                case PlayerState.Hidden:
                    isAlive = false;
                    enabled = false;
                    
                    getReady.SetActive(false);
                    gameOver.SetActive(false); 
                    playerScoreText.gameObject.SetActive(false);
                    playerNameText.gameObject.SetActive(false);
                    
                    _playerState = PlayerState.Hidden;
                    break;
            }
        }
    }
    
    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }
    
    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        
        _direction = Vector3.zero;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || triangleButtonDown)
        {
            _direction = Vector3.up * strength;
        }
        
        _direction.y += gravity * Time.deltaTime;
        transform.position += _direction * Time.deltaTime;
    }

    void AnimateSprite()
    {
        _spriteIndex++;

        if (_spriteIndex >= sprites.Length)
        {
            _spriteIndex = 0;
        }
        
        _spriteRenderer.sprite = sprites[_spriteIndex];
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
        }
    }
}
