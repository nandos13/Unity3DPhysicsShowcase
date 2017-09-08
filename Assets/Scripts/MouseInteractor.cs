using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInteractor : MonoBehaviour
{

    private LayerMask _layerMask = Physics.AllLayers;
    private bool _mouseDown = false;

    void Update()
    {
        if (Input.GetAxisRaw("Fire1") != 0)
        {
            if (!_mouseDown)
            {
                _mouseDown = true;
                ScreenPick();
            }
        }
        else
            _mouseDown = false;
    }

    private void ScreenPick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo, 1000.0f, _layerMask, QueryTriggerInteraction.Collide);

        // Was something hit?
        if (hitInfo.transform)
        {
            IInteractable i = hitInfo.transform.GetComponentInParent<IInteractable>();
            if (i != null)
                i.Interact();
        }
    }
}
