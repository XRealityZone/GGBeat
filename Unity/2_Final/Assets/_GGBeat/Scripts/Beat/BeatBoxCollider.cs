using UnityEngine;

public class BeatBoxCollider : MonoBehaviour
{
  [SerializeField] private Collider collisionComponent;
  [SerializeField] private ParticleSystem m_particleSystem;

  [SerializeField] private GameObject m_mesh;

  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip hitSound;

  [HideInInspector]
  public bool isHit = false;

  public void Beat()
  {
    isHit = true;
    // 在这里实现击中效果
    Debug.Log("Box被击中了!");

    // 触发粒子效果
    if (m_particleSystem != null)
    {
      m_particleSystem.Play();
    }
    // 播放音效
    if (audioSource != null && hitSound != null)
    {
      audioSource.PlayOneShot(hitSound);
      Debug.Log("音效播放了");
    }
    Destroy(m_mesh);
  }
}
