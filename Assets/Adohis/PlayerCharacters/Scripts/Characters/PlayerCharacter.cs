using Adohi.Characters.Controller;
using Adohi.Characters.Manager;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adohi.Characters
{
    public class PlayerCharacter : MonoBehaviour
    {
        [Serializable]
        public struct CharacterLevelData
        {
            public float scale;
            public float stunCooltime;
            public float moveSpeed;
            public float maxStamina;
        }

        private PlayerCharacterController playerCharacterController;

        [Header("Level")]
        public int currentLevel;

        [Header("Stun")]
        public float stunDuration;
        public float currentStunCooltime;
        private bool IsStunAvailable => currentLevel >= 1;


        [Header("LevelConfig")]
        public CharacterLevelData[] levelData;

        private void Awake()
        {
            playerCharacterController = GetComponent<PlayerCharacterController>();
            LevelUp(0);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsStunAvailable)
            {
                if (currentStunCooltime <= 0f)
                {
                    Stun();
                    currentStunCooltime = levelData[currentLevel].stunCooltime;
                }
                else
                {
                    currentStunCooltime -= Time.deltaTime;
                }
            }

        }

        [Button]
        public void OnLevelUp()
        {
            if (currentLevel < 4)
            {
                currentLevel++;
                LevelUp(currentLevel);
                //playerCharacterController.SwapModel(currentLevel);
            }
        }

        public async UniTask LevelUpAsync()
        {
            Time.timeScale = 0f;
            await UniTask.Delay(2000);
            Time.timeScale = 1f;
        }

        public void Stun()
        {
            CharacterManager.Instance.OnStun?.Invoke(stunDuration);
            Debug.Log("stun start");

            StunAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }

        public async UniTask StunAsync()
        {
            playerCharacterController.SwapModel(false);
            await UniTask.Delay((int)(stunDuration * 1000f));
            playerCharacterController.SwapModel(true);
        }

        private void LevelUp(int level)
        {
            transform.localScale = Vector3.one * levelData[level].scale;
            playerCharacterController.moveSpeed = levelData[level].moveSpeed;
            playerCharacterController.maxStamina = levelData[level].maxStamina;
        }


    }

}

