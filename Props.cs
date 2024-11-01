using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    [Header("Рандомный угол при старте")]
    public bool randomStartRotate;
    public Vector2 rotateXrange;
    public Vector2 rotateYrange;
    public Vector2 rotateZrange;

    void Start()
    {
        if(randomStartRotate)
        {
            transform.rotation = Quaternion.Euler(Random.Range(rotateXrange.x,rotateXrange.y), Random.Range(rotateYrange.x,rotateYrange.y), Random.Range(rotateZrange.x,rotateZrange.y));
        }
    }
}
