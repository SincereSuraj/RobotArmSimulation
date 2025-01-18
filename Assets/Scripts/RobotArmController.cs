using System.Collections;
using System.Linq;
using UnityEngine;

public class RobotArmController : MonoBehaviour
{
    public Transform centerTransform;
    public Transform hookTransform;
    public HookController hookController;
    public ItemPlaceSO itemPlacementData;
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    public void StopMovement()
    {
        StopAllCoroutines();
    }
    Coroutine activeCoroutine = null;
    public void MoveObject()
    {
        CarryObjectInfo carryObject = FindAnyObjectByType<CarryObjectInfo>();

        if (carryObject == null)
        {
            Debug.Log("CarryObject missing");
            return;
        }
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ArmMovementSequence(carryObject));
    }
    private IEnumerator ArmMovementSequence(CarryObjectInfo carryItem)
    {
        //move arm to top of object
        Vector3 beforeGrabbingPosition = carryItem.transform.position + 0.2f * Vector3.up;
        yield return StartCoroutine(MoveArmCoroutine(beforeGrabbingPosition));

        //open clamp then move arm to the holding position
        yield return StartCoroutine(hookController.OpenHookCoroutine());
        yield return StartCoroutine(MoveArmCoroutine(carryItem.transform.position));

        //close Clamp to look like is caught, parent pbject to move
        yield return StartCoroutine(hookController.CloseHookCoroutine());
        carryItem.transform.SetParent(hookController.grabber);
        yield return null;
        carryItem.GiveControlToParent();

        //move arm to top of place object position
        Vector3 targetPos = itemPlacementData.GetPlacementData(carryItem.type);
        yield return StartCoroutine(MoveArmCoroutine(targetPos + new Vector3(0f, 0.2f, 0f)));
        yield return StartCoroutine(MoveArmCoroutine(targetPos));

        //release carry item by unparenting.
        carryItem.transform.SetParent(null);
        carryItem.ResetControlToPhysics();

        //Revert robot arm back
        yield return StartCoroutine(MoveArmCoroutine(targetPos + new Vector3(0f, 0.5f, 0f)));
        activeCoroutine = null;
    }



    public float constantMoveSpeed = 1.0f;

    private IEnumerator MoveArmCoroutine(Vector3 targetPos)
    {
        // Calculate initial spherical coordinates relative to the center
        Vector3 initialOffset = hookTransform.position - centerTransform.position;
        float initialRadius = initialOffset.magnitude;
        float initialPolarAngle = Mathf.Acos(initialOffset.y / initialRadius);
        float initialAzimuthalAngle = Mathf.Atan2(initialOffset.z, initialOffset.x);

        // Calculate target spherical coordinates relative to the center
        Vector3 targetOffset = targetPos - centerTransform.position;
        float targetRadius = targetOffset.magnitude;
        float targetPolarAngle = Mathf.Acos(targetOffset.y / targetRadius);
        float targetAzimuthalAngle = Mathf.Atan2(targetOffset.z, targetOffset.x);

        float totalDistance = Vector3.Distance(hookTransform.position, targetPos);
        float distanceCovered = 0;

        while (distanceCovered < totalDistance)
        {
            float t = distanceCovered / totalDistance;
            float easedT = Mathf.SmoothStep(0, 1, t);

            // Interpolate radius and angles for spherical lerp
            float currentRadius = Mathf.Lerp(initialRadius, targetRadius, easedT);
            float currentPolarAngle = Mathf.Lerp(initialPolarAngle, targetPolarAngle, easedT);

            // Calculate shortest path for azimuthal angle without using LerpAngle
            float angleDifference = Mathf.DeltaAngle(initialAzimuthalAngle * Mathf.Rad2Deg, targetAzimuthalAngle * Mathf.Rad2Deg);
            float adjustedAzimuthalAngle = initialAzimuthalAngle + (angleDifference * easedT * Mathf.Deg2Rad);

            // Calculate the position on the spherical path
            float x = currentRadius * Mathf.Sin(currentPolarAngle) * Mathf.Cos(adjustedAzimuthalAngle);
            float y = currentRadius * Mathf.Cos(currentPolarAngle);
            float z = currentRadius * Mathf.Sin(currentPolarAngle) * Mathf.Sin(adjustedAzimuthalAngle);

            Vector3 targetPosition = centerTransform.position + new Vector3(x, y, z);

            // Update position
            hookTransform.position = targetPosition;

            // Move forward by constant speed based on distance
            distanceCovered += constantMoveSpeed * Time.deltaTime;

            yield return null;
        }

        // Final adjustment to ensure exact target alignment
        hookTransform.position = targetPos;
    }

}
