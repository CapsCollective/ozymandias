using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [ReorderableList] public List<GameObject> AllBuildings = new List<GameObject>();

    public static BuildingManager instance;

    public static BuildingManager BuildManager
    {
        get
        {
            if (!instance)
                instance = FindObjectsOfType<BuildingManager>()[0];
            return instance;
        }
    }

}
