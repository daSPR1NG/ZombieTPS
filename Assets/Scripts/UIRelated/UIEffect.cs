//using System.Collections.Generic;
//using UnityEngine;

//namespace Khynan_Coding
//{
//    [DisallowMultipleComponent]
//    public class UIEffect : MonoBehaviour
//    {
//        [Header("DEPENDENCIES")]
//        public Transform _parent;
//        public GameObject _effectUIBoxPf;
//        public List<UIEffectBox> _UIBoxes;

//        void OnEnable()
//        {
//            Actions.OnAddingEffect += AddUIBox;
//            Actions.OnRemovingEffect += RemoveUIBox;

//            Helper.CreateUIParent(_parent, transform.GetChild(0));
//        }

//        void OnDisable()
//        {
//            Actions.OnAddingEffect -= AddUIBox;
//            Actions.OnRemovingEffect -= RemoveUIBox;
//        }

//        void AddUIBox(Effect effect)
//        {
//            // If there is no created box then you can create one
//            if (_UIBoxes.Count == 0) 
//            {
//                CreateBox(effect);
//                return;
//            }

//            // Else check if the effect is different than the one already stored
//            for (int i = _UIBoxes.Count - 1; i >= 0; i--)
//            {
//                if (effect.GetName() != _UIBoxes[i].GetEffect().GetName())
//                {
//                    CreateBox(effect);
//                }
//            }
//        }

//        void CreateBox(Effect effect)
//        {
//            //Set all datas that need it at the start of the game
//            GameObject instance = Instantiate(_effectUIBoxPf, _parent);

//            UIEffectBox uiBox = instance.GetComponent<UIEffectBox>();
//            uiBox.Setup(effect);

//            _UIBoxes.Add(uiBox);
//        }

//        void RemoveUIBox(Effect effect)
//        {
//            for (int i = _UIBoxes.Count - 1; i >= 0; i--)
//            {
//                if (effect.GetName() == _UIBoxes[i].GetEffect().GetName())
//                {
//                    _UIBoxes[i].GetRemoved(_UIBoxes);
//                }
//            }
//        }

//        private void OnValidate()
//        {
//            Helper.CreateUIParent(_parent, transform.GetChild(0));
//        }
//    }
//}