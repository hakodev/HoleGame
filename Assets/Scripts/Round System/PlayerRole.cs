using Alteruna;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerRole : AttributesSync
{
    [Header("design")]
    [SerializeField] private float roleDisplayTime;

    [Header("nerd shit - programming")]
    Alteruna.Avatar avatar;
    [SynchronizableField] Roles role;
    public static Roles localClientRole;

    [SerializeField] CanvasGroup infiltratorCanvas;
    [SerializeField] CanvasGroup machineCanvas;


    [SynchronizableField] public int VotedCount=0;
    [SynchronizableField] public bool IsTaskManager = false;
    [SerializeField] private GameObject ceoFlashScreen;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        ceoFlashScreen.SetActive(false);

    }
    [SynchronizableMethod]
    public void DisplayRole()
    {
        if (!avatar.IsMe) { return; }
        // Display the local player's role
        if (role == Roles.Infiltrator)
        {
            StartCoroutine(DisplayRole(infiltratorCanvas));
        }

        if (role == Roles.Machine)
        {
            StartCoroutine(DisplayRole(machineCanvas));
        }

    }
    private IEnumerator DisplayRole(CanvasGroup roleCanvas)
    {
        roleCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(roleDisplayTime); // How many seconds to display it on screen
        roleCanvas.DOFade(0f, 1f);
    }

    private void Update()
    {
        if (IsTaskManager)
        {
            ceoFlashScreen.SetActive(true);
        }
        else
        {
            ceoFlashScreen.SetActive(false);
        }
    }

    public Roles GetRole()
    {
        return role;
    }
    [SynchronizableMethod]
    public void SetRole(Roles newRole)
    {
        role = newRole;
        localClientRole = newRole;
    }
}
    