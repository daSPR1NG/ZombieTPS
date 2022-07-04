using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Khynan_Coding
{
    public enum RigBodyPart
    {
        Unassigned, None, 
        Head, Neck, 
            L_Shoulder, L_Elbow, L_Hand, 
                R_Shoulder, R_Elbow, R_Hand, 
        Waist, 
            L_Knee, L_Foot, 
                R_Knee, R_Foot,
        HeldObject_Shoot_Aim, HeldObject_Shoot_NoAim,
        HeldObject_Reloading, L_Hand_Reload
    }

    [System.Serializable]
    public class RigData
    {
        [SerializeField] private string _name = "new RigData";
        [SerializeField] private RigBodyPart _rigBodyPart = RigBodyPart.Unassigned;
        [SerializeField] private Rig _rig;
        [SerializeField] private TwoBoneIKConstraint _twoBoneIKConstraint;
        [SerializeField] private MultiAimConstraint _aimConstraint;

        public RigBodyPart GetRigBodyPart()
        {
            return _rigBodyPart;
        }

        public Rig GetRig()
        {
            return _rig;
        }

        public float GetRigWeight()
        {
            return _rig.weight;
        }

        public void SetRigWeight(float weight)
        {
            if (!_rig || _rig.weight == weight) { return; }

            _rig.weight = weight;
        }

        public TwoBoneIKConstraint GetTwoBoneIKConstraint()
        {
            return _twoBoneIKConstraint;
        }

        public void SetTwoBoneIKConstraintWeight(float weight)
        {
            if (!_rig || _twoBoneIKConstraint.weight == weight) { return; }

            _twoBoneIKConstraint.weight = weight;
        }

        public MultiAimConstraint GetMultiAimConstraint()
        {
            return _aimConstraint;
        }

        public void SetMultiAimConstraintWeight(float weight)
        {
            if (!_rig || _aimConstraint.weight == weight) { return; }

            _aimConstraint.weight = weight;
        }
    }

    public class RigBuilderHelper : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private GameObject _heldObjectPivot;
        [SerializeField] private Animator _animator;
        [SerializeField] private List<RigData> _rigDatas = new ();   

        private RigBuilder _rigBuilder;

        private void Awake() => Init();

        private void Init()
        {
            _rigBuilder = GetComponentInChildren<RigBuilder>();
        }

        #region Enable / Disable - multiple
        public void EnablePrincipalRigLayers()
        {
            // Weapon Aim
            _rigBuilder.layers[1].active = true;

            // Hand hold
            _rigBuilder.layers[2].active = true;

            // Weapon shoot no Aim
            _rigBuilder.layers[3].active = true;

            //for (int i = 0; i < _rigBuilder.layers.Count; i++)
            //{
            //    if (_rigBuilder.layers[i].active) { continue; }

            //    _rigBuilder.layers[i].active = true;
            //}
        }

        public void DisablePrincipalRigLayers(bool disableHoldingRig)
        {
            // Weapon Aim
            _rigBuilder.layers[1].active = false;

            // Hand hold
            _rigBuilder.layers[2].active = false;

            // Weapon shoot no Aim
            if (disableHoldingRig) { _rigBuilder.layers[3].active = false; }

            //for (int i = 0; i < _rigBuilder.layers.Count; i++)
            //{
            //    if (!_rigBuilder.layers[i].active) { continue; }

            //    _rigBuilder.layers[i].active = false;
            //}
        }
        #endregion

        #region Enable / Disable - specific by int or rig component
        public void EnableThisRigLayer(int index, Rig rig = null)
        {
            if (rig)
            {
                for (int i = 0; i < _rigBuilder.layers.Count; i++)
                {
                    if (_rigBuilder.layers[i].rig == rig)
                    {
                        _rigBuilder.layers[i].active = true;
                    }
                }
                return;
            }

            _rigBuilder.layers[index].active = true;
        }

        public void DisableThisLayer(int index, Rig rig = null)
        {
            if (rig)
            {
                for (int i = 0; i < _rigBuilder.layers.Count; i++)
                {
                    if (_rigBuilder.layers[i].rig == rig)
                    {
                        _rigBuilder.layers[i].active = false;
                    }
                }
                return;
            }

            _rigBuilder.layers[index].active = false;
        }
        #endregion

        #region Held object - Display / Hide
        public void DisplayHeldObject()
        {
            _heldObjectPivot.gameObject.SetActive(true);
        }

        public void HideHeldObject()
        {
            _heldObjectPivot.gameObject.SetActive(false);
        }
        #endregion

        #region Hand rig data - Get
        public RigData GetRigData(RigBodyPart rigBodyPart)
        {
            if (_rigDatas.Count == 0) { return null; }

            for (int i = 0; i < _rigDatas.Count; i++)
            {
                if (_rigDatas[i].GetRigBodyPart() != rigBodyPart) { continue; }

                return _rigDatas[i];
            }

            return null;
        }
        #endregion

        public void SetRigWeight(RigBodyPart rigBodyPart, float weight)
        {
            GetRigData(rigBodyPart).SetRigWeight(weight);
        }

        public Animator GetAnimator() { return _animator; }

        private bool IsHoldingRigDisabled()
        {
            if (!GetRigData(RigBodyPart.L_Hand).GetRig().enabled && !GetRigData(RigBodyPart.R_Hand).GetRig().enabled)
            {
                return true;
            }

            return false;
        }

        private bool IsHeldObjectAimPoseDisabled()
        {
            if (!GetRigData(RigBodyPart.HeldObject_Shoot_Aim).GetRig().enabled)
            {
                return true;
            }

            return false;
        }

        private bool IsHeldObjectShootPoseDisabled()
        {
            if (!GetRigData(RigBodyPart.HeldObject_Shoot_NoAim).GetRig().enabled)
            {
                return true;
            }

            return false;
        }

        #region Rig Handle
        public void FreeHandsWhileInAir(bool hideHeldObject)
        {
            // Set the hand free state
            DisablePrincipalRigLayers(hideHeldObject);

            if (!hideHeldObject) { return; }

            HideHeldObject();

            //Debug.Log("FreeHandsWhileInAir");
        }

        public void ReAssignHoldingPoseOnTouchingGround()
        {
            if (/*IsHoldingRigDisabled() ||*/ IsHeldObjectAimPoseDisabled() && IsHeldObjectShootPoseDisabled()) { return; }

            // Display and re-enable the holding weapon state
            EnablePrincipalRigLayers();
            DisplayHeldObject();

            //Debug.Log("ReAssignHandsOnTouchingGround : " + IsHeldObjectAimPoseDisabled() + " | " + IsHeldObjectShootPoseDisabled());
        }
        #endregion
    }
}