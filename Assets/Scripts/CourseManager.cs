using UnityEngine;
using UnityEngine.SceneManagement;

// Runs the obstacle course: checkpoints, falling/respawn, timer, finish line and the on-screen text.
public class CourseManager : MonoBehaviour
{
    public float killHeight = -8f;

    Transform dino;
    DinoController ctrl;
    Transform[] checkpoints = new Transform[0];
    Transform finish;

    Vector3 startPos;
    Vector3 respawnPos;
    int reached;
    int falls;
    float timer;
    bool started;
    bool won;
    float finalTime;

    GUIStyle small, big, center;

    void Start()
    {
        var d = GameObject.Find("Dino");
        if (d == null) { enabled = false; return; }
        dino = d.transform;
        ctrl = d.GetComponent<DinoController>();
        startPos = dino.position;
        respawnPos = startPos;

        var cpRoot = GameObject.Find("Checkpoints");
        if (cpRoot != null)
        {
            checkpoints = new Transform[cpRoot.transform.childCount];
            for (int i = 0; i < checkpoints.Length; i++) checkpoints[i] = cpRoot.transform.GetChild(i);
        }
        var f = GameObject.Find("Finish");
        if (f != null) finish = f.transform;
    }

    static bool Inside(Vector3 p, Transform zone)
    {
        Vector3 d = p - zone.position;
        Vector3 s = zone.lossyScale;
        return Mathf.Abs(d.x) <= s.x * 0.5f && Mathf.Abs(d.y) <= s.y * 0.5f && Mathf.Abs(d.z) <= s.z * 0.5f;
    }

    void Update()
    {
        if (dino == null) return;

        if (KeyInput.MenuPressed)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (won)
        {
            if (KeyInput.ConfirmPressed || KeyInput.RestartPressed) Restart();
            return;
        }

        if (!started && (KeyInput.Up || KeyInput.Down || KeyInput.Left || KeyInput.Right || KeyInput.JumpPressed))
            started = true;
        if (started) timer += Time.deltaTime;

        if (KeyInput.RestartPressed) Respawn();

        if (dino.position.y < killHeight)
        {
            falls++;
            Respawn();
        }

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (Inside(dino.position, checkpoints[i]))
            {
                Transform c = checkpoints[i];
                respawnPos = new Vector3(c.position.x, c.position.y - c.lossyScale.y * 0.5f + 0.1f, c.position.z);
                if (i + 1 > reached) reached = i + 1;
            }
        }

        if (finish != null && Inside(dino.position, finish))
        {
            won = true;
            finalTime = timer;
            GameResult.Time = finalTime;
            GameResult.Falls = falls;
            SceneManager.LoadScene("Ending");
        }
    }

    void Respawn()
    {
        ctrl.Teleport(respawnPos, Quaternion.identity);
    }

    void Restart()
    {
        respawnPos = startPos;
        reached = 0;
        falls = 0;
        timer = 0f;
        started = false;
        won = false;
        ctrl.Teleport(startPos, Quaternion.identity);
    }

    void MakeStyles()
    {
        if (small != null) return;
        int baseSize = Mathf.Max(14, Screen.height / 34);
        small = new GUIStyle(GUI.skin.label) { fontSize = baseSize, fontStyle = FontStyle.Bold };
        small.normal.textColor = Color.white;
        big = new GUIStyle(GUI.skin.label) { fontSize = baseSize * 3, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        big.normal.textColor = new Color(1f, 0.9f, 0.3f);
        center = new GUIStyle(GUI.skin.label) { fontSize = baseSize * 3 / 2, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        center.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        MakeStyles();
        float t = won ? finalTime : timer;
        int total = checkpoints.Length;

        GUI.Label(new Rect(16, 12, 600, 40), "Time: " + t.ToString("0.0") + "s", small);
        GUI.Label(new Rect(16, 12 + small.fontSize + 6, 600, 40), "Checkpoint: " + reached + " / " + total + "     Falls: " + falls, small);
        GUI.Label(new Rect(16, Screen.height - small.fontSize - 20, Screen.width, 40),
            "Arrows: walk / turn    Space: jump    R: back to checkpoint    Esc: main menu", small);

        if (won)
        {
            GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, Screen.height * 0.15f), "YOU WIN!", big);
            GUI.Label(new Rect(0, Screen.height * 0.45f, Screen.width, Screen.height * 0.1f),
                "Time " + finalTime.ToString("0.0") + "s  -  Falls " + falls + "     (Enter to play again)", center);
        }
    }
}
