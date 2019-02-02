using UnityEngine;
using BEPUphysics.Entities;

namespace BEPUUnity
{
    [ExecuteInEditMode]
    public abstract class ShapeBase : MonoBehaviour
    {
        [SerializeField] protected BEPUutilities.Vector3 m_startPosition;
        [SerializeField] protected BEPUutilities.Quaternion m_startOrientation;
        [SerializeField] protected Fix32 m_mass;

        protected Entity m_entity = null;

        public Entity GetEntity()
        {
            return m_entity;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
				CopyTransformToBEPU();

			}
        }

		[ContextMenu("CopyTransformToBEPU")]
		private void CopyTransformToBEPU() {
			m_startPosition.X = transform.position.x.ToFix32();
			m_startPosition.Y = transform.position.y.ToFix32();
			m_startPosition.Z = transform.position.z.ToFix32();

			m_startOrientation.X = transform.rotation.x.ToFix32();
			m_startOrientation.Y = transform.rotation.y.ToFix32();
			m_startOrientation.Z = transform.rotation.z.ToFix32();
			m_startOrientation.W = transform.rotation.w.ToFix32();
		}
#endif

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
