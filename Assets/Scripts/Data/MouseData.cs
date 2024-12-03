using UnityEngine;

public static class MouseData
{
    public static float GetMouseX()
    {
        return Input.GetAxis("Mouse X");
    }
    
    public static float GetMouseY()
    {
        return Input.GetAxis("Mouse Y");
    }
    
    public static Vector2 GetMouseAxis()
    {
        return new Vector2(GetMouseX(), GetMouseY());
    }
}