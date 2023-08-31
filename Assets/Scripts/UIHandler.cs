using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button restartButton;

    private Globals globals;
 
    void Start() {
        globals = FindObjectOfType<Globals>();
    }

    void Update() {
        score.text = globals.Score.ToString();

        if (globals.GameOver)
            restartButton.gameObject.SetActive(true);
    }

    public void OnRestart() {
        SceneManager.LoadScene($"Scenes/{SceneManager.GetActiveScene().name}");
    }
}
