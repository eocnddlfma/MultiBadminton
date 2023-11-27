using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSCript : MonoBehaviour
{
    void Start()
    {
        print("effect is on");
        Destroy(gameObject, 0.25f);
    }
}
