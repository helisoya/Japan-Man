using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TitleScreen : MonoBehaviour
{

    [SerializeField] private Button loadButton;

    [Header("Roots")]
    [SerializeField] private GameObject difficultyScreen;
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject extraScreen;
    [SerializeField] private GameObject achievementsScreen;
    [SerializeField] private GameObject bossRushScreen;

    [Header("Event System")]
    [SerializeField] private GameObject firstDifficulty;
    [SerializeField] private GameObject firstSettings;
    [SerializeField] private GameObject firstExtra;
    [SerializeField] private GameObject firstAchievements;
    [SerializeField] private GameObject firstBossRush;
    private GameObject lastMain;
    private GameObject lastExtra;

    [Header("Settings")]
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown dropdownResolution;
    private Resolution[] resolutions;

    [Header("Achievements")]
    [SerializeField] private GameObject prefabAchievementBox;
    [SerializeField] private Transform achievementsRoot;

    void Start()
    {
        GameAudio.PlayBGM("TitleScreen");
        lastExtra = firstExtra;
        if (!System.IO.File.Exists(FileManager.savPath + "save.sav"))
        {
            loadButton.interactable = false;
            loadButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }

        sliderBGM.SetValueWithoutNotify(GameManager.instance.globalSave.bgmSound);
        sliderSFX.SetValueWithoutNotify(GameManager.instance.globalSave.sfxSound);
        sliderMaster.SetValueWithoutNotify(GameManager.instance.globalSave.masterSound);

        if (Application.platform == RuntimePlatform.Android)
        {
            Destroy(fullScreenToggle.gameObject);
            Destroy(dropdownResolution.gameObject);
        }
        else
        {
            resolutions = Screen.resolutions;

            List<string> names = new List<string>();
            dropdownResolution.ClearOptions();
            int correctIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                if (resolution.width == GameManager.instance.globalSave.screenWidth &&
                resolution.height == GameManager.instance.globalSave.screenHeight)
                {
                    correctIndex = i;
                }
                names.Add(resolution.width + "x" + resolution.height);
            }
            dropdownResolution.AddOptions(names);
            dropdownResolution.SetValueWithoutNotify(correctIndex);
            fullScreenToggle.SetIsOnWithoutNotify(GameManager.instance.globalSave.fullscreen);
        }




        foreach (Achievement achievement in GameManager.instance.globalSave.achievements)
        {
            Instantiate(prefabAchievementBox, achievementsRoot).GetComponent<AchievementCard>().Refresh(achievement);
        }

        FadeSystem.ForceAlpha(1);
        FadeSystem.FadeTo(0, 2);
    }

    public void BossRush(int difficulty)
    {
        GameManager.instance.BossRush(difficulty);
    }

    public void LoadGame()
    {
        GameManager.instance.LoadGame();
    }

    public void ToDifficulties()
    {
        difficultyScreen.SetActive(true);
        mainScreen.SetActive(false);
        lastMain = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(firstDifficulty);
    }

    public void ToMain()
    {
        difficultyScreen.SetActive(false);
        settingsScreen.SetActive(false);
        extraScreen.SetActive(false);
        mainScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(lastMain);
    }


    public void ToSettings()
    {
        lastMain = EventSystem.current.currentSelectedGameObject;
        mainScreen.SetActive(false);
        settingsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSettings);
    }

    public void ToBossRush()
    {
        lastExtra = EventSystem.current.currentSelectedGameObject;
        extraScreen.SetActive(false);
        bossRushScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstBossRush);
    }

    public void ToExtras(bool fromMain)
    {
        if (fromMain) lastMain = EventSystem.current.currentSelectedGameObject;
        mainScreen.SetActive(false);
        extraScreen.SetActive(true);
        bossRushScreen.SetActive(false);
        achievementsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(lastExtra);
    }

    public void ToAchievements()
    {
        lastExtra = EventSystem.current.currentSelectedGameObject;
        extraScreen.SetActive(false);
        achievementsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstAchievements);
    }

    public void ChangeMasterSettings(float newVal)
    {
        GameManager.instance.ChangeMasterSettings(newVal);
    }

    public void ChangeSFXSettings(float newVal)
    {
        GameManager.instance.ChangeSFXSettings(newVal);
    }

    public void ChangeBGMSettings(float newVal)
    {
        GameManager.instance.ChangeBGMSettings(newVal);
    }

    public void ChangeFullScreenSettings(bool newVal)
    {
        GameManager.instance.ChangeFullScreenSettings(newVal);
    }

    public void ChangeResolutionsSettings(int newVal)
    {
        Resolution resolution = resolutions[newVal];
        GameManager.instance.ChangeResolutionSettings(resolution.width, resolution.height, resolution.refreshRate);
    }

    public void NewGame(int difficulty)
    {
        GameManager.instance.NewGame(difficulty);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
