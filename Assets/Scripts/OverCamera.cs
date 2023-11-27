using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverCamera : MonoBehaviour
{
    public Transform shuttlecock;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (shuttlecock.position.y > transform.position.y)
        {
            transform.position = new Vector3(shuttlecock.position.x, transform.position.y);
            _spriteRenderer.enabled = true;
        }
        else
        {
            _spriteRenderer.enabled = false;
        }
        
    }
}
