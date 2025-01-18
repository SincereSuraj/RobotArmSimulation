using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public GameObject CarryObject;
    public RobotArmController robotController;

    public float spawnRange = 2.7f;
    public float SpawnY = 0.8f;
    private void Start()
    {
        robotController.MoveObject();
    }
    public void RestartSimulation() //Called from UI
    {
        StartCoroutine(nameof(GetObject));
    }
    private IEnumerator GetObject()
    {
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<CarryObjectInfo>().ResetControlToPhysics();
        yield return null;
        CarryObject.GetComponent<CarryObjectInfo>().RandomizeItemData();
        yield return null;

        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(1f, spawnRange);
        Vector3 spawnPos = distance * new Vector3(direction.x, 0, direction.y) + SpawnY * Vector3.up;
        Collider[] coll = Physics.OverlapSphere(spawnPos, 1, LayerMask.GetMask("PlacementArea"));
        if (coll.Length > 0)
        {
            spawnPos = new Vector3(-spawnPos.x, spawnPos.y, -spawnPos.z);
        }
        CarryObject.transform.position = spawnPos;
        yield return new WaitForSeconds(0.5f);
        robotController.MoveObject();
    }
    public void CancelSimulation()  //Called from UI
    {
        robotController.StopMovement();
    }
}
