using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPlacementData", menuName = "SO/ItemPlacementData", order = 0)]
public class ItemPlaceSO : ScriptableObject
{
    public List<ItemPlaceData> data;

    public Vector3 GetPlacementData(ItemType type)
    {
        return data.First((x) => x.type.Equals(type)).placementPosition;
    }
    public Color GetItemColor(ItemType type)
    {
        return data.First((x) => x.type.Equals(type)).itemColor;
    }
}
[Serializable]
public class ItemPlaceData
{
    public ItemType type;
    public Vector3 placementPosition;
    public Color itemColor;
}
public enum ItemType
{
    RedBottle,
    YellowBottle,
    BlueBottle
}