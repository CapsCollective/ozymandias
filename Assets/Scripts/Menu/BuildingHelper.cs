using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuildingHelper : MonoBehaviour
{
    BuildingStats buildingStats;

    public void FillText(GameObject originalObject)
    {
        buildingStats = originalObject.GetComponent<BuildingSelect>().buildingPrefab.GetComponent<BuildingStats>();
        transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "~ "+buildingStats.name+" ~";
        transform.Find("Text").GetComponent<TextMeshProUGUI>().text = buildingStats.description;
        
    }
}
