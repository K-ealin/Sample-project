using UnityEngine;
using UnityEngine.SceneManagement;

// Remembers how the last run went so the ending screen can show it.
public static class GameResult
{
    public static float Time = -1f;
    public static int Falls = 0;
}

// Ending screen: "YOU WIN!", the run stats, and buttons to play again or go back to the menu.
public class EndingScreen : MonoBehaviour
{
    public string gameScene = "SampleScene";
    public string menuScene = "MainMenu";

    GUIStyle titleStyle, shadowStyle, subStyle, buttonStyle, hintStyle;
    Texture2D btnGreen, btnGreenHover, btnGreenActive, btnBlue, btnBlueHover, btnBlueActive;
    int builtForHeight = -1;

    void Start()
    {
        UnityEngine.Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (KeyInput.ConfirmPressed || KeyInput.RestartPressed) PlayAgain();
        if (KeyInput.MenuPressed) GoToMenu();
    }

    void PlayAgain() { SceneManager.LoadScene(gameScene); }
    void GoToMenu() { SceneManager.LoadScene(menuScene); }

    static Texture2D Solid(Color c)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        t.hideFlags = HideFlags.HideAndDontSave;
        return t;
    }

    GUIStyle MakeButton(int fontSize, Texture2D n, Texture2D h, Texture2D a)
    {
        var s = new GUIStyle(GUI.skin.button) { fontSize = fontSize, fontStyle = FontStyle.Bold };
        s.normal.background = n; s.hover.background = h; s.active.background = a;
        s.normal.textColor = Color.white; s.hover.textColor = Color.white; s.active.textColor = Color.white;
        return s;
    }

    void BuildStyles()
    {
        if (builtForHeight == Screen.height && titleStyle != null) return;
        builtForHeight = Screen.height;
        int unit = Mathf.Max(12, Screen.height / 30);

        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = unit * 3, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = new Color(1f, 0.92f, 0.3f);
        shadowStyle = new GUIStyle(titleStyle);
        shadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.8f);
        subStyle = new GUIStyle(GUI.skin.label) { fontSize = unit * 3 / 2, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        subStyle.normal.textColor = Color.white;
        hintStyle = new GUIStyle(GUI.skin.label) { fontSize = unit, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        hintStyle.normal.textColor = Color.white;

        if (btnGreen == null)
        {
            btnGreen = Solid(new Color(0.18f, 0.65f, 0.25f));
            btnGreenHover = Solid(new Color(0.28f, 0.80f, 0.35f));
            btnGreenActive = Solid(new Color(0.12f, 0.50f, 0.18f));
            btnBlue = Solid(new Color(0.20f, 0.45f, 0.85f));
            btnBlueHover = Solid(new Color(0.30f, 0.58f, 0.95f));
            btnBlueActive = Solid(new Color(0.14f, 0.34f, 0.68f));
        }
        buttonStyle = MakeButton(unit * 3 / 2, btnGreen, btnGreenHover, btnGreenActive);
    }

    void OnGUI()
    {
        BuildStyles();
        float w = Screen.width, h = Screen.height;

        // Title with a little pulse
        float pulse = 1f + Mathf.Sin(UnityEngine.Time.time * 4f) * 0.04f;
        int oldSize = titleStyle.fontSize;
        titleStyle.fontSize = Mathf.RoundToInt(oldSize * pulse);
        shadowStyle.fontSize = titleStyle.fontSize;
        GUI.Label(new Rect(3, h * 0.04f + 3, w, h * 0.16f), "YOU WIN!", shadowStyle);
        GUI.Label(new Rect(0, h * 0.04f, w, h * 0.16f), "YOU WIN!", titleStyle);
        titleStyle.fontSize = oldSize;

        string stats = GameResult.Time >= 0f
            ? "Time " + GameResult.Time.ToString("0.0") + "s     Falls " + GameResult.Falls
            : "Nice run!";
        GUI.Label(new Rect(2, h * 0.19f + 2, w, h * 0.08f), stats, new GUIStyle(subStyle) { normal = { textColor = new Color(0f, 0f, 0f, 0.8f) } });
        GUI.Label(new Rect(0, h * 0.19f, w, h * 0.08f), stats, subStyle);

        // Buttons
        float bw = Mathf.Max(220f, w * 0.20f);
        float bh = Mathf.Max(56f, h * 0.10f);
        float gap = w * 0.03f;
        float y = h * 0.82f;
        var playRect = new Rect(w * 0.5f - bw - gap * 0.5f, y, bw, bh);
        var menuRect = new Rect(w * 0.5f + gap * 0.5f, y, bw, bh);
        if (GUI.Button(playRect, "PLAY AGAIN", buttonStyle)) PlayAgain();
        var menuStyle = MakeButton(buttonStyle.fontSize, btnBlue, btnBlueHover, btnBlueActive);
        if (GUI.Button(menuRect, "MAIN MENU", menuStyle)) GoToMenu();

        GUI.Label(new Rect(0, y + bh + 6, w, h * 0.06f), "Enter / R: play again     Esc: main menu", hintStyle);
    }
}
