using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class AspectCombination
    {
        public string Name;
        public bool IsActive = false;
        public List<GameObject> Items = new();
    }

    public class RandomizeAspect : MonoBehaviour
    {
        [SerializeField] private List<Material> _materials = new();
        [SerializeField] private List<AspectCombination> _aspectCombinations = new();
        [SerializeField] private List<AnimatorOverrideController> _animatorOverriders = new();

        private SkinnedMeshRenderer _skinnedMesh;

        private List<Material> _aspectMaterials = new();

        #region Public References

        #endregion

        void Start() => Init();

        void Init()
        {
            _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

            SetRandomAspectCombination();
            SetRandomMaterial();
            SetAnimatorOverrideController();
        }

        #region Random Aspect
        void SetRandomAspectCombination()
        {
            if (_aspectCombinations.Count == 0) { return; }

            int random = Random.Range(0, _materials.Count);
            AspectCombination aspectCombination = _aspectCombinations[random];

            for (int i = 0; i < _aspectCombinations[random].Items.Count; i++)
            {
                if (aspectCombination.Items.Count == 0)
                {
                    Debug.LogError("No items found in the combination !");
                    return;
                }

                if (aspectCombination.Items[i].Equals(null)) { continue; }

                aspectCombination.Items[i].SetActive(true);
            }

            aspectCombination.IsActive = true;

            GetAspectDefaultMaterials();
        }

        public AspectCombination GetCurrentAspectCombination()
        {
            if (_aspectCombinations.Count == 0) { return null; }

            for (int i = 0; i < _aspectCombinations.Count; i++)
            {
                if (_aspectCombinations[i].IsActive) 
                {
                    //Debug.Log(_aspectCombinations[i].Name, transform);
                    return _aspectCombinations[i]; 
                }
            }

            return null;
        }
        #endregion

        #region Material Handle
        void SetRandomMaterial()
        {
            if (_materials.Count == 0) { return; }

            int random = Random.Range(0, _materials.Count);

            _skinnedMesh.material = null;
            _skinnedMesh.material = _materials[random];
        }

        private void GetAspectDefaultMaterials()
        {
            if (GetCurrentAspectCombination().Items.Count == 0) { return; }

            for (int i = 0; i < GetCurrentAspectCombination().Items.Count; i++)
            {
                MeshRenderer meshRenderer = GetCurrentAspectCombination().Items[i].GetComponent<MeshRenderer>();
                //Debug.Log("Mesh Renderer found " + meshRenderer.name);
                //Debug.Log("Material found " + meshRenderer.sharedMaterial.name);

                _aspectMaterials.Add(meshRenderer.material);
                //Debug.Log(meshRenderer.material.name);
            }
        }

        public void SetAspectDefaultMaterials()
        {
            if (_aspectMaterials.Count == 0) { return; }

            for (int i = 0; i < _aspectMaterials.Count; i++)
            {
                MeshRenderer meshRenderer = GetCurrentAspectCombination().Items[i].GetComponent<MeshRenderer>();

                meshRenderer.material = _aspectMaterials[i];
            }
        }

        public void SetAspectMaterial(Material material)
        {
            if (GetCurrentAspectCombination().Items.Count == 0) { return; }

            for (int i = 0; i < GetCurrentAspectCombination().Items.Count; i++)
            {
                MeshRenderer meshRenderer = GetCurrentAspectCombination().Items[i].GetComponent<MeshRenderer>();

                meshRenderer.material = material;
            }
        }
        #endregion

        #region 
        private void SetAnimatorOverrideController()
        {
            if (_animatorOverriders.Count == 0) { return; }

            int random = Random.Range(0, _animatorOverriders.Count);

            Animator animator = transform.GetChild(0).GetComponent<Animator>();
            animator.runtimeAnimatorController = _animatorOverriders[random];
        }
        #endregion
    }
}