using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// сохранения
// переменные для сохранения должны называтся одинаково в Globals и GameData
// сохраняются все переменные Globals из GameData

[System.Serializable]
public class GameData
{
    public int completedLevelID;
    public int gameCompletesCount;
    public float musicVolume;
    public float soundVolume;
}

