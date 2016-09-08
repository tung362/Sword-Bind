using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Control")]
    public float Speed = 7;
    public float StrafeSpeed = 4;

    [Header("Animation Settings")]
    public float DashDistance = 6;
    public float DashSpeed = 80;
    public float StrafeAttackDistance = 2;
    public float NormalAttackDistance = 2.5f;
    public float RunAttackDistance = 2.5f;
    public float AttackSpeed = 18;

    //Attack Assist Settings
    public float AttackAssistMaxRadius = 3;

    //Strafe
    public float StrafeMaxLockOnRadius = 10;
    private bool FindLockOnTarget = true;
    private GameObject LockedOnTarget;

    //Teleports
    [HideInInspector]
    private bool DashRunOnce = true;
    private bool AttackRunOnce = true;
    private Vector3 DashOrigin;
    private Vector3 AttackOrigin;

    [Header("Character's Camera")]
    public GameObject Camera;

    private GlobalVars Global;
    private Rigidbody TheRigidbody;
    private PlayerManager TheManager;
    private Animator TheAnimator;

    void Start()
    {
        Global = FindObjectOfType<GlobalVars>();
        TheRigidbody = GetComponent<Rigidbody>();
        TheManager = GetComponent<PlayerManager>();
        TheAnimator = GetComponentInChildren<Animator>();
    }

    //Use this like a table of contents
    public void Control()
    {
        AttackAssist();
        Idle();
        Walk();
        Strafe();
        Block();
        Dash();
        Attack();
    }

    #region AttackAssist
    void AttackAssist()
    {
        //Assist in aiming at target if attack assist is enabled in the game options
        if (Global.UseAttackAssist == true)
        {
            //Finding Closest Target
            GameObject AttackAssistTarget = null;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies[0] == null) { }
            else
            {
                float closestDistance = Vector3.Distance(transform.position, enemies[0].transform.position);
                AttackAssistTarget = enemies[0];
                for (int i = 1; i < enemies.Length; ++i)
                {
                    float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                    if (closestDistance > distance)
                    {
                        closestDistance = distance;
                        AttackAssistTarget = enemies[i];
                    }
                }
            }

            //Block override
            if (TheAnimator.GetNextAnimatorStateInfo(0).IsName("PlayerBlockIdle"))
            {
                if (Global.UseAttackAssist == true)
                {
                    //Rotation
                    float targetDistanceToPlayer = Vector3.Distance(AttackAssistTarget.transform.position, transform.position);
                    if (AttackAssistTarget != null && targetDistanceToPlayer <= AttackAssistMaxRadius) transform.LookAt(new Vector3(AttackAssistTarget.transform.position.x, transform.position.y, AttackAssistTarget.transform.position.z));
                }
            }

            //Attack override
            if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleAttack") ||
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack1") ||
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2") ||
                TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3"))
            {
                if (Global.UseAttackAssist == true)
                {
                    //Rotation
                    float targetDistanceToPlayer = Vector3.Distance(AttackAssistTarget.transform.position, transform.position);
                    if (AttackAssistTarget != null && targetDistanceToPlayer <= AttackAssistMaxRadius) transform.LookAt(new Vector3(AttackAssistTarget.transform.position.x, transform.position.y, AttackAssistTarget.transform.position.z));
                }
            }
        }
    }
    #endregion

    #region Idle
    void Idle()
    {
        if (TheAnimator.GetBool("IsIdle"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            TheRigidbody.velocity = new Vector3(0, TheRigidbody.velocity.y, 0);
        }
    }
    #endregion

    #region Walk
    void Walk()
    {
        Vector3 rotationMovement = Vector3.zero;
        Vector3 movement = Vector3.zero;

        if(TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRun") || TheAnimator.GetNextAnimatorStateInfo(0).IsName("PlayerRun"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            //Movement
            if (TheManager.UP)
            {
                rotationMovement.y = 1;
                movement = transform.forward;
            }
            if (TheManager.DOWN)
            {
                rotationMovement.y = -1;
                movement = transform.forward;
            }
            if (TheManager.RIGHT)
            {
                rotationMovement.x = 1;
                movement = transform.forward;
            }
            if (TheManager.LEFT)
            {
                rotationMovement.x = -1;
                movement = transform.forward;
            }

            //Rotation
            if (rotationMovement != Vector3.zero)
            {
                float rotationAngle = Vector3.Angle(new Vector3(0, 1, 0), rotationMovement);
                Vector3 cross = Vector3.Cross(new Vector3(0, 1, 0), rotationMovement);
                if (-cross.z < 0) rotationAngle = -rotationAngle;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.transform.rotation.eulerAngles.y + rotationAngle, transform.rotation.eulerAngles.z);
            }

            //Applying Movement
            TheRigidbody.velocity = new Vector3(movement.x * Speed, TheRigidbody.velocity.y, movement.z * Speed);
        }
    }
    #endregion

    #region Strafe
    void Strafe()
    {
        Vector3 movement = Vector3.zero;

        //Finding closest target
        if (TheAnimator.GetBool("IsStrafe") && TheAnimator.GetBool("ReachSword") == false)
        {
            if (FindLockOnTarget == true)
            {
                LockedOnTarget = null;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies[0] == null) { }
                else
                {
                    float closestDistance = Vector3.Distance(transform.position, enemies[0].transform.position);
                    LockedOnTarget = enemies[0];
                    for (int i = 1; i < enemies.Length; ++i)
                    {
                        float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                        if (closestDistance > distance)
                        {
                            closestDistance = distance;
                            LockedOnTarget = enemies[i];
                        }
                    }
                }
                FindLockOnTarget = false;
            }

            //Rotation
            float targetDistanceToPlayer = Vector3.Distance(LockedOnTarget.transform.position, transform.position);
            if (targetDistanceToPlayer <= StrafeMaxLockOnRadius && LockedOnTarget != null)
            {
                if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDash") == false) transform.LookAt(new Vector3(LockedOnTarget.transform.position.x, transform.position.y, LockedOnTarget.transform.position.z));
            }
            else TheAnimator.SetBool("IsStrafe", false);
        }
        else FindLockOnTarget = true;

        if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("Strafe") || TheAnimator.GetNextAnimatorStateInfo(0).IsName("Strafe"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            //Movement
            if (TheManager.UP) movement = Camera.transform.forward;
            if (TheManager.DOWN) movement = -Camera.transform.forward;
            if (TheManager.RIGHT) movement = Camera.transform.right;
            if (TheManager.LEFT) movement = -Camera.transform.right;


            //Applying Movement
            TheRigidbody.velocity = new Vector3(movement.x * StrafeSpeed, TheRigidbody.velocity.y, movement.z * StrafeSpeed);
        }
    }
    #endregion

    #region Dash
    void Dash()
    {
        if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDash"))
        {
            if (DashRunOnce == true)
            {
                DashOrigin = transform.position;
                DashRunOnce = false;
            }

            float distance = Vector3.Distance(DashOrigin, transform.position);
            gameObject.layer = LayerMask.NameToLayer("DodgeGhost"); //Ghost

            //Apply Movement
            TheRigidbody.velocity = new Vector3(transform.forward.x, 0, transform.forward.z) * DashSpeed;

            //how far the player is from origin vs modified limit distance
            if (distance >= DashDistance) TheRigidbody.velocity = Vector3.zero;
        }
        else DashRunOnce = true;
    }
    #endregion

    #region Block
    void Block()
    {
    }
    #endregion

    #region Attack
    void Attack()
    {
        if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleAttack") ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack1") ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2") ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3"))
        {
            if (AttackRunOnce == true)
            {
                AttackOrigin = transform.position;
                AttackRunOnce = false;
            }

            gameObject.layer = LayerMask.NameToLayer("Player");
            float distanceFromOrigin = Vector3.Distance(AttackOrigin, transform.position);

            //Apply Movement
            TheRigidbody.velocity = new Vector3(transform.forward.x, 0, transform.forward.z) * AttackSpeed;

            //Strafe
            if (TheAnimator.GetBool("IsStrafe") == false)
            {
                //Normal
                if (TheAnimator.GetBool("IsIdle") == true)
                {
                    if (distanceFromOrigin >= NormalAttackDistance) TheRigidbody.velocity = Vector3.zero;
                }
                //Run
                else
                {
                    if (distanceFromOrigin >= RunAttackDistance) TheRigidbody.velocity = Vector3.zero;
                }
            }
            else
            {
                if (distanceFromOrigin >= StrafeAttackDistance) TheRigidbody.velocity = Vector3.zero;
            }
        }
        else AttackRunOnce = true;
    }
#endregion
}
