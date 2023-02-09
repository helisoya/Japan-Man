using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AchievementCard : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI achievementName;
    [SerializeField] private TextMeshProUGUI achievementDesc;


    public void Refresh(Achievement achievement)
    {
        image.sprite = Resources.Load<Sprite>("Achievements/" + achievement.internalName);
        fillBar.fillAmount = (float)(achievement.currentPhase) / (float)(achievement.maxPhase);
        achievementName.text = achievement.achievementName;
        achievementDesc.text = achievement.description;
    }
}
