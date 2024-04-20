using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public float speed = 1f;
    
    private float _destroyPosition;

    private void Start()
    {
        _destroyPosition = Vector3.left.x * 14f;
    }

    void Update()
    {
        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x <= _destroyPosition)
        {
            Destroy(gameObject);
        }
    }
}
