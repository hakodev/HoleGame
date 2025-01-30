using UnityEngine;

public class BeachBallKickBehaviour : MonoBehaviour {
    [SerializeField] private Vector3 extents;
    [SerializeField] private float kickForce = 12f;

    private Collider[] currentColliders;

    private void Update() {
        CheckForBallCollision();
    }

    private void CheckForBallCollision() {
        currentColliders = Physics.OverlapBox(transform.position, extents, Quaternion.identity);

        if(currentColliders.Length == 0) return;

        foreach(Collider collider in currentColliders) {
            if(!collider.gameObject.CompareTag("Ball")) continue;

            Rigidbody ballRigidbody = collider.gameObject.GetComponent<Rigidbody>();

            if(ballRigidbody) {
                Vector3 direction = (ballRigidbody.transform.position - this.transform.position).normalized;
                ballRigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, extents * 2);
    }
}
