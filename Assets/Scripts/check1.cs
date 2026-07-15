using UnityEngine;

public class check1 : MonoBehaviour
{
    public GameObject confettiExplosion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            confettiExplosion.SetActive(true);
            
            ParticleSystem ps = confettiExplosion.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }
    }
}