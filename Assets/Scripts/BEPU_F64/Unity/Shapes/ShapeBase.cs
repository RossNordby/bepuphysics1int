using FixMath.NET;
using UnityEngine;

namespace BEPUUnity
{
    public abstract class ShapeBase : MonoBehaviour
    {
        [SerializeField] private Fix64 m_mass;

        public abstract void OnRegister(Space space);
    }
}