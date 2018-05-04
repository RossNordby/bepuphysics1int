using FixMath.NET;
using UnityEngine;
using BEPUphysics.Entities;

namespace BEPUUnity
{
    [ExecuteInEditMode]
    public abstract class ShapeBase : MonoBehaviour
    {
        [SerializeField] protected BEPUutilities.Vector3 m_startPosition;
        [SerializeField] protected Fix64 m_mass;

        protected Entity m_entity = null;

        public Entity GetEntity()
        {
            return m_entity;
        }

        private void Update()
        {
            if(!Application.isPlaying)
            {
                m_startPosition.X = (Fix64)transform.position.x;
                m_startPosition.Y = (Fix64)transform.position.y;
                m_startPosition.Z = (Fix64)transform.position.z;
            }
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                transform.position = new Vector3((float)m_entity.position.X, (float)m_entity.position.Y, (float)m_entity.position.Z);
                transform.rotation = new Quaternion((float)m_entity.orientation.X, (float)m_entity.orientation.Y, (float)m_entity.orientation.Z, (float)m_entity.orientation.W);
            }
        }
    }
}