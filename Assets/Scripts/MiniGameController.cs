using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniGameController : MonoBehaviour
{
    public RectTransform barArea;
    public RectTransform line;
    public GameObject targetPrefab;
    public int targetCount = 5;
    public float lineSpeed = 400f;
    public int maxMisses = 3;
    public AudioSource hitSound;
    public AudioSource errorSound;
    public GeneratorController generator;

    private List<GameObject> activeTargets = new List<GameObject>();
    private float direction = 1f;
    private int missCount = 0;
    private bool isRunning = false;

    void OnEnable()
    {
        ResetMiniGame();
    }

    void Update()
    {
        if (!isRunning) return;

        // ✅ Escape to exit mini-game without completing it
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed – exiting mini-game.");
            isRunning = false;
            gameObject.SetActive(false);

            if (generator != null)
            {
                generator.ExitMiniGameEarly(); // Don't complete the game
            }
            return;
        }

        float move = lineSpeed * direction * Time.deltaTime;
        line.localPosition += new Vector3(move, 0f, 0f);

        float halfLineWidth = line.rect.width / 2f;
        float minX = -barArea.rect.width / 2f + halfLineWidth;
        float maxX = barArea.rect.width / 2f - halfLineWidth;

        if (line.localPosition.x <= minX)
        {
            line.localPosition = new Vector3(minX, 0f, 0f);
            direction = 1f;
        }
        else if (line.localPosition.x >= maxX)
        {
            line.localPosition = new Vector3(maxX, 0f, 0f);
            direction = -1f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }

        UpdateTargetHighlights();
    }

    void CheckClick()
    {
        bool hit = false;
        float lineX = line.localPosition.x;

        foreach (var target in new List<GameObject>(activeTargets))
        {
            if (target == null) continue;

            RectTransform targetRect = target.GetComponent<RectTransform>();
            float targetX = targetRect.localPosition.x;
            float halfWidth = targetRect.rect.width / 2f;

            if (lineX >= targetX - halfWidth && lineX <= targetX + halfWidth)
            {
                Debug.Log("Target HIT: " + target.name);
                hitSound.Play();
                activeTargets.Remove(target);
                StartCoroutine(FadeAndRemoveTarget(target));
                hit = true;
            }
        }

        if (hit)
        {
            if (activeTargets.Count == 0)
            {
                CompleteMiniGame();
            }
        }
        else
        {
            missCount++;
            if (errorSound != null) errorSound.Play();

            if (missCount >= maxMisses)
            {
                ResetMiniGame();
            }
        }
    }

    void CompleteMiniGame()
    {
        isRunning = false;
        gameObject.SetActive(false);
        generator.CompleteMiniGame();
    }

    void ResetMiniGame()
    {
        foreach (var obj in activeTargets)
        {
            if (obj != null)
            {
                Image[] images = obj.GetComponentsInChildren<Image>(true);
                foreach (Image img in images)
                {
                    if (img.gameObject.name == "Outline")
                    {
                        Color c = img.color;
                        c.a = 0f;
                        img.color = c;
                    }
                }
                Destroy(obj);
            }
        }

        activeTargets.Clear();
        missCount = 0;

        float barWidth = barArea.rect.width;
        float padding = 40f;
        float usableWidth = barWidth - (padding * 2);
        float spacing = (targetCount > 1) ? usableWidth / (targetCount - 1) : 0f;
        float startX = (targetCount > 1) ? -((targetCount - 1) * spacing) / 2f : 0f;

        for (int i = 0; i < targetCount; i++)
        {
            GameObject newTarget = Instantiate(targetPrefab, barArea);
            RectTransform rect = newTarget.GetComponent<RectTransform>();
            float xPos = startX + (spacing * i);
            rect.localPosition = new Vector3(xPos, 0f, 0f);

            Image[] images = newTarget.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Outline")
                {
                    Color c = img.color;
                    c.a = 0f;
                    img.color = c;
                }
            }

            activeTargets.Add(newTarget);
        }

        line.localPosition = new Vector3(barArea.rect.width / 2f, 0f, 0f);
        direction = -1f;
        isRunning = true;
    }

    IEnumerator FadeAndRemoveTarget(GameObject target)
    {
        Image[] outlines = target.GetComponentsInChildren<Image>(true);
        foreach (Image img in outlines)
        {
            if (img.gameObject.name == "Outline")
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }

        Image imgMain = target.GetComponent<Image>();
        if (imgMain == null)
        {
            Destroy(target);
            yield break;
        }

        float duration = 0.25f;
        float t = 0f;
        Color originalColor = imgMain.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            imgMain.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(target);
    }

    void UpdateTargetHighlights()
    {
        float lineX = line.localPosition.x;

        foreach (var target in activeTargets)
        {
            if (target == null) continue;

            RectTransform targetRect = target.GetComponent<RectTransform>();
            float targetX = targetRect.localPosition.x;
            float halfWidth = targetRect.rect.width / 2f;
            bool isHovered = (lineX >= targetX - halfWidth && lineX <= targetX + halfWidth);

            Image[] images = target.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Outline")
                {
                    Color c = img.color;
                    c.a = isHovered ? 1f : 0f;
                    img.color = c;
                }
            }
        }
    }
}
