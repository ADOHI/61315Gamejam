using Adohi.Characters.Manager;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Adohi.Characters.Cameras
{
    public class PlayerFollowCamera : MonoBehaviour
    {
        private Camera camera;
        public GameObject target;

        public float lerpValue = 10f;

        [Header("Distance")]
        public float currentFollowDistance = 5f;
        public float zoomInDistance;
        public float zoomOutDistance;


        private void Awake()
        {
            camera = GetComponentInChildren<Camera>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Follow();
        }

        private void Follow()
        {
            target = CharacterManager.Instance.character;
            camera.orthographicSize = currentFollowDistance;
            transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0f, 0f, -10f), lerpValue * Time.deltaTime);
        }

        public void Zoom(float distance, float duration)
        {
            DOTween.To(() => currentFollowDistance, x => currentFollowDistance = x, distance, duration);
        }

        public void ZoomOut()
        {
            Zoom(zoomOutDistance, 1f);
        }

        [Button]
        public void OnStun()
        {
            ShakeCamera(2f, strength : 1f);
        }

        public void ShakeCamera(float time, float strength)
        {
            camera.DOShakePosition(2f, strength: 1f);
        }
    }

}
