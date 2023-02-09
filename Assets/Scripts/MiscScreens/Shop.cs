using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private GameObject continueButton;


    [Header("Buttons")]
    [SerializeField] private Button hp;
    [SerializeField] private Button higherJumps;
    [SerializeField] private Button stamina;
    [SerializeField] private Button moreJumps;
    [SerializeField] private Button speed;

    [Header("Prices")]

    [SerializeField] private int priceHp;
    [SerializeField] private int priceHigherJumps;
    [SerializeField] private int priceStamina;
    [SerializeField] private int priceMoreJumps;
    [SerializeField] private int priceSpeed;

    void ResetText()
    {
        points.text = "x" + GameManager.instance.save.points;
    }

    void Start()
    {
        GameAudio.PlayBGM("Shop");
        ResetText();
        hp.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = priceHp + " Coins";
        higherJumps.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = priceHigherJumps + " Coins";
        stamina.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = priceStamina + " Coins";
        moreJumps.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = priceMoreJumps + " Coins";
        speed.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = priceSpeed + " Coins";

        hp.interactable = !GameManager.instance.save.upgradeHP;
        higherJumps.interactable = !GameManager.instance.save.upgradeHigherJump;
        stamina.interactable = !GameManager.instance.save.upgradeStamina;
        moreJumps.interactable = !GameManager.instance.save.upgradeMoreJumps;
        speed.interactable = !GameManager.instance.save.upgradeSpeed;

        FadeSystem.ForceAlpha(1);
        FadeSystem.FadeTo(0, 2);
    }


    public void BuyHp()
    {
        if (GameManager.instance.save.points >= priceHp)
        {
            GameManager.instance.save.points -= priceHp;
            GameManager.instance.save.upgradeHP = true;
            hp.interactable = false;
            ResetText();
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void BuyStamina()
    {
        if (GameManager.instance.save.points >= priceStamina)
        {
            GameManager.instance.save.points -= priceStamina;
            GameManager.instance.save.upgradeStamina = true;
            stamina.interactable = false;
            ResetText();
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void BuyHigherJumps()
    {
        if (GameManager.instance.save.points >= priceHigherJumps)
        {
            GameManager.instance.save.points -= priceHigherJumps;
            GameManager.instance.save.upgradeHigherJump = true;
            higherJumps.interactable = false;
            ResetText();
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void BuyMoreJumps()
    {
        if (GameManager.instance.save.points >= priceMoreJumps)
        {
            GameManager.instance.save.points -= priceMoreJumps;
            GameManager.instance.save.upgradeMoreJumps = true;
            moreJumps.interactable = false;
            ResetText();
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void BuySpeed()
    {
        if (GameManager.instance.save.points >= priceSpeed)
        {
            GameManager.instance.save.points -= priceSpeed;
            GameManager.instance.save.upgradeSpeed = true;
            speed.interactable = false;
            ResetText();
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void Continue()
    {
        SAVEFILE file = GameManager.instance.save;
        if (file.upgradeHigherJump && file.upgradeHP && file.upgradeMoreJumps && file.upgradeSpeed && file.upgradeStamina)
        {
            GameManager.instance.IncrementAchievement("POWER");
        }
        GameManager.instance.EndShop();
    }
}
