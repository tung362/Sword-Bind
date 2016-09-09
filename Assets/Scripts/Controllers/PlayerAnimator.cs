using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    //Attack
    [Header("Attack Animation Settings")]
    public float LastTimeAttackedReset = 0.5f;
    private float LastTimeAttackedTimer = 0;
    private bool CanAttack = true;
    public float AttackChainCooldown = 0.5f;
    private float AttackChainCooldownTimer = 0;

    //Fall
    private Vector3 GroundPosition;

    //Sword Throw
    [HideInInspector]
    public bool HasSword = true;

    private GlobalVars Global;
    private PlayerManager TheManager;
    private PlayerController ThePlayerController;
    private Animator TheAnimator;

    void Start()
    {
        Global = FindObjectOfType<GlobalVars>();
        TheManager = GetComponent<PlayerManager>();
        ThePlayerController = GetComponent<PlayerController>();
        TheAnimator = GetComponentInChildren<Animator>();
    }

    //Use this like a table of contents
    public void Animate()
    {
        Walk();
        Strafe();
        Dash();
        Block();
        Attack();
    }

    #region Walk
    void Walk()
    {
        //Default outputs when keys aren't pressed
        float moveForward = 0;
        float moveSide = 0;
        bool isIdle = true;

        if (TheManager.UP)
        {
            isIdle = false;
            moveForward += Vector3.Dot(transform.forward, new Vector3(0, 0, 1));
            moveSide += Vector3.Dot(transform.right, new Vector3(0, 0, 1));
        }
        if (TheManager.DOWN)
        {
            isIdle = false;
            moveForward += Vector3.Dot(transform.forward, new Vector3(0, 0, -1));
            moveSide += Vector3.Dot(transform.right, new Vector3(0, 0, -1));
        }
        if (TheManager.RIGHT)
        {
            isIdle = false;
            moveForward += Vector3.Dot(transform.forward, new Vector3(1, 0, 0));
            moveSide += Vector3.Dot(transform.right, new Vector3(1, 0, 0));
        }
        if (TheManager.LEFT)
        {
            isIdle = false;
            moveForward += Vector3.Dot(transform.forward, new Vector3(-1, 0, 0));
            moveSide += Vector3.Dot(transform.right, new Vector3(-1, 0, 0));
        }

        //Applying outputs after modifications
        TheAnimator.SetBool("IsIdle", isIdle);
        TheAnimator.SetFloat("MoveForward", moveForward);
        TheAnimator.SetFloat("MoveSide", moveSide);
    }
    #endregion

    #region Strafe
    void Strafe()
    {
        if (TheManager.LOCKON)
        {
            if (TheAnimator.GetBool("IsStrafe")) TheAnimator.SetBool("IsStrafe", false);
            else TheAnimator.SetBool("IsStrafe", true);
        }
    }
    #endregion

    #region Dash
    void Dash()
    {
        if (TheManager.DODGE)
        {
            if (TheAnimator.GetBool("IsDash") == false && TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDash") == false) TheAnimator.SetBool("IsDash", true);
        }
    }
    #endregion

    #region Block
    void Block()
    {
        if (TheManager.BLOCK) TheAnimator.SetBool("IsBlock", true);
        else TheAnimator.SetBool("IsBlock", false);
    }
    #endregion

    #region Attack
    void Attack()
    {
        AttackChainReset();
        if (TheManager.ATTACK && TheManager.DODGE == false)
        {
            //Add more if there is more attack chains
            if (TheAnimator.GetInteger("AttackID") == 0 && CanAttack == true &&
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleAttack") == false && 
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack1") == false &&
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2") == false &&
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3") == false)
            {
                TheAnimator.SetInteger("CurrentAttackChain", TheAnimator.GetInteger("CurrentAttackChain") + 1);

                //Reset attack chain once max chain has been reach (Note: change the number if there is more chains)
                if (TheAnimator.GetInteger("CurrentAttackChain") > 4)
                {
                    TheAnimator.SetInteger("CurrentAttackChain", 0);
                    CanAttack = false;
                }

                TheAnimator.SetInteger("AttackID", TheAnimator.GetInteger("CurrentAttackChain"));
            }
            LastTimeAttackedTimer = 0;
        }
    }
    #endregion

    #region AttackChainReset
    void AttackChainReset()
    {
        //Start reseting if the chain is currently transitioning
        if(TheAnimator.GetInteger("CurrentAttackChain") > 0)
        {
            LastTimeAttackedTimer += Time.deltaTime;
            if (LastTimeAttackedTimer >= LastTimeAttackedReset)
            {
                TheAnimator.SetInteger("CurrentAttackChain", 0);
                LastTimeAttackedTimer = 0;
            }
        }

        //Start cooldown
        if(CanAttack == false)
        {
            AttackChainCooldownTimer += Time.deltaTime;
            if(AttackChainCooldownTimer >= AttackChainCooldown)
            {
                CanAttack = true;
                AttackChainCooldownTimer = 0;
            }
        }
    }
    #endregion

    #region Fall
    public void Fall()
    {
        //Raycast to ground to check if player is falling or not
        if (RayCastGroundCheck(transform.position, -transform.up, 100) < 1.5f) TheAnimator.SetBool("Fall", false);
        else TheAnimator.SetBool("Fall", true);

        //Fail safe, if the player cliped through the ground, fix the position
        if (GetComponent<Collider>().bounds.min.y + 0.1f < GroundPosition.y)
        {
            Vector3 previousVelocity = GetComponent<Rigidbody>().velocity;
            Vector3 previousPosition = transform.position;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, GroundPosition.y + Vector3.Distance(transform.position, GetComponent<Collider>().bounds.min), transform.position.z);

            //Raycast to ground once more to check if there is indeed a ground, if not proceed to fall without failsafe
            if(RayCastGroundCheck(transform.position, -transform.up, 100) == int.MaxValue)
            {
                GetComponent<Rigidbody>().velocity = previousVelocity;
                transform.position = previousPosition;
            }
        }
    }
    #endregion

    #region RayCastGroundCheck
    float RayCastGroundCheck(Vector3 Start, Vector3 End, float Length)
    {
        RaycastHit[] hits = Physics.RaycastAll(Start, End, Length);

        //If no collisions then just assume falling
        if (hits.Length == 0) return int.MaxValue;

        float distanceToGround = int.MaxValue;
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Player") || hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hits[i].point);
                if (distanceToGround > distance)
                {
                    distanceToGround = distance;
                    GroundPosition = hits[i].point;
                }
            }
        }
        return distanceToGround;
    }
    #endregion

    #region Throw
    void Throw()
    {
        if(TheManager.THROW && HasSword == true)
        {
            HasSword = false;
        }
    }
    #endregion

    #region Reach
    void Reach()
    {
        if (TheManager.REACH)
        {

        }
    }
    #endregion
}
