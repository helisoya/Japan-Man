using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("Global Variables")]
    public SAVEFILE save;
    public bool inCutscene = false;
    public bool cameraFollowPlayerX = true;
    public bool cameraFollowPlayerY = true;
    public GLOBALSAVE globalSave;
    public bool inBossRush = false;
    public int bossRushDifficulty = 1;

    public List<string> remainingBosses;

    private Dictionary<string, string> achievementsGoogle;

    public int actualDifficulty
    {
        get
        {
            return inBossRush ? bossRushDifficulty : save.difficulty; ;
        }
    }


    [HideInInspector] public bool useCheckpoint = false;
    [HideInInspector] public Vector3 checkPointPosition;
    [HideInInspector] public int pointsAtCheckpoint;
    [HideInInspector] public int pointsInLevel;
    [HideInInspector] public string nextLevel;
    [SerializeField] private AudioMixer mixer;

    private Coroutine changingMap;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            achievementsGoogle.Add("EASY", "CgkIqLDgk-ocEAIQAQ");
            achievementsGoogle.Add("NORMAL", "CgkIqLDgk-ocEAIQAg");
            achievementsGoogle.Add("HARD", "CgkIqLDgk-ocEAIQAw");
            achievementsGoogle.Add("POWER", "CgkIqLDgk-ocEAIQBA");
            achievementsGoogle.Add("STOMP", "CgkIqLDgk-ocEAIQBQ");
            achievementsGoogle.Add("DEATH", "CgkIqLDgk-ocEAIQBg");
            achievementsGoogle.Add("BOSS_RUSH_EASY", "CgkIqLDgk-ocEAIQBw");
            achievementsGoogle.Add("BOSS_RUSH_NORMAL", "CgkIqLDgk-ocEAIQCA");
            achievementsGoogle.Add("BOSS_RUSH_HARD", "CgkIqLDgk-ocEAIQCQ");

            LoadGlobal();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ReloadMixerPrefs()
    {
        mixer.SetFloat("BGM", globalSave.bgmSound);
        mixer.SetFloat("SFX", globalSave.sfxSound);
        mixer.SetFloat("Master", globalSave.masterSound);
    }

    void ResetVariables()
    {
        inCutscene = false;
        useCheckpoint = false;
        Time.timeScale = 1;
        cameraFollowPlayerX = true;
        cameraFollowPlayerY = true;
        pointsInLevel = 0;
        inBossRush = false;
    }

    public void BossRush(int difficulty)
    {
        ResetVariables();
        inBossRush = true;
        bossRushDifficulty = difficulty;

        remainingBosses = new List<string>();
        for (int i = 1; i <= 7; i++)
        {
            remainingBosses.Add("BossRush_" + i);
        }

        changingMap = StartCoroutine(CR_StartBossRush());
    }

    public void BossRushIteration()
    {
        if (remainingBosses.Count == 0)
        {
            inBossRush = false;
            IncrementAchievement("BOSS_RUSH_EASY");
            if (bossRushDifficulty >= 1) IncrementAchievement("BOSS_RUSH_NORMAL");
            if (bossRushDifficulty >= 2) IncrementAchievement("BOSS_RUSH_HARD");
            ToMainMenu();
        }
        else
        {
            int index = Random.Range(0, remainingBosses.Count);
            SceneManager.LoadScene(remainingBosses[index]);
            remainingBosses.RemoveAt(index);
        }

    }

    public void NewGame(int difficulty)
    {
        save = new SAVEFILE();
        save.difficulty = difficulty;
        ResetVariables();
        LaunchLevel("Level1");
    }

    public void SaveGlobal()
    {
        FileManager.SaveJSON(FileManager.savPath + "global.sav", globalSave);
    }

    public void LoadGlobal()
    {
        if (System.IO.File.Exists(FileManager.savPath + "global.sav"))
        {
            globalSave = FileManager.LoadJSON<GLOBALSAVE>(FileManager.savPath + "global.sav");
            ReloadMixerPrefs();
            RefreshResolution();
        }
        else
        {
            globalSave.refreshRate = Screen.currentResolution.refreshRate;
            globalSave.screenWidth = Screen.currentResolution.width;
            globalSave.screenHeight = Screen.currentResolution.height;
            SaveGlobal();
        }
    }

    public void IncrementAchievement(string achievementName)
    {
        foreach (Achievement achievement in globalSave.achievements)
        {
            if (achievement.internalName.Equals(achievementName) &&
                achievement.maxPhase > achievement.currentPhase)
            {
                if (achievement.maxPhase != 1)
                {
                    PlayGamesPlatform.Instance.IncrementAchievement(achievementsGoogle[achievementName], 1, (bool success) => { });
                }
                achievement.currentPhase++;
                SaveGlobal();
                if (achievement.currentPhase == achievement.maxPhase)
                {
                    if (achievement.maxPhase == 1)
                    {
                        Social.ReportProgress(achievementsGoogle[achievementName], 100.0f, (bool success) =>
                        {

                        });
                    }
                    GameAudio.PlaySFX("Checkpoint");
                }
                return;
            }
        }
    }

    public void ChangeBGMSettings(float newValue)
    {
        globalSave.bgmSound = newValue;
        mixer.SetFloat("BGM", newValue);
        SaveGlobal();
    }

    public void ChangeMasterSettings(float newValue)
    {
        globalSave.masterSound = newValue;
        mixer.SetFloat("Master", newValue);
        SaveGlobal();
    }

    public void ChangeSFXSettings(float newValue)
    {
        globalSave.sfxSound = newValue;
        mixer.SetFloat("SFX", newValue);
        SaveGlobal();
    }

    public void ChangeResolutionSettings(int newWidth, int newHeight, int newRefreshRate)
    {
        globalSave.refreshRate = newRefreshRate;
        globalSave.screenWidth = newWidth;
        globalSave.screenHeight = newHeight;
        RefreshResolution();
    }

    public void ChangeFullScreenSettings(bool newVal)
    {
        globalSave.fullscreen = newVal;
        RefreshResolution();
    }

    public void RefreshResolution()
    {
        if (Application.platform == RuntimePlatform.Android) return;
        Screen.SetResolution(globalSave.screenWidth, globalSave.screenHeight, globalSave.fullscreen, globalSave.refreshRate);
    }

    public void SaveGame()
    {
        FileManager.SaveJSON(FileManager.savPath + "save.sav", save);
    }

    public void LoadGame()
    {
        if (System.IO.File.Exists(FileManager.savPath + "save.sav"))
        {
            save = FileManager.LoadJSON<SAVEFILE>(FileManager.savPath + "save.sav");
            LaunchLevel(save.level);
        }
    }

    public void LaunchLevel(string name)
    {
        if (changingMap != null) return;
        save.level = name;
        changingMap = StartCoroutine(CR_LoadMap(name, true));
    }

    public void EnableCheckpoint(Vector3 postion)
    {
        pointsAtCheckpoint = pointsInLevel;
        checkPointPosition = postion;
        useCheckpoint = true;
    }

    public void ResetLevel()
    {
        useCheckpoint = false;
        cameraFollowPlayerX = true;
        cameraFollowPlayerY = true;
        RestartLevel();
    }

    public void FinishLevel()
    {
        if (changingMap != null) return;
        GameAudio.PlayBGM(null);
        nextLevel = Level.instance.nextLevel;
        save.points += pointsInLevel;
        save.totalPoints += pointsInLevel;
        if (nextLevel.Equals("END"))
        {
            IncrementAchievement("EASY");
            if (save.difficulty >= 1) IncrementAchievement("NORMAL");
            if (save.difficulty >= 2) IncrementAchievement("HARD");
            changingMap = StartCoroutine(CR_LoadMap("End", true));
        }
        else
        {
            changingMap = StartCoroutine(CR_LoadMap("Shop", true));
        }

    }

    public void EndShop()
    {
        LaunchLevel(nextLevel);
        SaveGame();
    }

    public void RestartLevel()
    {
        if (changingMap != null) return;
        cameraFollowPlayerX = true;
        cameraFollowPlayerY = true;
        inCutscene = false;
        Time.timeScale = 0;

        if (useCheckpoint)
        {
            pointsInLevel = pointsAtCheckpoint;
        }
        else
        {
            pointsInLevel = 0;
        }
        changingMap = StartCoroutine(CR_LoadMap(SceneManager.GetActiveScene().name, false));
    }


    public void ToMainMenu()
    {
        if (changingMap != null) return;
        GameAudio.PlayBGM(null);
        changingMap = StartCoroutine(CR_LoadMap("Title Screen", true));
    }



    IEnumerator CR_LoadMap(string map, bool resetVariables)
    {
        FadeSystem.FadeTo(1, 2);
        while (FadeSystem.isFading)
        {
            yield return new WaitForEndOfFrame();
        }
        if (resetVariables) ResetVariables();
        Time.timeScale = 1;
        SceneManager.LoadScene(map);
        changingMap = null;
    }

    IEnumerator CR_StartBossRush()
    {
        FadeSystem.FadeTo(1, 2);
        while (FadeSystem.isFading)
        {
            yield return new WaitForEndOfFrame();
        }
        changingMap = null;
        BossRushIteration();
    }
}
