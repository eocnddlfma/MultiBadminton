using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderControll : MonoBehaviour
{
    public GameObject effect;
    private BoxCollider2D col;
    public PlayerInput pi;
    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("shuttleCock"))
        {
            TurnOff();
            spawnEffect(other.transform.position);
        }
    }

    public void TurnOff()
    {
        pi.AnimationHitSlow();
        col.enabled = false;
    }
    public void TurnOn()
    {
        col.enabled = true;
    }

    public void spawnEffect(Vector3 pos)
    {
        GameObject GOeffect = Instantiate(effect);
        GOeffect.transform.position = pos;
        GOeffect.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        GOeffect.transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg /*+ (isleft?-1:1 * 90)*/);
    }
}
