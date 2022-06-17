using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class ModulesManager : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private bool _usesModules = true;
        [SerializeField] private int _moduleAount = 5;
        [SerializeField] private Transform _moduleParent;
        [SerializeField] private GameObject _moduleBlockPf;

        [Header("GRID SETTINGS")]
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private float _gridCellXSize = 35;
        [SerializeField] private float _gridCellYSize = 35;

        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        void Start() => Init();

        void Init()
        {

        }

        private void CreateModuleBlocks()
        {
            for (int i = 0; i < _moduleAount; i++)
            {

            }
        }

        private void SetGridCellSize()
        {

        }

        private void OnValidate()
        {
            //CreateModuleBlocks();
        }
    }
}