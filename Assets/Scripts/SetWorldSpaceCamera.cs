using UnityEngine;
/// <summary>
/// Sets the world camera to main camera on Start
/// </summary>

public class SetWorldSpaceCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
