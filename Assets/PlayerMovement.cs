using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float RotSpeed;
    public bool GameOver;
    public GameObject GameOverObj;
    public GameObject Explosion;
    public Text ScoreText;
    public Text MathQuestionText;
    public InputField AnswerInputField;
    public Text TimerText;
    public Button RestartButton;
    public Button SubmitAnswerButton;
    private int score = 0;

    private float timer = 5f; // 5 seconds timer for the math question
    private bool isAnsweringQuestion = false;
    private int correctAnswer;

    private float invincibilityDuration = 5f; // Duration of invincibility after solving the question
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private Camera mainCamera;
    private bool edgeCollision = false; // Flag to check if collision was with edge

    private AudioManager audioManager; // Reference to AudioManager

    private string currentScene;

    // Memory challenge variables
    private string memorySequence;
    private int memoryLength = 3; // Length of the memory sequence
    private float memoryDisplayTime = 3f; // Time to display the memory sequence

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;

        mainCamera = Camera.main;
        UpdateScoreText();
        MathQuestionText.gameObject.SetActive(false);
        AnswerInputField.gameObject.SetActive(false);
        TimerText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        SubmitAnswerButton.gameObject.SetActive(false);
        SubmitAnswerButton.onClick.AddListener(CheckAnswer); // Add listener to the button

        // Find and store the reference to the AudioManager
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (!GameOver)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.Rotate(new Vector3(0, 0, -1) * RotSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.Rotate(new Vector3(0, 0, 1) * RotSpeed * Time.deltaTime);
            }

            CheckEdges();

            if (isInvincible)
            {
                invincibilityTimer -= Time.deltaTime;
                if (invincibilityTimer <= 0)
                {
                    isInvincible = false;
                }
            }
        }

        if (isAnsweringQuestion)
        {
            timer -= Time.deltaTime;
            TimerText.text = "Time left: " + Mathf.Round(timer).ToString();

            if (timer <= 0)
            {
                RestartGame();
            }
        }
    }

    private void CheckEdges()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            if (!isInvincible)
            {
                edgeCollision = true;
                TriggerGameOver();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isInvincible)
            {
                edgeCollision = false;
                TriggerGameOver();
            }
        }
    }

    private void TriggerGameOver()
    {
        GameOver = true;
        Instantiate(Explosion, transform.position, Quaternion.identity);

        // Play death sound
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.death);
        }

        GameOverObj.SetActive(true);
        RestartButton.gameObject.SetActive(true);
        SubmitAnswerButton.gameObject.SetActive(true);

        if (currentScene == "Level1")
        {
            GenerateMathQuestion();
        }
        else if (currentScene == "Level2")
        {
            StartCoroutine(ShowMemorySequence());
        }
    }

    private void GenerateMathQuestion()
    {
        MathQuestionText.gameObject.SetActive(true);
        AnswerInputField.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);

        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int c = Random.Range(1, 10);
        correctAnswer = a + b + c;

        MathQuestionText.text = $"  Devam etmek için  \n\n   ({a} + {b} + {c}) = ?";
        isAnsweringQuestion = true;
        timer = 50f;
    }

    private IEnumerator ShowMemorySequence()
    {
        // Generate a random sequence of numbers
        memorySequence = "";
        for (int i = 0; i < memoryLength; i++)
        {
            memorySequence += Random.Range(0, 10).ToString();
        }

        // Display the sequence to the player
        MathQuestionText.text = $"Hatırla : {memorySequence}";
        MathQuestionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(memoryDisplayTime);

        // Hide the sequence and prompt for input
        MathQuestionText.gameObject.SetActive(false);
        AnswerInputField.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);
        isAnsweringQuestion = true;
        timer = 50f;
    }

    public void CheckAnswer()
    {
        if (currentScene == "Level1")
        {
            CheckMathAnswer();
        }
        else if (currentScene == "Level2")
        {
            CheckMemoryAnswer();
        }
    }

    private void CheckMathAnswer()
    {
        if (int.TryParse(AnswerInputField.text, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                ContinueGame();
            }
            else
            {
                RestartGame();
            }
        }
        else
        {
            RestartGame();
        }
    }

    private void CheckMemoryAnswer()
    {
        if (AnswerInputField.text == memorySequence)
        {
            ContinueGame();
        }
        else
        {
            RestartGame();
        }
    }

    private void ContinueGame()
    {
        isAnsweringQuestion = false;
        MathQuestionText.gameObject.SetActive(false);
        AnswerInputField.gameObject.SetActive(false);
        TimerText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        SubmitAnswerButton.gameObject.SetActive(false);
        GameOverObj.SetActive(false);
        GameOver = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        // Play correct answer sound
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.correctAnswer);
        }

        // Activate invincibility
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        if (edgeCollision)
        {
            // Respawn player at the center
            transform.position = Vector3.zero;
            edgeCollision = false;
        }
    }

    public void EnemyKilled()
    {
        score++;
        UpdateScoreText();

        if (currentScene == "Level1" && score >= 10)
        {
            LoadNextLevel();
        }
        else if (currentScene == "Level2" && score >= 20)
        {
            // Game completion logic or end game screen
            Debug.Log("Congratulations! You have completed the game.");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level2");
    }

    private void UpdateScoreText()
    {
        ScoreText.text = "Score: " + score;
    }
}
