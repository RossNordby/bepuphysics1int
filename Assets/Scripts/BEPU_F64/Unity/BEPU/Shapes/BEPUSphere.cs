using UnityEngine;
using BEPUphysics.Entities.Prefabs;

namespace BEPUUnity
{
    public class BEPUSphere : ShapeBase
    {
        [SerializeField] private Fix32 m_radius = Fix32.One;

        private void Awake()
        {
            m_entity = new Sphere(m_startPosition, m_radius, m_mass);
            m_entity.Orientation = m_startOrientation;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ((float)m_mass > 0) ? Color.green : Color.red;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, (float)m_radius);
        }
#endif
    }
}