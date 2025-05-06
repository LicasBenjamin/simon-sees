using UnityEngine;
using UnityEngine.Rendering;

public class CutsceneEffects : MonoBehaviour
{
    public Volume clearVolume;
    public Volume blurryVolume;

    public void ActivateBlur()
    {
        clearVolume.gameObject.SetActive(false);
        blurryVolume.gameObject.SetActive(true);
    }
}
