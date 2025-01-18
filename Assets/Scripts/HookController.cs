using System.Collections;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public Transform clamp1, clamp2;
    public Transform grabber;
    private readonly float clampOpenAngle = 30, clampCloseAngle = 7f;
    public IEnumerator OpenHookCoroutine()
    {
        float currentAngle = clamp2.localEulerAngles.x;
        while (currentAngle < clampOpenAngle)
        {
            currentAngle += 50 * Time.deltaTime;
            clamp1.transform.localRotation = Quaternion.Euler(-currentAngle, 0, 0);
            clamp2.transform.localRotation = Quaternion.Euler(currentAngle, 0, 0);
            yield return null;
        }
    }
    public IEnumerator CloseHookCoroutine()
    {
        float currentAngle = clamp2.localEulerAngles.x;
        while (currentAngle > clampCloseAngle)
        {
            currentAngle -= 50 * Time.deltaTime;
            clamp1.transform.localRotation = Quaternion.Euler(-currentAngle, 0, 0);
            clamp2.transform.localRotation = Quaternion.Euler(currentAngle, 0, 0);
            yield return null;
        }
    }
}
