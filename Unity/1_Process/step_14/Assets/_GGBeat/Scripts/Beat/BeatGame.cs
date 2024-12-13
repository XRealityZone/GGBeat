using UnityEngine;
using System.Collections;
using UnityEngine.XR.Hands;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Management;
using GGBeat;

/// <summary>
///  打击游戏的主控制器
/// </summary>
public class BeatGame : MonoBehaviour
{
    public Material[] skyboxMaterials;


    public TextAsset[] beatMapJsons;
    /// <summary>
    /// Background music to play during the game
    /// </summary>
    public AudioClip[] backgroundMusics;

    /// <summary>
    /// Box Prefab to spawn in the game
    /// </summary>
    public GameObject boxPrefab;

    /// <summary>
    /// Distance between the camera and the box when it spawns
    /// </summary>
    public float spawnDistance = 6f;

    /// <summary>
    /// Duration of the box's movement
    /// </summary>
    public float moveDuration = 3f;

    public float waitARPlaneDuration = 2f;

    public GameObject leftHandVisual;
    public GameObject rightHandVisual;

    public SkyBoxManager skyBoxManager;

    private XRHandSubsystem handSubsystem;
    public XROrigin xrOrigin;

    private const float BPM = 120f;

    private const float baseYOffset = 1.3f;
    private const float SECONDS_PER_BEAT = 60f / BPM;

    private AudioSource audioSource;
    private BeatMapData beatMapData;
    private float startTime;
    private int currentNoteIndex = 0;

    private Transform initialCameraTransform;

    private float totalTime = 0;
    private bool isGameStarted = false;

    private void Start()
    {
        InitializeXRAndHandTracking();
        initialCameraTransform = CloneTransform(xrOrigin.Camera.transform);
        initialCameraTransform.rotation = Quaternion.Euler(0, initialCameraTransform.rotation.eulerAngles.y, 0);
        LoadBeatMapData((int)AppManager.Instance.songType);
        LoadSong((int)AppManager.Instance.songType);
        LoadSkyboxMaterial((int)AppManager.Instance.sceneType);
    }

    private void Update()
    {
        UpdateHandPositions();
        CheckGameStart();
    }

    private void CheckGameStart()
    {
        if (isGameStarted)
        {
            return;
        }
        totalTime += Time.deltaTime;
        if (skyBoxManager == null || totalTime > waitARPlaneDuration)
        {
            Transform portalTransform = CloneTransform(initialCameraTransform.transform);
            Debug.Log("portalTransform.position: " + portalTransform.position);
            portalTransform.position += initialCameraTransform.forward * 2f;
            Debug.Log("portalTransform.position: " + portalTransform.position);
            portalTransform.position += Vector3.up * baseYOffset;
            // make portal green axis look at camera
            portalTransform.rotation = Quaternion.LookRotation(Vector3.up, initialCameraTransform.forward.Inverse());
            Debug.Log("portalTransform.rotation: " + portalTransform.rotation);
            skyBoxManager.SetPortalPositionAndRotation(portalTransform.position, portalTransform.rotation);
            skyBoxManager.PlayPortalAnimation();
            skyBoxManager.isTracking = false;
            isGameStarted = true;
            StartCoroutine(StartGame());
            Debug.Log("游戏开始,使用的是默认位置");
        }
        else if (skyBoxManager.trackedPlane != null)
        {
            Transform portalTransform = CloneTransform(skyBoxManager.trackedPlane.transform);
            skyBoxManager.SetPortalPositionAndRotation(portalTransform.position, portalTransform.rotation);
            initialCameraTransform.LookAt(portalTransform);
            // keep the camera's forward is the horizontal
            initialCameraTransform.rotation = Quaternion.Euler(0, initialCameraTransform.rotation.eulerAngles.y, 0);
            skyBoxManager.PlayPortalAnimation();
            skyBoxManager.isTracking = false;
            isGameStarted = true;
            StartCoroutine(StartGame());
            Debug.Log("游戏开始,使用的是跟踪到的平面");
        }
        else
        {
            // 等到下一次更新
        }
    }

    private void LoadSong(int songIndex)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusics[songIndex];
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void LoadSkyboxMaterial(int sceneIndex)
    {

        Material material = skyboxMaterials[sceneIndex];
        skyBoxManager.UpdateSkyboxMaterial(material);
    }

    private void LoadBeatMapData(int index)
    {
        if (beatMapJsons.Length > 0)
        {
            beatMapData = JsonUtility.FromJson<BeatMapData>(beatMapJsons[index].text);
            Debug.Log($"加载了 {beatMapData._notes.Count} 个音符");
        }
        else
        {
            Debug.LogError("未设置 Beat Map JSON 文件");
        }
    }

    private void InitializeXRAndHandTracking()
    {
        if (xrOrigin == null)
        {
            Debug.LogError("未找到 XROrigin");
            return;
        }

        var xrGeneralSettings = XRGeneralSettings.Instance;
        if (xrGeneralSettings == null)
        {
            Debug.LogError("XR general settings not set");
        }

        var manager = xrGeneralSettings.Manager;
        if (manager != null)
        {
            var loader = manager.activeLoader;
            if (loader != null)
            {
                handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                if (!CheckHandSubsystem())
                    return;

                handSubsystem.Start();
            }
            else
            {
                Debug.LogError("未找到 loader");
            }
        }
        else
        {
            Debug.LogError("未找到 xrGeneralSettings.Manager");
        }
    }

    bool CheckHandSubsystem()
    {
        if (handSubsystem == null)
        {
            Debug.LogError("Could not find Hand Subsystem");
            return false;
        }

        return true;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        startTime = Time.time;
        audioSource.Play();
        StartCoroutine(SpawnBoxes());
    }

    private IEnumerator SpawnBoxes()
    {
        while (currentNoteIndex < beatMapData._notes.Count)
        {
            BeatMapNote note = beatMapData._notes[currentNoteIndex];
            float spawnTime = note._time * SECONDS_PER_BEAT;

            while (Time.time - startTime < spawnTime)
            {
                yield return null;
            }

            SpawnBox(note);
            currentNoteIndex++;
        }
    }

    private void SpawnBox(BeatMapNote note)
    {
        Vector3 spawnPosition = CalculateSpawnPosition(note);
        GameObject box = Instantiate(boxPrefab, spawnPosition, Quaternion.identity);
        // make box look at xrOrigin， only rotate on the y axis
        box.transform.rotation = Quaternion.LookRotation(-initialCameraTransform.forward, Vector3.up);
        StartCoroutine(MoveBox(box));
    }

    private Vector3 CalculateSpawnPosition(BeatMapNote note)
    {
        float xOffset = (note._lineIndex - 1.5f) * 0.5f;
        float yOffset = note._lineLayer * 0.1f + baseYOffset;

        Transform origin = initialCameraTransform;
        return origin.position +
               origin.right * xOffset +
               origin.up * yOffset +
               origin.forward * spawnDistance;
    }

    private IEnumerator MoveBox(GameObject box)
    {
        Vector3 startPosition = box.transform.position;
        Vector3 endPosition = initialCameraTransform.position -
                              initialCameraTransform.forward * spawnDistance +
                              initialCameraTransform.up * baseYOffset;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration && !box.GetComponent<BeatBoxCollider>().isHit)
        {
            if (box == null)
            {
                yield return null;
            }
            box.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5.0f);

        Destroy(box);
    }

    private void UpdateHandPositions()
    {
        if (CheckHandSubsystem())
        {
            UpdateHandVisual(leftHandVisual, handSubsystem.leftHand);
            UpdateHandVisual(rightHandVisual, handSubsystem.rightHand);
        }
    }

    private void UpdateHandVisual(GameObject handVisual, XRHand hand)
    {
        if (handVisual != null && hand.isTracked)
        {
            handVisual.transform.position = hand.rootPose.position;
            handVisual.transform.rotation = hand.rootPose.rotation;
        }
    }

    private Transform CloneTransform(Transform transform)
    {
        Transform clone = new GameObject("Clone").transform;
        clone.position = transform.position;
        clone.rotation = transform.rotation;
        clone.localScale = transform.localScale;
        return clone;
    }
}
