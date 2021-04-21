using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public static bool LeftMouseClick;

    private void Update()
    {
        LeftMouseClick = KeyPressed(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EntityManager.RemoveExhaust();
        }
    }
    private bool KeyPressed(KeyCode key)
    {
        if (Input.GetKeyDown(key)) 
            return true;
        else 
            return false;
    }
}
