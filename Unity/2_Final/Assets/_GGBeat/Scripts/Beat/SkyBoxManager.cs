using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SkyBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject skybox;

    [HideInInspector]
    public ARPlane? trackedPlane;


    [HideInInspector]
    public bool isTracking = true;

    private bool isPortalActive = true;

    [SerializeField] private XROrigin xrOrigin;

    [SerializeField] private bool isPlayPortalAnimationOnStart = false;

    private void Start()
    {
        if (isPlayPortalAnimationOnStart)
        {
            PlayPortalAnimation();
        }
    }

    public void ToggleSkyboxAndPortal()
    {
        isPortalActive = !isPortalActive;
        SetSkyboxAndPortalActive(isPortalActive);
    }

    private void SetSkyboxAndPortalActive(bool active)
    {
        if (portal == null || skybox == null)
        {
            Debug.LogWarning("Skybox未设置!");
            return;
        }
        portal.SetActive(active);
        skybox.SetActive(!active);
    }

    public void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        Debug.Log("OnPlanesChanged");
        if (trackedPlane != null || !isTracking)
        {
            return;
        }
        // find first vertical plane, and set door to its center
        foreach (var plane in args.added)
        {
            Debug.Log("plane.alignment: " + plane.alignment + " " + plane.transform.position);
            if (
                plane.alignment == PlaneAlignment.Vertical
                && plane.classifications.HasFlag(PlaneClassifications.WallFace)
                // plane is in front of the camera
                && Vector3.Dot(plane.transform.position - xrOrigin.Camera.transform.position, xrOrigin.Camera.transform.forward) > 0
                && plane.transform.position.magnitude < 3
                )
            {
                Debug.Log("找到垂直平面: " + plane.alignment + " " + plane.classifications);
                trackedPlane = plane;
                break;
            }
            else
            {
                Debug.Log("未找到垂直平面: " + plane.alignment + " " + plane.classifications);
            }
        }
    }

    public void SetPortalPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (portal == null)
        {
            return;
        }
        portal.transform.SetPositionAndRotation(position, rotation);
    }

    public void PlayPortalAnimation()
    {
        if (portal == null)
        {
            return;
        }
        portal.GetComponentInChildren<Animator>().Play("Play");
    }

    public void UpdateSkyboxMaterial(Material material)
    {

        if (skybox != null)
        {
            skybox.GetComponentInChildren<MeshRenderer>().material = material;
        }
        if (portal != null)
        {
            portal.GetNamedChild("CutSphere").GetComponent<MeshRenderer>().material = material;
        }
    }
}
