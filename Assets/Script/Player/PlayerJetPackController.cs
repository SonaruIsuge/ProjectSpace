using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerJetPackController : MonoBehaviour
{
    [SerializeField] private float jetPackAcceleration;
    [SerializeField] private float maxJetPackVelocity;

    public float CurrentJetPackVelocity { get; private set; }
    
    private CharacterController characterController;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }


    public void Jet(float direction)
    {
        if (direction == 0)
        {
            CurrentJetPackVelocity = 0;
            return;
        }
        CurrentJetPackVelocity =
            Mathf.Min(Mathf.Abs(CurrentJetPackVelocity) + jetPackAcceleration * Time.deltaTime, maxJetPackVelocity) * direction;
        characterController.Move(Vector3.up * (CurrentJetPackVelocity * Time.deltaTime));
    }


    // public void Reset()
    // {
    //     CurrentJetPackVelocity = 0;
    // }
}
