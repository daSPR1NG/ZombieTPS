using UnityEngine;

namespace Khynan_Coding
{
    public class ScoreNotificationHandler : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Transform _parentHolder;
        [SerializeField] private GameObject _scoreNotificationCompartmentPf;

        private void OnEnable()
        {
            Actions.OnScoreNotificationNeed += CreateAndSetAScoreNotification;
        }

        private void OnDisable()
        {
            Actions.OnScoreNotificationNeed -= CreateAndSetAScoreNotification;
        }

        void Start() => Init();

        void Init()
        {
            //Set all datas that need it at the start of the game
        }

        private void CreateAndSetAScoreNotification(ScoreData scoreData)
        {
            if (scoreData.ScoreRelatedActionName == ScoreRelatedActionName.OnHit) { return; }

            GameObject scoreNotificationInstance = Instantiate(_scoreNotificationCompartmentPf, _parentHolder);

            ScoreNotificationCompartment scoreNotificationCompartment = scoreNotificationInstance.GetComponent<ScoreNotificationCompartment>();
            string notificationTextContent = string.Empty;

            switch (scoreData.ScoreRelatedActionName)
            {
                case ScoreRelatedActionName.OnHit:
                    //notificationTextContent = "+" + scoreData.Value.ToString() + " Zombie - Dégâts infligés.";
                    break;
                case ScoreRelatedActionName.OnDeath:
                    notificationTextContent = "+" + scoreData.Value.ToString() + " Zombie - Élimination.";
                    break;
            }

            scoreNotificationCompartment.SetContentText(notificationTextContent);
        }
    }
}