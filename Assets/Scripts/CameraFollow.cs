using UnityEngine;

// Third-person camera that follows behind the dinosaur and turns with it.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 12f;
    public float height = 6f;
    public float lookAhead = 3f;
    public float lookHeight = 2f;
    public float positionSharpness = 6f;
    public float turnSharpness = 3f;

    float yaw;

    void Start()
    {
        if (target == null)
        {
            var go = GameObject.Find("Dino");
            if (go != null) target = go.transform;
        }
        if (target != null)
        {
            yaw = target.eulerAngles.y;
            Snap();
        }
    }

    void Snap()
    {
        Quaternion r = Quaternion.Euler(0f, yaw, 0f);
        transform.position = target.position + r * new Vector3(0f, height, -distance);
        transform.rotation = Quaternion.LookRotation(LookPoint(r) - transform.position);
    }

    Vector3 LookPoint(Quaternion r)
    {
        return target.position + Vector3.up * lookHeight + r * Vector3.forward * lookAhead;
    }

    void LateUpdate()
    {
        if (target == null) return;
        float dt = Time.deltaTime;

        yaw = Mathf.LerpAngle(yaw, target.eulerAngles.y, 1f - Mathf.Exp(-turnSharpness * dt));
        Quaternion r = Quaternion.Euler(0f, yaw, 0f);
        Vector3 desired = target.position + r * new Vector3(0f, height, -distance);

        // If the dinosaur was teleported (respawn), jump straight to it.
        if ((desired - transform.position).sqrMagnitude > 400f)
        {
            Snap();
            return;
        }

        transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-positionSharpness * dt));
        Quaternion look = Quaternion.LookRotation(LookPoint(r) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 1f - Mathf.Exp(-8f * dt));
    }
}
