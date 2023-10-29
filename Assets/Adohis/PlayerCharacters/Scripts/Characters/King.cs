using Adohi.Characters.Manager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Adohi.Characters.King
{
    public class King : MonoBehaviour
    {
        private Renderer renderer;

        [Header("Model")]
        public GameObject model;

        [Header("UI")]
        public TextMeshProUGUI pressKeyText;

        [Header("Crown")]
        public GameObject crown;
        public Vector3 fromOffset;
        public Vector3 toOffset;
        public float giveDuration;
        public float remainDuration;

        private void Awake()
        {
            renderer = GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            Rotate();
        }

        public async UniTask GiveCrown()
        {
            crown.SetActive(true);

            await crown.transform.DOMove(CharacterManager.Instance.character.transform.position + toOffset, giveDuration).From(transform.position + fromOffset);
            await UniTask.Delay((int)(remainDuration * 1000f));

            crown.SetActive(false);
        }

        public void Interact(bool isInteractable)
        {
            if (isInteractable)
            {
                renderer.material.EnableKeyword("OUTBASE_ON");
                pressKeyText.gameObject.SetActive(true);
            }
            else
            {
                renderer.material.DisableKeyword("OUTBASE_ON");
                pressKeyText.gameObject.SetActive(false);
            }
        }

        public void Desolve(float time)
        {
            GetComponentInChildren<Collider>().enabled = false;
            renderer.material.DOFloat(1f, "_FadeAmount", time).OnComplete(() => Destroy(gameObject));
        }

        private void Rotate()
        {
            if (CharacterManager.Instance.character.transform.position.x > transform.position.x)
            {
                model.transform.localScale = Vector3.one.normalized * model.transform.localScale.magnitude;
            }
            else
            {
                model.transform.localScale = new Vector3(-1f, 1f, 1f).normalized * model.transform.localScale.magnitude;
            }
        }
    }

}
