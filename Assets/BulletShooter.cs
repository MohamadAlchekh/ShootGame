using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletObj;
    public Transform shootingPoint;
    private float Timer;
    public float resetTimer;
    private PlayerMovement playerSc;
    private AudioManager audioManager; // Reference to AudioManager

    private void Start()
    {
        playerSc = GetComponent<PlayerMovement>();
        audioManager = FindObjectOfType<AudioManager>(); // Find and store the reference to the AudioManager
    }

    void Update()
    {
        if (playerSc.GameOver == false)
        {
            if (Input.GetKey(KeyCode.Space) && Timer <= 0)
            {
                Instantiate(bulletObj, shootingPoint.position, transform.rotation);
                Timer = resetTimer;

                // Play shoot sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.shoot);
                }
            }
            else
            {
                Timer -= Time.deltaTime;
            }
        }
    }
}
