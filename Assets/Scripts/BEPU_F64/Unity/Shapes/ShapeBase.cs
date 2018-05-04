using FixMath.NET;
using UnityEngine;
using BEPUphysics.Entities;

namespace BEPUUnity
{
    public abstract class ShapeBase : MonoBehaviour
    {
        [SerializeField] protected Fix64 m_mass;

        protected Entity m_entity = null;

        public Entity GetEntity()
        {
            return m_entity;
        }

        private void LateUpdate()
        {
            transform.position = new Vector3((float)m_entity.position.X, (float)m_entity.position.Y, (float)m_entity.position.Z);
            transform.rotation = new Quaternion((float)m_entity.orientation.X, (float)m_entity.orientation.Y, (float)m_entity.orientation.Z, (float)m_entity.orientation.W);
        }
    }
}