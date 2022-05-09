using UnityEngine;

namespace Khynan_Coding
{
    public class NPCController : InteractiveElement
    {
        [Header("NPC DEPENDENCIES")]
        [SerializeField] private GameObject uiWindow;

        public override void StartInteraction(Transform interactionActor, float interactionSpeedMultiplier = 1)
        {
            base.StartInteraction(interactionActor, interactionSpeedMultiplier);

            //Helper.DisplayUIWindow(uiWindow);
        }

        public override void ExitInteraction()
        {
            base.ExitInteraction();

            //Helper.HideUIWindow(uiWindow);
        }
    }
}