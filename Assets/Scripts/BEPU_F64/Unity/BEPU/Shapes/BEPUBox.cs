
using UnityEngine;
using BEPUphysics.Entities.Prefabs;

namespace BEPUUnity
{
    public class BEPUBox : ShapeBase
    {
        [SerializeField] private Fix32 m_width = Fix32.One;
        [SerializeField] private Fix32 m_height = Fix32.One;
        [SerializeField] private Fix32 m_length = Fix32.One;

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
