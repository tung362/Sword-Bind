using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    //Inputs
    [HideInInspector]
    public bool UP;
    [HideInInspector]
    public bool DOWN;
    [HideInInspector]
    public bool LEFT;
    [HideInInspector]
    public bool RIGHT;
    [HideInInspector]
    public bool LOCKON;
    [HideInInspector]
    public bool DODGE;
    [HideInInspector]
    public bool BLOCK;
    [HideInInspector]
    public bool ATTACK;
    [HideInInspector]
    public bool WEAPON1;
    [HideInInspector]
    public bool WEAPON2;
    [HideInInspector]
    public bool WEAPON3;
    [HideInInspector]
    public bool SKILLSELECT;
    [HideInInspector]
    public bool PAUSE;
    [HideInInspector]
    public bool RELOAD;
    [HideInInspector]
    public bool DEBUG;
    [HideInInspector]
    public bool THROW;
    [HideInInspector]
    public bool REACH;

    //Components this script will be managing
    private PlayerAnimator TheAnimator;
    private PlayerController TheController;

    void Start()
    {
        TheAnimator = GetComponent<PlayerAnimator>();
        TheController = GetComponent<PlayerController>();
    }

    void Update()
    {
        Inputs();
        TheAnimator.Animate();
        TheController.Control();
    }

    void FixedUpdate()
    {
        TheAnimator.Fall();
    }

    void LateUpdate()
    {

    }

    void Inputs()
    {
        UP = Input.GetButton("Up");
        DOWN = Input.GetButton("Down");
        LEFT = Input.GetButton("Left");
        RIGHT = Input.GetButton("Right");
        LOCKON = Input.GetButtonDown("LockOn");
        DODGE = Input.GetButton("Dodge");
        BLOCK = Input.GetButton("Block");
        ATTACK = Input.GetButton("Attack");
        WEAPON1 = Input.GetButton("Weapon1");
        WEAPON2 = Input.GetButton("Weapon2");
        WEAPON3 = Input.GetButton("Weapon3");
        SKILLSELECT = Input.GetButton("SkillSelect");
        PAUSE = Input.GetButton("Pause");
        RELOAD = Input.GetButton("Reload");
        DEBUG = Input.GetButton("Debug");
        THROW = Input.GetButton("Throw");
        REACH = Input.GetButton("Reach");
    }
}
