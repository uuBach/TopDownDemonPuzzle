//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.1
//     from Assets/Inputs/ItemsInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @ItemsInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ItemsInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ItemsInput"",
    ""maps"": [
        {
            ""name"": ""Key"",
            ""id"": ""50fbcc82-d264-4f8d-ac18-91eb74c2ebde"",
            ""actions"": [
                {
                    ""name"": ""DoorOpen"",
                    ""type"": ""Button"",
                    ""id"": ""872af2e9-1287-4d05-b2b4-18322cf1f508"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1df5a3a8-efe2-4f87-83f3-f230e87a6fa2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DoorOpen"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Key
        m_Key = asset.FindActionMap("Key", throwIfNotFound: true);
        m_Key_DoorOpen = m_Key.FindAction("DoorOpen", throwIfNotFound: true);
    }

    ~@ItemsInput()
    {
        UnityEngine.Debug.Assert(!m_Key.enabled, "This will cause a leak and performance issues, ItemsInput.Key.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Key
    private readonly InputActionMap m_Key;
    private List<IKeyActions> m_KeyActionsCallbackInterfaces = new List<IKeyActions>();
    private readonly InputAction m_Key_DoorOpen;
    public struct KeyActions
    {
        private @ItemsInput m_Wrapper;
        public KeyActions(@ItemsInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @DoorOpen => m_Wrapper.m_Key_DoorOpen;
        public InputActionMap Get() { return m_Wrapper.m_Key; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyActions set) { return set.Get(); }
        public void AddCallbacks(IKeyActions instance)
        {
            if (instance == null || m_Wrapper.m_KeyActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_KeyActionsCallbackInterfaces.Add(instance);
            @DoorOpen.started += instance.OnDoorOpen;
            @DoorOpen.performed += instance.OnDoorOpen;
            @DoorOpen.canceled += instance.OnDoorOpen;
        }

        private void UnregisterCallbacks(IKeyActions instance)
        {
            @DoorOpen.started -= instance.OnDoorOpen;
            @DoorOpen.performed -= instance.OnDoorOpen;
            @DoorOpen.canceled -= instance.OnDoorOpen;
        }

        public void RemoveCallbacks(IKeyActions instance)
        {
            if (m_Wrapper.m_KeyActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IKeyActions instance)
        {
            foreach (var item in m_Wrapper.m_KeyActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_KeyActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public KeyActions @Key => new KeyActions(this);
    public interface IKeyActions
    {
        void OnDoorOpen(InputAction.CallbackContext context);
    }
}
