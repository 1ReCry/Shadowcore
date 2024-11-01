using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [Header("ID предмета в лутбоксе (0 = пусто)")]
    public int lootItemID = 0;
    [Header("ID меча в лутбоксе (-1 = пусто)")]
    public int lootSwordID = -1;

    [Header("\nModel ID 0 - Обычный сундук\nModel ID 1 - Редкий сундук\nModel ID 2 - Эпический сундук\nModel ID 3 - Shadow")]
    public int modelID = 0;

    [Header("Уже залутан/не залутан")]
    public bool isLooted = false;

    [Header("Компонент анимации")]
    public Animation anim;

    //[Header("Количество золота в лутбоксе")]
    //public int goldCount = 0;

    [Header("В префабе должна быть включена только ID1 модель\nПривязка моделей сундука:")]
    public GameObject Model_id0_box;
    public GameObject Model_id0_cap;
    public GameObject Model_id1_box;
    public GameObject Model_id1_cap;
    public GameObject Model_id2_box;
    public GameObject Model_id2_cap;
    public GameObject Model_id3_box;
    public GameObject Model_id3_cap;

    void Start()
    {
        Model_id0_box.SetActive(false);
        Model_id0_cap.SetActive(false);
        anim = GetComponent<Animation>();
        anim.Stop();

        if(isLooted)
        {
            anim.Play();
        }

        if(modelID == 0){
            Model_id0_box.SetActive(true);
            Model_id0_cap.SetActive(true);
        }
        if(modelID == 1){
            Model_id1_box.SetActive(true);
            Model_id1_cap.SetActive(true);
        }
        if(modelID == 2){
            Model_id2_box.SetActive(true);
            Model_id2_cap.SetActive(true);
        }
        if(modelID == 3){
            Model_id3_box.SetActive(true);
            Model_id3_cap.SetActive(true);
        }
    }
}
