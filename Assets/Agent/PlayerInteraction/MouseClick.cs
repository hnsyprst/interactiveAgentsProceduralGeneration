using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MouseClick : MonoBehaviour
{
    public float Distance = 50f;
    public LayerMask GroundTileLayer;
    public LayerMask ResourceTileLayer;

    public Camera MainCamera;

    public GameObject Particles;

    public ClickEvent MouseClicked;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Distance, GroundTileLayer))
            {
                Instantiate(Particles, hit.point, Quaternion.Euler(-90, 0, 0));
                InvokeMouseClicked(hit.point);
            }
        }
    }

    public void InvokeMouseClicked(Vector3 Position)
    {
        MouseClicked.Invoke(Position);
    }
}

[System.Serializable]
public class ClickEvent : UnityEvent<Vector3> { }

