using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberGold;

    void Start()
    {

        GameAudio.PlayBGM("End");
        numberGold.text = GameManager.instance.save.totalPoints.ToString();
        FadeSystem.ForceAlpha(1);
        FadeSystem.FadeTo(0, 2);
    }

    public void ToMainMenu()
    {
        GameManager.instance.ToMainMenu();
    }
}
