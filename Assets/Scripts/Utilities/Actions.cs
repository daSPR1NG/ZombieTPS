using System;
using UnityEngine;

namespace Khynan_Coding
{
    public static class Actions
    {
        public static Action OnGameStateChanged;
        public static Action OnGamePaused;

        public static Action<Weapon> OnInitializingWeapon;
        public static Action<Weapon, int> OnEquippingWeapon;

        public static Action<Weapon> OnShooting;

        public static Action OnReloadStarted;
        public static Action<Weapon> OnReloadStartedSetWeaponData;
        public static Action<Weapon> OnReloadEndedSetWeaponData;
        public static Action OnReloadEnded;

        public static Action<CharacterType> OnAimingInteractable;

        public static Action<int> OnAddingScore;
        public static Action<int> OnRemovingScore;

        public static Action<ScoreData> OnScoreNotificationNeed;

        public static Action OnHittingValidTarget;
        public static Action OnEnemyDeath;

        public static Action<float> OnKillChainStarted;
        public static Action<int, float> OnKillCountValueChanged;
        public static Action OnKillCountStepReached;
        public static Action<float> OnScoreMultiplierValueChanged;

        public static Action<int, bool> OnGlobalScoreValueChanged;

        public static Action<float, float> OnPlayerHealthValueInitialized;
        public static Action<float, float, HealthInteraction> OnPlayerHealthValueChanged;

        public static Action<StatsManager> OnEnemyHealthValueChanged;

        public static Action OnWaveBegun;
        public static Action OnWaveEnded;
        public static Action<int> OnWaveCountValueChanged;

        public static Action<Transform, InteractionData> OnPlayerInteractionPossible;
        public static Action OnPlayerInteractionImpossible;
    }
}