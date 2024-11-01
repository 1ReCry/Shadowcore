using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;

public static class Globals
{
    // основные игровые переменные
    public static int completedLevelID = 0;
    public static int gameCompletesCount;

    public static bool survivalMode;

    // программные переменные
    public static int Money;

    public static int MenuCameraPosID;
    public static int selectedItemIDinscript;
    public static int selectedSlotIDinscript;
    public static int newItemInSlodID = 0;

    public static bool notAddedWarning;

    public static bool navUpdateRequest;
    public static bool openDoorsRequest;

    public static bool cursorMoving;
    //айди уровней и их названия
    public static int CurrentLevelID;
    public static string[] levelName = new string[13];

    //айди объектов и их пути
    public static string[] Item = new string[25];
    public static string[] Sword = new string[25];
    public static int InventoryLength = 7;

    public static float BossbarActiveTimer;
    public static float BossHealthMax;
    public static float BossHealth;
    public static int BossPhase;
    public static bool BossMultiplePhases;

    public static bool activateBlink;

    public static bool menuOpened;
    
    //Volumes
    public static float musicVolume = 1f;
    public static float soundVolume = 1f;

    //save load
    private static string filePath = Path.Combine(Application.persistentDataPath, "saveData.json");

    public static bool firstLoadAlready;

    static Globals()
    {
        // Предметы
        Item[0] = "ID 0 not avaible!"; // первого предмета нет
        Item[1] = "ScriptableObjects/Items/SpeedStick"; //палочка бега
        Item[2] = "ScriptableObjects/Items/DashSphere"; // дэш
        Item[3] = "ScriptableObjects/Items/BladeOfFury"; // ускорение атаки
        Item[4] = "ScriptableObjects/Items/HeartCylon"; // постепенный хил
        Item[5] = "ScriptableObjects/Items/HealPotion"; // зелье здоровья +100 хп
        Item[6] = "ScriptableObjects/Items/SpeedWand"; // улучшенная палочка
        Item[7] = "ScriptableObjects/Items/WaterBlade"; //скорость атаки и скорость
        Item[8] = "ScriptableObjects/Items/WaterCylon"; //хил ускоренеие и скорость атаки
        Item[9] = "ScriptableObjects/Items/ShadowSphere"; // теневая форма
        Item[10] = "ScriptableObjects/Items/DamageAmulet"; //х2 дамаг
        Item[11] = "ScriptableObjects/Items/Torch"; // факел
        Item[12] = "ScriptableObjects/Items/IceHeart"; // хил 7 хп каждые 5 сек
        Item[13] = "ScriptableObjects/Items/FogSword"; // ускоряет атаку в 10 раз на 2 сек
        Item[14] = "ScriptableObjects/Items/LavaHeart";
        Item[15] = "ScriptableObjects/Items/TorchOfWrath";
        Item[16] = "ScriptableObjects/Items/ShadowBlade";
        Item[17] = "ScriptableObjects/Items/ShadowStick";
        Item[18] = "ScriptableObjects/Items/ShadowWand";
        Item[19] = "ScriptableObjects/Items/SpeedSphere";
        Item[20] = "ScriptableObjects/Items/ShadowHeart";

        //Мечи
        Sword[0] = "ScriptableObjects/Swords/ClassicSword";
        Sword[1] = "ScriptableObjects/Swords/MetalHeavy";
        Sword[2] = "ScriptableObjects/Swords/WaterSpasher";
        Sword[3] = "ScriptableObjects/Swords/ShadowFury";
        Sword[4] = "ScriptableObjects/Swords/IceBlade";
        Sword[5] = "ScriptableObjects/Swords/IceSaber";
        Sword[6] = "ScriptableObjects/Swords/FieryWrath";
        Sword[7] = "ScriptableObjects/Swords/FireStaff";
        Sword[8] = "ScriptableObjects/Swords/ShadowSaber";
        Sword[9] = "ScriptableObjects/Swords/ShadowHammer";
        Sword[10] = "ScriptableObjects/Swords/ShadowSaberBoss";

        //Уровни
        levelName[0] = "Debug & Dev";
        levelName[1] = "<color=#b0ffc8>Начало";
        levelName[2] = "<color=#61ff91>Поляна жизни";
        levelName[3] = "<color=#c2c2c2>Тёмные пещеры";
        levelName[4] = "<color=#806d9c>Пустошь теней";
        levelName[5] = "<color=#6c74a1>Река забвения";
        levelName[6] = "<color=#8b8299>Павший город";
        levelName[7] = "<color=#b3eeff>Туманный Хребет";
        levelName[8] = "<color=#ff4800>Пик Гнева";
        levelName[9] = "<color=#666666>Чёрная Башня";
        levelName[10] = "<color=#5c14e0>Теневое Ядро";
        levelName[12] = "<color=#4d9e4c>Выживание";
    }

    //Слоты игровые
    public static int[] Inventory = new int[InventoryLength];
    public static bool UpdateInventorySignal = true;
    public static bool SetMaxItemEnergyInCurSlot = true;

    public static bool CheckInventoryIsFull()
    {
        // Проходим по всем слотам инвентаря
        for (int cycleId = 1; cycleId < Inventory.Length; cycleId++)
        {
            Debug.Log("Check Cyc ID: "+cycleId +"  Inv slot: " + Inventory[cycleId]);
            if (Inventory[cycleId] == 0)
            {
                return false;
            }
        }
        Debug.LogWarning("Инвентарь полон. Невозможно добавить предмет.");
        notAddedWarning = true;
        return true;
    }

    public static void AddItem(int id)
    {
        bool added = false;
        selectedItemIDinscript = id;
        // Проходим по всем слотам инвентаря
        for (int cycleId = 1; cycleId < Inventory.Length; cycleId++)
        {
            Debug.Log("Cyc ID: "+cycleId + "   Id: " + id + "  Inv slot: " + Inventory[cycleId]);
            if (Inventory[cycleId] == 0)
            {
                Inventory[cycleId] = id;
                selectedSlotIDinscript = cycleId;
                newItemInSlodID = cycleId;
                SetMaxItemEnergyInCurSlot = true;
                Debug.Log("Item ID "+ id +" added to inventory slot " + cycleId);
                Debug.Log("Inventory check in slot " + cycleId + ", item ID is: " + Inventory[cycleId]);
                Debug.Log("full Inventory: " + string.Join(", ", Inventory));
                UpdateInventorySignal = true;
                added = true;
                return;
            }
            if (cycleId == Inventory.Length - 1 && !added)
            {
                Debug.LogWarning("Инвентарь полон. Невозможно добавить предмет под ID " + id);
                notAddedWarning = true;
                return;
            }
        }
    }


    // SAVE LOAD
    public static void SaveData()
    {
        GameData data = new GameData();

        Type type = typeof(Globals);
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        foreach (var field in fields)
        {
            var value = field.GetValue(null);
            var fieldName = field.Name;
            var dataField = typeof(GameData).GetField(fieldName);
            if (dataField != null)
            {
                dataField.SetValue(data, value);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    public static void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            Type type = typeof(Globals);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                var fieldName = field.Name;
                var dataField = typeof(GameData).GetField(fieldName);
                if (dataField != null)
                {
                    var value = dataField.GetValue(data);
                    field.SetValue(null, value);
                }
            }

            Debug.Log("Data loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + filePath);
        }
    }
}
