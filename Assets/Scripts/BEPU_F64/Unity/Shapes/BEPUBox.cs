using FixMath.NET;
using UnityEngine;
using BEPUphysics.Entities.Prefabs;

namespace BEPUUnity
{
    public class BEPUBox : ShapeBase
    {
        [SerializeField] private Fix64 m_width;
        [SerializeField] private Fix64 m_height;
        [SerializeField] private Fix64 m_length;

        private void Awake()
        {
            m_entity = new Box(m_startPosition, m_width, m_height, m_length, m_mass);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, 0.0f), (float)m_width);
        }
#endif
    }
}