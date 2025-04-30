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

        foreach (var target in new List<GameObject>(activeTargets)) {
            if (target == null) continue;

            RectTransform targetRect = target.GetComponent<RectTransform>();
            float targetX = targetRect.localPosition.x;
            float halfWidth = targetRect.rect.width / 2f;

            if (lineX >= targetX - halfWidth && lineX <= targetX + halfWidth) {
                Debug.Log("Target HIT: " + target.name);

                // ✅ Remove target before fading it
                activeTargets.Remove(target);
                StartCoroutine(FadeAndRemoveTarget(target));

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
            if (obj != null) {
                // Reset outline before destroy
                Image[] images = obj.GetComponentsInChildren<Image>(true);
                foreach (Image img in images) {
                    if (img.gameObject.name == "Outline") {
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
        float spacing = usableWidth / (targetCount - 1);

        for (int i = 0; i < targetCount; i++) {
            GameObject newTarget = Instantiate(targetPrefab, barArea);

            if (newTarget == null) {
                Debug.LogError("Failed to instantiate targetPrefab!");
                continue;
            }

            RectTransform rect = newTarget.GetComponent<RectTransform>();
            float xPos = -barWidth / 2 + padding + (spacing * i);
            rect.localPosition = new Vector3(xPos, 0f, 0f);

            // Reset outline on new target
            Image[] images = newTarget.GetComponentsInChildren<Image>(true);
            foreach (Image img in images) {
                if (img.gameObject.name == "Outline") {
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

    IEnumerator FadeAndRemoveTarget(GameObject target) {
        Debug.Log("Starting fade and destroy for: " + target.name);

        // Reset outline before fading
        Image[] outlines = target.GetComponentsInChildren<Image>(true);
        foreach (Image img in outlines) {
            if (img.gameObject.name == "Outline") {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }

        Image imgMain = target.GetComponent<Image>();
        if (imgMain == null) {
            Destroy(target);
            yield break;
        }

        float duration = 0.25f;
        float t = 0f;
        Color originalColor = imgMain.color;

        while (t < duration) {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            imgMain.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
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

            // Update all outlines only if still active
            Image[] images = target.GetComponentsInChildren<Image>(true);
            foreach (Image img in images) {
                if (img.gameObject.name == "Outline") {
                    Color c = img.color;
                    c.a = isHovered ? 1f : 0f;
                    img.color = c;
                }
            }
        }
    }
}
