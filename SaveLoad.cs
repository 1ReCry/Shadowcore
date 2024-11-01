using System.Data.Common;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public static bool isDataLoaded = false;
    private float saveTimer = 3;

    private void Start()
    {
        if (!isDataLoaded)
        {
            Globals.LoadData();
            isDataLoaded = true;
        }
    }

    private void FixedUpdate()
    {
        saveTimer -= Time.deltaTime;
        if (saveTimer <= 0 && isDataLoaded)
        {
            saveTimer = 2;
            Globals.SaveData();
        }
        // Сохраняем данные при нажатии кнопки S
        //if (Input.GetKeyDown(KeyCode.S))
        //{   
        //}
    }
}

