using DG.Tweening;
using TMPro;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public bool movementEnabled = true;
    public Vector3 rotateAround;
    public bool currentlyRotating { get; private set; } = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            
    }
    // Update is called once per frame
    void Update()
    {
        if (!movementEnabled) return;
        if (Input.GetKeyDown(KeyCode.A)) RotateAroundWorldPivot(gameObject, rotateAround, Vector3.up, 45f);
        if (Input.GetKeyDown(KeyCode.D)) RotateAroundWorldPivot(gameObject, rotateAround, Vector3.up, -45f);
    }

    public void ResetRotation()
    {
        RotateAroundWorldPivot(gameObject, rotateAround, Vector3.up, -0f);
    }
    public void RotateAroundWorldPivot(GameObject obj, Vector3 pivot, Vector3 axis, float degrees, float duration = 0.3f)
    {
        if (currentlyRotating) return;
        currentlyRotating = true;
        Vector3 dir = obj.transform.position - pivot;

        Quaternion rotation = Quaternion.AngleAxis(degrees, axis);
        Vector3 targetDir = rotation * dir;
        Vector3 targetPos = pivot + targetDir;

        Quaternion targetRot = Quaternion.AngleAxis(degrees, axis) * obj.transform.rotation;

        obj.transform.DOMove(targetPos, duration).SetEase(Ease.InOutSine);
        
        var tween = obj.transform.DORotateQuaternion(targetRot, duration).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine)
        .OnUpdate(() =>
        {
            obj.transform.LookAt(pivot);
        })
        .OnComplete(() =>
        {
            obj.transform.LookAt(pivot);
            obj.transform.rotation.eulerAngles.Scale(new Vector3(1f, 1f, 0f));
            currentlyRotating = false;
        });
        
    }
}
