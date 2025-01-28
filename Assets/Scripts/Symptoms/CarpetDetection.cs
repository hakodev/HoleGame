using UnityEngine;

public class CarpetDetection : MonoBehaviour
{
    [SerializeField] private Vector3 extents;
    public LayerMask carpetMask;
    Collider[] currentColliders;
    PlayerController playerController;
    PlayerRole playerRole;
    Alteruna.Avatar avatar;

    private AudioSource[] audioSources;

    LobbySystem lobby;
    void Start()
    {
        playerController = transform.root.GetComponent<PlayerController>();
        playerRole = transform.root.GetComponent<PlayerRole>();
        lobby = transform.root.GetComponentInChildren<LobbySystem>();
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCarpetCollision();

    }
    
    void CheckForCarpetCollision()
    {
        currentColliders = Physics.OverlapBox(transform.position, extents, Quaternion.identity);

        if (currentColliders.Length == 0) return;

        foreach (Collider collider in currentColliders)
        {
            if (collider.gameObject.tag != "Carpet") return;

            if (collider.gameObject.GetComponent<CarpetData>())
            {
                CarpetData carpetData = collider.gameObject.GetComponent<CarpetData>();
                if (lobby == null)
                {
                    if (carpetData.IsCorrupted)
                    {
                        if (playerRole.GetRole() != Roles.Machine) return;

                        playerController.Jump();

                        foreach (AudioSource source in audioSources)
                        {
                            source.Play();
                        }
                    }
                }
                else
                {
                    if (carpetData.IsCorruptedLocal)
                    {
                        if (!avatar.IsMe) return;

                        playerController.Jump();

                        foreach (AudioSource source in audioSources)
                        {
                            source.Play();
                        }
                    }


                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, extents * 2);
    }

}
