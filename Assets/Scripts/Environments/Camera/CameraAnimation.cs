using DG.Tweening;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    [Header("Bidding Phase")]
    public Vector3 biddingPosition = Vector3.zero;          
    public Vector3 biddingRotation; 
    public float biddingDuration = 0;

    [Header("Action Phase")]
    public Vector3 actionPosition = Vector3.zero;          
    public Vector3 actionRotation; 

    public float actionDuration = 0;
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;


    private void Start()
    {
        initialPosition = this.transform.position;
        initialRotation = this.transform.rotation;
    }

    //Bidding Phase 

    public void BiddingCamera()
    {
        AnimateToTarget(biddingPosition,biddingRotation,biddingDuration);
    }

    //Action Phase

    public void ActionCamera()
    {
        AnimateToTarget(actionPosition,actionRotation,actionDuration);
    }

    private void AnimateToTarget(Vector3 targetPosition, Vector3 targetRotationEuler, float duration)
    {
        // Position animation
        transform.DOMove(targetPosition, duration);

        // Rotation animation, convert Euler to Quaternion
        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
        transform.DORotateQuaternion(targetRotation, duration);
    }

    public void ResetCamera(float duration = 2)
    {
        transform.DOMove(initialPosition, duration);
        transform.DORotateQuaternion(initialRotation, duration);
    }

}