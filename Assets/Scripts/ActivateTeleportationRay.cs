using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject leftTeleportationRay;
    public GameObject rightTeleportationRay;

    public InputActionProperty leftRayActive;
    public InputActionProperty rightRayActive;

    public InputActionProperty leftCancel;
    public InputActionProperty rightCancel;

    public XRRayInteractor leftRay;
    public XRRayInteractor rightRay;

    void Update()
    {
        bool isLeftRayHovering = leftRay.TryGetHitInfo(out Vector3 leftPos, out Vector3 leftNormal, out int leftNumber, out bool leftValid);
        bool isRightRayHovering = rightRay.TryGetHitInfo(out Vector3 rightPos, out Vector3 rightNormal, out int rightNumber, out bool rightValid);



        leftTeleportationRay.SetActive(!isLeftRayHovering && 
            leftCancel.action.ReadValue<float>() == 0 && 
            leftRayActive.action.ReadValue<float>() > 0.1f);
        rightTeleportationRay.SetActive(!isRightRayHovering && 
            rightCancel.action.ReadValue<float>() == 0 && 
            rightRayActive.action.ReadValue<float>() > 0.1f);
    }
}
