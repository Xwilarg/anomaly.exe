using UnityEngine;
using System.Collections;
using MiniJameGam9.Character;
using MiniJameGam9.SO;

namespace MiniJameGam9.Weapon
{
    public class Projectile : MonoBehaviour
    {
        public ACharacterController Author { set; get; }
        public WeaponInfo Weapon { set; get; }

        private void OnCollisionEnter(Collision collision)
        {
            if (Weapon.ExplosionRadius > 0f)
                StartCoroutine(Explode());
            else
                Damage(collision.collider.transform);
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(Weapon.TimeBeforeExplode);

            Collider[] colliders = Physics.OverlapSphere(transform.position, Weapon.ExplosionRadius);
            foreach (Collider collider in colliders)
                Damage(collider.transform);
        }

        private void Damage(Transform target)
        {
            if (target.CompareTag("Player"))
            {
                Author.Profile.DamageDealt += Weapon.Damage;
                if (target.GetComponent<ACharacterController>().TakeDamage(Weapon.Damage, (Author.transform.position - transform.position).normalized))
                {
                    Author.Profile.Kill++;
                }
            }
            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Weapon.ExplosionRadius);
        }
    }
}
