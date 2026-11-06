using UnityEngine;

// Falling confetti made from small colored squares.
public class ConfettiRain : MonoBehaviour
{
    public Material[] materials;
    public int count = 100;

    Transform[] pieces;
    float[] speed;
    Vector3[] spin;

    void Start()
    {
        pieces = new Transform[count];
        speed = new float[count];
        spin = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = "Confetti" + i;
            var col = g.GetComponent<Collider>();
            if (col != null) Destroy(col);
            g.transform.SetParent(transform, false);
            g.transform.localScale = new Vector3(0.22f, 0.14f, 0.02f);
            g.transform.position = RandomTop(true);
            g.transform.rotation = Random.rotation;
            if (materials != null && materials.Length > 0)
                g.GetComponent<Renderer>().sharedMaterial = materials[Random.Range(0, materials.Length)];
            pieces[i] = g.transform;
            speed[i] = Random.Range(1.2f, 3.0f);
            spin[i] = Random.insideUnitSphere * 220f;
        }
    }

    Vector3 RandomTop(bool anyHeight)
    {
        float y = anyHeight ? Random.Range(-1f, 13f) : 13f;
        return new Vector3(Random.Range(-9f, 9f), y, Random.Range(-3f, 6f));
    }

    void Update()
    {
        if (pieces == null) return;
        float dt = Time.deltaTime;
        for (int i = 0; i < pieces.Length; i++)
        {
            var p = pieces[i];
            p.position += Vector3.down * speed[i] * dt;
            p.Rotate(spin[i] * dt, Space.World);
            if (p.position.y < -1.5f) p.position = RandomTop(false);
        }
    }
}
