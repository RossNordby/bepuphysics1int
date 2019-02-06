#if WINDOWS
using System.Collections.Generic;
using BEPUik;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{

    /// <summary>
    /// Test environment for the inverse kinematics solver.
    /// </summary>
    public class InverseKinematicsTestDemo2 : StandardDemo
    {



        void BuildStick(Vector3 position, int linkCount, out List<Bone> bones, out List<Entity> boneEntities)
        {
            //Set up a bone chain.
            bones = new List<Bone>();
            boneEntities = new List<Entity>();
            var previousBoneEntity = new Cylinder(position, 1.ToFix(), .2m.ToFix());
            var previousBone = new Bone(previousBoneEntity.Position, previousBoneEntity.Orientation, previousBoneEntity.Radius, previousBoneEntity.Height);
            bones.Add(previousBone);
            boneEntities.Add(previousBoneEntity);

            
            for (int i = 1; i < linkCount; i++)
            {
                var boneEntity = new Cylinder(previousBone.Position + new Vector3(0.ToFix(), 1.ToFix(), 0.ToFix()), 1.ToFix(), .2m.ToFix());
                var bone = new Bone(boneEntity.Position, boneEntity.Orientation, boneEntity.Radius, boneEntity.Height);
                bones.Add(bone);
                boneEntities.Add(boneEntity);

                //Make a relationship between the two bones and entities.
                CollisionRules.AddRule(previousBoneEntity, boneEntity, CollisionRule.NoBroadPhase);
                Vector3 anchor = (previousBoneEntity.Position + boneEntity.Position) / 2.ToFix();
                //var dynamicsBallSocketJoint = new BallSocketJoint(previousBoneEntity, boneEntity, anchor);
                //var dynamicsAngularFriction = new NoRotationJoint(previousBoneEntity, boneEntity);
                //Space.Add(dynamicsBallSocketJoint);
                //Space.Add(dynamicsAngularFriction);
                var ballSocket = new IKBallSocketJoint(previousBone, bone, anchor);
                var angularJoint = new IKAngularJoint(previousBone, bone);

   
                previousBone = bone;
                previousBoneEntity = boneEntity;
            }
        }


        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public InverseKinematicsTestDemo2(DemosGame game)
            : base(game)
        {
            game.Camera.Position = new Vector3(0.ToFix(), 3.ToFix(), 5.ToFix());
            Box ground = new Box(new Vector3(0.ToFix(), (-3).ToFix(), 0.ToFix()), 30.ToFix(), 1.ToFix(), 30.ToFix());
            Space.Add(ground);
            Space.ForceUpdater.Gravity = new Vector3(0.ToFix(), (-9.81m).ToFix(), 0.ToFix());

            var solver = new IKSolver();

            solver.ActiveSet.UseAutomass = true;
            //solver.AutoscaleControlImpulses = true;
            //solver.AutoscaleControlMaximumForce = Fix64.MaxValue;
            solver.ControlIterationCount = 20;
            solver.FixerIterationCount = 0;
            solver.VelocitySubiterationCount = 3;


            List<Bone> bones;
            List<Entity> boneEntities;
            int boneCount = 10;
            BuildStick(new Vector3(0.ToFix(), 0.5m.ToFix(), 0.ToFix()), boneCount, out bones, out boneEntities);

            DragControl dragger = new DragControl { TargetBone = bones[boneCount - 1], MaximumForce = Fix32.MaxValue };
            dragger.LinearMotor.Rigidity = 16.ToFix();
            dragger.LinearMotor.LocalOffset = new Vector3(0.ToFix(), 0.5m.ToFix(), 0.ToFix());
            dragger.LinearMotor.TargetPosition = new Vector3(10.ToFix(), 0.ToFix(), 0.ToFix());


            bones[0].Pinned = true;

            var controls = new List<Control>();
            controls.Add(dragger);

            solver.Solve(controls);

            var tipLocation = bones[boneCount - 1].Position + Matrix3x3.CreateFromQuaternion(bones[boneCount - 1].Orientation).Up * 0.5m.ToFix();

            for (int i = 0; i < bones.Count; ++i)
            {
                boneEntities[i].Position = bones[i].Position;
                boneEntities[i].Orientation = bones[i].Orientation;
                Space.Add(boneEntities[i]);
            }



        }




        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Inverse Kinematics Test 2"; }
        }

    }
}
#endif