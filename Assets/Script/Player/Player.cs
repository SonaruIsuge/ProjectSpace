
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IGravityAffectable
{
    [field: Header("Component")]
    [field: SerializeField] public CharacterController Cc { get; private set; }
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
    public bool IsGround => Physics.Raycast(GroundCheckPoint.position, Vector3.down, .01f);

    [field: Header("Interact")]
    [field: SerializeField] public Transform ClawTransform { get; private set; }
    [field: SerializeField] public float InteractRange { get; private set; }
    [SerializeField] private float pushForce;
    [field: SerializeField] public Transform InteractPoint { get; private set; }
    [field: SerializeField] public float ClawSpeed { get; private set; }
    

    // Player Components
    private IPlayerMove[] playerMoves;
    private IPlayerGravityController[] playerGravityControllers;
    public IPlayerInput PlayerInput { get; private set; }
    public IPlayerMove PlayerMovement { get; set; }
    public IPlayerGravityController PlayerGravityController { get; set; }
    public IPlayerInteract PlayerInteractController { get; set; }
    
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
        
        playerMoves = new IPlayerMove[] { new PlayerCCMovement(this), new PlayerRbMovement(this) };
        playerGravityControllers = new IPlayerGravityController[] { new PlayerCCGravityController(this), new PlayerRbGravityController(this) };
        
        PlayerInput = new PlayerInput(this);
        PlayerMovement = playerMoves[0];
        PlayerGravityController = playerGravityControllers[0];
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
        if(Cc) Cc.enabled = !rigidbodyEnable;
        if(playerPhysics) playerPhysics.SetActive(rigidbodyEnable);

        if (GetComponent<RbOnly>() != null)
        {  
           if(playerPhysics) playerPhysics.SetActive(true);
           ToggleSpawnJoint(rigidbodyEnable);
           PlayerGravityController.AddGravity(false, GroundCheckPoint.position, 0, gravityInitialVelocity);
           Debug.Log("Rb Only");
           return;
        }
        
        PlayerMovement = rigidbodyEnable ? playerMoves[1] : playerMoves[0];
        PlayerGravityController = rigidbodyEnable ? playerGravityControllers[1] : playerGravityControllers[0];
        PlayerGravityController.AddGravity(false, GroundCheckPoint.position, 0, gravityInitialVelocity);
    }


    public void ToggleSpawnJoint(bool enable)
    {
        if (enable) Joint = playerPhysics.AddComponent<FixedJoint>();
        else Destroy(Joint);
        Rb.angularVelocity = Vector3.zero;
    }

    
    public void ApplyGravity(float gravitySize)
    {
        var useGravity = UnderGravity && !IgnoreGravity;
        PlayerGravityController.AddGravity(useGravity, GroundCheckPoint.position, gravitySize, gravityInitialVelocity);
    }
    
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.collider.attachedRigidbody;
        if(!rb || !rb.TryGetComponent<Item>(out var item)) return;
        
        if(item.ItemData.Size != ItemSize.Large && item.ItemData.Size != ItemSize.ExtraLarge) 
            rb.AddForce(hit.moveDirection * pushForce, ForceMode.Impulse);
    }
}