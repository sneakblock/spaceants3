using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Queen : MonoBehaviour
{
    GameManager _gm;

    Rigidbody2D _rigidbody;
    Collider2D _collider;
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    GameObject _antGO;

    public int maxNumAnts;
    public int numActiveAnts;

    [SerializeField]
    public List<Ant> _ants;

    [SerializeField]
    float _antSpawnRadius;

    public AntCommand currentCommand;
    public QueenType queenType;
    
    [Header("Queen")]
    [Tooltip("If the speed of the colliding object is this much faster than this object's speed, destroy this object.")]
    public float queenSpeedKillThreshold;

    [Header("Ants")]
    public float antMass;
    public float antDrag;
    public float antAngularDrag;
    public float attractForce;
    public float repulseForce;
    public float suckMultipler;
    [Tooltip("If the speed of the colliding object is this much faster than this object's speed, destroy this object.")]
    public float antSpeedKillThreshold;

    // ======== MOVEMENT ========
    public Brain brain;
    public Vector3 velocity;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameManager.Instance;

        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        initAnts();
        EnableAnts();
        assignColor();

    }

    public void EnableAnts()
    {
        for (int i = 0; i < numActiveAnts; i++)
        {
            enableAnt();
        }
    }

    [ContextMenu("Initialize Ants")]
    // Pools all possible ants, disables them.
    void initAnts()
    {
        for (int i = 0; i < maxNumAnts; i++)
        {
            Ant a = GameObject.Instantiate(_antGO).GetComponent<Ant>();
            a.gameObject.tag = gameObject.tag;
            a.gameObject.layer = gameObject.layer;
            a._queenGO = this.gameObject;
            _ants.Add(a);
            a.transform.parent = transform;
            a.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Enable Ant")]
    // Enables one ant, and gives it a random position inside the spawn range.
    void enableAnt()
    {
        foreach (Ant a in _ants)
        {
            if (!a.gameObject.activeInHierarchy)
            {
                a.gameObject.transform.position = ((Vector2)transform.position + Random.insideUnitCircle) * _antSpawnRadius;
                Color col = a.GetComponent<SpriteRenderer>().color;
                col.a = 1f;
                a.GetComponent<SpriteRenderer>().color = col;
                a.GetComponent<Collider2D>().enabled = true;
                a.gameObject.transform.parent = transform;
                a.GetComponent<Rigidbody2D>().mass = antMass;
                a.GetComponent<Rigidbody2D>().drag = antDrag;
                a.GetComponent<Rigidbody2D>().angularDrag = antAngularDrag;
                a.speedKillThreshold = antSpeedKillThreshold;
                a.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void assignColor()
    {
        if (gameObject.tag == "Player")
        {
            _spriteRenderer.color = _gm.playerCol;
        } else if (gameObject.tag == "Enemy")
        {
            switch (gameObject.name)
            {
                case "Chaser":
                    GetComponent<SpriteRenderer>().color = _gm.chaserCol;
                    break;
                case "Turtle":
                    GetComponent<SpriteRenderer>().color = _gm.turtleCol;
                    break;
                case "Sniper":
                    GetComponent<SpriteRenderer>().color = _gm.sniperCol;
                    break;
            }
        }

        foreach (Ant a in _ants)
        {
            a.gameObject.GetComponent<SpriteRenderer>().color = _spriteRenderer.color;
        }
    }

    public void SetColor(Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
        foreach (Ant a in _ants)
        {
            a.gameObject.GetComponent<SpriteRenderer>().color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Ant a in _ants)
        {
            if (a.gameObject.activeInHierarchy)
            {
                if (currentCommand == AntCommand.ATTRACT)
                {
                    a.Attract(attractForce);
                }
                else if (currentCommand == AntCommand.REPULSE)
                {
                    a.Repulse(repulseForce);
                } else if (currentCommand == AntCommand.SUCK)
                {
                    a.Attract(attractForce * suckMultipler);
                }
            }
        }

    }

    public void SetAttractForce(float force)
    {
        attractForce = force;
    }

    public void SetRepulseForce(float force)
    {
        repulseForce = force;
    }

    public void SetAntMass(float mass)
    {
        foreach (Ant a in _ants)
        {
            a.GetComponent<Rigidbody2D>().mass = mass;
        }
    }
    
    public void SetAntDrag(float drag)
    {
        foreach (Ant a in _ants)
        {
            a.GetComponent<Rigidbody2D>().drag = drag;
        }
    }
    
    public void SetAntAngularDrag(float angularDrag)
    {
        foreach (Ant a in _ants)
        {
            a.GetComponent<Rigidbody2D>().angularDrag = angularDrag;
        }
    }

    public void ScaleAntSize(float scaleFactor)
    {
        foreach (Ant a in _ants)
        {
            a.GetComponent<Transform>().localScale *= scaleFactor;
        }
    }

    public void SetQueenSpeedKillThreshold(float threshold)
    {
        queenSpeedKillThreshold = threshold;
    }
    
    public void SetAntSpeedKillThreshold(float threshold)
    {
        foreach (Ant a in _ants)
        {
            a.speedKillThreshold = threshold;
        }
    }

    public void Move(Vector2 moveToPoint)
    {
        Vector3 direction = new Vector3(moveToPoint.x - transform.position.x, moveToPoint.y - transform.position.y).normalized;
        velocity = direction * moveSpeed;
        _rigidbody.position += (Vector2)velocity * Time.deltaTime;
    }

    public void DetachAnts()
    {
        Debug.Log("Detaching ants.");
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        foreach (Ant a in _ants)
        {
            if (a.gameObject.activeInHierarchy)
            {
                a.GetComponent<Collider2D>().enabled = false;
                a.gameObject.transform.parent = null;
                StartCoroutine(FadeAnt(a.GetComponent<SpriteRenderer>(), 2f));
            }
        }
    }

    IEnumerator FadeAnt(SpriteRenderer sr, float aTime)
    {
        float alpha = sr.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(alpha, 0f,t));
            sr.color = newColor;
            yield return null;
        }
        foreach (var a in _ants)
        {
            a.gameObject.transform.parent = gameObject.transform;
            a.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        _gm.numCurrEnemies--;
    }
}
