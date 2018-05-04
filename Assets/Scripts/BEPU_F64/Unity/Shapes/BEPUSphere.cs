using FixMath.NET;
using UnityEngine;
using BEPUphysics.Entities.Prefabs;

namespace BEPUUnity
{
    public class BEPUSphere : ShapeBase
    {
        [SerializeField] private Fix64 m_radius;

        private void Awake()
        {
            var position = new BEPUutilities.Vector3((Fix64)transform.position.x, (Fix64)transform.position.y, (Fix64)transform.position.z);
            m_entity = new Sphere(position, m_radius, m_mass);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), (float)m_radius);
        }
#endif
    }
}