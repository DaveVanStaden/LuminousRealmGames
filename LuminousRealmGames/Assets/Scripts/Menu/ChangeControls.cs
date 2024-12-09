using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using MalbersAnimations;
using UnityEngine.InputSystem;

public class ChangeControls : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction = null;
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private PlayerManager playerManager = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);
        //playerManager.PlayerInput.PlayerMovement.Disable();

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        rebindingOperation.Dispose();
        playerManager.PlayerInput.PlayerMenu.Enable();
        playerManager.PlayerInput.PlayerMovement.Disable();
        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
    }
}
