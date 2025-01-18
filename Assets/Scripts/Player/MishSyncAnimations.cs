using Alteruna;
using UnityEngine;
using System.Collections.Generic;
using System;
public class MishSyncAnimations : AttributesSync
{
    [SynchronizableField] public bool Jumping;
    [SynchronizableField] bool Shooting;

    [SynchronizableField] private StanceEnum stance;

    private Alteruna.Avatar avatar;
    private Animator animator;
    private static List<MishSyncAnimations> mishSyncs = new List<MishSyncAnimations>();

    private Transform mixamo;
    private Transform human;

    [SerializeField] float animationSmoothing;
    [SynchronizableField] Vector2 currentAnimDot = Vector3.zero;
    [SynchronizableField] Vector2 targetAnimDot = Vector3.zero;

    public Vector2 GetTargetAnimDot()
    {
        return targetAnimDot;
    }
    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }
    private void Start()
    {
        animator = transform.Find("Animation").GetComponent<Animator>(); // Automatically assign Animator if not set
        mishSyncs.Add(this);

        mixamo = animator.transform.Find("mixamorig:Hips");
        human = animator.transform.Find("Human 2.001");

        stance = StanceEnum.Walking;
    }
    public void SetStance(StanceEnum newStance)
    {
        stance = newStance;
        Commit();
    }
    public void SetJumping(bool newState)
    {
        Jumping = newState;
        Commit();
    }
    public void SetShooting(bool newState)
    {
        if (stance == StanceEnum.Dead) { return; }

        Shooting = newState;
        Commit();
    }
    public void SetInputDirection(Vector2 newInputDirection)
    {
        //open the fluid tree
        if (stance == StanceEnum.Running)
        {
            targetAnimDot = newInputDirection;
            // if (newInputDirection.x != 0 && newInputDirection.y != 0) targetAnimDot = new Vector2(targetAnimDot.x * Mathf.Sign(targetAnimDot.x), targetAnimDot.y * Mathf.Sign(targetAnimDot.y));
        }
        else
        {
            targetAnimDot = newInputDirection / 2;
            //  if (newInputDirection.x != 0 && newInputDirection.y != 0) targetAnimDot = new Vector2(targetAnimDot.x * Mathf.Sign(targetAnimDot.x), targetAnimDot.y * Mathf.Sign(targetAnimDot.y));
        }
        Commit();
    }

    private void Update()
    {
        FixAnimatorOffset();
        UpdateCurrentAnimDot();
        TranslateStatesIntoAnimationStates();
        FreezeAtEndOfDeath();
    }
    private void UpdateCurrentAnimDot()
    {
        //there is a weird sliding movement, maybe if direction is completely opposite tp the current to the idle

        if (targetAnimDot != Vector2.zero && currentAnimDot != Vector2.zero && Mathf.Abs(currentAnimDot.x - targetAnimDot.x) < 0.05f) currentAnimDot.x = targetAnimDot.x;
        if (targetAnimDot != Vector2.zero && currentAnimDot != Vector2.zero && Mathf.Abs(currentAnimDot.y - targetAnimDot.y) < 0.05f) currentAnimDot.y = targetAnimDot.y;



        if (currentAnimDot.x < targetAnimDot.x) currentAnimDot.x += Time.deltaTime * animationSmoothing;
        if (currentAnimDot.x > targetAnimDot.x) currentAnimDot.x -= Time.deltaTime * animationSmoothing;
        if (currentAnimDot.y < targetAnimDot.y) currentAnimDot.y += Time.deltaTime * animationSmoothing;
        if (currentAnimDot.y > targetAnimDot.y) currentAnimDot.y -= Time.deltaTime * animationSmoothing;

    }
    private void TranslateStatesIntoAnimationStates()
    {
        //misc variables
        if (Shooting)
        {
            SetShooting(false);
            animator.SetTrigger("Shooting");
        }

        animator.SetBool("Jumping", Jumping);

        //stances
        foreach (StanceEnum s in Enum.GetValues(typeof(StanceEnum)))
        {
            if (stance != s)// && s != StanceEum.Ignore)
            {
                animator.SetBool(s.ToString(), false);
            }
        }
        //if (stance != StanceEum.Ignore)
        animator.SetBool(stance.ToString(), true);


        //direction


        animator.SetFloat("Horizontal", currentAnimDot.x);
        animator.SetFloat("Vertical", currentAnimDot.y);
    }


    bool playedDeadAnimOnce = false;
    private void FreezeAtEndOfDeath()
    {
        if (!playedDeadAnimOnce && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            playedDeadAnimOnce = true;
            animator.speed = 0f;
        }
    }


    private new void LateUpdate()
    {
        foreach (MishSyncAnimations player in mishSyncs)
        {
            FixAnimatorOffset();
        }
    }
    public void FixAnimatorOffset()
    {

        if (stance == StanceEnum.Dead) { return; }


        animator.transform.localPosition = Vector3.zero;
        animator.transform.rotation = transform.rotation;

        Vector3 temp = mixamo.localPosition;
        mixamo.localPosition = new Vector3(0, temp.y, 0);
        human.localPosition = Vector3.zero;
    }
}

public enum StanceEnum
{
    Dead,
    Crouching,
    Walking,
    Running
}