using UnityEngine;

public class BeachBallBehaviour : MonoBehaviour {
    [SerializeField] private float kickForce = 11f;

    private CharacterController characterController;
    private PlayerController playerController;

    [SerializeField] private Vector3 extents;
    [SerializeField] private Vector3 offset;
    private Collider[] currentColliders;

    AudioSource source;
    private void Start() {
        characterController = transform.root.gameObject.GetComponent<CharacterController>();
        playerController = transform.root.gameObject.GetComponent<PlayerController>();
    }


    Rigidbody ballRigidbody;
    GameObject previouslyCollidedWithThisBall;
    int ignoreSelfPlayerLayer = ~(1 << 10);

    private void FixedUpdate()
    {
        DoBalls();
    }

    private void DoBalls()
    {
        currentColliders = Physics.OverlapBox(transform.position + offset, extents, Quaternion.identity);

        if (currentColliders.Length == 0) return;

        foreach (Collider conHit in currentColliders)
        {
            if (conHit.gameObject.CompareTag("Ball")) //yes this is only for yoga balls
            {
                if (previouslyCollidedWithThisBall != conHit.gameObject)
                {
                    previouslyCollidedWithThisBall = conHit.gameObject;
                    ballRigidbody = previouslyCollidedWithThisBall.GetComponent<Rigidbody>();
                }

                if (characterController.isGrounded)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height, ignoreSelfPlayerLayer)) //last number ignores selfplayer layer              
                    {
                        if (hit.collider == conHit)
                        {
                            playerController.Jump();
                            PlayerAudioManager.Instance.PlaySound(conHit.gameObject, source, PlayerAudioManager.Instance.GetBouncyBall);


                            RaycastHit ballHitGround;
                            if (!Physics.Raycast(previouslyCollidedWithThisBall.transform.position, Vector3.down, out ballHitGround, 0.55f))
                            {
                                ballRigidbody.linearVelocity = -ballRigidbody.linearVelocity / 2f;
                                PlayerAudioManager.Instance.PlaySound(conHit.gameObject, source, PlayerAudioManager.Instance.GetBouncyBall);
                            }
                        }
                        else
                        {
                            Vector3 direction = (ballRigidbody.transform.position - this.transform.position).normalized;
                            ballRigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
                        }
                    }
                }
            }           
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position+ offset, extents * 2);
    }

}
