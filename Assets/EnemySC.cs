using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Ensure you include this namespace

public class EnemySC : MonoBehaviour
{
    public GameObject target;
    public float speed;
    private PlayerMovement playerSc;
    public GameObject Explosion;
    private AudioManager audioManager; // Reference to AudioManager
    private int hitCount = 0; // Track number of hits
    private int maxHits; // Maximum hits needed to destroy

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        playerSc = target.GetComponent<PlayerMovement>();
        audioManager = FindObjectOfType<AudioManager>(); // Find and store the reference to the AudioManager

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            maxHits = 2; // Set maximum hits for Level 2
            speed *= 1.1f; // Increase speed by 50% for Level 2
        }
        else
        {
            maxHits = 1; // Set maximum hits for Level 1
        }
    }

    void Update()
    {
        if (playerSc.GameOver == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject); // Destroy bullet

            hitCount++;
            if (hitCount >= maxHits)
            {
                Destroy(gameObject);
                Instantiate(Explosion, transform.position, Quaternion.identity);

                // Play strike sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.strike);
                }

                if (playerSc != null)
                {
                    playerSc.EnemyKilled();
                    Debug.Log("Enemy Killed. Score should be updated.");
                }
                else
                {
                    Debug.LogError("PlayerMovement script not found on player object.");
                }
            }
            else
            {
                // Indicate enemy is hit but not destroyed
                GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
}
