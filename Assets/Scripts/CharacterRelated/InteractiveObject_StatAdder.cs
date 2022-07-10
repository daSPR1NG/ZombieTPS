using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class InteractiveObject_StatAdder : InteractiveElement
    {
        public int Cost = 450;
        public float CostModifier = 2;
        public StatModifier StatModifier;

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

        public override void StartInteraction( Transform interactionActor, float interactionSpeedMultiplier = 1 )
        {
            base.StartInteraction( interactionActor, interactionSpeedMultiplier );

            if ( !_scoreManager || _scoreManager.GlobalScore < Cost ) return;

            BuyFX.SetActive( true );

            StatsManager statsManager = interactionActor.GetComponent<StatsManager>();

            float currentHealthBeforeApplyingModifier = 0;
            if ( StatModifier.ModifiedStatType == StatAttribute.Health )
                currentHealthBeforeApplyingModifier = statsManager.GetStat( StatAttribute.Health ).GetCurrentValue();

            statsManager.GetStat( StatModifier.ModifiedStatType ).AddModifier( new StatModifier( 
                StatModifier.ModifierType, 
                StatModifier.ModifierValue, 
                this, 
                StatModifier.ModifiedStatType ));

            WeaponSystem weaponSystem = interactionActor.GetComponent<WeaponSystem>();

            switch ( StatModifier.ModifiedStatType )
            {
                case StatAttribute.Health:
                    float health = Mathf.FloorToInt(statsManager.GetStat(StatAttribute.Health).GetCurrentValue());

                    statsManager.GetStat( StatAttribute.Health ).SetCurrentValue( currentHealthBeforeApplyingModifier );

                    Actions.OnPlayerHealthValueChanged?.Invoke(
                        currentHealthBeforeApplyingModifier, 
                        statsManager.GetStat( StatAttribute.Health ).GetMaxValue(), 
                        HealthInteraction.None );

                    Actions.OnPlayerHealthValueAugmented( 
                        currentHealthBeforeApplyingModifier, 
                        statsManager.GetStat( StatAttribute.Health ).GetMaxValue() );

                    //StatModifier.ModifierValue /= ( CostModifier / 2 );
                    break;
                case StatAttribute.AttackDamage:
                    weaponSystem.EquippedWeapon.SetDamage( ( int ) statsManager.GetStat( StatModifier.ModifiedStatType ).GetCurrentValue() );

                    //StatModifier.ModifierValue /= (CostModifier / 2);
                    break;
                case StatAttribute.ReloadSpeed:
                    weaponSystem.EquippedWeapon.SetReloadingSpeed( statsManager.GetStat( StatModifier.ModifiedStatType ).GetCurrentValue() );

                    //StatModifier.ModifierValue /= ( CostModifier / 2 );
                    break;
                case StatAttribute.FireRate:
                    weaponSystem.EquippedWeapon.SetFireRate( statsManager.GetStat( StatModifier.ModifiedStatType ).GetCurrentValue() );

                    //StatModifier.ModifierValue /= ( CostModifier / 2 );
                    break;
                case StatAttribute.MaxAmmoBonus:
                    weaponSystem.EquippedWeapon.SetMaxAmmo( weaponSystem.EquippedWeapon.GetMaxAmmo() +
                        ( int )statsManager.GetStat( StatModifier.ModifiedStatType ).GetCurrentValue() );
                    Actions.OnGettingMaxAmmo?.Invoke( weaponSystem.EquippedWeapon );
                    break;
            }

            _scoreManager.RemoveValueToGlobalScore( Cost );

            int modifiedCost = Cost + ( int )(Cost / CostModifier);
            Cost = modifiedCost;

            AudioHelper.PlaySound( AudioSource, BuySFX, AudioSource.volume );

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

            UpdateActionFeedbackTextContent( 0, false );

            Debug.Log( "DisplayInteractionUI in InteractiveObject - Ammo" );
        }

        private void UpdateActionFeedbackTextContent( int i, bool b )
        {
            _scoreManager = GameManager.Instance.ActivePlayer.GetComponent<ScoreManager>();

            ActionTextFeedback.SetText( string.Empty );
            ActionTextFeedback.SetText( ActionTextFeedbackContent + " [Cost:" + Cost.ToString() + "]" );

            ActionTextFeedback.color = _scoreManager.GlobalScore < Cost ? Color.red : Color.white;
        }
    }
}