using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChaserBrain : Brain
{
    [Header("Behaviors")] public float attackDistance;

    private GameManager _gm;
    private GameObject _playerGo;
    private Rigidbody2D _rb;
    private Queen _queen;

    public float innerRange;
    
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameManager.Instance;
        _playerGo = _gm.player;
        _rb = GetComponent<Rigidbody2D>();
        _queen = GetComponent<Queen>();
        attackDistance = Random.Range(1f, 8f);
        innerRange = Random.Range(1f, 3f);
    }

    public override void Move()
    {
        if (_playerGo != null)
        {
            Vector3 dir = new Vector3(_playerGo.transform.position.x - gameObject.transform.position.x,
                _playerGo.transform.position.y - gameObject.transform.position.y, 0).normalized;
            _queen.velocity = dir * _queen.moveSpeed;
            _rb.position += (Vector2)GetComponent<Queen>().velocity * Time.deltaTime;
        }
    }

    public override void Behave()
    {
        // if (DistanceFromPlayer() <= attackDistance)
        // {
        //     StartCoroutine(SwitchCommandAndWait(AntCommand.SUCK, Random.Range(1f, 3f)));
        //     StartCoroutine(SwitchCommandAndWait(AntCommand.REPULSE, Random.Range(3f, 10f)));
        // }
        // else
        // {
        //     StartCoroutine(SwitchCommandAndWait(AntCommand.SUCK, Random.Range(1f, 3f)));
        // }
        bool allAntsInsideInner = true;
        if (_queen != null)
        {
            foreach (var ant in _queen._ants)
            {
                if (ant.gameObject.activeInHierarchy)
                {
                    if (Distance(ant.gameObject.transform.position) > innerRange)
                    {
                        allAntsInsideInner = false;
                        break;
                    }
                }
            }
        }
        
        bool allAntsOutsideInner = true;
        if (_queen != null)
        {
            foreach (var ant in _queen._ants)
            {
                if (ant.gameObject.activeInHierarchy)
                {
                    if (Distance(ant.gameObject.transform.position) < innerRange)
                    {
                        allAntsOutsideInner = false;
                        break;
                    }
                }
            }
        }

        if (DistanceFromPlayer() <= attackDistance && allAntsInsideInner)
        {
            // StartCoroutine(SwitchCommandAndWait(AntCommand.SUCK, Random.Range(2f, 4f)));
            // StartCoroutine(SwitchCommandAndWait(AntCommand.REPULSE, Random.Range(5f, 10f)));
            if (_queen != null)
            {
                _queen.currentCommand = AntCommand.REPULSE;
            }
        }
        else if (DistanceFromPlayer() <= attackDistance && allAntsOutsideInner)
        {
            _queen.currentCommand = AntCommand.SUCK;
        } else if (DistanceFromPlayer() > attackDistance)
        {
            _queen.currentCommand = AntCommand.SUCK;
        }
    }

    IEnumerator SwitchCommandAndWait(AntCommand command, float waitTime)
    {
        _queen.currentCommand = command;
        // Debug.Log(_queen.currentCommand.ToString());
        yield return new WaitForSeconds(waitTime);
        if (_queen.currentCommand == AntCommand.REPULSE)
        {
            _queen.currentCommand = AntCommand.SUCK;
            // Debug.Log(_queen.currentCommand.ToString());
        } else if (_queen.currentCommand == AntCommand.SUCK)
        {
            _queen.currentCommand = AntCommand.ATTRACT;
            // Debug.Log(_queen.currentCommand.ToString());
        }
    }

    public override float DistanceFromPlayer()
    {
        if (_playerGo != null)
        {
            return new Vector2(_playerGo.transform.position.x - gameObject.transform.position.x,
                _playerGo.transform.position.y - gameObject.transform.position.y).magnitude;
        }
        else
        {
            return 0f;
        }
    }
    
    public float Distance(Vector2 pos)
    {
        return new Vector2(pos.x - gameObject.transform.position.x,
            pos.y - gameObject.transform.position.y).magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, attackDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, innerRange);
    }
}
