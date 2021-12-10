using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ant : MonoBehaviour
{

    [Header("Health")] 
    [Tooltip("If this ant is hit by another ant traveling x faster than it is, it will die.")]
    public float speedKillThreshold;

    Collider2D _collider;
    public SpriteRenderer spriteRenderer;

    public GameObject _queenGO;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attract(float force)
    {
        var queenPos = _queenGO.GetComponent<Transform>().position;
        var thisPos = transform.position;
        var dir = new Vector2(queenPos.x - thisPos.x, queenPos.y - thisPos.y).normalized;
        GetComponent<Rigidbody2D>().AddForce(dir * force * Time.deltaTime);

    }

    public void Repulse(float force)
    {
        var queenPos = _queenGO.GetComponent<Transform>().position;
        var thisPos = transform.position;
        var dir = new Vector2(thisPos.x - queenPos.x, thisPos.y - queenPos.y).normalized;
        GetComponent<Rigidbody2D>().AddForce(dir * force * Time.deltaTime);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag(gameObject.tag)) return;
        if (collision.collider.GetComponent<Ant>() != null)
        {
            if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > (collision.rigidbody.velocity.magnitude * (collision.gameObject.GetComponent<Ant>().speedKillThreshold)))
            {
                Debug.Log("Killed a(n) " + collision.gameObject.tag + " ant.");
                collision.gameObject.SetActive(false);
            }
        } 
        else if (collision.collider.gameObject.GetComponent<Queen>() != null)
        {
            if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > collision.rigidbody.velocity.magnitude * collision.gameObject.GetComponent<Queen>().queenSpeedKillThreshold)
            {
                Debug.Log("Killed a(n) " + collision.gameObject.tag + " queen.");
                collision.gameObject.GetComponent<Queen>().DetachAnts();
                // collision.gameObject.SetActive(false);
            }
        }

    }
}
