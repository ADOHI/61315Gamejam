using Adohi.Characters.King;
using Adohi.Characters.Manager;
using Cysharp.Threading.Tasks;
using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Adohi.Managers.GameFlow
{
    public class GameFlowManager : Singleton<GameFlowManager>
    {
        public enum GameFlowType
        {
            Title,
            Ingame,
            Ending
        }

        public GameFlowType gameFlowType = GameFlowType.Title;



        [Header("TitleSetting")]
        public GameObject kingPrefab;
        [HideInInspector] public GameObject kingObject;
        [HideInInspector] public King king;
        [HideInInspector] public Renderer kingRenderer;
        public Vector3 initialKingPosition;
        public float interactableDistance = 2f;
        public float movableDistance = 10f;
        public KeyCode startKey = KeyCode.Space;

        public GameObject titleText;
        public float titleRevealDuration;


        [Header("BGMs")]
        public AudioClip titleBgm;
        public AudioClip ingameBgm;

        [Header("Events")]
        public UnityEvent onWaitGame;
        public UnityEvent onStartGame;

        private void Awake()
        {
            kingObject = Instantiate(kingPrefab);
            kingObject.transform.position = initialKingPosition;
            kingObject.SetActive(true);
            king = kingObject.GetComponent<King>();
            kingRenderer = GetComponentInChildren<Renderer>();
        }

        private void Start()
        {
            FlowGameAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();

            SoundManager.PlayMusic(titleBgm, fadeinTime: 3f);
        }

        private void Update()
        {
            if (gameFlowType == GameFlowType.Title)
            {
                king.Interact(IsGameStartAvailable());
            }

        }

        public async UniTask FlowGameAsync()
        {


            await UniTask.WaitUntil(() => IsGameStartAvailable() && Input.GetKeyDown(startKey));

            onWaitGame?.Invoke();
            SoundManager.StopMusic(titleBgm, fadeoutTime: 3f);
            await king.GiveCrown();
            SoundManager.StopAll();
            SoundManager.PlayMusic(ingameBgm, fadeinTime: 3f);
            king.Desolve(5f);
            onStartGame?.Invoke();
            gameFlowType = GameFlowType.Ingame;

            ShowTitleAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();


        }

        public bool IsGameStartAvailable()
        {
            return (gameFlowType == GameFlowType.Title)
                &&
                Vector3.Distance(
                    kingObject.transform.position,
                    CharacterManager.Instance.character.transform.position
                ) < interactableDistance;

        }

        public bool IsInDistance(Vector3 characterPosition)
        {
            return Vector3.Distance(characterPosition, kingObject.transform.position) < movableDistance;
        }

        public Vector3 ClampedPosition(Vector3 characterPosition)
        {
            if (IsInDistance(characterPosition))
            {
                return characterPosition;
            }
            else
            {
                var direction = (characterPosition - kingObject.transform.position).normalized;
                return kingObject.transform.position + direction * (movableDistance - float.Epsilon);
            }
        }

        private void OnStartAvailable()
        {

        }

        [Button]
        public void SceneTest()
        {
            SceneManager.LoadSceneAsync(0);
        }



        public async UniTask ShowTitleAsync()
        {
            titleText.SetActive(true);
            await UniTask.Delay((int)(titleRevealDuration * 1000f));
            titleText.SetActive(false);
        }



        public async UniTask GameOver()
        {

        }

        public async UniTask GameEnd()
        {

        }
    }

}
