
using System;
using Script.Other;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IGravityAffectable
{
    public int PlayerIndex { get; private set; }
    [field: Header("Component")]
    [field: SerializeField] public CharacterController Cc { get; private set; } // Will remove this after rigidbody only test finish.
    [field: SerializeField] public Rigidbody Rb { get; private set; }
    [field: SerializeField] public Collider Col { get; private set; }
    [field: SerializeField] public Joint Joint { get; private set; }
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private GameObject playerPhysics;
    [field: SerializeField] public float MoveSpeedInGround { get; private set; }
    [field: SerializeField] public float MoveSpeedInSpace { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
    
    [field: Header("Jetpack")]
    [field: SerializeField] public float JetPackAcceleration { get; private set; }
    [field: SerializeField] public float MaxJetPackVelocity {get; private set; }

    [field: Header("Gravity Control")]
    [field: SerializeField] public Transform GroundCheckPoint { get; private set; }
    [SerializeField] private float gravityInitialVelocity;
    public bool IsGround => Physics.Raycast(GroundCheckPoint.position, Vector3.down, .05f, LayerMask.GetMask("Ground"));

    [field: Header("Interact")]
    [field: SerializeField] public Transform ClawTransform { get; private set; }
    [field: SerializeField] public Transform ClawBaseTransform { get; private set; }
    [field: SerializeField] public Transform ClawHeadTransform { get; private set; }
    [field: SerializeField] public float InteractRange { get; private set; }
    [field: SerializeField] public Transform InteractPoint { get; private set; }
    [field: SerializeField] public float ClawSpeed { get; private set; }
    [field: SerializeField] public Transform HeadPoint { get; private set; }
    
    [field: Header("VFX")]
    [field: SerializeField] public GameObject JetVFX {get; private set; }
    [field: SerializeField] public GameObject MoveVFX { get; private set; }
    

    // Player Components
    public IPlayerInput PlayerInput { get; private set; }
    public IPlayerMove PlayerMovement { get; private set; }
    public IPlayerGravityController PlayerGravityController { get; private set; }
    public IPlayerInteract PlayerInteractController { get; private set; }
    
    public bool UnderGravity { get; set; }
    public bool IgnoreGravity => PlayerInput.JetDirection != 0;
    public bool IsActive { get; private set; }

    private bool enableMove;
    private bool enableJet;
    private bool enableInteract;
    
     
    public void SetActive(bool active) => IsActive = active;

    public void EnableMove(bool enable) => enableMove = enable;
    public void EnableJet(bool enable) => enableJet = enable;
    public void EnableInteract(bool enable) => enableInteract = enable;
     

    // For debug use
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(InteractPoint.position, InteractRange);
    }
    

    private void Awake()
    {
        //Joint = null;
        IsActive = false;
        enableMove = true;
        enableJet = true;
        enableInteract = true;

        PlayerInput = new PlayerInput(this);
        PlayerMovement = new RbOnlyMove(this);
        PlayerGravityController = new RbOnlyGravity(this);
        PlayerInteractController = new PlayerGrabInteractController(this);
    }


    private void OnEnable()
    {
        PlayerInput.RegisterInput();
        PlayerInput.Enable(true);
    }


    private void OnDisable()
    {
        PlayerInput.UnregisterInput();
    }


    public void SetPlayerIndex(int i)
    {
        PlayerIndex = i;
    }


    public void Move(Vector3 movement)
    {
        if (!enableMove) movement = Vector3.zero;
        else if (!enableJet) movement = Vector3.Scale(movement, new Vector3(1, 0, 1));
        
        var speed = IsGround ? MoveSpeedInGround : MoveSpeedInSpace;
        PlayerMovement.Move(movement, speed, JetPackAcceleration, MaxJetPackVelocity);
        PlayerMovement.CalcInertia(!UnderGravity);
        
        PlayerAnimator.SetBool("IsGround", IsGround);
        PlayerAnimator.SetBool("Move", Vector3.Scale(movement, new Vector3(1, 0, 1)) != Vector3.zero);
        PlayerAnimator.SetFloat("MoveSpeed", PlayerInput.Run && IsGround ? 4 : 2);
    }


    public void DetectInteract()
    {
        if(!enableInteract) return;
        
        PlayerInteractController.UpdateInteract();
        
        if(PlayerInput.TapInteract) PlayerInteractController.Interact(this, InteractType.Tap);
        if(PlayerInput.HoldInteract) PlayerInteractController.Interact(this, InteractType.Hold);
        if(PlayerInput.ReleaseInteract) PlayerInteractController.Interact(this, InteractType.Release);
    }

    
    public void SwitchToRigidbodyMove(bool rigidbodyEnable)
    {
        if (rigidbodyEnable) Joint = playerPhysics.AddComponent<FixedJoint>();
        else Destroy(Joint);
        Rb.angularVelocity = Vector3.zero;
        
        PlayerGravityController.AddGravity(false, GroundCheckPoint.position, 0, gravityInitialVelocity);
    }


    public void ApplyGravity(float gravitySize)
    {
        var useGravity = UnderGravity && !IgnoreGravity;
        PlayerGravityController.AddGravity(useGravity, GroundCheckPoint.position, gravitySize, gravityInitialVelocity);
    }


    public bool spawnCollisionVFX;
    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Player")) return;

        var player = collision.gameObject.GetComponent<Player>();
        if (player.spawnCollisionVFX)
        {
            spawnCollisionVFX = false;
            return;
        }
        
        FXController.Instance.InitVFX(VFXType.PlayerCollision, collision.GetContact(0).point);
        spawnCollisionVFX = true;
    }
}