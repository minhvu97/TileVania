using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1.0f;

    Rigidbody2D myRigidbody;

    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = Random.value * 3 * moveSpeed;
        myRigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        // transform.localScale = =
    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.velocity = new Vector2(moveSpeed, 0.0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Platforms Tilemap")
        {
            transform.localScale = new Vector2(-transform.localScale.x, 1.0f);
            moveSpeed = -moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Player", "Bullets"))
        )
        {
            transform.localScale = new Vector2(-transform.localScale.x, 1.0f);
            moveSpeed = -moveSpeed;
        }
    }
}
