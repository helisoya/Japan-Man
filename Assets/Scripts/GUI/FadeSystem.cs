using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystem : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private static FadeSystem instance;

    private Coroutine fading;

    public static bool isFading
    {
        get { return instance.fading != null; }
    }

    void Awake()
    {
        instance = this;
    }

    public static void ForceAlpha(float value)
    {
        if (instance.fading != null)
        {
            instance.StopCoroutine(instance.fading);
        }
        Color color = instance.fadeImage.color;
        color.a = value;
        instance.fadeImage.color = color;
    }

    public static void FadeTo(float value, float speed)
    {
        if (instance.fading != null)
        {
            instance.StopCoroutine(instance.fading);
        }
        instance.fading = instance.StartCoroutine(instance.CR_FadingTo(value, speed));
    }

    IEnumerator CR_FadingTo(float value, float speed)
    {
        float alpha = fadeImage.color.a;
        Color col = fadeImage.color;
        int side = alpha < value ? 1 : -1;


        while (alpha != value)
        {
            alpha = Mathf.Clamp(alpha + Time.unscaledDeltaTime * speed * side, side == 1 ? 0 : value, side == -1 ? 1 : value);
            col.a = alpha;
            fadeImage.color = col;
            yield return new WaitForEndOfFrame();
        }
        fading = null;
    }
}
