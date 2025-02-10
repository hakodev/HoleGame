using UnityEngine;
using static Unity.VisualScripting.Member;

public class FootstepAudio : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterController characterController;
    float timer = 0;
    private bool bPos;

    public float interval = 1;
    private float originalInterval;

    private AudioSource source;
    void Start()
    {
        originalInterval = interval;
        source = GetComponent<AudioSource>();
        playerController = GetComponentInParent<PlayerController>();
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isMoving && characterController.isGrounded)
        {
            interval = originalInterval;
            if (playerController.isRunning)
            {
                interval = originalInterval * 0.7f;
            }
            if (timer >= interval)
            {
                source.PlayOneShot(source.clip);
                timer = 0;
            }
            timer += Time.deltaTime;

        }

    }
}
