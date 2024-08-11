using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    [SerializeField] private Reticle reticleManager;
    [SerializeField] private Transform cannonHole; // Reference to the cannon's hole

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    reticleManager.Selected(this.gameObject);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                reticleManager.Deselect();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                reticleManager.Selected(this.gameObject);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            reticleManager.Deselect();
        }
    }

    // Provide a method to get the cannon hole position
    public Transform GetCannonHole()
    {
        return cannonHole;
    }
}
