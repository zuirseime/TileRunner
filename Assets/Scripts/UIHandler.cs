using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI startHint;
    [SerializeField] private TextMeshProUGUI gameOverCurrenScore;
    [SerializeField] private TextMeshProUGUI gameOverBestScore;
    [SerializeField] private TextMeshProUGUI money;

    [SerializeField] private GameObject restartMenu;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject buttonPanel;

    [SerializeField] private Sprite unlockedFrame;
    [SerializeField] private Sprite lockedFrame;

    private Globals globals;
    private Player player;
 
    void Start() {
        globals = FindObjectOfType<Globals>();
        player = FindAnyObjectByType<Player>();

        startHint.gameObject.SetActive(true);

        FillPlaySetMenu();

        StartCoroutine(TextBlink(startHint, 28f, 36f, 1f));
    }

    void Update() {
        score.text = globals.Score.ToString();
        money.text = Globals.MoneyCount.ToString();

        if (globals.GameOver) {
            startHint.gameObject.SetActive(false);
            restartMenu.SetActive(true);
            inventory.SetActive(false);
            gameOverCurrenScore.text = globals.Score.ToString();
            gameOverBestScore.text = Globals.BestScore.ToString();
        }

        if (globals.IsStarted) {
            buttonPanel.SetActive(false);
            inventory.SetActive(false);

            StartCoroutine(FadeHintOut(startHint, .25f));
        }
    }

    private void FillPlaySetMenu() {
        GameObject item = inventory.transform.GetChild(0).gameObject;
        GameObject playSet, locker, price, frame;

        for (int i = 0; i < globals.PlaySets.Count; i++) {
            playSet = Instantiate(item, inventory.transform);
            playSet.transform.GetChild(0).GetComponent<Image>().sprite = globals.PlaySets[i].Icon;

            frame = playSet.transform.GetChild(1).gameObject;
            locker = playSet.transform.GetChild(2).gameObject;
            price = locker.transform.GetChild(1).gameObject;

            price.GetComponent<TextMeshProUGUI>().text = globals.PlaySets[i].Price.ToString();

            playSet.GetComponent<Button>().AddEventListener(i, frame, locker, OnChangePlaySet);

            if (globals.PlaySets[i].IsPurchased || globals.PlaySets[i].Price == 0) {
                frame.GetComponent<Image>().sprite = unlockedFrame;
                Destroy(locker);
            } else {
                frame.GetComponent<Image>().sprite = lockedFrame;
            }
        }

        Destroy(item);
    }

    /// <summary>
    /// Restarts current scene
    /// </summary>
    public void OnRestart() {
        SceneManager.LoadScene($"Scenes/{SceneManager.GetActiveScene().name}");
    }

    public void OnOpenInventory() {
        bool menuActivity = inventory.activeSelf;
        inventory.SetActive(!menuActivity);
        player.enabled = menuActivity;
    }

    private void OnChangePlaySet(int id, GameObject frame, GameObject locker) {
        PlaySet set = globals.PlaySets[id];

        if (set.IsPurchased || set.Price == 0) {
            Globals.CurrentPlaySet = set.Name;
            OnRestart();
        } else {
            if (Globals.MoneyCount >= set.Price) {
                frame.GetComponent<Image>().sprite = unlockedFrame;
                set.IsPurchased = true;
                Globals.MoneyCount -= set.Price;
                PlayerPrefs.SetInt($"{(int)set.Name}-isPurchased", 1);
                PlayerPrefs.Save();
                Destroy(locker);
            }
        }
    }

    /// <summary>
    /// Smoothly makes the text blinks
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="minSize">The minimum size value</param>
    /// <param name="maxSize">The maximum size velue</param>
    /// <param name="duration">Change duration (in seconds)</param>
    /// <returns></returns>
    private IEnumerator TextBlink(TextMeshProUGUI hint, float minSize, float maxSize, float duration) {
        while (true) {
            StartCoroutine(ChangeTextSize(hint, minSize, maxSize, duration));
            yield return new WaitForSecondsRealtime(duration);
            StartCoroutine(ChangeTextSize(hint, maxSize, minSize, duration));
            yield return new WaitForSecondsRealtime(duration);
        }
    }

    /// <summary>
    /// Smoothly changes text size
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="v1">The start value</param>
    /// <param name="v2">The end value</param>
    /// <param name="t">Change duration</param>
    /// <returns></returns>
    private IEnumerator ChangeTextSize(TextMeshProUGUI hint, float v1, float v2, float t) {
        float elapsedTime = 0f;
        while (elapsedTime < t) {
            float newSize = Mathf.Lerp(v1, v2, elapsedTime / t);
            hint.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hint.fontSize = v2;
    }

    /// <summary>
    /// Hides the hint
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="duration">Fade duration</param>
    /// <returns></returns>
    private IEnumerator FadeHintOut(TextMeshProUGUI hint, float duration) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            hint.color = new Color(hint.material.color.r, hint.material.color.g, hint.material.color.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hint.color = new Color(hint.material.color.r, hint.material.color.g, hint.material.color.b, 0f);

        yield return new WaitForSecondsRealtime(1f);
        hint.gameObject.SetActive(false);
    }
}

public static class ButtonExtention {
    public static void AddEventListener<T>(this Button btn, T param, Action<T> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param); });
    }

    public static void AddEventListener<T1, T2, T3>(this Button btn, T1 param1, T2 param2, T3 param3, Action<T1, T2, T3> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param1, param2, param3); });
    }
}