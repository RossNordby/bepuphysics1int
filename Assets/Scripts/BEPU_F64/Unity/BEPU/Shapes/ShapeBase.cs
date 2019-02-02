using UnityEngine;
using BEPUphysics.Entities;

namespace BEPUUnity
{
    public abstract class ShapeBase : MonoBehaviour
    {
        [SerializeField] protected BEPUutilities.Vector3 m_startPosition;
        [SerializeField] protected BEPUutilities.Quaternion m_startOrientation;
        [SerializeField] protected Fix32 m_mass;

        protected Entity m_entity = null;

		private Space space;

        public Entity GetEntity()
        {
            return m_entity;
		}

		protected abstract void SetEntity();

		private void OnEnable() {
			if (m_entity == null) SetEntity();
			if (null == space) space = GetComponentInParent<Space>();
			// register to a space
			space.Add(this);
		}

		private void OnDisable() {
			// register to a space
			space.Remove(this);
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
                transform.position = new Vector3(m_entity.position.X.ToFloat(), m_entity.position.Y.ToFloat(), m_entity.position.Z.ToFloat());
                transform.rotation = new Quaternion(m_entity.orientation.X.ToFloat(), m_entity.orientation.Y.ToFloat(), m_entity.orientation.Z.ToFloat(), m_entity.orientation.W.ToFloat());
            }
        }
    }
}
