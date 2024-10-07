using UnityEngine;
using Unity.PolySpatial;
using UnityEngine.Serialization;

namespace PolySpatial.Samples
{
    public class VolumeContentAnchor : MonoBehaviour
    {
        [SerializeField]
        VolumeCamera m_VolumeCamera;

        [SerializeField]
        Transform m_BottomCenterAnchor;

        [SerializeField]
        Transform m_FrontLeftCornerB;

        [SerializeField]
        Transform m_FrontRightCornerB;

        [SerializeField]
        Transform m_BackLeftCornerB;

        [SerializeField]
        Transform m_BackRightCornerB;

        [SerializeField]
        Transform m_FrontLeftCornerT;

        [SerializeField]
        Transform m_FrontRightCornerT;

        [SerializeField]
        Transform m_BackLeftCornerT;

        [SerializeField]
        Transform m_BackRightCornerT;

        bool m_ScaleWithVolume = true;

        void OnEnable()
        {
            m_VolumeCamera.WindowStateChanged.AddListener(OnWindowStateChanged);
        }

        void OnWindowStateChanged(VolumeCamera camera, VolumeCamera.WindowState state)
        {
            if (state.WindowEvent == VolumeCamera.WindowEvent.Resized && !m_VolumeCamera.ScaleWithWindow)
            {
                var contentSize = state.ContentDimensions;
                var yPosition = -state.ContentDimensions.y / 2;
                m_BottomCenterAnchor.position = new Vector3(m_BottomCenterAnchor.position.x, yPosition + 0.05f, m_BottomCenterAnchor.position.z);

                m_FrontLeftCornerB.position = new Vector3(-contentSize.x / 2, -contentSize.y / 2, -contentSize.z / 2);
                m_FrontRightCornerB.position = new Vector3(contentSize.x / 2, -contentSize.y / 2, -contentSize.z / 2);
                m_BackLeftCornerB.position = new Vector3(-contentSize.x / 2, -contentSize.y / 2, contentSize.z / 2);
                m_BackRightCornerB.position = new Vector3(contentSize.x / 2, -contentSize.y / 2, contentSize.z / 2);

                m_FrontLeftCornerT.position = new Vector3(-contentSize.x / 2, contentSize.y / 2, -contentSize.z / 2);
                m_FrontRightCornerT.position = new Vector3(contentSize.x / 2, contentSize.y / 2, -contentSize.z / 2);
                m_BackLeftCornerT.position = new Vector3(-contentSize.x / 2, contentSize.y / 2, contentSize.z / 2);
                m_BackRightCornerT.position = new Vector3(contentSize.x / 2, contentSize.y / 2, contentSize.z / 2);
            }
        }

        public void ResetPosition()
        {
            m_ScaleWithVolume = !m_ScaleWithVolume;

            if (m_ScaleWithVolume)
            {
                m_BottomCenterAnchor.position = new Vector3(0.0f, -0.5f, 0.0f);
                m_FrontLeftCornerB.position = new Vector3(-0.5f, -0.5f, -0.5f);
                m_FrontRightCornerB.position = new Vector3(0.5f, -0.5f, -0.5f);
                m_BackLeftCornerB.position = new Vector3(-0.5f, -0.5f, 0.5f);
                m_BackRightCornerB.position = new Vector3(0.5f, -0.5f, 0.5f);
                m_FrontLeftCornerT.position = new Vector3(-0.5f, 0.5f, -0.5f);
                m_FrontRightCornerT.position = new Vector3(0.5f, 0.5f, -0.5f);
                m_BackLeftCornerT.position = new Vector3(-0.5f, 0.5f, 0.5f);
                m_BackRightCornerT.position = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
