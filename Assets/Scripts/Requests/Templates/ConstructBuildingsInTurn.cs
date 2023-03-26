using Managers;
using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class ConstructBuildingsInTurn : Request
    {
        public override string Description => $"Build {Required} Buildings in a Single Turn";

        protected override int RequiredScaled => 2 + Tokens * 2;

        public override void Start()
        {
            Structures.Structures.OnBuild += OnBuild;
            State.OnNewTurn += OnNewTurn;

        }
        
        public override void Complete()
        {
            Structures.Structures.OnBuild -= OnBuild;
            State.OnNewTurn -= OnNewTurn;
        }

        private void OnNewTurn()
        {
            Completed = 0;
        }

        private void OnBuild(Structure structure)
        {
            Completed++;
        }
    }
}
