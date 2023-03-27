
using Script.Other;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IGravityAffectable
{
    [field: Header("Component")]
    [field: SerializeField] public CharacterController Cc { get; private set; } // Will remove this after rigidbody only test finish.
    [field: SerializeField] public Rigidbody Rb { get; private set; }
    [field: SerializeField] public Joint Joint { get; private set; }
    [field: SerializeField] public Animator playerAnimator { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private GameObject playerPhysics;
    [field: SerializeField] public float MoveSpeed { get; private set; }
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
    
     
    public void SetActive(bool active) => IsActive = active;
     

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


    public void Move(Vector3 movement)
    {
        PlayerMovement.Move(movement, MoveSpeed, JetPackAcceleration, MaxJetPackVelocity);
        PlayerMovement.CalcInertia(!UnderGravity);
        
        playerAnimator.SetBool("Move", Vector3.Scale(movement, new Vector3(1, 0, 1)) != Vector3.zero);
        playerAnimator.SetFloat("MoveSpeed", PlayerInput.Run && IsGround ? 4 : 2);
    }


    public void DetectInteract()
    {
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
}