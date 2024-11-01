using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    public Vector3 cameraOffset;
    private Vector3 originalPosition;
    
    // Переменные для тряски
    private float shakeDuration = 0f;     // Текущая продолжительность тряски
    private float shakeMagnitude = 0f;    // Текущая сила тряски

    private float totalShakeDuration = 0f; // Общая продолжительность (накапливаемая)
    private float totalShakeMagnitude = 0f; // Общая сила тряски (накапливаемая)

    void Start()
    {
        player = GameObject.Find("Player");
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + cameraOffset;
            originalPosition = player.transform.position + cameraOffset;

            if (totalShakeDuration > 0)
            {
                ApplyShakeEffect();
            }
        }
    }

    // Метод для добавления эффекта тряски
    public void AddShake(float duration, float magnitude)
    {
        // Добавляем эффект тряски к текущим значениям
        totalShakeDuration = Mathf.Max(totalShakeDuration, duration);
        totalShakeMagnitude += magnitude; // накапливаем силу тряски

        // Запускаем корутину, если еще не идет тряска
        if (shakeDuration <= 0)
        {
            StartCoroutine(Shake());
        }
    }

    // Метод для применения текущего накопленного эффекта тряски
    private void ApplyShakeEffect()
    {
        // Генерация случайного смещения на основе общей силы тряски
        float xOffset = Random.Range(-1f, 1f) * totalShakeMagnitude;
        float yOffset = Random.Range(-1f, 1f) * totalShakeMagnitude;

        // Применяем смещение к позиции камеры
        transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

        // Уменьшаем продолжительность тряски и убираем тряску по завершению
        totalShakeDuration -= Time.deltaTime;
        if (totalShakeDuration <= 0)
        {
            totalShakeMagnitude = 0f;
            transform.localPosition = originalPosition; // Возвращаем камеру в исходную позицию
        }
    }

    // Корутина для тряски камеры (управляет временем тряски)
    private IEnumerator Shake()
    {
        while (totalShakeDuration > 0)
        {
            // Ждем следующего кадра
            yield return null;
        }
    }
}


/*
Новый скрипт

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    private Vector3 curPos;
    private Quaternion curRot;
    public Vector3 cameraDefaultPosition;
    public Quaternion cameraDefaultRotation;
    public Vector3 cameraLookaroundPosition;
    public Quaternion cameraLookaroundRotation;
    public float smoothValue = 1f;

    public KeyCode cameraChangeViewKey = KeyCode.LeftAlt;
    public int cameraPosID = 0;

    void Start(){
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if(Input.GetKeyDown(cameraChangeViewKey)) cameraPosID = 1;
        if(!Input.GetKeyUp(cameraChangeViewKey)) cameraPosID = 0;

        if(player!=null){
            if(cameraPosID == 0)
            {
                curPos = player.transform.position + cameraDefaultPosition;
                curRot = cameraDefaultRotation;
            }
            if(cameraPosID == 1)
            {
                curPos = player.transform.position + cameraLookaroundPosition;
                curRot = cameraLookaroundRotation;
            }

            //установ позицию и поворот камеры
            transform.position = Vector3.Lerp(transform.position, curPos, smoothValue*Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, smoothValue*Time.deltaTime);
        }
    }
}
*/