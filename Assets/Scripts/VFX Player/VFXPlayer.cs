using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnedPrefab = new();
    [SerializeField] private ParticleSystem particle;

    public void PlayVFX(GameObject particleToSpawn, float deleteTime)
    {
        GameObject vfx = Instantiate(particleToSpawn, transform.position, Quaternion.identity);

        spawnedPrefab.Add(vfx);

        StartCoroutine(DestroyParticle(vfx, deleteTime));
    }

    private IEnumerator DestroyParticle(GameObject vfx, float deleteTime)
    {
        yield return new WaitForSeconds(deleteTime);

        spawnedPrefab.Remove(vfx);

        Destroy(vfx);
    }

    public void PlayVFX(ParticleSystem particleSystemToPlay)
    {
        this.particle = particleSystemToPlay;
        particle.Play();
    }
}