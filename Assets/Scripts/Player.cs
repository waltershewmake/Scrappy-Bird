using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float gravity = -9.81f;
    public float strength = 5f;
    public Sprite[] sprites;

    public bool circleButtonDown = false;
    public bool triangleButtonDown = false;
    
    private SpriteRenderer _spriteRenderer;
    private Vector3 _direction;
    private int _spriteIndex = 0;
    
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
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
            GameManager.Instance.GameOver();
        } else if (other.gameObject.tag == "Scoring")
        {
            GameManager.Instance.IncreaseScore();
        }
    }
}
