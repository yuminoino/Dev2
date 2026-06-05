using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Esempi pratici per leggere gli input dei controller Meta Quest 3
/// usando l'Input System gia' presente nel progetto.
///
/// I binding usano i path generici "<XRController>" che hai gia' anche
/// nell'asset "InputSystem_Actions", quindi restano compatibili se poi
/// passi a un setup OpenXR completo.
/// </summary>
public class OpenXRInputStubExample : MonoBehaviour
{
    [Header("Log Examples")]
    [SerializeField] private bool logDeviceDiscovery = true;
    [SerializeField] private bool logButtonEvents = true;
    [SerializeField] private bool logAnalogValues = true;
    [SerializeField] private bool logTrackingPose = true;

    [Header("Log Tuning")]
    [SerializeField, Range(0.05f, 1f)] private float analogThreshold = 0.15f;
    [SerializeField, Min(0.05f)] private float analogLogInterval = 0.25f;
    [SerializeField, Min(0.05f)] private float trackingLogInterval = 0.5f;

    private InputAction leftThumbstickAction;
    private InputAction rightThumbstickAction;
    private InputAction leftTriggerAction;
    private InputAction rightTriggerAction;
    private InputAction leftGripAction;
    private InputAction rightGripAction;

    private InputAction leftPrimaryButtonAction;
    private InputAction leftSecondaryButtonAction;
    private InputAction rightPrimaryButtonAction;
    private InputAction rightSecondaryButtonAction;
    private InputAction leftTriggerPressedAction;
    private InputAction rightTriggerPressedAction;
    private InputAction leftGripPressedAction;
    private InputAction rightGripPressedAction;
    private InputAction leftThumbstickClickedAction;
    private InputAction rightThumbstickClickedAction;

    private InputAction leftTrackedAction;
    private InputAction rightTrackedAction;
    private InputAction leftPositionAction;
    private InputAction rightPositionAction;
    private InputAction leftRotationAction;
    private InputAction rightRotationAction;

    private InputAction[] runtimeActions;
    private float nextAnalogLogTime;
    private float nextTrackingLogTime;

    private void Awake()
    {
        CreateRuntimeActions();
    }

    private void OnEnable()
    {
        EnableRuntimeActions();
        InputSystem.onDeviceChange += HandleDeviceChange;

        if (logDeviceDiscovery)
        {
            DumpXRDevices("XR devices rilevati all'avvio");
        }
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
        DisableRuntimeActions();
    }

    private void Update()
    {
        ReadQuestButtonExamples();
        ReadQuestAnalogExamples();
        ReadQuestTrackingExamples();
    }

    private void CreateRuntimeActions()
    {
        leftThumbstickAction = CreateValueAction("Left Thumbstick", "<XRController>{LeftHand}/{Primary2DAxis}", "Vector2");
        rightThumbstickAction = CreateValueAction("Right Thumbstick", "<XRController>{RightHand}/{Primary2DAxis}", "Vector2");

        leftTriggerAction = CreateValueAction("Left Trigger", "<XRController>{LeftHand}/trigger", "Axis");
        rightTriggerAction = CreateValueAction("Right Trigger", "<XRController>{RightHand}/trigger", "Axis");
        leftGripAction = CreateValueAction("Left Grip", "<XRController>{LeftHand}/grip", "Axis");
        rightGripAction = CreateValueAction("Right Grip", "<XRController>{RightHand}/grip", "Axis");

        leftPrimaryButtonAction = CreateButtonAction("Left Primary (X)", "<XRController>{LeftHand}/primaryButton");
        leftSecondaryButtonAction = CreateButtonAction("Left Secondary (Y)", "<XRController>{LeftHand}/secondaryButton");
        rightPrimaryButtonAction = CreateButtonAction("Right Primary (A)", "<XRController>{RightHand}/primaryButton");
        rightSecondaryButtonAction = CreateButtonAction("Right Secondary (B)", "<XRController>{RightHand}/secondaryButton");

        leftTriggerPressedAction = CreateButtonAction("Left Trigger Pressed", "<XRController>{LeftHand}/triggerPressed");
        rightTriggerPressedAction = CreateButtonAction("Right Trigger Pressed", "<XRController>{RightHand}/triggerPressed");
        leftGripPressedAction = CreateButtonAction("Left Grip Pressed", "<XRController>{LeftHand}/gripPressed");
        rightGripPressedAction = CreateButtonAction("Right Grip Pressed", "<XRController>{RightHand}/gripPressed");
        leftThumbstickClickedAction = CreateButtonAction("Left Thumbstick Click", "<XRController>{LeftHand}/thumbstickClicked");
        rightThumbstickClickedAction = CreateButtonAction("Right Thumbstick Click", "<XRController>{RightHand}/thumbstickClicked");

        leftTrackedAction = CreateButtonAction("Left Is Tracked", "<XRController>{LeftHand}/isTracked");
        rightTrackedAction = CreateButtonAction("Right Is Tracked", "<XRController>{RightHand}/isTracked");
        leftPositionAction = CreateValueAction("Left Position", "<XRController>{LeftHand}/devicePosition", "Vector3");
        rightPositionAction = CreateValueAction("Right Position", "<XRController>{RightHand}/devicePosition", "Vector3");
        leftRotationAction = CreateValueAction("Left Rotation", "<XRController>{LeftHand}/deviceRotation", "Quaternion");
        rightRotationAction = CreateValueAction("Right Rotation", "<XRController>{RightHand}/deviceRotation", "Quaternion");

        runtimeActions = new[]
        {
            leftThumbstickAction,
            rightThumbstickAction,
            leftTriggerAction,
            rightTriggerAction,
            leftGripAction,
            rightGripAction,
            leftPrimaryButtonAction,
            leftSecondaryButtonAction,
            rightPrimaryButtonAction,
            rightSecondaryButtonAction,
            leftTriggerPressedAction,
            rightTriggerPressedAction,
            leftGripPressedAction,
            rightGripPressedAction,
            leftThumbstickClickedAction,
            rightThumbstickClickedAction,
            leftTrackedAction,
            rightTrackedAction,
            leftPositionAction,
            rightPositionAction,
            leftRotationAction,
            rightRotationAction
        };
    }

    private void EnableRuntimeActions()
    {
        if (runtimeActions == null)
        {
            return;
        }

        foreach (InputAction action in runtimeActions)
        {
            action.Enable();
        }
    }

    private void DisableRuntimeActions()
    {
        if (runtimeActions == null)
        {
            return;
        }

        foreach (InputAction action in runtimeActions)
        {
            action.Disable();
        }
    }

    private void ReadQuestButtonExamples()
    {
        if (!logButtonEvents)
        {
            return;
        }

        LogButtonEdge(leftPrimaryButtonAction, "Left primary (X)");
        LogButtonEdge(leftSecondaryButtonAction, "Left secondary (Y)");
        LogButtonEdge(rightPrimaryButtonAction, "Right primary (A)");
        LogButtonEdge(rightSecondaryButtonAction, "Right secondary (B)");

        LogButtonEdge(leftTriggerPressedAction, "Left trigger premuto");
        LogButtonEdge(rightTriggerPressedAction, "Right trigger premuto");
        LogButtonEdge(leftGripPressedAction, "Left grip premuto");
        LogButtonEdge(rightGripPressedAction, "Right grip premuto");
        LogButtonEdge(leftThumbstickClickedAction, "Click thumbstick sinistro");
        LogButtonEdge(rightThumbstickClickedAction, "Click thumbstick destro");
    }

    private void ReadQuestAnalogExamples()
    {
        if (!logAnalogValues || Time.unscaledTime < nextAnalogLogTime)
        {
            return;
        }

        Vector2 leftStick = leftThumbstickAction.ReadValue<Vector2>();
        Vector2 rightStick = rightThumbstickAction.ReadValue<Vector2>();
        float leftTrigger = leftTriggerAction.ReadValue<float>();
        float rightTrigger = rightTriggerAction.ReadValue<float>();
        float leftGrip = leftGripAction.ReadValue<float>();
        float rightGrip = rightGripAction.ReadValue<float>();

        bool shouldLog =
            leftStick.magnitude >= analogThreshold ||
            rightStick.magnitude >= analogThreshold ||
            leftTrigger >= analogThreshold ||
            rightTrigger >= analogThreshold ||
            leftGrip >= analogThreshold ||
            rightGrip >= analogThreshold;

        if (!shouldLog)
        {
            return;
        }

        Debug.Log(
            $"[Quest 3][Analog] " +
            $"LeftStick {FormatVector2(leftStick)} | " +
            $"RightStick {FormatVector2(rightStick)} | " +
            $"LeftTrigger {leftTrigger:F2} | " +
            $"RightTrigger {rightTrigger:F2} | " +
            $"LeftGrip {leftGrip:F2} | " +
            $"RightGrip {rightGrip:F2}",
            this);

        nextAnalogLogTime = Time.unscaledTime + analogLogInterval;
    }

    private void ReadQuestTrackingExamples()
    {
        if (!logTrackingPose || Time.unscaledTime < nextTrackingLogTime)
        {
            return;
        }

        bool leftTracked = leftTrackedAction.ReadValue<float>() > 0.5f;
        bool rightTracked = rightTrackedAction.ReadValue<float>() > 0.5f;

        if (!leftTracked && !rightTracked)
        {
            return;
        }

        Vector3 leftPosition = leftPositionAction.ReadValue<Vector3>();
        Vector3 rightPosition = rightPositionAction.ReadValue<Vector3>();
        Quaternion leftRotation = leftRotationAction.ReadValue<Quaternion>();
        Quaternion rightRotation = rightRotationAction.ReadValue<Quaternion>();

        Debug.Log(
            $"[Quest 3][Tracking] " +
            $"Left tracked: {leftTracked}, pos {FormatVector3(leftPosition)}, rot {FormatEuler(leftRotation)} | " +
            $"Right tracked: {rightTracked}, pos {FormatVector3(rightPosition)}, rot {FormatEuler(rightRotation)}",
            this);

        nextTrackingLogTime = Time.unscaledTime + trackingLogInterval;
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!logDeviceDiscovery || !IsXRLikeDevice(device))
        {
            return;
        }

        if (change == InputDeviceChange.Added ||
            change == InputDeviceChange.Reconnected ||
            change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Removed)
        {
            Debug.Log(
                $"[Quest 3][Device] {change}: {device.displayName} | layout={device.layout} | usages={FormatUsages(device)}",
                this);
        }
    }

    private void DumpXRDevices(string label)
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (!IsXRLikeDevice(device))
            {
                continue;
            }

            Debug.Log(
                $"[Quest 3][Device] {label}: {device.displayName} | layout={device.layout} | usages={FormatUsages(device)}",
                this);
        }
    }

    private static InputAction CreateButtonAction(string name, string binding)
    {
        InputAction action = new InputAction(name: name, type: InputActionType.Button, expectedControlType: "Button");
        action.AddBinding(binding);
        return action;
    }

    private static InputAction CreateValueAction(string name, string binding, string expectedControlType)
    {
        InputAction action = new InputAction(name: name, type: InputActionType.Value, expectedControlType: expectedControlType);
        action.AddBinding(binding);
        return action;
    }

    private void LogButtonEdge(InputAction action, string label)
    {
        if (action.WasPressedThisFrame())
        {
            Debug.Log($"[Quest 3][Button] {label} premuto", this);
        }

        if (action.WasReleasedThisFrame())
        {
            Debug.Log($"[Quest 3][Button] {label} rilasciato", this);
        }
    }

    private static bool IsXRLikeDevice(InputDevice device)
    {
        if (device == null)
        {
            return false;
        }

        if (ContainsIgnoreCase(device.layout, "XR") ||
            ContainsIgnoreCase(device.layout, "Oculus") ||
            ContainsIgnoreCase(device.layout, "OpenXR"))
        {
            return true;
        }

        foreach (InternedString usage in device.usages)
        {
            string usageName = usage.ToString();
            if (usageName == "LeftHand" || usageName == "RightHand")
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsIgnoreCase(string source, string value)
    {
        return !string.IsNullOrEmpty(source) &&
               source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string FormatUsages(InputDevice device)
    {
        string usagesText = string.Empty;

        foreach (InternedString usage in device.usages)
        {
            if (!string.IsNullOrEmpty(usagesText))
            {
                usagesText += ", ";
            }

            usagesText += usage.ToString();
        }

        return string.IsNullOrEmpty(usagesText) ? "none" : usagesText;
    }

    private static string FormatVector2(Vector2 value)
    {
        return $"({value.x:F2}, {value.y:F2})";
    }

    private static string FormatVector3(Vector3 value)
    {
        return $"({value.x:F2}, {value.y:F2}, {value.z:F2})";
    }

    private static string FormatEuler(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        return $"({euler.x:F1}, {euler.y:F1}, {euler.z:F1})";
    }
}
