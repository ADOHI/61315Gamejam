using Adohi.Characters.Manager;
using Adohi.Managers.GameFlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage : MonoBehaviour
{
    public List<MagicCircle> magicCircles = new();
    public int currentCircle = 0;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject player => CharacterManager.Instance.character;
    public bool isInsideCircle = false;

    public static Stage instance = null;

    public Tilemap tileMap;

    [SerializeField] int initialGenAmount = 50;
    [SerializeField] List<int> additionalGenAmount = new List<int> { 30, 30, 30, 30 };

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameFlowManager.Instance.onStartGame.AddListener(OnGameStart);

        /*        for(int i=0; i<60; i++)
                {
                    CreateEnemy();
                }
                magicCircles[currentCircle].Activate();
                print(tileMap.localBounds);*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Vector2.Distance(magicCircles[currentCircle].transform.position, player.transform.position) < 2)
        {
            isInsideCircle = true;
        }
        else isInsideCircle = false;
        */
    }

    private void OnGameStart()
    {
        for (int i = 0; i < initialGenAmount; i++)
        {
            CreateEnemy();
        }
        magicCircles[currentCircle].Activate();
    }

    void CreateEnemy()
    {
        bool succeed = false;
        Vector2 point = Vector2.zero;
        while (!succeed)
        {
            succeed = true;
            point = Random.insideUnitCircle * 20;
            if (Vector2.Distance(point, player.transform.position) < 10)
            {
               succeed = false;
            }
            for(int i=0; i<magicCircles.Count; i++)
            {
                if (Vector2.Distance(magicCircles[i].transform.position, transform.position) < 3)
                {
                    succeed = false;
                }
            }
        }
        Instantiate(enemyPrefab, point, Quaternion.identity);
    }

    public void CircleFinished()
    {
        if (currentCircle < 4)
        {
            for (int i = 0; i < additionalGenAmount[currentCircle]; i++)
            {
                CreateEnemy();
            }
            currentCircle++;
            magicCircles[currentCircle].Activate();
        }
        else GameClear();
    }

    void GameClear()
    {

    }

    public void GameOver()
    {

    }
}

