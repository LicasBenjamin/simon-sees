using UnityEngine;

public class SpriteController : MonoBehaviour
{
    public GameObject spriteObject;

    public void ShowSprite()
    {
        Debug.Log("Sprite activated!");
        spriteObject.SetActive(true);
    }
}
