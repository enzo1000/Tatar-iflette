using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int speed = 1;

    private float hori = 0.0f;
    private float vert = 0.0f;

    // Update is called once per frame
    void Update()
    {
        hori = Input.GetAxis("Horizontal") * speed;
        vert = Input.GetAxis("Vertical") * speed;
    }

    private void FixedUpdate()
    {
        gameObject.transform.Translate(Vector2.right * hori * Time.fixedDeltaTime);
        gameObject.transform.Translate(Vector2.up * vert * Time.fixedDeltaTime);
    }
}
