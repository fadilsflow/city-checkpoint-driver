using UnityEngine;

public class coint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin berhasil diambil");
            Destroy(gameObject);
        }
    }
}