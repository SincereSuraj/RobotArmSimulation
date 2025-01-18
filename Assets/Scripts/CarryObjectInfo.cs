using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryObjectInfo : MonoBehaviour
{
    public ItemPlaceSO itemData;
    public ItemType type;
    private Rigidbody rb;
    private Material carryObjectMaterial;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        carryObjectMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        type = (ItemType)Random.Range(0, 3);
        carryObjectMaterial.color = itemData.GetItemColor(type);
    }
    public void GiveControlToParent()
    {
        rb.useGravity = false;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = false;
    }
    public void ResetControlToPhysics()
    {
        GetComponent<Collider>().enabled = true;
        rb.useGravity = true;
    }
    public void RandomizeItemData()
    {
        type = (ItemType)Random.Range(0, 3);
        carryObjectMaterial.color = itemData.GetItemColor(type);
    }
}
