using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;


public class CameraManager : MonoBehaviour
{
    [SerializeField] float cameraHeight = 1f;
    [SerializeField] float lookHeight = 0.5f;
    [SerializeField] float distance = 2f;
    [SerializeField] float distanceMax = 3f;
    [SerializeField] float distanceMin = 1.5f;
    [SerializeField] float cameraSpeed = 15f;
    [SerializeField] float minPitch = -75f;
    [SerializeField] float maxPitch = 75f;
    [SerializeField] float radius = 0.1f;
    [SerializeField] float zoomSpeed = -0.1f;
    float deltaX = 0f;
    float deltaY = 0f;

    GameObject playerGO;
    InputManager inputManager;
    StatusPlayer statusPlayer;
    Transform player;
    LayerMask groundLayer;

    CancellationToken destroyToken;
    bool isGameOver;
    Vector3 gameOverTargetPos = new Vector3(0f, 1f, 0f);
    Vector3 gameOverCurPos;
    bool isGameOverCurPosSetted;
    bool isMoveStop;


    void Awake()
    {
        playerGO = GameObject.FindWithTag("Player");
        inputManager = playerGO.GetComponent<InputManager>();
        statusPlayer = playerGO.GetComponent<StatusPlayer>();
        player = playerGO.transform;
        groundLayer = LayerMask.GetMask("Ground");
        distance = distanceMax;
    }
    void OnEnable()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        statusPlayer.OnGameOverDropOut += RefreshGameOver;
        isGameOver = false;
        isGameOverCurPosSetted = false;
        isMoveStop = false;
    }
    void OnDisable()
    {
        statusPlayer.OnGameOverDropOut -= RefreshGameOver;
    }

    void LateUpdate()
    {        
        if (inputManager.isCameraMove)
        {
            deltaX += inputManager.delta.x * cameraSpeed *
                GameSystem.Config.MouseSensitivityCamera * Time.deltaTime;
            deltaY -= inputManager.delta.y * cameraSpeed *
                GameSystem.Config.MouseSensitivityCamera * Time.deltaTime;
            deltaY = Mathf.Clamp(deltaY, minPitch, maxPitch);
        }
        Vector3 playerPos = player.position;
        Quaternion deltaRot = Quaternion.Euler(deltaY, deltaX, 0f);
        Vector3 offset = deltaRot * new Vector3(0f, 0f, -distance);

        if (!isGameOverCurPosSetted)
        {
            isGameOverCurPosSetted = true;
            gameOverCurPos = offset;
        }
        if (isGameOver)
        {
            gameOverCurPos = Vector3.Lerp(
                gameOverCurPos,
                gameOverTargetPos,
                2f * Time.deltaTime);
            offset = gameOverCurPos;
        }

        Vector3 pos = playerPos + offset + Vector3.up * cameraHeight;
        Vector3 originPos = playerPos + Vector3.up * lookHeight;
        Quaternion lookRot = Quaternion.LookRotation(
            originPos - pos
            ).normalized;
        transform.rotation = lookRot;

        Vector3 offsetDir = lookRot * Vector3.forward;

        var hitCheck = Physics.SphereCast(
            originPos, radius, -offsetDir, out var hit, distance - 0.5f, groundLayer);

        if (!isMoveStop)
        {
            if (playerPos.y < -30f && !isGameOver) RefreshGameOver();

            if (hitCheck)
            {
                transform.position = originPos - (hit.distance) * offsetDir;
            }
            else
            {
                transform.position = pos;
            }
        }
        else
        {
            
        }

        distance += inputManager.zoom.y * zoomSpeed;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
    }

    public void RefreshGameOver()
    {
        isGameOver = true;
        GameOverAsync(destroyToken).Forget(
            ex => Debug.LogError("CameraManagerのGameOverAsyncで例外エラー"));
    }

    async UniTask GameOverAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(1000, cancellationToken: ct);
            isMoveStop = true;
        }
        catch (OperationCanceledException ex)
        {
            Log.D($"CameraManagerのGameOverAsyncがキャンセルされました: {ex}");
        }
    }
}
