using UnityEngine;

public class BeachBallBehaviour : MonoBehaviour {
    [SerializeField] private Vector3 extents;
    [SerializeField] private float kickForce = 11f;
    //[SerializeField] private CarpetDetection carpetDetection;

    private CharacterController characterController;
    private PlayerController playerController;

    private void Start() {
        characterController = transform.root.gameObject.GetComponent<CharacterController>();
        playerController = transform.root.gameObject.GetComponent<PlayerController>();
    }

    private void Update() {
    }

    Rigidbody ballRigidbody;
    GameObject previouslyCollidedWithThisBall;
    int ignoreSelfPlayerLayer = ~(1 << 10);
    void OnControllerColliderHit(ControllerColliderHit conHit)
    {
        if (conHit.gameObject.CompareTag("Ball")) //yes this is only for yoga balls
        {
            if (previouslyCollidedWithThisBall != conHit.gameObject)
            {
                Debug.Log("oop new ball");
                previouslyCollidedWithThisBall = conHit.gameObject;
                ballRigidbody = previouslyCollidedWithThisBall.GetComponent<Rigidbody>();
            }

            if (characterController.isGrounded)
            {
                RaycastHit hit;
                Debug.DrawRay(transform.position, Vector3.down* (characterController.height), Color.magenta);
                if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height, ignoreSelfPlayerLayer)) //last number ignores selfplayer layer
                {
                    Debug.Log("oop raycast");
                    if (hit.collider.transform.gameObject == previouslyCollidedWithThisBall)
                    {
                        Debug.Log("oop obj");
                        playerController.Jump();
                    }
                    else
                    {
                        Debug.Log("oop jump");
                        Vector3 direction = (ballRigidbody.transform.position - this.transform.position).normalized;
                        ballRigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
                    }
                }
            }
        }

    }


    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, extents * 2);
    }
}
