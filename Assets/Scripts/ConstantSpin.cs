using UnityEngine;

public class ConstantSpin : MonoBehaviour

{
    public float speed = 30f;
    public Vector3 axis = Vector3.up;
    private void FixedUpdate()
    {
        transform.Rotate(axis, speed * Time.fixedDeltaTime);
    }
}
