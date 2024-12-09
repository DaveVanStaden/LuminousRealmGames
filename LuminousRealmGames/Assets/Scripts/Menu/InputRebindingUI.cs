using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MalbersAnimations.InputSystem;
using MalbersAnimations.Controller;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

#endif

public class InputRebindingUI : MonoBehaviour
{
    [System.Serializable]
    public class InputActionUI
    {
        public InputActionReference actionReference; // Reference to the action
        public string displayName;                  // Display name in the UI
        public Button rebindButton;                 // Button to initiate rebinding
        public TMP_Text bindingText;                // Text to display current binding
        public int bindingIndex;               // Index of the binding for choose the correct input
    }

    public InputActionUI[] actionsToRebind;         // List of actions to display/rebind

    public const string RebindsKey = "rebinds";

    private const float WarningDisplayDuration = 2f; // Duration to display warning in seconds

    bool alreadyLoaded = false;

    public MAnimal player;

    void Start()
    {
        if (!alreadyLoaded)
        {
            // Load saved bindings if they exist
            LoadBindings();
        }

        // Initialize UI elements
        foreach (var actionUI in actionsToRebind)
        {
            UpdateBindingDisplay(actionUI); // Update the button text to show current binding

            actionUI.rebindButton.onClick.AddListener(() =>
            {
                StartRebinding(actionUI); // Start rebinding on button click
            });
        }
    }

    private void UpdateBindingDisplay(InputActionUI actionUI)
    {
        if (actionUI.actionReference != null && actionUI.actionReference.action != null)
        {
            // Get the first binding for simplicity (customize for multiple bindings if needed)
            string bindingName = actionUI.actionReference.action.bindings[actionUI.bindingIndex].ToDisplayString();
            actionUI.bindingText.text = $"{bindingName}";
        }
    }

    private void StartRebinding(InputActionUI actionUI)
    {


        var action = actionUI.actionReference.action;
        if (action == null) return;

        // Disable the action before rebinding
        if (player)
            player.UpdateInputSource(false);

        action.Disable();

        // Disable the button text during rebinding
        actionUI.bindingText.text = "Press a key...";

        // Start the rebinding process
        action.PerformInteractiveRebinding(actionUI.bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // Check if the new binding is already in use by another action
                var newBinding = action.bindings[actionUI.bindingIndex].effectivePath;
                if (IsKeyAlreadyBound(newBinding, action, actionUI.bindingIndex))
                {
                    action.RemoveBindingOverride(actionUI.bindingIndex); // Cancel rebind
                    StartCoroutine(ShowTemporaryWarning(actionUI, "Key already in use!"));
                }
                else
                {
                    SaveBindings();      // Save all bindings after rebind
                    UpdateBindingDisplay(actionUI); // Update the UI with the new binding
                }

                operation.Dispose(); // Dispose of the operation
                                     // Re-enable the action after rebinding
                if (player)
                    player.UpdateInputSource(true);

                action.Enable();
            })
            .Start();


    }

    private bool IsKeyAlreadyBound(string newBinding, InputAction actionToRebind, int bindingIndex)
    {
        foreach (var actionUI in actionsToRebind)
        {
            var action = actionUI.actionReference.action;
            if (action != null && action != actionToRebind)
            {
                // Check all bindings of other actions
                foreach (var binding in action.bindings)
                {
                    if (binding.effectivePath == newBinding)
                    {
                        return true; // Key is already in use
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator ShowTemporaryWarning(InputActionUI actionUI, string warningText)
    {
        // Show the warning text
        actionUI.bindingText.text = warningText;

        // Wait for the warning display duration
        yield return new WaitForSeconds(WarningDisplayDuration);

        // Restore the original binding display
        UpdateBindingDisplay(actionUI);
    }

    private void SaveBindings()
    {
        // Save the binding overrides for all actions to PlayerPrefs
        foreach (var actionUI in actionsToRebind)
        {
            var action = actionUI.actionReference.action;
            if (action != null)
            {
                string rebinds = action.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString($"{RebindsKey}_{action.name}", rebinds);
            }
        }
        PlayerPrefs.Save();
    }

    public void LoadBindingsKey()
    {
        LoadBindings();
        alreadyLoaded = true;
    }

    private void LoadBindings()
    {
        foreach (var actionUI in actionsToRebind)
        {
            var action = actionUI.actionReference.action;
            if (action != null && PlayerPrefs.HasKey($"{RebindsKey}_{action.name}"))
            {
                // Load the saved binding overrides from PlayerPrefs
                string bindingJson = PlayerPrefs.GetString($"{RebindsKey}_{action.name}");
                action.LoadBindingOverridesFromJson(bindingJson);
            }
        }
    }

    public void ResetBindings()
    {
        // Reset all binding overrides
        foreach (var actionUI in actionsToRebind)
        {
            var action = actionUI.actionReference.action;
            action?.RemoveAllBindingOverrides();
        }

        // Clear saved bindings
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Refresh UI display
        foreach (var actionUI in actionsToRebind)
        {
            UpdateBindingDisplay(actionUI);
        }
    }

    public void ClearRebindsOnly()
    {
        List<string> keysToDelete = new List<string>();

        // Collect all keys that start with the RebindsKey prefix
        foreach (var actionUI in actionsToRebind)
        {
            string key = $"{RebindsKey}_{actionUI.actionReference.action.name}";
            if (PlayerPrefs.HasKey(key))
            {
                keysToDelete.Add(key);
            }
        }

        // Delete the specific rebind keys
        foreach (string key in keysToDelete)
        {
            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.Save();

        // Optionally reset the bindings to default and refresh the display
        foreach (var actionUI in actionsToRebind)
        {
            actionUI.actionReference.action?.RemoveAllBindingOverrides();
            UpdateBindingDisplay(actionUI);
        }
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(InputRebindingUI))]
public class InputRebindingUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector UI
        DrawDefaultInspector();

        // Reference to the target component
        InputRebindingUI inputRebindingUI = (InputRebindingUI)target;

        // Add a button to reset only rebinds
        if (GUILayout.Button("Reset Rebinds Only"))
        {
            inputRebindingUI.ClearRebindsOnly();
        }
    }
}

#endif