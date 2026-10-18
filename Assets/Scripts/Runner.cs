using UnityEngine;

public sealed class Runner : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D shape;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[4];
    private float axis;
    private float coyote;
    private float jumpBuffer;
    private bool cutJump;
    private const float Speed = 6;
    private const float JumpSpeed = 11;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        shape = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        axis = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
             - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) jumpBuffer = 0.12f;
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) cutJump = true;
        if (axis != 0)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(axis);
            transform.localScale = scale;
        }
    }

    private void FixedUpdate()
    {
        var filter = new ContactFilter2D();
        filter.SetLayerMask(1 << 8);
        filter.useTriggers = false;
        bool grounded = false;
        int count = shape.Cast(Vector2.down, filter, hits, 0.08f);
        for (int i = 0; i < count; i++) grounded |= hits[i].normal.y > 0.6f;
        coyote = grounded && body.linearVelocity.y <= 0.1f ? 0.10f : coyote - Time.fixedDeltaTime;
        var velocity = body.linearVelocity;
        velocity.x = axis * Speed;
        if (jumpBuffer > 0 && coyote > 0)
        {
            velocity.y = JumpSpeed;
            jumpBuffer = 0;
            coyote = 0;
        }
        if (cutJump && velocity.y > 0) velocity.y *= 0.5f;
        cutJump = false;
        jumpBuffer -= Time.fixedDeltaTime;
        body.linearVelocity = velocity;
    }

    public void ResetJump() { coyote = 0; jumpBuffer = 0; cutJump = false; }
}
