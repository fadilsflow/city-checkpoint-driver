using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public CheckpointGroup group;
    public int index;
    public GameObject visualRoot;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (visualRoot != null) visualRoot.SetActive(active);
        if (triggerCollider != null) triggerCollider.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled || group == null) return;
        if (other.GetComponentInParent<CarController3D>() == null) return;
        group.HitCheckpoint(this);
    }
}
