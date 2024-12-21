using UnityEngine;
using DG.Tweening;

public class GunBehaviour : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private int maxShotsBeforeReloading;
    [SerializeField] private float bulletMaxDistance;
    [SerializeField] private float cameraNormalFOV;
    [SerializeField] private float cameraZoomedFOV;
    [SerializeField] private float fovToggleDuration;
    private int shotsTaken = 0;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            Fire();
        }

        if(Input.GetKeyUp(KeyCode.Mouse1)) {
            NormalAim();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1)) {
            ZoomAim();
        }

        if(Input.GetKeyDown(KeyCode.R) && shotsTaken > 0) {
            Reload();
        }

        if(shotsTaken == maxShotsBeforeReloading) {
            Debug.Log("Out of ammo, reloading...");
            Reload();
        }
    }

    private void Fire() {
        Debug.Log("Fired weapon");
        if(Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, bulletMaxDistance)) {
            if(hit.collider.gameObject.CompareTag("Player")) {
                hit.collider.gameObject.GetComponent<Health>().DamagePlayer(Random.Range(3f, 7f)); // Change later
                Debug.Log("BULLSEYE!");
            }
        }
        shotsTaken++;
    }

    private void NormalAim() {
        playerCamera.DOFieldOfView(cameraNormalFOV, fovToggleDuration);
    }

    private void ZoomAim() {
        playerCamera.DOFieldOfView(cameraZoomedFOV, fovToggleDuration);
    }

    private void Reload() {
        Debug.Log("Reloading...");
        shotsTaken = 0;
    }
}
