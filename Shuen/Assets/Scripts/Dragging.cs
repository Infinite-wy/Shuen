using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    Vector2 mousePos;
    Vector2 distance;
    Rigidbody2D rb2D;
    void Start()
    {
        rb2D=GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        distance=new Vector2(transform.position.x,transform.position.y)-mousePos;
    }

    private void OnMouseDrag()
    {
        Vector3 targetPosition = mousePos + distance;
        rb2D.MovePosition(targetPosition); // 使用物理引擎的移动方法
        rb2D.gravityScale=0;
        rb2D.velocity=Vector2.zero;
    }

    private void OnMouseUpAsButton()
    {
        rb2D.gravityScale=20f;
    }
}
