using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HandCollider : MonoBehaviour
{
  [SerializeField] private Collider collisionComponent;


  private void OnValidate()
  {
    // 自动获取并设置 Collider 和 AudioSource
    if (collisionComponent == null)
    {
      collisionComponent = GetComponent<Collider>();
    }
  }

  private void OnCollisionEnter(Collision other)
  {
    Debug.Log("OnCollisionEnter");
    if (other.gameObject.GetComponentInParent<BeatBoxCollider>() is BeatBoxCollider beatBoxCollider)
    {
      if (beatBoxCollider.isHit)
      {
        return;
      }
      Debug.Log("BeatBoxCollider found on the collision object");
      // 调用 BeatBoxCollider 对象的 Beat 方法
      beatBoxCollider.Beat();
    }
    else
    {
      Debug.Log("No BeatBoxCollider found on the collision object");
    }
  }
}
