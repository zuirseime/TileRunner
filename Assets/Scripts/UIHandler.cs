using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button restartButton;
 
    // Start is called before the first frame update
    void Start()
    {
        restartButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        score.text = MovementController.Score.ToString();

        if (TileGenerator.GameOver)
            restartButton.gameObject.SetActive(true);
    }

    public void OnRestart()
    {
        SceneManager.LoadScene($"Scenes/{SceneManager.GetActiveScene().name}");
    }
}
