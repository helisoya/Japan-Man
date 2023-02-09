using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
[System.Serializable]
public class GameAudio : MonoBehaviour
{
    private static GameAudio instance;

    [SerializeField] private GameObject sfxPrefab;

    [SerializeField] private AudioSource bgmSource;

    private string currentBGM = "";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        GameManager.instance.ReloadMixerPrefs();
    }

    public static void PlaySFX(string fileName, Vector3 position)
    {
        bool global = position.Equals(Vector3.positiveInfinity);
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + fileName);
        if (clip != null)
        {
            GameObject obj = Instantiate(instance.sfxPrefab, global ? Vector3.zero : position, Quaternion.identity);
            obj.name = "SFX-" + fileName;
            if (global)
            {
                obj.transform.parent = FindObjectOfType<PlayerMovement>().transform;
                obj.transform.localPosition = Vector3.zero;
            }
            obj.GetComponent<AudioSource>().clip = clip;
            obj.GetComponent<AudioSource>().Play();
            Destroy(obj, clip.length);
        }
    }

    public static void PlaySFX(string filename)
    {
        PlaySFX(filename, Vector3.positiveInfinity);
    }


    public static void PlayBGM(string newBGM)
    {

        if (newBGM == null)
        {
            instance.currentBGM = "";
            instance.bgmSource.Stop();
        }

        if (instance.currentBGM.Equals(newBGM)) return;

        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/" + newBGM);

        if (clip != null)
        {
            instance.currentBGM = newBGM;
            instance.bgmSource.Stop();
            instance.bgmSource.clip = clip;
            instance.bgmSource.Play();
        }
    }
}
