using Samples.PolySpatial.SwiftUI.Scripts;
using Unity.PolySpatial;
using UnityEngine;

namespace PolySpatial.Samples
{
    public class VolumeCameraSwiftUIManager : MonoBehaviour
    {
        [SerializeField]
        VolumeCamera m_VolumeCamera;

        [SerializeField]
        SwiftUIDriver m_SwiftUIDriver;

        [SerializeField]
        MeshSwiftUIDriver m_MeshSwiftUIDriver;

        void OnEnable()
        {
            if (m_VolumeCamera != null)
            {
                m_VolumeCamera.WindowStateChanged.AddListener(VolumeCameraEventListener);
            }
        }

        void OnDisable()
        {
            if (m_VolumeCamera != null)
            {
                m_VolumeCamera.WindowStateChanged.RemoveListener(VolumeCameraEventListener);
            }
        }

        void VolumeCameraEventListener(VolumeCamera volumeCamera, VolumeCamera.WindowState windowState)
        {
            if (volumeCamera != m_VolumeCamera)
                return;

            // our volume camera containing the main content in the scene has been closed, we should force close
            // the SwiftUI window to make sure the scene property reloads when closed and reopened
            if (windowState.WindowEvent == VolumeCamera.WindowEvent.Closed)
            {
                if (m_SwiftUIDriver != null)
                {
                    m_SwiftUIDriver.ForceCloseWindow();
                }

                if (m_MeshSwiftUIDriver != null)
                {
                    m_MeshSwiftUIDriver.ForceCloseWindow();
                }
            }
        }
    }
}
