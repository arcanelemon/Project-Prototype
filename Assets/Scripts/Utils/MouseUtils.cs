using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtils : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public static void LockMouse() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public static void ReleaseMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
