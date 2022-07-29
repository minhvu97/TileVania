using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D myRigidbody;

    float bulletSpeed = 10.0f;

    float spinSpeed = 10.0f;

    float initBulletSpeed = 10.0f;

    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.velocity = new Vector2(bulletSpeed, 0.0f);
        myRigidbody.angularVelocity = spinSpeed;
        if (Mathf.Abs(myRigidbody.velocity.x) < 0.1) Destroy(gameObject);
    }

    public void setBulletSpeed(float value)
    {
        bulletSpeed = value;
        initBulletSpeed = value;
    }

    public void setSpinSpeed(float value)
    {
        spinSpeed = value;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (
            myRigidbody
                .IsTouchingLayers(LayerMask
                    .GetMask("Player", "Bullets", "Ground", "Hazards"))
        )
        {
            bulletSpeed = -Random.value * 3 * initBulletSpeed;
            initBulletSpeed = -initBulletSpeed;
            count++;
        }

        if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            count++;
            Destroy (gameObject);
            Destroy(other.gameObject);
        }

        if (count > 2)
        {
            Destroy (gameObject);
        }
    }
}
