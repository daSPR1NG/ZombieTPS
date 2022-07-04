using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class InteractiveObject : InteractiveElement
    {
        public int Cost = 450;
        public TMP_Text ActionTextFeedback;

        ScoreManager _scoreManager;

        public override void StartInteraction(Transform interactionActor, float interactionSpeedMultiplier = 1)
        {
            base.StartInteraction(interactionActor, interactionSpeedMultiplier);

            if ( !_scoreManager || _scoreManager.GlobalScore < Cost ) return;

            _scoreManager.RemoveValueToGlobalScore( Cost );

            WeaponSystem weaponSystem = interactionActor.GetComponent<WeaponSystem>();
            Weapon weapon = weaponSystem.EquippedWeapon;

            weapon.SetCurrentAmmo( weapon.GetMaxMagAmmo() );
            weapon.SetCurrentMaxAmmo( weapon.GetMaxAmmo() );

            Actions.OnGettingMaxAmmo?.Invoke( weapon );

            ExitInteraction( interactionActor );
        }

        public override void ExitInteraction( Transform interactionActor )
        {
            base.ExitInteraction( interactionActor );

            PlayerInteractionHandler playerInteractionHandler = interactionActor.GetComponent<PlayerInteractionHandler>();
            playerInteractionHandler.CancelInteraction();
            playerInteractionHandler.IsInteracting = false;
        }

        public override void DisplayInteractionUI()
        {
            base.DisplayInteractionUI();

            _scoreManager = GameManager.Instance.ActivePlayer.GetComponent<ScoreManager>();

            string actionTextFeedbackContent = ActionTextFeedback.text;
            ActionTextFeedback.SetText(actionTextFeedbackContent + " [Cost:" + Cost.ToString() + "]");
            ActionTextFeedback.ForceMeshUpdate();

            ActionTextFeedback.color = _scoreManager.GlobalScore < Cost ? Color.red : Color.white;

            Debug.Log( "DisplayInteractionUI in InteractiveObject - Ammo" );
        }
    }
}