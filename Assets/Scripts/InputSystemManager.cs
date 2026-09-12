using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemManager
{
    PlayerInput playerInput;
    GameState curState;

    public InputSystemManager(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
    }
    public void SetState(GameState state)
    {
        if (state == curState) return;
        curState = state;
        playerInput.actions.Disable();
        switch (state)
        {
            case GameState.Title:
                playerInput.actions.FindActionMap("UI").Enable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.GamePlay:
                playerInput.actions.FindActionMap("GamePlay").Enable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Pause:
                playerInput.actions.FindActionMap("UI").Enable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Dead:
                playerInput.actions.FindActionMap("UI").Enable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.SceneChange:
                playerInput.actions.FindActionMap("UI").Enable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
    public GameState GetState() => curState;
}
