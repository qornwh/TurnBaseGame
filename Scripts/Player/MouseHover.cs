using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    private Camera mainCam;
    private const float MaxDistance = 100f;
    private List<IMouseHoverHandler> mouseHoverHandlers;
    
    void Start()
    {
        mouseHoverHandlers = new List<IMouseHoverHandler>();
    }

    void Update()
    {
        if (mainCam == null)
        {
            var gm = GameInstance.GetInstance().GameManager;
            mainCam = gm.GameCamera;
        }
        
        List<IMouseHoverHandler> preMouseHoverHandlers = mouseHoverHandlers;
        mouseHoverHandlers = new List<IMouseHoverHandler>();
        
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        foreach (var hit in Physics.RaycastAll(ray, MaxDistance, 5))
        {
            if (hit.collider != null)
            {
                GameObject obj = hit.collider.gameObject;
                IMouseHoverHandler handler = obj.GetComponent<IMouseHoverHandler>();
                if (handler != null)
                {
                    preMouseHoverHandlers.Remove(handler);
                    mouseHoverHandlers.Add(handler);
                    if (Input.GetMouseButtonDown(0))
                    {
                        handler.OnMouseDown();
                    }
                    else
                    {
                        handler.OnMouseEnter();
                    }
                }
            }
        }
        
        foreach (var handler in preMouseHoverHandlers)
        {
            handler.OnMouseExit();
        }
        preMouseHoverHandlers.Clear();
    }
}
