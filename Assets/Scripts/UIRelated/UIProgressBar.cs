//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Khynan_Coding
//{
//    public class UIProgressBar : MonoBehaviour
//    {
//        [Header("SETUP")]
//        [SerializeField] private bool isUsingFilledProgressBarType = true;
//        [Tooltip("Notes that the element 0 is always the filled progress bar type, the element 1 is always the other type")]
//        [Space][SerializeField] private List<ProgressBarElements> progressBarElements = new();
//        private bool _isProgressive;

//        [Header("FILL IMAGE SETTINGS")]
//        [SerializeField] private bool doesItFillsPositively = false;
//        [SerializeField] private float refreshMultiplier = 1f;
//        private float _currentFillValue = 0f;
//        private float _maxFillValue = 0f;

//        [Header("LOOK SETTINGS")]
//        [SerializeField] private Image backgroundImage;
//        [SerializeField] private List<BackgroundLookElements> backgroundLooks = new();

//        #region Public references
//        public float CurrentFillValue { get => _currentFillValue; private set => _currentFillValue = Mathf.Clamp(value, 0, MaxFillValue); }
//        public float MaxFillValue { get => _maxFillValue; private set => _maxFillValue = value; }
//        #endregion

//        [System.Serializable]
//        public class ProgressBarElements
//        {
//            [SerializeField] private string m_Name;

//            [Header("SETUP")]
//            [SerializeField] private GameObject progressBarContentToActivate;
//            [SerializeField] private TMP_Text actionText;

//            //Used when its a filled progress bar type
//            [SerializeField] private Image fillImage;
//            [SerializeField] private TMP_Text progressTimerText;

//            //Used when its a updated image progress bar type
//            [SerializeField] private Image imageToUpdate;
//            [SerializeField] private Sprite[] imageSprites;

//            #region Public references
//            public GameObject ProgressBarContentToActivate { get => progressBarContentToActivate; }
//            public TMP_Text ActionText { get => actionText; }
//            public Image FillImage { get => fillImage; }
//            public TMP_Text ProgressTimerText { get => progressTimerText; }
//            public Image ImageToUpdate { get => imageToUpdate; }
//            public Sprite[] ImageSprites { get => imageSprites; }
//            #endregion

//            public void SetName(string stringValue)
//            {
//                if (m_Name == stringValue) { return; }

//                m_Name = stringValue;
//            }
//        }

//        [System.Serializable]
//        public class BackgroundLookElements
//        {
//            [SerializeField] private string m_Name;
//            [SerializeField] private InteractionType linkedInteractionType;
//            [SerializeField] private Sprite backgroundLookSprite;

//            #region Public references
//            public InteractionType LinkedInteractionType { get => linkedInteractionType; }
//            public Sprite BackgroundLookSprite { get => backgroundLookSprite; private set => backgroundLookSprite = value; }
//            #endregion

//            public void SetName(string stringValue)
//            {
//                if (m_Name == stringValue) { return; }

//                m_Name = stringValue;
//            }

//            public void AssignMyBackgroundSpriteTo(Image image)
//            {
//                image.sprite = BackgroundLookSprite;
//            }
//        }

//        protected virtual void Start() => HideProgressBar();

//        #region Initialization - Action Name / Fill Amount
//        protected virtual void Init(InteractionType type, float currentValue, float maxValue, bool isProgressive, string attachedTextContent = null)
//        {
//            if (!isProgressive) { return; }

//            DisplayProgressBar(type);
//            InitImageFillAmount(currentValue, maxValue);

//            if (!string.IsNullOrEmpty(attachedTextContent)) 
//            {
//                InitProgressBarActionNameText(attachedTextContent);
//            }

//            _isProgressive = isProgressive;
//        }

//        private void InitProgressBarActionNameText(string stringValue)
//        {
//            switch (isUsingFilledProgressBarType)
//            {
//                case true:
//                    progressBarElements[0].ActionText.SetText(stringValue);
//                    break;
//                case false:
//                    progressBarElements[1].ActionText.SetText(stringValue);
//                    break;
//            }
//        }

//        private void InitImageFillAmount(float currentValue, float maxValue)
//        {
//            CurrentFillValue = currentValue;
//            MaxFillValue = maxValue;

//            SetImageFillAmount(currentValue, maxValue);

//            progressBarElements[0].ProgressTimerText.SetText(
//               CurrentFillValue.ToString() + "s" + " / " + (maxValue % 60).ToString("0.00") + "s");
//        }
//        #endregion

//        #region Update / Set - Fill Amount value
//        protected virtual void UpdateImageFillAmout(float maxValue)
//        {
//            if (!progressBarElements[0].ProgressBarContentToActivate.activeInHierarchy || !_isProgressive)
//            {
//                Debug.Log("The progres bar is not active or the interaction type is not progressive. The fill amount won't update.");
//                return; 
//            }

//            Debug.Log("Update image fill amount");

//            CurrentFillValue += doesItFillsPositively ? Time.deltaTime * refreshMultiplier : -Time.deltaTime * refreshMultiplier;

//            //The index is 0 because only the first type - which is a filled type - is used here !
//            progressBarElements[0].ProgressTimerText.SetText(
//                CurrentFillValue.ToString("0.00") /*+ "s"*/ + " / " + (MaxFillValue % 60).ToString("0.00") + "s");
//            SetImageFillAmount(CurrentFillValue, maxValue);
//        }

//        private void SetImageFillAmount(float current, float max)
//        {
//            progressBarElements[0].FillImage.fillAmount = current / max;
//        }
//        #endregion

//        #region ProgressBar set active - Display / Hide
//        private void DisplayProgressBar(InteractionType type)
//        {
//            if (progressBarElements.Count == 0)
//            {
//                Debug.LogError("The list progressBarElements has only one element or is empty and cannot execute the following code.");
//                return;
//            }

//            SetBackground(type);

//            switch (isUsingFilledProgressBarType)
//            {
//                case true:
//                    progressBarElements[0].ProgressBarContentToActivate.SetActive(true);
//                    break;
//                case false:
//                    progressBarElements[1].ProgressBarContentToActivate.SetActive(true);
//                    break;
//            }
//        }

//        protected void HideProgressBar()
//        {
//            if (progressBarElements.Count == 0)
//            {
//                Debug.LogError("The list progressBarElements has only one element or is empty and cannot execute the following code.");
//                return;
//            }

//            switch (isUsingFilledProgressBarType)
//            {
//                case true:
//                    progressBarElements[0].ProgressBarContentToActivate.SetActive(false);
//                    break;
//                case false:
//                    progressBarElements[1].ProgressBarContentToActivate.SetActive(false);
//                    break;
//            }
//        }
//        #endregion

//        private void SetBackground(InteractionType type)
//        {
//            if (backgroundLooks.Count == 0) 
//            {
//                Debug.LogError("Problem : backgroundLooks.Count is empty");
//                return; 
//            }

//            for (int i = 0; i < backgroundLooks.Count; i++)
//            {
//                if (backgroundLooks[i].LinkedInteractionType != type) { continue; }

//                backgroundLooks[i].AssignMyBackgroundSpriteTo(backgroundImage);
//            }
//        }

//        #region OnValidate
//        private void OnValidate()
//        {
//            SetProgressBarElementsName();
//            SetBackgroundLooksName();
//        }

//        private void SetProgressBarElementsName()
//        {
//            if (progressBarElements.Count == 0) { return; }

//            for (int i = 0; i < progressBarElements.Count; i++)
//            {
//                if (!progressBarElements[i].ProgressBarContentToActivate)
//                {
//                    progressBarElements[i].SetName(progressBarElements[i].ProgressBarContentToActivate.name);
//                }
//            }
//        }

//        private void SetBackgroundLooksName()
//        {
//            if (backgroundLooks.Count == 0) { return; }

//            for (int i = 0; i < backgroundLooks.Count; i++)
//            {
//                backgroundLooks[i].SetName(backgroundLooks[i].LinkedInteractionType.ToString());
//            }
//        }
//        #endregion
//    }
//}