using Adohi.Characters.Cameras;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Adohi.Characters.Manager
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        public GameObject characterPrefab;
        [HideInInspector] public GameObject character;
        [HideInInspector] public PlayerFollowCamera followCamera;



        [Header("Stun")]
        public UnityEvent<float> OnStun;

        private void Awake()
        {
            SpawnCharacter();
            followCamera = Camera.main.GetComponent<PlayerFollowCamera>();
        }



        public void SpawnCharacter()
        {
            character = Instantiate(characterPrefab);
            character.SetActive(true);
        }
    }

}
