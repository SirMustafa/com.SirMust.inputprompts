using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SetTextToBox : MonoBehaviour
{
    private const string Placeholder = "BUTTONPROMPT";

    [Header("References")]
    [SerializeField] private InputActionReference inputActionReference;
    [SerializeField] private InputPromptSpriteLibrary spriteLibrary;

    [Header("Platform")]
    [SerializeField] private bool forcePlayStationOnPS5 = false;

    private static InputPromptDevice _lastUsedDevice = InputPromptDevice.KeyboardMouse;
    private static bool _deviceListenerInitialized;
    private static event Action OnLastUsedDeviceChanged;

    private TextMeshProUGUI _textBox;
    private string _templateText;

    private void Awake()
    {
        _textBox = GetComponent<TextMeshProUGUI>();
        _templateText = _textBox.text;

        InitializeDeviceTracking();
        DetectInitialDevice();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;
        OnLastUsedDeviceChanged += Refresh;
        StartCoroutine(RefreshNextFrame());
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;
        OnLastUsedDeviceChanged -= Refresh;
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null;
        Refresh();
    }

    public void SetTemplateFromExternal(string value)
    {
        _templateText = value;
        Refresh();
    }

    public void Refresh()
    {
        if (_textBox == null)
            return;

        if (string.IsNullOrEmpty(_templateText))
        {
            _textBox.text = string.Empty;
            return;
        }

        if (!_templateText.Contains(Placeholder))
        {
            _textBox.text = _templateText;
            return;
        }

        if (inputActionReference == null || inputActionReference.action == null || spriteLibrary == null)
        {
            _textBox.text = _templateText.Replace(Placeholder, "?");
            return;
        }

        InputPromptDevice device = GetCurrentDevice();
        string prompt = BuildPrompt(inputActionReference.action, device);

        _textBox.text = _templateText.Replace(Placeholder, prompt);

        Canvas.ForceUpdateCanvases();
        _textBox.ForceMeshUpdate(true);
    }

    private static void InitializeDeviceTracking()
    {
        if (_deviceListenerInitialized)
            return;

        _deviceListenerInitialized = true;
        InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPressed);
    }

    private static void OnAnyButtonPressed(InputControl control)
    {
        InputPromptDevice newDevice;

        if (control?.device is Gamepad gamepad)
            newDevice = DetectGamepadType(gamepad);
        else
            newDevice = InputPromptDevice.KeyboardMouse;

        if (newDevice != _lastUsedDevice)
        {
            _lastUsedDevice = newDevice;
            OnLastUsedDeviceChanged?.Invoke();
        }

        InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPressed);
    }

    private void DetectInitialDevice()
    {
#if UNITY_PS5
        _lastUsedDevice = InputPromptDevice.PlayStation;
#else
        if (Gamepad.current != null && Gamepad.current.enabled)
            _lastUsedDevice = DetectGamepadType(Gamepad.current);
        else
            _lastUsedDevice = InputPromptDevice.KeyboardMouse;
#endif
    }

    private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added ||
            change == InputDeviceChange.Removed ||
            change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Reconnected)
        {
            DetectInitialDevice();
            Refresh();
        }
    }

    private InputPromptDevice GetCurrentDevice()
    {
#if UNITY_PS5
        return InputPromptDevice.PlayStation;
#else
        if (forcePlayStationOnPS5)
            return InputPromptDevice.PlayStation;

        return _lastUsedDevice;
#endif
    }

    private string BuildPrompt(InputAction action, InputPromptDevice device)
    {
        TMP_SpriteAsset spriteAsset = spriteLibrary.Get(device);
        if (spriteAsset == null)
            return "?";

        _textBox.spriteAsset = spriteAsset;

        if (TryBuildCompositePrompt(action, device, spriteAsset, out string compositePrompt))
            return compositePrompt;

        string path = GetBestBindingPath(action, device);
        if (string.IsNullOrEmpty(path))
            return "?";

        string normalized = NormalizeBindingPath(path, device);
        return MakeSpriteTag(normalized);
    }

    private bool TryBuildCompositePrompt(InputAction action, InputPromptDevice device, TMP_SpriteAsset spriteAsset, out string result)
    {
        result = null;

        if (device == InputPromptDevice.KeyboardMouse)
        {
            string left = GetCompositePartPath(action, "left", true);
            string right = GetCompositePartPath(action, "right", true);

            if (!string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(right))
            {
                result =
                    $"{MakeSpriteTag(NormalizeBindingPath(left, device))}   " +
                    $"{MakeSpriteTag(NormalizeBindingPath(right, device))}";
                return true;
            }
        }
        else
        {
            string up = GetCompositePartPath(action, "up", false);
            if (!string.IsNullOrEmpty(up))
            {
                result = MakeSpriteTag(NormalizeBindingPath(up, device));
                return true;
            }
        }

        return false;
    }

    private string GetBestBindingPath(InputAction action, InputPromptDevice device)
    {
        string expectedGroup = GetBindingGroup(device);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite || binding.isPartOfComposite)
                continue;

            if (!BindingMatchesGroup(binding, expectedGroup))
                continue;

            string path = GetBindingPath(binding);
            if (!string.IsNullOrEmpty(path))
                return path;
        }

        for (int i = 0; i < action.bindings.Count; i++)
        {
            string path = GetBindingPath(action.bindings[i]);
            if (!string.IsNullOrEmpty(path))
                return path;
        }

        return null;
    }

    private string GetCompositePartPath(InputAction action, string partName, bool keyboardOnly)
    {
        string expectedGroup = GetBindingGroup(GetCurrentDevice());

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (!binding.isPartOfComposite || binding.name != partName)
                continue;

            if (keyboardOnly)
            {
                if (!BindingMatchesGroup(binding, "KeyboardMouse") && !PathLooksKeyboard(GetBindingPath(binding)))
                    continue;
            }
            else
            {
                if (!BindingMatchesGroup(binding, expectedGroup) && PathLooksKeyboard(GetBindingPath(binding)))
                    continue;
            }

            string path = GetBindingPath(binding);
            if (!string.IsNullOrEmpty(path))
                return path;
        }

        return null;
    }

    private static string GetBindingPath(InputBinding binding)
    {
        return string.IsNullOrEmpty(binding.overridePath) ? binding.effectivePath : binding.overridePath;
    }

    private static bool BindingMatchesGroup(InputBinding binding, string group)
    {
        if (string.IsNullOrEmpty(group) || string.IsNullOrEmpty(binding.groups))
            return false;

        ReadOnlyArray<string> groups = binding.groups.Split(InputBinding.Separator);
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i] == group)
                return true;
        }

        return false;
    }

    private static bool PathLooksKeyboard(string path)
    {
        return !string.IsNullOrEmpty(path) && path.StartsWith("<Keyboard>");
    }

    private static string GetBindingGroup(InputPromptDevice device)
    {
        return device switch
        {
            InputPromptDevice.KeyboardMouse => "KeyboardMouse",
            InputPromptDevice.Xbox => "Gamepad",
            InputPromptDevice.PlayStation => "Gamepad",
            _ => "KeyboardMouse"
        };
    }

    private static InputPromptDevice DetectGamepadType(Gamepad gamepad)
    {
#if UNITY_PS5
        return InputPromptDevice.PlayStation;
#else
        string name = gamepad.displayName;

        if (!string.IsNullOrEmpty(name))
        {
            if (name.Contains("DualSense") || name.Contains("DualShock") || name.Contains("PlayStation"))
                return InputPromptDevice.PlayStation;

            if (name.Contains("Xbox") || name.Contains("XInput"))
                return InputPromptDevice.Xbox;
        }

        return InputPromptDevice.Xbox;
#endif
    }

    private static string NormalizeBindingPath(string path, InputPromptDevice device)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        if (path.StartsWith("<Gamepad>/"))
        {
            path = device == InputPromptDevice.PlayStation
                ? path.Replace("<Gamepad>/", "DualShockGamepad_")
                : path.Replace("<Gamepad>/", "XInputController_");
        }
        else if (path.StartsWith("<DualShockGamepad>/"))
        {
            path = path.Replace("<DualShockGamepad>/", "DualShockGamepad_");
        }
        else if (path.StartsWith("<DualSenseGamepad>/"))
        {
            path = path.Replace("<DualSenseGamepad>/", "DualShockGamepad_");
        }
        else if (path.StartsWith("<DualSenseGamepadHID>/"))
        {
            path = path.Replace("<DualSenseGamepadHID>/", "DualShockGamepad_");
        }
        else if (path.StartsWith("<DualShockGamepadHID>/"))
        {
            path = path.Replace("<DualShockGamepadHID>/", "DualShockGamepad_");
        }
        else if (path.StartsWith("<XInputController>/"))
        {
            path = path.Replace("<XInputController>/", "XInputController_");
        }
        else if (path.StartsWith("<Keyboard>/"))
        {
            path = path.Replace("<Keyboard>/", "Keyboard_");
        }

        path = path.Replace("/", "_");
        path = path.Replace("<", "");
        path = path.Replace(">", "");

        return path;
    }

    private static string MakeSpriteTag(string spriteName)
    {
        return $"<sprite name=\"{spriteName}\">";
    }
}