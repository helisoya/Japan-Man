using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class GameGUI : MonoBehaviour
{
    public static GameGUI instance;




    [Header("LifeBar")]
    [SerializeField] private Image lifeBarFill;


    [Header("Stamina")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color depletedColor;


    [Header("Focus Effect")]
    [SerializeField] private PostProcessVolume volumeDeath;
    [SerializeField] private PostProcessVolume volumeFocus;
    private Coroutine changingVolume;


    [Header("Points")]
    [SerializeField] private TextMeshProUGUI pointsText;


    [Header("Boss LifeBar")]
    [SerializeField] private Image bossLifeBarFill;
    [SerializeField] private GameObject bossLifeBarRoot;


    [Header("Victory Screen")]
    [SerializeField] private TextMeshProUGUI victoryLevelName;
    [SerializeField] private TextMeshProUGUI victoryPoints;


    [Header("Roots")]
    [SerializeField] private GameObject guiRoot;
    [SerializeField] private GameObject deathRoot;
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private GameObject victoryRoot;

    [Header("EventSystem")]
    [SerializeField] private GameObject selectedPause;
    [SerializeField] private GameObject selectedDeath;
    [SerializeField] private GameObject selectedVictory;

    [Header("Pause Menu")]

    public bool canPauseTheGame = true;
    public bool paused = false;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown dropdownResolution;
    private Resolution[] resolutions;

    [Header("Android")]
    [SerializeField] private GameObject[] toDeleteIfNotAndroid;
    private PlayerMovement player;

    public void Android_Jump()
    {
        player.EnableJump();
    }

    public void Android_MoveLeft(int move)
    {
        player.EnableMoveLeft(move);
    }

    public void Android_MoveRight(int move)
    {
        player.EnableMoveRight(move);
    }

    public void Android_PauseMenu()
    {
        if (paused) ClosePauseMenu();
        else OpenPauseMenu();
    }

    public void Android_FocusMonde()
    {
        player.EnableFocusMode();
    }

    public void Android_ShowHideIcons(bool value)
    {
        foreach (GameObject obj in toDeleteIfNotAndroid)
        {
            obj.SetActive(value);
        }
    }


    void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            for (int i = 0; i < toDeleteIfNotAndroid.Length; i++)
            {
                Destroy(toDeleteIfNotAndroid[i]);
            }
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

            bgmSlider.SetValueWithoutNotify(GameManager.instance.globalSave.bgmSound);
            sfxSlider.SetValueWithoutNotify(GameManager.instance.globalSave.sfxSound);
            masterSlider.SetValueWithoutNotify(GameManager.instance.globalSave.masterSound);
        }
        else
        {
            Destroy(fullScreenToggle.gameObject);
            Destroy(dropdownResolution.gameObject);
        }
        UpdatePoints();
        FadeSystem.ForceAlpha(1);
        FadeSystem.FadeTo(0, 2);
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause") && canPauseTheGame && !GameManager.instance.inCutscene)
        {
            if (!paused) OpenPauseMenu();
            else ClosePauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        if (Application.platform == RuntimePlatform.Android) Android_ShowHideIcons(false);
        paused = true;
        pauseRoot.SetActive(true);
        guiRoot.SetActive(false);
        Time.timeScale = 0;
        EventSystem.current.SetSelectedGameObject(selectedPause);
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

    public void ClosePauseMenu()
    {
        if (Application.platform == RuntimePlatform.Android) Android_ShowHideIcons(true);
        paused = false;
        pauseRoot.SetActive(false);
        guiRoot.SetActive(true);
        Time.timeScale = 1;
    }

    public void SetBossBarActive(bool value)
    {
        bossLifeBarRoot.SetActive(value);
    }

    public void UpdateBossHealth(float health, float maxHealth)
    {
        bossLifeBarFill.fillAmount = health / maxHealth;
    }
    public void UpdatePoints()
    {
        pointsText.text = "x" + GameManager.instance.pointsInLevel;
    }


    public void UpdateHealth(float health, float maxHealth)
    {
        lifeBarFill.fillAmount = health / maxHealth;
    }

    public void UpdateStamina(float stamina, float maxStamina, bool depleted)
    {
        staminaFill.fillAmount = stamina / maxStamina;
        staminaFill.color = depleted ? depletedColor : normalColor;
    }


    public void SetSlowMotionEffect(bool value)
    {
        SetEffectTo(volumeFocus, value);
    }

    public void SetDeathEffect(bool value)
    {
        canPauseTheGame = false;
        guiRoot.SetActive(!value);
        deathRoot.SetActive(value);
        EventSystem.current.SetSelectedGameObject(selectedDeath);
        SetEffectTo(volumeDeath, value);
    }

    public void FinishLevel()
    {
        canPauseTheGame = false;
        StartCoroutine(CR_FinishingLevel());
    }


    void SetEffectTo(PostProcessVolume volume, bool activate)
    {
        if (changingVolume != null)
        {
            StopCoroutine(changingVolume);
        }
        changingVolume = StartCoroutine(CR_ChangingVolume(volume, activate ? 1 : 0, 5));
    }

    IEnumerator CR_ChangingVolume(PostProcessVolume volume, float changeTo, float speed)
    {
        while (volume.weight != changeTo)
        {
            if (volume.weight < changeTo)
            {
                volume.weight = Mathf.Clamp(volume.weight + speed * Time.unscaledDeltaTime, 0, 1);
            }
            else
            {
                volume.weight = Mathf.Clamp(volume.weight - speed * Time.unscaledDeltaTime, 0, 1);
            }

            yield return new WaitForEndOfFrame();

        }
    }

    IEnumerator CR_FinishingLevel()
    {
        Time.timeScale = 0;

        FadeSystem.FadeTo(1, 2);
        while (FadeSystem.isFading)
        {
            yield return new WaitForEndOfFrame();
        }


        if (GameManager.instance.inBossRush)
        {
            Time.timeScale = 1;
            GameManager.instance.BossRushIteration();
        }
        else
        {
            victoryRoot.SetActive(true);
            EventSystem.current.SetSelectedGameObject(selectedVictory);
            victoryLevelName.text = Level.instance.levelName;
            victoryPoints.text = GameManager.instance.pointsInLevel + "/" + Level.instance.maxPoints;

            FadeSystem.FadeTo(0, 2);
            while (FadeSystem.isFading)
            {
                yield return new WaitForEndOfFrame();
            }
        }

    }

    public void Event_VictoryRestartLevel()
    {
        GameManager.instance.ResetLevel();
    }


    public void Event_VictoryContinueGame()
    {
        GameManager.instance.FinishLevel();
    }

    public void Event_ReplayLevel()
    {
        GameManager.instance.RestartLevel();
    }

    public void Event_QuitToMainMenu()
    {
        GameManager.instance.ToMainMenu();
    }
}
