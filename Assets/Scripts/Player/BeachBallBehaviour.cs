using UnityEngine;

public class BeachBallBehaviour : MonoBehaviour {
    [SerializeField] private float kickForce = 11f;

    private CharacterController characterController;
    private PlayerController playerController;

    private void Start() {
        characterController = transform.root.gameObject.GetComponent<CharacterController>();
        playerController = transform.root.gameObject.GetComponent<PlayerController>();
    }

    private void Update() {
        if((characterController.collisionFlags & CollisionFlags.CollidedBelow) != 0)
        {

        }
    }

    Rigidbody ballRigidbody;
    GameObject previouslyCollidedWithThisBall;
    int ignoreSelfPlayerLayer = ~(1 << 10);

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        DoBalls(hit);
    }

    private void DoBalls(ControllerColliderHit conHit)
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
                    if (hit.collider == conHit.collider)
                    {
                        playerController.Jump();


                        RaycastHit ballHitGround;
                        if (!Physics.Raycast(previouslyCollidedWithThisBall.transform.position, Vector3.down, out ballHitGround, 0.55f))
                        {
                            ballRigidbody.linearVelocity = -ballRigidbody.linearVelocity / 2f;
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
