using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    Camera cam;
    Transform transformer;
    public LayerMask movementMask;
    void Start()
    {
        cam = Camera.main;
        transformer = GetComponent<Transform>();
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, movementMask))
        {
            transformer.position = (new Vector3(Mathf.Round(hit.point.x * 2) / 2, hit.point.y + 0.05f, Mathf.Round(hit.point.z * 2) / 2));
        }
    }
}
