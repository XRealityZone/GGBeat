using System;
using Unity.PolySpatial;
using UnityEngine;

public class UserDirectionIndicator : MonoBehaviour
{
    [SerializeField]
    VolumeCamera m_VolumeCamera;

    [SerializeField]
    Transform m_DirectionIndicator;

    void OnEnable()
    {
        //m_VolumeCamera.ViewpointChanged.AddListener(VolumeViewpointChanged);
    }

    void OnDisable()
    {
        //m_VolumeCamera.ViewpointChanged.RemoveListener(VolumeViewpointChanged);
    }

    /*
    void VolumeViewpointChanged(VolumeCamera.Viewpoint viewpoint)
    {
        var eulerRotation = Quaternion.identity;
        switch (viewpoint)
        {
            case VolumeCamera.Viewpoint.Left:
                eulerRotation = Quaternion.Euler(0, -90, 0);
                break;
            case VolumeCamera.Viewpoint.Right:
                eulerRotation = Quaternion.Euler(0, 90, 0);
                break;
            case VolumeCamera.Viewpoint.Front:
                eulerRotation = Quaternion.Euler(0, -180, 0);
                break;
            case VolumeCamera.Viewpoint.Back:
                eulerRotation = Quaternion.Euler(0, 0, 0);
                break;
        }

        m_DirectionIndicator.rotation = eulerRotation;
    }
    */
}
