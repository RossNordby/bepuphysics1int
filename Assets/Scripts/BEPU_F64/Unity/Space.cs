using UnityEngine;
using FixMath.NET;

namespace BEPUUnity
{
    public class Space : MonoBehaviour
    {
        // we need a serialized Vector3
        [SerializeField] private Fix64 m_gravity;

        private BEPUphysics.Space m_space;

        private void Awake()
        {
            m_space = new BEPUphysics.Space();
            m_space.ForceUpdater.gravity = new BEPUutilities.Vector3(Fix64.Zero, m_gravity, Fix64.Zero);
        }

        private void Update()
        {
            m_space.Update();
        }

        public void Add(ShapeBase shape)
        {
            m_space.Add(shape.GetEntity());
        }
    }
}
