using UnityEngine;
using UnityEngine.SceneManagement;

// Main menu: a funny picture of the dino and cat as the background, a title and a START button.
public class MainMenu : MonoBehaviour
{
    public Texture2D background;
    public string gameScene = "SampleScene";

    GUIStyle titleStyle, shadowStyle, buttonStyle, hintStyle;
    Texture2D btnNormal, btnHover, btnActive, shade;
    int builtForHeight = -1;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (KeyInput.ConfirmPressed || KeyInput.JumpPressed) StartGame();
    }

    void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    static Texture2D Solid(Color c)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        t.hideFlags = HideFlags.HideAndDontSave;
        return t;
    }

    void BuildStyles()
    {
        if (builtForHeight == Screen.height && titleStyle != null) return;
        builtForHeight = Screen.height;
        int unit = Mathf.Max(12, Screen.height / 30);

        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = unit * 3, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = new Color(1f, 0.92f, 0.3f);
        shadowStyle = new GUIStyle(titleStyle);
        shadowStyle.normal.textColor = new Color(0.1f, 0.15f, 0.1f, 0.9f);

        if (btnNormal == null)
        {
            btnNormal = Solid(new Color(0.18f, 0.65f, 0.25f));
            btnHover = Solid(new Color(0.28f, 0.80f, 0.35f));
            btnActive = Solid(new Color(0.12f, 0.50f, 0.18f));
            shade = Solid(new Color(0f, 0f, 0f, 0.5f));
        }
        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = unit * 2, fontStyle = FontStyle.Bold };
        buttonStyle.normal.background = btnNormal;
        buttonStyle.hover.background = btnHover;
        buttonStyle.active.background = btnActive;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = Color.white;

        hintStyle = new GUIStyle(GUI.skin.label) { fontSize = unit, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        hintStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        BuildStyles();
        float w = Screen.width, h = Screen.height;

        if (background != null)
            GUI.DrawTexture(new Rect(0, 0, w, h), background, ScaleMode.ScaleAndCrop);

        // Dark band behind the title so the text is easy to read on any picture
        GUI.DrawTexture(new Rect(0, 0, w, h * 0.30f), shade);

        // Title
        var titleRect = new Rect(0, h * 0.05f, w, h * 0.16f);
        var shadowRect = new Rect(3, h * 0.05f + 3, w, h * 0.16f);
        GUI.Label(shadowRect, "DINO & CAT", shadowStyle);
        GUI.Label(titleRect, "DINO & CAT", titleStyle);
        var subRect = new Rect(0, h * 0.20f, w, h * 0.07f);
        GUI.Label(new Rect(2, h * 0.20f + 2, w, h * 0.07f), "The Obstacle Run", hintStyle);
        GUI.Label(subRect, "The Obstacle Run", hintStyle);

        // Start button
        float bw = Mathf.Max(220f, w * 0.22f);
        float bh = Mathf.Max(60f, h * 0.11f);
        var btnRect = new Rect((w - bw) * 0.5f, h * 0.80f, bw, bh);
        if (GUI.Button(btnRect, "START", buttonStyle)) StartGame();

        GUI.Label(new Rect(0, h * 0.80f + bh + 6, w, h * 0.06f), "Arrows: move    Space: jump    (Enter also starts)", hintStyle);
    }
}
