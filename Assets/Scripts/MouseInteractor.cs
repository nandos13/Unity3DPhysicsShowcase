using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            hitInfo.transform.SendMessageUpwards("Interact", SendMessageOptions.DontRequireReceiver);
        }
    }
}
