using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GLOBALSAVE
{
    public float masterSound;
    public float sfxSound;
    public float bgmSound;
    public bool fullscreen = true;
    public int screenWidth;
    public int screenHeight;
    public int refreshRate;
    [SerializeField] public List<Achievement> achievements;
}
