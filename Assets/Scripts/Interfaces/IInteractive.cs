using UnityEngine;

namespace Khynan_Coding
{
    public interface IInteractive
    {
        public abstract void StartInteraction(Transform interactionActor, float interactionSpeedMultiplier = 1);
        public abstract void ExitInteraction( Transform interactionActor );
    }
}