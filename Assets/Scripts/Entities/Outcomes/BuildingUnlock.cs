﻿using NaughtyAttributes;
using UnityEngine;
using static Managers.GameManager;

namespace Entities.Outcomes
{
    [CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
    public class BuildingUnlock : Outcome
    {
        public GameObject building;

        [Button]
        public override bool Execute()
        {
            return building && Manager.BuildingCards.Unlock(building);
        }

        public override string Description => "<color=#007000ff>Building Type Unlocked: " + building.name + "!</color>";
    }
}
