using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]    private Text _scoreText;
    [SerializeField]    private Image _livesImage;
    [SerializeField]    private Sprite[] _livesSprites;
    [SerializeField]    private Text _gameOverText;
    [SerializeField]    private Text _restartText;
    [SerializeField]    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUIScoreText(0);
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager in NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateUIScoreText(int scoreValue)
    {
        _scoreText.text = "Score: " + scoreValue;
    }

    public void UpdateCurrentLives(int currentLives)
    {
        if (currentLives < 0)
        {
            currentLives = 0;
        }
        _livesImage.sprite = _livesSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverSequence();

        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

}
