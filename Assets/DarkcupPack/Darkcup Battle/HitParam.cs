using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitParam
{
    public float damage;
    public float speed = 50f;
    public Vector3 direction;
    public float explosionRadius = 0f;
    public Transform owner;
    public Transform target;
}

