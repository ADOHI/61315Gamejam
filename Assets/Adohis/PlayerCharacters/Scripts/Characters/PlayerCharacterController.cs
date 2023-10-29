using Adohi.Managers.GameFlow;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adohi.Characters.Controller
{
    public class PlayerCharacterController : MonoBehaviour
    {
        private Vector2 moveDirection;
        private Rigidbody2D rb;
        private Animator animator;

        public bool isMoveAvailable = true;

        [Header("Move")]
        public float moveSpeed;
        public string moveParameterName = "isMove";

        [Header("Dash")]
        public KeyCode dashKey = KeyCode.LeftShift;
        public float maxStamina;
        public float staminaConsumeSpeed;
        public float staminaRegenSpeed;
        public float dashSpeedMultiplier = 1.5f;
        public float recoveryTime;
        [ShowInInspector] private bool isRegenerateAvailable = true;
        [ShowInInspector] private float currentStamina;
        [ShowInInspector] private float currentRecoveryTime;

        [Header("Model")]
        [HideInInspector] public GameObject currentCharacterModel;
        public float ModelScale = 1f;
        public GameObject normalModel;
        public GameObject metaModel;
        [Header("UI")]
        public GameObject staminaUIChunk;
        public GameObject staminaBar;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            currentCharacterModel = normalModel;
            animator = currentCharacterModel.GetComponentInChildren<Animator>();

        }

        private void Start()
        {
            GameFlowManager.Instance.onWaitGame.AddListener(OnPauseMove);
            GameFlowManager.Instance.onStartGame.AddListener(OnResumeMove);
        }

        private void Update()
        {
            UpdateUI();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            UpdateMoveDirection();
            Rotate();
            Move();
        }

        public void SwapModel(bool isNormal)
        {
            currentCharacterModel.SetActive(false);
            if (isNormal)
            {
                currentCharacterModel = normalModel;
                animator = currentCharacterModel.GetComponentInChildren<Animator>();
                currentCharacterModel.transform.localScale = metaModel.transform.localScale;
            }
            else
            {
                currentCharacterModel = metaModel;
                animator = currentCharacterModel.GetComponentInChildren<Animator>();
                currentCharacterModel.transform.localScale = normalModel.transform.localScale;
            }
            currentCharacterModel.SetActive(true);

        }

        private void UpdateMoveDirection()
        {

            moveDirection = Vector2.zero;

            if (!isMoveAvailable)
            {
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += Vector2.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection += Vector2.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection += Vector2.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += Vector2.right;
            }

            moveDirection = moveDirection.normalized;

            animator.SetBool(moveParameterName, moveDirection.magnitude > 0f);
        }

        private void Rotate()
        {
            if (moveDirection.x == 0)
            {
                return;
            }

            if (moveDirection.x > 0f)
            {
                currentCharacterModel.transform.localScale = new Vector3(1f, 1f, 1f) * ModelScale;
            }
            else if (moveDirection.x < 0f)
            {
                currentCharacterModel.transform.localScale = new Vector3(-1f, 1f, 1f) * ModelScale;
            }

        }

        private void Move()
        {
            //transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

            if (GameFlowManager.Instance.gameFlowType == GameFlowManager.GameFlowType.Title)
            {
                if (!GameFlowManager.Instance.IsInDistance(transform.position))
                {
                    transform.position = Vector3.zero;
                    return;
                }
            }
            
            if (Input.GetKey(dashKey) && currentStamina > 0f)
            {
                rb.velocity = (Vector3)moveDirection * moveSpeed * dashSpeedMultiplier;
                UseStamina();
            }
            else
            {
                rb.velocity = (Vector3)moveDirection * moveSpeed;
            }

            RegenerateStamina();


        }

        private void RegenerateStamina()
        {
            if (isRegenerateAvailable)
            {
                if (!Input.GetKey(dashKey))
                {
                    currentStamina = Mathf.Clamp(currentStamina + staminaRegenSpeed * Time.fixedDeltaTime, 0f, maxStamina);
                }
            }
            else
            {
                if (!Input.GetKey(dashKey))
                {
                    currentRecoveryTime += Time.fixedDeltaTime;

                    if (currentRecoveryTime >= recoveryTime)
                    {
                        isRegenerateAvailable = true;
                    }
                }
                else
                {
                    currentRecoveryTime = 0f;
                }
            }
        }

        private void UseStamina()
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaConsumeSpeed * Time.fixedDeltaTime, 0f, maxStamina);

            if (currentStamina == 0f)
            {
                isRegenerateAvailable = false;
            }
        }

        private void UpdateUI()
        {
            if (maxStamina == 0f)
            {
                return;
            }
            staminaUIChunk.SetActive(true);
            staminaBar.transform.localScale = new Vector3(currentStamina / maxStamina, 1f, 1f);
        }



        public void OnPauseMove()
        {
            isMoveAvailable = false;

        }

        public void OnResumeMove()
        {
            isMoveAvailable = true;
        }




        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("test"))
            {
                Debug.Log("GameOver");

                GameFlowManager.Instance.GameOver().Forget();
            }
        }
    }

}
