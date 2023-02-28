
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Component")]
    [field: SerializeField] public CharacterController Cc { get; private set; }
    [field: SerializeField] public Rigidbody Rb { get; private set; }
    [field: SerializeField] public Joint Joint { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private bool useFakeInput;
    [field: SerializeField] public float MoveSpeed { get; private set; }
    
    [field: Header("Jetpack")]
    [field: SerializeField] public float JetPackAcceleration { get; private set; }
    [field: SerializeField] public float MaxJetPackVelocity {get; private set; }
    
    [Header("Gravity Control")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform testGravityControlMachine;
    [SerializeField] private float gravity;
    [SerializeField] private float gravityInitialVelocity;

    [Header("Interact")] 
    [SerializeField] private GameObject playerPhysics;
    [SerializeField] private float interactRange;
    [SerializeField] private float pushForce;
    [field: SerializeField] public Transform InteractPoint { get; private set; }
    

    // Player Components
    private IPlayerMove[] playerMoves;
    private IPlayerGravityController[] playerGravityControllers;
    public IPlayerInput PlayerInput { get; private set; }
    public IPlayerMove PlayerMovement { get; private set; }
    public IPlayerGravityController PlayerGravityController { get; private set; }
    public PlayerInteractController PlayerInteractController { get; private set; }
    
    private bool underGravityInfluence;
    // private bool needMoveInertia;
    // private bool needJetInertia;


    // For debug use
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(InteractPoint.position, interactRange);
    }
    

    private void Awake()
    {
        //Joint = null;
        
        playerMoves = new IPlayerMove[] { new PlayerCCMovement(this), new PlayerRbMovement(this) };
        playerGravityControllers = new IPlayerGravityController[] { new PlayerCCGravityController(this), new PlayerRbGravityController(this) };
        
        PlayerInput = useFakeInput ? new PlayerTestKeyboardInput() : new PlayerInput();
        PlayerMovement = playerMoves[0];
        PlayerGravityController = playerGravityControllers[0];
        PlayerInteractController = new PlayerInteractController(this);
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
        
        underGravityInfluence = 
            PlayerInput.JetDirection == 0 && Vector3.Distance(transform.position, testGravityControlMachine.position) <= 8;
        
        PlayerGravityController.AddGravity(underGravityInfluence, groundCheckPoint.position, gravity, gravityInitialVelocity);
        
        PlayerMovement.CalcInertia(!underGravityInfluence);
    }


    public void DetectInteract()
    {
        PlayerInteractController.UpdateInteract(InteractPoint.position, interactRange);
        
        if(PlayerInput.TapInteract) PlayerInteractController.Interact(this, InteractType.Tap);
        if(PlayerInput.HoldInteract) PlayerInteractController.Interact(this, InteractType.Hold);
        if(PlayerInput.ReleaseInteract) PlayerInteractController.Interact(this, InteractType.Release);
    }

    
    public void SwitchToRigidbodyMove(bool rigidbodyEnable)
    {
        Cc.enabled = !rigidbodyEnable;
        playerPhysics.SetActive(rigidbodyEnable);
        
        PlayerMovement = rigidbodyEnable ? playerMoves[1] : playerMoves[0];
        PlayerGravityController = rigidbodyEnable ? playerGravityControllers[1] : playerGravityControllers[0];
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.collider.attachedRigidbody;
        if (rb && !rb.isKinematic)
        {
            rb.AddForce(hit.moveDirection * pushForce, ForceMode.Impulse);
        }
    }


    // void AddInertia()
    // {
    //     if (useIndividualInput)
    //     {
    //         // Check if horizontal movement need add inertia.
    //         if (Vector2.SqrMagnitude(PlayerInput.Movement) != 0) if(!needMoveInertia) needMoveInertia = true;
    //         if (Vector2.SqrMagnitude(PlayerInput.Movement) == 0 && needMoveInertia)
    //         {
    //             if(!underGravityInfluence) PlayerGravityController.AddInertia(PlayerCcMovement.LastMoveVelocity);
    //             needMoveInertia = false;
    //         }
    //     
    //         // Check if vertical movement need add inertia.
    //         if (PlayerInput.JetDirection != 0) if(!needJetInertia) needJetInertia = true;
    //         if (PlayerInput.JetDirection == 0 && needJetInertia)
    //         {
    //             if(!underGravityInfluence) PlayerGravityController.AddInertia(Vector3.up * PlayerCcMovement.LastJetVelocity);
    //             needJetInertia = false;
    //         }
    //     }
    // }
}