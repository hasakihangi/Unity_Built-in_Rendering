using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndSlowMove : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public float slowMoveRange = 0.2f;
    public float moveSpeed = 1f;

    private Vector3 originPosition;
    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotateSpeed, 0);
        float deltaMove = Mathf.Sin(Time.time * moveSpeed) * slowMoveRange;
        transform.localPosition = originPosition + new Vector3(0, 
            deltaMove, 0);
    }
}
