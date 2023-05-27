using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class TerrainRemoved : Outcome
    {
        protected override bool Execute()
        {
            Newspaper.OnNextClosed += RemoveTerrain;
            return true;
        }

        private void RemoveTerrain()
        {
            Manager.Structures.RemoveRandomNearbyTerrain();
        }

        protected override string Description => (
            customDescription != "" ? customDescription : "Nearby terrain has been cleared." 
        ).StatusColor(1);
    }
}
