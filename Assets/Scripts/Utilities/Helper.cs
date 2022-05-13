using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Khynan_Coding
{
    public static class Helper
    {
        #region Debug
        public static void DebugMessage(string messageContent, Transform source = null)
        {
            Debug.Log(messageContent, source);
        }

        public static void ThrowErrorMessage(string messageContent, Transform source = null)
        {
            Debug.Log(messageContent, source);
        }
        #endregion

        #region Main Camera
        public static Camera GetMainCamera()
        {
            return Camera.main;
        }

        public static CinemachineBrain GetCinemachineBrain()
        {
            return GetMainCamera().GetComponent<CinemachineBrain>();
        }

        public static ICinemachineCamera GetActiveVirtualCamera()
        {
            return GetCinemachineBrain().ActiveVirtualCamera;
        }
        #endregion

        #region Left and right click pressure check
        public static bool IsLeftClickPressed()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }
            
            return false;
        }

        public static bool IsRightClickPressed()
        {
            if (Input.GetMouseButtonDown(1))
            {
                return true;
            }
            
            return false;
        }
        #endregion

        #region Left and right key hold check
        public static bool IsLeftClickHeld()
        {
            if (Input.GetMouseButton(0))
            {
                return true;
            }
            
            return false;
        }

        public static bool IsRightClickHeld()
        {
            if (Input.GetMouseButton(1))
            {
                return true;
            }
            
            return false;
        }
        #endregion

        #region Left and right click on UI elements pressure check
        public static bool IsLeftClickPressedOnUIElement(PointerEventData requiredEventData)
        {
            if (requiredEventData.button == PointerEventData.InputButton.Left)
            {
                return true;
            }
            
            return false;
        }

        public static bool IsRightClickPressedOnUIElement(PointerEventData requiredEventData)
        {
            if (requiredEventData.button == PointerEventData.InputButton.Right)
            {
                return true;
            }
            
            return false;
        }
        #endregion

        #region Inputs pressure and hold check
#if ENABLE_LEGACY_INPUT_MANAGER
        public static bool IsKeyPressed(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode)) return true;
            
            return false;
        }

        public static bool IsKeyUnpressed(KeyCode keyCode)
        {
            if (Input.GetKeyUp(keyCode)) return true;
            
            return false;
        }

        public static bool IsKeyMaintained(KeyCode keyCode)
        {
            if (Input.GetKey(keyCode)) return true;
            
            return false;
        }

        public static bool IsKeyPressedOrMaintained(KeyCode key)
        {
            if (Input.GetKeyDown(key) || Input.GetKey(key)) return true;

            return false;
        }
#endif
        #endregion

        #region Cursor
        public static Vector3 GetCursorClickPosition(LayerMask layerMask)
        {
            Ray rayFromMainCameraToCursorPosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 hitPointPos = Vector3.zero;

            if (Physics.Raycast(rayFromMainCameraToCursorPosition, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                hitPointPos = hit.point;
            }

            Helper.DebugMessage("Cursor clicked position : " + hitPointPos);

            return hitPointPos;
        }

        public static void SetCursorLockMode(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
        }
        #endregion

        #region NavMeshAgent
        public static void SetAgentDestination(NavMeshAgent navMeshAgent, Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public static void SetAgentStoppingDistance(NavMeshAgent navMeshAgent, float distance, float multiplier)
        {
            navMeshAgent.stoppingDistance = navMeshAgent.radius + (distance * multiplier);
        }

        public static void ResetAgentDestination(NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = true;

            navMeshAgent.path.ClearCorners();
            navMeshAgent.ResetPath();
        }
        #endregion

        #region String - float value rounded to seconds or minutes
        public static string GetStringOfValueInMinutes(float value)
        {
            string minutes = Mathf.Floor(value / 60).ToString("0.00");

            return minutes;
        }

        public static string GetStringOfValueInSeconds(float value)
        {
            string seconds = Mathf.Floor(value % 60).ToString("0.00");

            return seconds;
        }

        public static string GetStringOfValueInMinutesAndSeconds(float value)
        {
            string minutes = Mathf.Floor(value / 60).ToString("0");
            string seconds = Mathf.Floor(value % 60).ToString("00");

            return (minutes + " : " + seconds);
        }
        #endregion

        #region UI
        public static void SetImageSprite(Image image, Sprite sprite)
        {
            if (image.sprite == sprite) { return; }

            image.sprite = sprite;
        }

        public static void DisplayUIWindow(GameObject window)
        {
            if (!window || window.activeInHierarchy) { return; }

            window.SetActive(true);
        }

        public static void HideUIWindow(GameObject window)
        {
            if (!window || !window.activeInHierarchy) { return; }

            window.SetActive(false);
        }

        public static bool IsThisUIWindowDisplayed(GameObject component)
        {
            if (component.activeInHierarchy)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Unity Utilities replacement(s)
        public static T GetComponentInChildren<T>(Transform parent) where T : Component
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).GetComponent<T>() == null) { continue; }

                return parent.GetChild(i).GetComponent<T>();
            }

            return default;
        }
        #endregion

        public static IEnumerator Delay(float delayValue)
        {
            yield return new WaitForSeconds(delayValue);
        }

        public static float GetPercentage(float a, float b, float multiplier = 1)
        {
            return (a / b) * multiplier;
        }

        #region Input System inputs enabled state
#if ENABLE_INPUT_SYSTEM
        public static void EnableInput(InputAction inputAction)
        {
            inputAction.Enable();
        }

        public static void DisableInput(InputAction inputAction)
        {
            inputAction.Disable();
        }

        public static void EnableAllInputs(InputActionAsset inputActionAsset)
        {
            inputActionAsset.Enable();
        }

        public static void DisableAllInputs(InputActionAsset inputActionAsset)
        {
            inputActionAsset.Disable();
        }
#endif
        #endregion
    }
}