using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    [SerializeField]
    GameObject m_Prefab;

    void Start()
    {
        Instantiate(m_Prefab, Vector3.zero, Quaternion.identity, transform);
    }
}
