using UnityEngine;

// Procedural walk / jump animation for the dinosaur.
// It swings the leg, arm and tail pivots and bobs the whole model while walking.
public class DinoAnimator : MonoBehaviour
{
    DinoController ctrl;
    Transform legL, legR, armL, armR, tail, model;
    Vector3 modelBase;
    float phase;
    float legLAngle, legRAngle, armLAngle, armRAngle, tailYaw;

    static Transform FindDeep(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    void Start()
    {
        ctrl = GetComponent<DinoController>();
        legL = FindDeep(transform, "LegL_Pivot");
        legR = FindDeep(transform, "LegR_Pivot");
        armL = FindDeep(transform, "ArmL_Pivot");
        armR = FindDeep(transform, "ArmR_Pivot");
        tail = FindDeep(transform, "Tail_Pivot");
        model = FindDeep(transform, "Model");
        if (model != null) modelBase = model.localPosition;
    }

    void Update()
    {
        if (ctrl == null) return;
        float dt = Time.deltaTime;
        float speed = ctrl.HorizontalSpeed;
        float norm = Mathf.Clamp01(speed / ctrl.moveSpeed);
        phase += speed * dt * 1.1f;

        float swing = Mathf.Sin(phase) * 32f * norm;
        float targetLegL, targetLegR, targetArmL, targetArmR;

        if (!ctrl.Grounded)
        {
            // In the air: legs tuck forward, arms lift.
            targetLegL = -30f; targetLegR = -22f;
            targetArmL = -35f; targetArmR = -35f;
        }
        else
        {
            targetLegL = swing;
            targetLegR = -swing;
            targetArmL = -swing * 0.6f;
            targetArmR = swing * 0.6f;
        }

        float k = 1f - Mathf.Exp(-18f * dt);
        legLAngle = Mathf.Lerp(legLAngle, targetLegL, k);
        legRAngle = Mathf.Lerp(legRAngle, targetLegR, k);
        armLAngle = Mathf.Lerp(armLAngle, targetArmL, k);
        armRAngle = Mathf.Lerp(armRAngle, targetArmR, k);

        if (legL != null) legL.localRotation = Quaternion.Euler(legLAngle, 0f, 0f);
        if (legR != null) legR.localRotation = Quaternion.Euler(legRAngle, 0f, 0f);
        if (armL != null) armL.localRotation = Quaternion.Euler(armLAngle, 0f, 0f);
        if (armR != null) armR.localRotation = Quaternion.Euler(armRAngle, 0f, 0f);

        // Tail sways when walking and gently when idle.
        float targetTail = Mathf.Sin(phase) * 14f * norm + Mathf.Sin(Time.time * 1.6f) * 4f;
        tailYaw = Mathf.Lerp(tailYaw, targetTail, k);
        if (tail != null) tail.localRotation = Quaternion.Euler(0f, tailYaw, 0f);

        // Body bob.
        if (model != null)
        {
            float bob = ctrl.Grounded ? Mathf.Abs(Mathf.Sin(phase)) * 0.12f * norm : 0f;
            Vector3 target = modelBase + Vector3.up * bob;
            model.localPosition = Vector3.Lerp(model.localPosition, target, k);
        }
    }
}
