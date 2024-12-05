using UnityEngine;

public static class MouseData
{
    public static float GetMouseX()
    {
        return Input.GetAxisRaw("Mouse X");
    }

    public static float GetMouseY()
    {
        return Input.GetAxisRaw("Mouse Y");
    }

    public static Vector2 GetMouseAxis()
    {
        return new Vector2(GetMouseX(), GetMouseY());
    }

    public static float GetMouseScroll()
    {
        return Input.GetAxisRaw("Mouse ScrollWheel");
    }
}