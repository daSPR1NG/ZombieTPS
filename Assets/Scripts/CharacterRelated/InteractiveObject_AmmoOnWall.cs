using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class InteractiveObject_AmmoOnWall : InteractiveElement
    {
        public int Cost = 450;

        public string ActionTextFeedbackContent;
        public TMP_Text ActionTextFeedback;

        public AudioClip BuySFX;
        public AudioSource AudioSource;

        public GameObject BuyFX;

        ScoreManager _scoreManager;

        private void OnEnable()
        {
            Actions.OnGlobalScoreValueChanged += UpdateActionFeedbackTextContent;
            BuyFX.SetActive( false );
        }
        private void OnDisable() => Actions.OnGlobalScoreValueChanged -= UpdateActionFeedbackTextContent;

        public override void StartInteraction(Transform interactionActor, float interactionSpeedMultiplier = 1)
        {
            base.StartInteraction(interactionActor, interactionSpeedMultiplier);

            if ( !_scoreManager || _scoreManager.GlobalScore < Cost ) return;

            BuyFX.SetActive( true );

            _scoreManager.RemoveValueToGlobalScore( Cost );

            WeaponSystem weaponSystem = interactionActor.GetComponent<WeaponSystem>();
            Weapon weapon = weaponSystem.EquippedWeapon;

            if ( weaponSystem.IsReloading ) weaponSystem.CancelReloading();

            weapon.SetCurrentAmmo( weapon.GetMaxMagAmmo() );
            weapon.SetCurrentMaxAmmo( weapon.GetMaxAmmo() );

            AudioHelper.PlaySound( AudioSource, BuySFX, AudioSource.volume );

            Actions.OnGettingMaxAmmo?.Invoke( weapon );

            ExitInteraction( interactionActor );
        }

        public override void ExitInteraction( Transform interactionActor )
        {
            PlayerInteractionHandler playerInteractionHandler = interactionActor.GetComponent<PlayerInteractionHandler>();
            playerInteractionHandler.CancelInteraction();
            playerInteractionHandler.IsInteracting = false;

            base.ExitInteraction( interactionActor );
        }

        public override void DisplayInteractionUI()
        {
            base.DisplayInteractionUI();

            UpdateActionFeedbackTextContent(0, false);

            Debug.Log( "DisplayInteractionUI in InteractiveObject - Ammo" );
        }

        private void UpdateActionFeedbackTextContent(int i, bool b)
        {
            _scoreManager = GameManager.Instance.ActivePlayer.GetComponent<ScoreManager>();

            ActionTextFeedback.SetText( string.Empty );
            ActionTextFeedback.SetText( ActionTextFeedbackContent + " [Cost:" + Cost.ToString() + "]" );

            ActionTextFeedback.color = _scoreManager.GlobalScore < Cost ? Color.red : Color.white;
        }
    }
}