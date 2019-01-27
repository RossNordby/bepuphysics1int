using FixMath.NET;
using UnityEngine;
using BEPUphysics.Entities.Prefabs;

namespace BEPUUnity
{
    public class BEPUBox : ShapeBase
    {
        [SerializeField] private Fix64 m_width = Fix64.One;
        [SerializeField] private Fix64 m_height = Fix64.One;
        [SerializeField] private Fix64 m_length = Fix64.One;

        private void Awake()
        {
            m_entity = new Box(m_startPosition, m_width, m_height, m_length, m_mass);
            m_entity.Orientation = m_startOrientation;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ((float)m_mass > 0) ? Color.green : Color.red;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, new Vector3((float) m_width, (float) m_height, (float) m_length));
        }
#endif
    }
}
