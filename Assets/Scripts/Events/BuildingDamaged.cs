using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingDamaged : Outcome, IOutcome
{
    [EventField(typeof(string))]
    public string BuildingName;
}