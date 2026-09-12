using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour, ITickable
{
    [SerializeField] GameManager gameManager;
    [SerializeField] StatusPlayer statusPlayer;

    [SerializeField] public float moveSpeed = 2.5f;
    [SerializeField] float backSpeed = 1f;
    [SerializeField] float leftrightSpeed = 2f;
    [SerializeField] float aimSpeed = 10f;
    [SerializeField] float lookSpeed = 10f;

    public float deltaY;
    public Vector2 delta;
    public bool isCameraMove;
    public Vector2 zoom;

    public Quaternion deltaAngleX;

    [SerializeField] Vector2 move;
    public Vector2 Move => move;

    public bool isFireButtonHold;

    Player player;
    GroundMoter groundMoter;
    Anim anim;


    public void Initialize(Player player, GroundMoter groundMoter, Anim anim)
    {
        this.player = player;
        this.groundMoter = groundMoter;
        this.anim = anim;
    }
    void OnEnable()
    {
        anim = GetComponent<Anim>();
    }

    public void Tick()
    {
        if (groundMoter.gravityDamageInfo.DamageType == DamageType.None)
        {
            float forwardSpeed = moveSpeed;
            if (move.y < 0) forwardSpeed = backSpeed;
            Vector3 deltaMove = transform.right * move.x * leftrightSpeed +
                transform.forward * move.y * forwardSpeed;
            if (anim.IsAimState) deltaMove *= 0.75f;            
            groundMoter.Move = groundMoter.Move + deltaMove;
        }

        if (!isCameraMove)
        {
            deltaAngleX = Quaternion.AngleAxis(delta.x * lookSpeed * Time.deltaTime, Vector3.up);
            transform.rotation *= deltaAngleX;
        }
    }
    public void LateTick() { }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started && groundMoter.gravityDamageInfo.DamageType == DamageType.None)
        {
            bool tmpBool = anim.Animator.GetBool("isAiming");
            tmpBool = !tmpBool;
            anim.Animator.SetBool("isAiming", tmpBool);            
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        delta = context.ReadValue<Vector2>();

        float dx = delta.x;
        float dy = delta.y;

        // デッドゾーン
        if (Mathf.Abs(dx) < 0.01f) delta.x = 0f;
        if (Mathf.Abs(dy) < 0.01f) delta.y = 0f;

        if (!isCameraMove)
        {
            deltaY -= delta.y * aimSpeed * Time.deltaTime;
            deltaY = Mathf.Clamp(deltaY, -75f, 75f);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFireButtonHold = true;
            gameManager.InputSystemManager.SetState(GameState.GamePlay);
        }
        if (context.performed) isFireButtonHold = true;
        if (context.canceled) isFireButtonHold = false;
    }
    public void OnCamera(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCameraMove = true;
            gameManager.InputSystemManager.SetState(GameState.GamePlay);
        }
        if (context.canceled) isCameraMove = false;
    }
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            gameManager.InputSystemManager.SetState(GameState.Pause);
            statusPlayer.RaisePause();
        }
    }
    public void OnZoom(InputAction.CallbackContext context)
    {
        zoom = context.ReadValue<Vector2>();
    }
}
