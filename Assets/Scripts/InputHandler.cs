using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    public Writer Writer;
    public Master Master;
    private void Awake()
    {
        mainCamera = Camera.main;
        Debug.Log("Awake");
    }

    public void onClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var rayhit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayhit.collider) return;

        if (rayhit.collider.gameObject.name == "Button") return;
        Master.nextTarget(rayhit.collider.gameObject);

    }
}
