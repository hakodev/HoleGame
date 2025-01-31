using UnityEngine;

public class BeachBallBehaviour : DynamicInteractableObject {
    [SerializeField] private Vector3 extents;
    [SerializeField] private float kickForce = 2f;
    [SerializeField] private CarpetDetection carpetDetection;

    private Collider[] currentColliders;
    private CharacterController characterController;
    private PlayerController playerController;

    protected override void Start() {
        base.Start();
        characterController = transform.root.gameObject.GetComponent<CharacterController>();
        playerController = transform.root.gameObject.GetComponent<PlayerController>();
    }

    protected override void Update() {
        base.Update();
        CheckForBallCollision();
    }


    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {

    }
    public override void Use() 
    {
    
    }


    private void CheckForBallCollision() {
        currentColliders = Physics.OverlapBox(transform.position, extents, Quaternion.identity);

        if(currentColliders.Length == 0) return;

        foreach(Collider collider in currentColliders) {
            if(collider.gameObject.GetComponent<Rigidbody>() && collider.gameObject.CompareTag("Ball")) {
                Rigidbody ballRigidbody = collider.gameObject.GetComponent<Rigidbody>();

                if(characterController.isGrounded && carpetDetection.IsStandingOnCarpet) {
                    Vector3 direction = (ballRigidbody.transform.position - this.transform.position).normalized;
                    ballRigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
                } else {
                    playerController.Jump();
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, extents * 2);
    }
}
