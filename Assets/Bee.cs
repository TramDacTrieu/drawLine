using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    Transform player;
    float speed = 2f;

    Rigidbody2D rb2d;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime);
        rb2d.velocity = (player.position - transform.position).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("Game Over!");
        }
    }
}
