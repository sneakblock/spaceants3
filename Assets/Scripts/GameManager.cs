using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }
    
    // ========== CAMERA ==========
    private Camera _currCamera;
    private ScreenColliderManager _screenColliderManager;

    [Header("Queen Prefab")]
    // ========= QUEEN INFO =========
    public GameObject queenGO;

    // ====== PLAYER ======
    [Header("Player Info")]
    public bool controlsWithMouse;
    
    public float playerSpeed;
    
    public float playerAttractForce;
    public float playerRepulseForce;
    public float suckMultiplier;
    public float playerAntKillThreshold;
    public float playerQueenKillThreshold;

    public int playerMaxNumAnts;
    public int playerCurNumAnts;
    public float playerInitAntMass;
    public float playerInitAntDrag;
    public float playerInitAntAngularDrag;
    
    public GameObject player;
    private Queen playerQueen;
    private Vector3 _velocity;
    
    // ========== ENEMIES ==========
    [Header("Enemies")]
    
    public int maxNumEnemies = 10;
    
    public float minEnemyMoveSpeed;
    public float maxEnemyMoveSpeed;
    public float enemyAttractForce;
    public float enemyRepulseForce;
    public float enemySuckMultiplier;
    
    public float enemyAntMass;
    public float enemyAntDrag;
    public float enemyAntAngularDrag;
    public float enemyAntKillThreshold;
    public float enemyQueenKillThreshold;
    
    private List<GameObject> enemies;
    [SerializeField] public int numCurrEnemies;
    
    
    // ======= SPAWNING ======
    private List<Rect> spawnZones;

    //====== COLORS =======

    [Header("Colors")]
    public Color playerCol;
    public Color chaserCol;
    public Color turtleCol;
    public Color sniperCol;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ======= PLAYER =======
        InitPlayer();
        
        // ==== CAMERA AND SCREEN BORDERS ===
        _currCamera = Camera.main;
        _screenColliderManager = gameObject.AddComponent<ScreenColliderManager>();
        
        // ======= ENEMIES =======
        enemies = new List<GameObject>();
        spawnZones = new List<Rect>();
        InitSpawnZones();
        InitEnemies();
        
    }

    void InitPlayer()
    {
        player = GameObject.Instantiate(queenGO);
        player.name = "Player";
        player.tag = "Player";
        player.layer = 6;
        playerQueen = player.GetComponent<Queen>();
        playerQueen.attractForce = playerAttractForce;
        playerQueen.repulseForce = playerRepulseForce;
        playerQueen.suckMultipler = suckMultiplier;
        playerQueen.maxNumAnts = playerMaxNumAnts;
        playerQueen.numActiveAnts = playerCurNumAnts;
        playerQueen.antMass = playerInitAntMass;
        playerQueen.antDrag = playerInitAntDrag;
        playerQueen.antAngularDrag = playerInitAntAngularDrag;
        playerQueen.antSpeedKillThreshold = playerAntKillThreshold;
        playerQueen.queenSpeedKillThreshold = playerQueenKillThreshold;
        playerQueen.queenType = QueenType.Player;
    }

    void InitSpawnZones()
    {
        Vector3 leftCenter = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(-.5f, .5f));
        Rect leftSpawnZone = new Rect((Vector2) leftCenter + new Vector2(-3f, -3f), Vector2.one * 6);

        Vector3 rightCenter = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(1.5f, .5f));
        Rect rightSpawnZone = new Rect((Vector2) rightCenter + new Vector2(-3f, -3f), Vector2.one * 6);

        Vector3 topCenter = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(.5f, 1.5f));
        Rect topSpawnZone = new Rect((Vector2) topCenter + new Vector2(-3f, -3f), Vector2.one * 6);

        Vector3 bottomCenter = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(.5f, -.5f));
        Rect bottomSpawnZone = new Rect((Vector2) bottomCenter + new Vector2(-3f, -3f), Vector2.one * 6);

        spawnZones.Add(leftSpawnZone);
        spawnZones.Add(rightSpawnZone);
        spawnZones.Add(topSpawnZone);
        spawnZones.Add(bottomSpawnZone);
        
        // Maybe later...
        // Rect bottomLeftEnemySpawnZone = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(-.5f, -.5f));
        // Rect topLeftEnemySpawnZone = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(-.5f, 1.5f));
        // Rect bottomRightEnemySpawnZone = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(1.5f, -.5f));
        // Rect topRightEnemySpawnZone = Camera.main.ViewportToWorldPoint((Vector3) new Vector2(1.5f, 1.5f));
    }

    [ContextMenu("Init Enemies")]
    public void InitEnemies()
    {
        for (int i = 0; i < maxNumEnemies; i++)
        {
            GameObject enemy = GameObject.Instantiate(queenGO);
            enemy.name = "Enemy";
            enemy.tag = "Enemy";
            enemy.layer = 7;
            if (enemy.GetComponent<Queen>() != null)
            {
                Queen enemyQueen = enemy.GetComponent<Queen>();
            }
            enemies.Add(enemy);
            enemy.SetActive(false);
        }
        
        // GameObject.Instantiate(queenGO);
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnEnemy()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                Rect chosenZone = spawnZones[Random.Range(0, spawnZones.Count)];
                float spawnX = Random.Range(chosenZone.x, chosenZone.x + chosenZone.width - 1);
                float spawnY = Random.Range(chosenZone.y, chosenZone.y + chosenZone.height - 1);

                enemy.transform.position = (Vector3) new Vector2(spawnX, spawnY);
                enemy.GetComponent<SpriteRenderer>().enabled = true;
                enemy.GetComponent<Collider2D>().enabled = true;
                Queen enemyQueen = enemy.GetComponent<Queen>();
                enemyQueen.maxNumAnts = playerQueen.maxNumAnts * 4;
                enemyQueen.numActiveAnts = (int) Random.Range(20, playerQueen.maxNumAnts * 2);
                enemyQueen.moveSpeed = Random.Range(minEnemyMoveSpeed, maxEnemyMoveSpeed);
                enemyQueen.antMass = enemyAntMass;
                enemyQueen.antDrag = enemyAntDrag;
                enemyQueen.antAngularDrag = enemyAntAngularDrag;
                enemyQueen.antSpeedKillThreshold = enemyAntKillThreshold;
                enemyQueen.queenSpeedKillThreshold = enemyQueenKillThreshold;
                enemyQueen.attractForce = enemyAttractForce;
                enemyQueen.repulseForce = enemyRepulseForce;
                enemyQueen.suckMultipler = enemySuckMultiplier;
                
                //Assigning enemy brain
                if (enemyQueen.brain != null)
                {
                    Destroy(enemyQueen.brain);
                }

                int coinToss = Random.Range(0, 6);

                switch (coinToss)
                {
                    case 0:
                    case 1:
                    case 2:
                        enemyQueen.brain = enemyQueen.gameObject.AddComponent<ChaserBrain>();
                        enemy.name = "Chaser";
                        enemyQueen.SetColor(chaserCol);
                        break;
                    case 3:
                        enemyQueen.brain = enemyQueen.gameObject.AddComponent<TurtleBrain>();
                        enemyQueen.numActiveAnts *= 2;
                        enemyQueen.SetAntSpeedKillThreshold(enemyAntKillThreshold / 2);
                        enemy.name = "Turtle";
                        enemyQueen.SetColor(turtleCol);
                        break;
                    case 4:
                    case 5:
                        enemyQueen.brain = enemyQueen.gameObject.AddComponent<SniperBrain>();
                        enemyQueen.numActiveAnts /= 2;
                        enemyQueen.repulseForce *= 2;
                        enemy.name = "Sniper";
                        enemyQueen.SetColor(sniperCol);
                        break;
                }

                enemyQueen.EnableAnts();
                numCurrEnemies++;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayer();
        UpdateEnemies();
        
        // DEBUG
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemy();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerQueen.ScaleAntSize(1.2f);
        }
    }

    void UpdatePlayer()
    {
        //======= FORCE =======
        if (Input.GetMouseButton(0))
        {
            playerQueen.currentCommand = AntCommand.REPULSE;
        } else
        {
            playerQueen.currentCommand = AntCommand.ATTRACT;
        }
        if (Input.GetMouseButton(1))
        {
            playerQueen.currentCommand = AntCommand.SUCK;
        }
    }

    void UpdateEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                enemy.GetComponent<Queen>().brain.Move();
                enemy.GetComponent<Queen>().brain.Behave();
            }
        }
    }

    private void FixedUpdate()
    {
        if (controlsWithMouse)
        {
            #if UNITY_EDITOR
                        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1) return;
            #else
                if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) return;
            #endif
            Vector3 mousePos = _currCamera.ScreenToWorldPoint(Input.mousePosition);
            player.GetComponent<Rigidbody2D>().position = mousePos;
        }
        else
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            Vector3 direction = input.normalized;
            _velocity = direction * playerSpeed;
            player.GetComponent<Rigidbody2D>().position += (Vector2)_velocity * Time.deltaTime;
        }
    }


    //====== DEBUGGIGN :) +++++++

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (spawnZones != null)
        {
            foreach (Rect r in spawnZones)
            {
                Gizmos.DrawWireCube((Vector3) r.center, (Vector3) new Vector2(r.width, r.height));
            }
        }
        
    }
}
