using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public static Level instance;

    public string levelName;
    public int maxPoints;

    public string nextLevel;

    public string levelBGM;

    void Start()
    {
        instance = this;
        GameAudio.PlayBGM(levelBGM);
        maxPoints = 0;
        foreach (Soldier soldier in FindObjectsOfType<Soldier>())
        {
            maxPoints += soldier.pointsReward;
        }

        foreach (Boss boss in FindObjectsOfType<Boss>())
        {
            maxPoints += boss.pointsReward;
        }
    }
}
