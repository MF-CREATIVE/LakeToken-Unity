using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int score;
    public string name;
    public float timePlayed;

    public GameData(int scoreInt, string nameStr, float timePlayedF)
    {
        score = scoreInt;
        name = nameStr;
        timePlayed = timePlayedF;
    }
}
