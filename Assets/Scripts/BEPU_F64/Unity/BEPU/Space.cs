using UnityEngine;

namespace BEPUUnity
{
    public class Space : MonoBehaviour
    {
        // we need a serialized Vector3
        [SerializeField] private BEPUutilities.Vector3 m_gravity;

        private BEPUphysics.Space m_space;

        private void Awake()
        {
            m_space = new BEPUphysics.Space();
            m_space.ForceUpdater.gravity = m_gravity;
        }

        private void FixedUpdate()
        {
            m_space.Update();
		}

		public void Add(ShapeBase shape) {
			m_space.Add(shape.GetEntity());
		}

		public void Remove(ShapeBase shape) {
			m_space.Remove(shape.GetEntity());
		}
	}
}
