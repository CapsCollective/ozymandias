using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuildingHelper : MonoBehaviour
{
    public TextMeshProUGUI title, description;
    private void Start()
    {
        BuildingStats buildingStats = transform.parent.GetComponent<BuildingSelect>().buildingPrefab.GetComponent<BuildingStats>();
        title.text = "~ " + buildingStats.name + " ~";
        description.text = buildingStats.description;
    }
}
