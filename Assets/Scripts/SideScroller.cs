using UnityEngine;

// The scene owns the bootstrap; all prototype art is generated locally at runtime.
public sealed class SideScroller : MonoBehaviour
{
    private Sprite pixel;
    private Runner player;
    private Camera view;
    private bool finished;
    private readonly Vector2 spawn = new Vector2(-7f, 0.5f);

    private void Start()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        pixel = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1);
        view = new GameObject("Side camera").AddComponent<Camera>();
        view.tag = "MainCamera";
        view.orthographic = true;
        view.orthographicSize = 5.5f;
        view.backgroundColor = new Color(0.10f, 0.16f, 0.25f);
        view.clearFlags = CameraClearFlags.SolidColor;
        view.transform.position = new Vector3(spawn.x + 3, 2, -10);

        Platform(new Vector2(0, -1.5f), new Vector2(22, 1));
        Platform(new Vector2(15, -1.5f), new Vector2(4, 1));
        Platform(new Vector2(23, -1.5f), new Vector2(6, 1));
        Platform(new Vector2(-2, 0.5f), new Vector2(3, 0.45f));
        Platform(new Vector2(3, 2), new Vector2(3, 0.45f));
        Platform(new Vector2(8, 1), new Vector2(3, 0.45f));
        Platform(new Vector2(18.5f, 0.5f), new Vector2(2, 0.45f));
        for (int i = 0; i < 10; i++)
            Block("Background", new Vector2(-10 + i * 5, 1), new Vector2(2.5f, 7 + i % 3),
                new Color(0.13f, 0.21f, 0.30f), -5);

        var hero = Block("Player", spawn, new Vector2(0.75f, 1.2f), new Color(1, 0.73f, 0.25f), 2);
        hero.AddComponent<BoxCollider2D>();
        var body = hero.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.gravityScale = 3;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        var material = new PhysicsMaterial2D("Frictionless player") { friction = 0, bounciness = 0 };
        hero.GetComponent<BoxCollider2D>().sharedMaterial = material;
        player = hero.AddComponent<Runner>();
        // Child eyes move with the character and make the facing direction readable.
        var eyes = Block("Eyes", Vector2.zero, new Vector2(0.20f, 0.14f), new Color(0.12f, 0.16f, 0.22f), 3);
        eyes.transform.SetParent(hero.transform, false);
        eyes.transform.localPosition = new Vector3(0.23f, 0.2f, 0);
        eyes.transform.localScale = new Vector3(0.20f / 0.75f, 0.14f / 1.2f, 1);
        Block("Finish pole", new Vector2(24, 0.5f), new Vector2(0.12f, 3), Color.white, 1);
        Block("Finish flag", new Vector2(24.6f, 1.5f), new Vector2(1.1f, 0.65f), new Color(0.35f, 0.9f, 0.65f), 1);
    }

    private GameObject Block(string label, Vector2 position, Vector2 size, Color color, int order = 0)
    {
        var obj = new GameObject(label);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(size.x, size.y, 1);
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = pixel;
        renderer.color = color;
        renderer.sortingOrder = order;
        return obj;
    }

    private void Platform(Vector2 position, Vector2 size)
    {
        var platform = Block("Platform", position, size, new Color(0.24f, 0.64f, 0.55f));
        platform.layer = 8;
        platform.AddComponent<BoxCollider2D>();
        Block("Grass", position + Vector2.up * (size.y / 2), new Vector2(size.x, 0.10f), new Color(0.48f, 0.88f, 0.65f), 1);
    }

    private void Update()
    {
        if (player == null) return;
        if (player.transform.position.y < -7 || Input.GetKeyDown(KeyCode.R)) Restart();
        if (player.transform.position.x > 23 && player.transform.position.y < 2.5f) finished = true;
    }

    private void LateUpdate()
    {
        if (player == null) return;
        var target = new Vector3(Mathf.Clamp(player.transform.position.x + 3, -4, 22),
            Mathf.Max(2, player.transform.position.y + 1), -10);
        view.transform.position = Vector3.Lerp(view.transform.position, target, 1 - Mathf.Exp(-6 * Time.deltaTime));
    }

    private void Restart()
    {
        player.GetComponent<Rigidbody2D>().position = spawn;
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.ResetJump();
        finished = false;
        view.transform.position = new Vector3(spawn.x + 3, 2, -10);
    }

    private void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.box) { fontSize = Mathf.Clamp(Screen.width / 60, 16, 26), alignment = TextAnchor.MiddleCenter };
        GUI.Box(new Rect(15, 15, Screen.width - 30, 65), "KOSHEY — SIDE SCROLLER\nA / D or arrows: move    Space / W / Up: jump    R: restart", style);
        if (finished)
            GUI.Box(new Rect(Screen.width / 2f - 200, Screen.height / 2f - 45, 400, 90), "FINISH!\nPress R to play again", style);
    }
}

