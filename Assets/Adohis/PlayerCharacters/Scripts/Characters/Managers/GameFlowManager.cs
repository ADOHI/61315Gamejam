using Adohi.Characters.Cameras;
using Adohi.Characters.King;
using Adohi.Characters.Manager;
using Adohi.Managers.UIs;
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

        [Header("Chunks")]
        public GameObject gameOverChunk;
        public GameObject endingChunk;

        [Header("BGMs")]
        public AudioClip titleBgm;
        public AudioClip ingameBgm;
        public AudioClip gameOverBgm;
        public AudioClip endingBgm;

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
            Time.timeScale = 1f;
            FlowGameAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();
            GlobalUIManager.Instance.Fade(2f, 0f);
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
            gameFlowType = GameFlowType.Ingame;

            await king.GiveCrown();
            SoundManager.StopAll();
            SoundManager.PlayMusic(ingameBgm, fadeinTime: 3f);

            king.Desolve(5f);
            onStartGame?.Invoke();

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
            Time.timeScale = 0f;
            SoundManager.StopMusic(ingameBgm, fadeoutTime: 3f);
            await GlobalUIManager.Instance.FadeAsync(3f, 1f);
            SoundManager.PlayMusic(gameOverBgm, fadeinTime: 3f);
            await UniTask.Delay(2000, ignoreTimeScale: true);
            gameOverChunk.SetActive(true);
            await UniTask.Delay(5000, ignoreTimeScale: true);
            SoundManager.StopAll();
            SceneManager.LoadSceneAsync(0);
        }

        public async UniTask GameEnd()
        {
            Time.timeScale = 0f;
            SoundManager.StopMusic(ingameBgm, fadeoutTime: 3f);
            await GlobalUIManager.Instance.FadeAsync(3f, 1f);
            SoundManager.PlayMusic(endingBgm, fadeinTime: 3f);
            await UniTask.Delay(2000, ignoreTimeScale: true);
            endingChunk.SetActive(true);
            await UniTask.Delay(5000, ignoreTimeScale: true);
            SoundManager.StopAll();
            SceneManager.LoadSceneAsync(0);
        }
    }

}
