using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class AreaManager : MonoBehaviour
    {
        public static AreaManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private List<GameAreaData> areas;

        [Header("Transition")]
        [SerializeField] private float cameraMoveDuration = 0.35f;
        [SerializeField] private bool instantMove = false;

        private GameAreaType _currentArea;
        private Coroutine _moveCameraCoroutine;

        private void Awake()
        {
            Instance = this;

            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Start()
        {
            ShowArea(GameAreaType.Kitchen, true);
        }

        public void ShowCooking()
        {
            ShowArea(GameAreaType.Kitchen);
        }

        public void ShowGarden()
        {
            ShowArea(GameAreaType.Garden);
        }

        public void ShowPond()
        {
            ShowArea(GameAreaType.Pond);
        }

        public void ShowArea(GameAreaType targetArea)
        {
            ShowArea(targetArea, false);
        }

        private void ShowArea(GameAreaType targetArea, bool forceInstant)
        {
            GameAreaData areaData = GetAreaData(targetArea);

            if (areaData == null)
            {
                Debug.LogError($"Area {targetArea} is not configured in AreaManager.");
                return;
            }

            _currentArea = targetArea;

            SwitchUI(targetArea);
            //SwitchInput(targetArea);

            if (forceInstant || instantMove)
            {
                MoveCameraInstant(areaData.cameraPoint);
            }
            else
            {
                MoveCameraSmooth(areaData.cameraPoint);
            }
        }

        private void SwitchUI(GameAreaType activeArea)
        {
            foreach (var area in areas)
            {
                if (area.areaUI != null)
                    area.areaUI.SetActive(area.areaType == activeArea);
            }
        }

        /*private void SwitchInput(GameAreaType activeArea)
    {
        foreach (var area in areas)
        {
            if (area.inputRoot != null)
                area.inputRoot.SetActive(area.areaType == activeArea);
        }
    }*/

        private void MoveCameraInstant(Transform target)
        {
            if (target == null) return;

            mainCamera.transform.position = target.position;
            mainCamera.transform.rotation = target.rotation;
        }

        private void MoveCameraSmooth(Transform target)
        {
            if (target == null) return;

            if (_moveCameraCoroutine != null)
                StopCoroutine(_moveCameraCoroutine);

            _moveCameraCoroutine = StartCoroutine(MoveCameraCoroutine(target));
        }

        private IEnumerator MoveCameraCoroutine(Transform target)
        {
            Vector3 startPosition = mainCamera.transform.position;
            Quaternion startRotation = mainCamera.transform.rotation;

            Vector3 targetPosition = target.position;
            Quaternion targetRotation = target.rotation;

            float elapsed = 0f;

            while (elapsed < cameraMoveDuration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / cameraMoveDuration;
                t = SmoothStep(t);

                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                yield return null;
            }

            mainCamera.transform.position = targetPosition;
            mainCamera.transform.rotation = targetRotation;
            _moveCameraCoroutine = null;
        }

        private float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        private GameAreaData GetAreaData(GameAreaType areaType)
        {
            foreach (var area in areas)
            {
                if (area.areaType == areaType)
                    return area;
            }

            return null;
        }

        public GameAreaType GetCurrentArea()
        {
            return _currentArea;
        }
    }
}