using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Camera))]
[ExecuteInEditMode()]
public class SlowMotionCamera : MonoBehaviour 
{
    public Transform ViewTarget = null;
    public float distanceOffset = 1.5f;
    public float heightOffset = 1.35f;
    public float cameraActiveDepth = 1;
    public float SlowMotionTimeScale = 0.3f;
    public float SlowMotionDuration = 2.2f;

    private Camera _camera;

    void Awake()
    {
        _camera = this.GetComponent<Camera>();
        _camera.depth = -1;
        this.enabled = false;
    }

    void Update()
    {
         
    }

    void LateUpdate()
    {
    }

    IEnumerator CameraOrbitOnTarget(Transform target, float duration)
    {
        float randomAngle = 0;
        Vector3 dampingPositionBegin = Util.GetRandomPositionSurrondPoint(ViewTarget.position, distanceOffset, heightOffset, 0, 240, out randomAngle);
        Vector3 dampingPositionEnd = Util.GetRandomPositionSurrondPoint(ViewTarget.position, distanceOffset, heightOffset + Random.Range(-0.4f, 0.65f), randomAngle + 60, randomAngle + 90);
        Debug.DrawLine(dampingPositionBegin, dampingPositionEnd, Color.red, 2000);
        _camera.transform.position = dampingPositionBegin;
        SetupRotation(ViewTarget.position);
        yield return null;
        float _start = Time.time;
        while ((Time.time - _start) <= duration)
        {
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, dampingPositionEnd, Time.deltaTime);
            SetupRotation(ViewTarget.position);
            yield return new WaitForEndOfFrame();
        }
        this.enabled = false;
    }

    void OnEnable()
    {
        _camera.depth = cameraActiveDepth;
        Time.timeScale = SlowMotionTimeScale;
        Time.fixedDeltaTime = SlowMotionTimeScale;
        StartCoroutine(CameraOrbitOnTarget(ViewTarget, SlowMotionDuration));
    }

    void OnDisable()
    {
        _camera.depth = -1;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 1;
        StopAllCoroutines();
    }

    void ApplyRandomPositionAroundTarget(Vector3 targetPoint)
    {
        _camera.transform.position = Util.GetRandomPositionSurrondPoint(targetPoint, distanceOffset, heightOffset, 0, 360);
    }

    void SetupRotation(Vector3 tartgetPoint)
    {
        Vector3 cameraPos = _camera.transform.position;
        Vector3 offsetToCenter = tartgetPoint - cameraPos;
        offsetToCenter.y += heightOffset;
        _camera.transform.rotation = Quaternion.LookRotation(offsetToCenter);

    }
}

