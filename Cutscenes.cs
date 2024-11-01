using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Cutscenes : MonoBehaviour
{
    [Header("Привязка TMP")]
    public TextMeshProUGUI ui_text;
    public TextMeshProUGUI next_text;
    [Header("Текущий/стартовый ID")]
    public int cur_id = 0;
    [Header("Имя следующей сцены")]
    public string next_scene_name;
    [Header("Возможность пропускать текст")]
    public bool skipByMouse;
    public bool skipByMouseAfterFullText;
    [Header("Для финала")]
    public AudioClip endSound;
    public bool endSoundEnabled;
    [Header("Текст и его экранное время по ID")]
    public string[] text;
    public float[] text_time;
    public float[] text_create_speed;

    private float text_timer;
    private string cur_text;
    private bool isTextFullyDisplayed = false;
    private Coroutine typingCoroutine;
    private AudioSource audiosource;

    void Start()
    {
        text_timer = text_time[cur_id];
        cur_text = text[cur_id];
        typingCoroutine = StartCoroutine(TypeText());
        audiosource = GetComponent<AudioSource>();
        next_text.gameObject.SetActive(false);
    }

    void Update()
    {
        if (text_timer > 0)
        {
            text_timer -= Time.deltaTime;
        }

        if (text_timer <= 0 && isTextFullyDisplayed)
        {
            NextText();
        }

        if(skipByMouseAfterFullText && isTextFullyDisplayed)
        {
            next_text.gameObject.SetActive(true);
        }
        else if(skipByMouseAfterFullText && !isTextFullyDisplayed)
        {
            next_text.gameObject.SetActive(false);
        }

        if(skipByMouse)
        {
            next_text.gameObject.SetActive(true);
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (isTextFullyDisplayed && skipByMouseAfterFullText)
            {
                text_timer = 0;
            }
            if(!isTextFullyDisplayed && skipByMouse)
            {
                // Если текст ещё не полностью показан, сразу покажем весь текст
                StopCoroutine(typingCoroutine);
                ui_text.text = cur_text;
                isTextFullyDisplayed = true;
            }
        }
    }

    void NextText()
    {
        cur_id += 1;

        if (cur_id >= text.Length)
        {
            SceneManager.LoadScene(next_scene_name);
            return;
        }

        if(cur_id == text.Length-1 && endSoundEnabled)
        {
            Debug.Log("Play Sound clip: " + endSound);
            audiosource.clip = endSound;
            audiosource.Play();
        }

        text_timer = text_time[cur_id];
        cur_text = text[cur_id];

        // Перезапуск корутины для следующего текста
        isTextFullyDisplayed = false;
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        ui_text.text = "";
        isTextFullyDisplayed = false;

        // Постепенно показываем текст по одному символу
        if(text_create_speed[cur_id] > 0)
        {
            for (int i = 0; i < cur_text.Length; i++)
            {
                ui_text.text += cur_text[i];
                yield return new WaitForSeconds(text_create_speed[cur_id]); // Задержка между символами (можно настроить)
            }
        }

        if(text_create_speed[cur_id] <= 0) ui_text.text = cur_text;

        isTextFullyDisplayed = true;
    }
}
