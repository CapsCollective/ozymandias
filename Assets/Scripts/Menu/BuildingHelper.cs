using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingHelper : MonoBehaviour
{
    BuildingStats buildingStats;

    public void FillText(GameObject originalObject)
    {
        buildingStats = originalObject.GetComponent<Click>().building.GetComponent<BuildingStats>();
        transform.Find("Title").GetComponent<Text>().text = buildingStats.type.ToString();
        transform.Find("Text").GetComponent<Text>().text = buildingStats.description;
    }
}
