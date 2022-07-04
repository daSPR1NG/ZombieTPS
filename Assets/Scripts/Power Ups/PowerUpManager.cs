using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class PowerUpManager : MonoBehaviour
    {
        [SerializeField] private List<PowerUpDurationData> _powerUpDurationDatas = new();

        [System.Serializable]
        public class PowerUpDurationData
        {
            public PowerUp PowerUp;
            public float Duration;

            public PowerUpDurationData(PowerUp powerUp, float duration)
            {
                PowerUp = powerUp;
                Duration = duration;
            }
        }

        private void LateUpdate() => ProcessPowerUpDuration();

        #region Add / Remove power up datas
        public void AddPowerUpToList(PowerUp powerUp)
        {
            if (_powerUpDurationDatas.Count == 0) 
            {
                _powerUpDurationDatas.Add(new PowerUpDurationData(powerUp, powerUp.GetDuration()));

                //Debug.Log("Add Power Up");
                return;
            }

            for (int i = 0; i < _powerUpDurationDatas.Count; i++)
            {
                if (_powerUpDurationDatas[i].PowerUp != powerUp)
                {
                    _powerUpDurationDatas.Add(new PowerUpDurationData(powerUp, powerUp.GetDuration()));

                    //Debug.Log("Add Power Up");
                }
            }
        }

        private void RemoveFromPowerUpList(PowerUp powerUp)
        {
            for (int i = 0; i < _powerUpDurationDatas.Count; i++)
            {
                if (_powerUpDurationDatas[i].PowerUp == powerUp)
                {
                    _powerUpDurationDatas[i].PowerUp.RemovePowerUpEffect(transform);
                    _powerUpDurationDatas.Remove(_powerUpDurationDatas[i]);

                    //Debug.Log("Remove Power Up");
                }
            }
        }
        #endregion

        #region Manage power up datas duration
        private void ProcessPowerUpDuration()
        {
            if (_powerUpDurationDatas.Count == 0) { return; }

            for (int i = _powerUpDurationDatas.Count - 1; i >= 0; i--)
            {
                _powerUpDurationDatas[i].Duration -= Time.deltaTime;

                if (_powerUpDurationDatas[i].Duration <= 0)
                {
                    _powerUpDurationDatas[i].Duration = 0;
                    RemoveFromPowerUpList(_powerUpDurationDatas[i].PowerUp);
                }
            }
        }

        public void RefreshPowerUpDuration(PowerUp powerUp)
        {
            for (int i = 0; i < _powerUpDurationDatas.Count; i++)
            {
                if (_powerUpDurationDatas[i].PowerUp == powerUp)
                {
                    // The collected power up is the same as another collected ealier, so we reset its duration without adding it to the pool.
                    GetPowerUpDurationData(powerUp).Duration = powerUp.GetDuration();

                    Debug.Log("Refresh Power Up");
                }
            }
        }
        #endregion

        #region Manage power up datas
        private PowerUpDurationData GetPowerUpDurationData(PowerUp powerUp)
        {
            if (_powerUpDurationDatas.Count == 0) { return null; }

            for (int i = 0; i < _powerUpDurationDatas.Count; i++)
            {
                if (_powerUpDurationDatas[i].PowerUp = powerUp) { return _powerUpDurationDatas[i]; }
            }

            return null;
        }

        public bool DoesPUpManagerContainsThisPUp(PowerUp powerUp)
        {
            if (_powerUpDurationDatas.Count == 0) { return false; }

            for (int i = _powerUpDurationDatas.Count - 1; i >= 0; i--)
            {
                if (_powerUpDurationDatas[i].PowerUp == powerUp) { return true; }
            }

            return false;
        }
        #endregion

    }
}