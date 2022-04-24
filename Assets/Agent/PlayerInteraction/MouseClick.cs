using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public float Distance = 50f;
    public LayerMask GroundTileLayer;
    public LayerMask ResourceTileLayer;

    public Camera MainCamera;

    public GameObject Particles;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Distance, GroundTileLayer))
            {
                Instantiate(Particles, hit.point, Quaternion.Euler(-90, 0, 0));
            }
        }
    }
}

