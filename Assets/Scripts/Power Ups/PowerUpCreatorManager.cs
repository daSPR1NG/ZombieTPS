using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class PowerUpCreatorManager : MonoBehaviour
    {
        public int CurrentStreak = 0;
        public int StreakLimit = 20;
        public List<PowerUpHolder> powerUpList = new();

        AudioSource powerUpAudioSource;

        void OnEnable() => Actions.OnEnemyDeath += IncrementeStreak;
        void OnDisable() => Actions.OnEnemyDeath -= IncrementeStreak;

        private void Start() => Init();
        private void Init()
        {
            powerUpAudioSource = GetComponent<AudioSource>();
        }

        void IncrementeStreak(Transform unit)
        {
            CurrentStreak++;

            int randomValue = Random.Range( 0, powerUpList.Count );

            if ( CurrentStreak >= StreakLimit )
            {
                PowerUpHolder pUpHolder = Instantiate(
                    powerUpList [ randomValue ],
                    new Vector3(
                        unit.position.x + powerUpList [ randomValue ].GetPowerUp().GetOffsetWSpawn().x,
                        unit.position.y + powerUpList [ randomValue ].GetPowerUp().GetOffsetWSpawn().y,
                        unit.position.z + powerUpList [ randomValue ].GetPowerUp().GetOffsetWSpawn().z ),
                    powerUpList [ randomValue ].transform.rotation );

                pUpHolder.SetAudioSource( powerUpAudioSource );

                CurrentStreak = 0;
            }
        }
    }
}