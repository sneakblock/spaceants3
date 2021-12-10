using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBrain : Brain
{
    [Header("Behaviors")] public float attackDistance;

    private GameManager _gm;
    private GameObject _playerGo;
    private Rigidbody2D _rb;
    private Queen _queen;
    
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameManager.Instance;
        _playerGo = _gm.player;
        _rb = GetComponent<Rigidbody2D>();
        _queen = GetComponent<Queen>();
        attackDistance = Random.Range(5f, 8f);
    }

    public override void Move()
    {
        Vector3 destPoint =
            Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width),
                Random.Range(0, Screen.height), 0));
        Vector3 dir = new Vector3(destPoint.x - transform.position.x, destPoint.y - transform.position.y, 0).normalized;
        _queen.velocity = dir * _queen.moveSpeed;
        _rb.position += (Vector2)GetComponent<Queen>().velocity * Time.deltaTime;
    }

    public override void Behave()
    {
        if (DistanceFromPlayer() <= attackDistance)
        {
            StartCoroutine(SwitchCommandAndWait(AntCommand.SUCK, Random.Range(2f, 4f)));
            StartCoroutine(SwitchCommandAndWait(AntCommand.REPULSE, Random.Range(5f, 10f)));
        }
        else
        {
            StartCoroutine(SwitchCommandAndWait(AntCommand.SUCK, Random.Range(1f, 3f)));
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
        return new Vector2(_playerGo.transform.position.x - gameObject.transform.position.x,
            _playerGo.transform.position.y - gameObject.transform.position.y).magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, attackDistance);
    }
}
