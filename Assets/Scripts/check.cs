using UnityEngine;

public class check : MonoBehaviour
{
    public GameObject confettiExplosion;
  
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu bóng chạm vào goal
        if (other.CompareTag("Ball"))
        {
            TriggerConfetti();
        }
    }

    void TriggerConfetti()
    {
        confettiExplosion.SetActive(true);
        ParticleSystem ps1 = confettiExplosion.GetComponent<ParticleSystem>();
        
        if (ps1 != null) ps1.Play();
    }
}