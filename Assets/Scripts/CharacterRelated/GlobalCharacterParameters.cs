using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Khynan_Coding
{
    public enum CharacterType
    {
        Unassigned, Player, IA_Enemy, IA_Ally, InteractiveObject
    }

    [DisallowMultipleComponent]
    public class GlobalCharacterParameters : MonoBehaviour
    {
        [SerializeField] private string _name = "";
        [SerializeField] private CharacterType _characterType = CharacterType.Unassigned;

        private RigBuilderHelper _rigBuilderHelper;

        #region Public References
        public CharacterType CharacterType { get => _characterType; }
        #endregion

        private void Awake() => Init();

        private void Init()
        {
            if (transform.GetChild(0).TryGetComponent(out RigBuilderHelper component))
            {
                _rigBuilderHelper = component;
            }
        }

        public RigBuilderHelper GetRigBuilderHelper()
        {
            return _rigBuilderHelper;
        }

        public string GetName() { return _name; }
    }
}