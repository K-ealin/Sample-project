using UnityEngine;

// Makes the dino, the cat on its back and the hat on its head dance together.
public class DinoDance : MonoBehaviour
{
    public float beat = 6f;          // dance speed (radians per second)
    public bool discoLights = true;

    Transform legL, legR, armL, armR, tailPivot, model, cat, hat, catTail, tongue;
    Vector3 modelBase, catBase, hatBase;
    Quaternion catTailBase = Quaternion.identity, tongueBase = Quaternion.identity;
    Light[] lights;
    float t;

    static Transform FindDeep(Transform root, string name)
    {
        foreach (Transform x in root.GetComponentsInChildren<Transform>(true))
            if (x.name == name) return x;
        return null;
    }

    void Start()
    {
        legL = FindDeep(transform, "LegL_Pivot");
        legR = FindDeep(transform, "LegR_Pivot");
        armL = FindDeep(transform, "ArmL_Pivot");
        armR = FindDeep(transform, "ArmR_Pivot");
        tailPivot = FindDeep(transform, "Tail_Pivot");
        model = FindDeep(transform, "Model");
        cat = FindDeep(transform, "Cat");
        hat = FindDeep(transform, "Hat");
        catTail = FindDeep(transform, "Cat_Tail");
        tongue = FindDeep(transform, "Dino_Tongue");

        if (model != null) modelBase = model.localPosition;
        if (cat != null) catBase = cat.localPosition;
        if (hat != null) hatBase = hat.localPosition;
        if (catTail != null) catTailBase = catTail.localRotation;
        if (tongue != null) tongueBase = tongue.localRotation;

        if (discoLights)
        {
            lights = new Light[3];
            for (int i = 0; i < lights.Length; i++)
            {
                var go = new GameObject("DiscoLight" + i);
                var l = go.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 18f;
                l.intensity = 4f;
                lights[i] = l;
            }
        }
    }

    void Update()
    {
        t += Time.deltaTime;
        float w = beat;
        float s1 = Mathf.Sin(w * t);
        float s2 = Mathf.Sin(2f * w * t);

        // The whole dino sways side to side, facing the audience.
        transform.rotation = Quaternion.Euler(0f, Mathf.Sin(t * 0.9f) * 35f, 0f);

        // Body bounce and a little roll.
        if (model != null)
        {
            model.localPosition = modelBase + Vector3.up * Mathf.Abs(s1) * 0.45f;
            model.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(w * 0.5f * t) * 7f);
        }

        // Legs kick in turn, arms wave up high, tail wags.
        if (legL != null) legL.localRotation = Quaternion.Euler(s1 * 40f, 0f, 0f);
        if (legR != null) legR.localRotation = Quaternion.Euler(-s1 * 40f, 0f, 0f);
        if (armL != null) armL.localRotation = Quaternion.Euler(-70f + s2 * 25f, 0f, 0f);
        if (armR != null) armR.localRotation = Quaternion.Euler(-70f - s2 * 25f, 0f, 0f);
        if (tailPivot != null) tailPivot.localRotation = Quaternion.Euler(0f, s2 * 30f, 0f);
        if (tongue != null) tongue.localRotation = tongueBase * Quaternion.Euler(s2 * 12f, 0f, 0f);

        // The cat hops on the beat (a little late), sways, wags its tail and does a spin every few seconds.
        if (cat != null)
        {
            cat.localPosition = catBase + Vector3.up * Mathf.Abs(Mathf.Sin(w * t - 0.9f)) * 0.55f;
            float spin = SpinCurve(Mathf.Repeat(t + 2f, 4f) / 4f);
            cat.localRotation = Quaternion.Euler(0f, spin, Mathf.Sin(w * t) * 10f);
        }
        if (catTail != null) catTail.localRotation = catTailBase * Quaternion.Euler(0f, 0f, s2 * 25f);

        // The hat pops up, tilts and spins on the dino's head.
        if (hat != null)
        {
            hat.localPosition = hatBase + Vector3.up * Mathf.Max(0f, Mathf.Sin(w * t - 1.6f)) * 0.45f;
            float spin = SpinCurve(Mathf.Repeat(t, 4f) / 4f);
            hat.localRotation = Quaternion.Euler(0f, spin, Mathf.Sin(w * t + 1f) * 14f);
        }

        // Colored disco lights circling the stage.
        if (lights != null)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                float a = t * 0.8f + i * 2.094f;
                lights[i].transform.position = new Vector3(Mathf.Cos(a) * 5f, 3.5f + Mathf.Sin(t + i) * 0.7f, 2.5f + Mathf.Sin(a) * 3f);
                lights[i].color = Color.HSVToRGB(Mathf.Repeat(t * 0.15f + i / 3f, 1f), 0.8f, 1f);
            }
        }
    }

    // 0..1 -> degrees: a full smooth spin during the first 40% of the cycle, then rest.
    static float SpinCurve(float p)
    {
        return p < 0.4f ? Mathf.SmoothStep(0f, 360f, p / 0.4f) : 0f;
    }
}
