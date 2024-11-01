using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshPro textc;
    public float damage_count;
    public float destroy_time;
    public float text_size;

    private float destroy_timer;
    private float time_from_spawn;
    private float text_size_increased;
    
    void Start()
    {
        textc = GetComponent<TextMeshPro>();
        destroy_timer = destroy_time;
    }

    void Update()
    {
        destroy_timer -= Time.deltaTime;
        time_from_spawn += Time.deltaTime;

        // Вычисляем размер текста
        text_size_increased = text_size * (1 - time_from_spawn / destroy_time);

        // Устанавливаем размер текста в компоненте
        textc.fontSize = Mathf.Max(text_size_increased, 0);  // Убедимся, что размер не становится отрицательным
        textc.text = Mathf.RoundToInt(damage_count).ToString();

        if (destroy_timer <= 0) Destroy(gameObject);
    }
}
