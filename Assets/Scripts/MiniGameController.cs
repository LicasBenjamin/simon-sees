using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniGameController : MonoBehaviour {
    public RectTransform barArea;
    public RectTransform line;
    public GameObject targetPrefab;
    public int targetCount = 5;
    public float lineSpeed = 400f;
    public int maxMisses = 3;
    public AudioSource errorSound;
    public GeneratorController generator;

    private List<GameObject> activeTargets = new List<GameObject>();
    private float direction = 1f;
    private int missCount = 0;
    private bool isRunning = false;

    void OnEnable() {
        ResetMiniGame();
    }

    void Update() {
        if (!isRunning) return;

        float move = lineSpeed * direction * Time.deltaTime;
        line.localPosition += new Vector3(move, 0f, 0f);

        float halfLineWidth = line.rect.width / 2f;
        float minX = -barArea.rect.width / 2f + halfLineWidth;
        float maxX = barArea.rect.width / 2f - halfLineWidth;

        if (line.localPosition.x <= minX) {
            line.localPosition = new Vector3(minX, 0f, 0f);
            direction = 1f;
        } else if (line.localPosition.x >= maxX) {
            line.localPosition = new Vector3(maxX, 0f, 0f);
            direction = -1f;
        }

        if (Input.GetMouseButtonDown(0)) {
            CheckClick();
        }

        UpdateTargetHighlights();
    }

    void CheckClick() {
        bool hit = false;
        float lineX = line.localPosition.x;

        foreach (var target in new List<GameObject>(activeTargets)) { // safe copy
            if (target == null) continue;

            RectTransform targetRect = target.GetComponent<RectTransform>();
            float targetX = targetRect.localPosition.x;
            float halfWidth = targetRect.rect.width / 2f;

            if (lineX >= targetX - halfWidth && lineX <= targetX + halfWidth) {
                Debug.Log("Target HIT: " + target.name);
                StartCoroutine(FadeAndRemoveTarget(target));
                activeTargets.Remove(target); // ✅ critical fix
                hit = true;
            }
        }

        if (hit) {
            Debug.Log("Remaining targets: " + activeTargets.Count);

            if (activeTargets.Count == 0) {
                Debug.Log("All targets cleared. Completing mini-game.");
                CompleteMiniGame();
            }
        } else {
            missCount++;
            Debug.Log("Missed! Count: " + missCount);
            if (errorSound != null) errorSound.Play();

            if (missCount >= maxMisses) {
                Debug.Log("Too many misses — restarting mini-game.");
                ResetMiniGame();
            }
        }
    }

    void CompleteMiniGame() {
        Debug.Log("Mini-game completed. Notifying generator.");
        isRunning = false;
        gameObject.SetActive(false);
        generator.CompleteMiniGame();
    }

    void ResetMiniGame() {
        Debug.Log("Resetting mini-game.");
        foreach (var obj in activeTargets) {
            if (obj != null) Destroy(obj);
        }

        activeTargets.Clear();
        missCount = 0;

        float barWidth = barArea.rect.width;
        float padding = 40f;
        float usableWidth = barWidth - (padding * 2);
        float spacing = usableWidth / (targetCount - 1);

        for (int i = 0; i < targetCount; i++) {
            GameObject newTarget = Instantiate(targetPrefab, barArea);
            RectTransform rect = newTarget.GetComponent<RectTransform>();
            float xPos = -barWidth / 2 + padding + (spacing * i);
            rect.localPosition = new Vector3(xPos, 0f, 0f);
            activeTargets.Add(newTarget);
        }

        line.localPosition = new Vector3(barArea.rect.width / 2f, 0f, 0f);
        direction = -1f;
        isRunning = true;
    }

    IEnumerator FadeAndRemoveTarget(GameObject target) {
        Debug.Log("Starting fade and destroy for: " + target.name);
        Image img = target.GetComponent<Image>();
        if (img == null) {
            Destroy(target);
            yield break;
        }

        float duration = 0.25f;
        float t = 0f;
        Color originalColor = img.color;

        while (t < duration) {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Debug.Log("Destroying: " + target.name);
        Destroy(target);
    }

    void UpdateTargetHighlights() {
        float lineX = line.localPosition.x;

        foreach (var target in activeTargets) {
            if (target == null) continue;

            RectTransform targetRect = target.GetComponent<RectTransform>();
            float targetX = targetRect.localPosition.x;
            float halfWidth = targetRect.rect.width / 2f;

            bool isHovered = (lineX >= targetX - halfWidth && lineX <= targetX + halfWidth);

            Transform outlineChild = target.transform.Find("Outline");
            if (outlineChild != null) {
                Image outlineImg = outlineChild.GetComponent<Image>();
                if (outlineImg != null) {
                    Color c = outlineImg.color;
                    c.a = isHovered ? 1f : 0f;
                    outlineImg.color = c;
                }
            }
        }
    }
}
