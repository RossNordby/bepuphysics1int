using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionShapes;
using System.Collections.Generic;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class MutableCompoundDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public MutableCompoundDemo(DemosGame game)
            : base(game)
        {

            List<CompoundShapeEntry> shapes = new List<CompoundShapeEntry>();
            Fix64 totalWeight = 0.ToFix();
            Fix64 density = 10.ToFix();


            Fix64 weight = density.Mul(2.ToFix());
            totalWeight += weight;
            for (int i = 0; i < 4; i++)
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(1.ToFix(), 1.ToFix(), 2.ToFix()),
                    new RigidTransform(
                    new Vector3((-.5m + i * 1).ToFix(), 0.ToFix(), 15.ToFix()),
                    Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2.Mul(2.ToFix())))
                    //Quaternion.Identity)
                    , weight));
            }



            var compound = new CompoundBody(shapes, totalWeight);
            //compound.Orientation = Quaternion.CreateFromYawPitchRoll(1, 1, 1);
            //compound.AngularVelocity = new Vector3(0, 1, 0);
            Entity<CompoundCollidable> compound2, compound3;
            CompoundHelper.SplitCompound(x => x.ShapeIndex >= shapes.Count / 2, compound, out compound2);
            CompoundHelper.SplitCompound(x => x.ShapeIndex >= 3 * shapes.Count / 4, compound2, out compound3);



            //compound.ActivityInformation.IsAlwaysActive = true;
            //compound.IsAffectedByGravity = false;
            //compound2.ActivityInformation.IsAlwaysActive = true;
            //compound2.IsAffectedByGravity = false;
            //compound3.ActivityInformation.IsAlwaysActive = true;
            //compound3.IsAffectedByGravity = false;
            //compound.Tag = "noDisplayObject";
            Space.Add(compound);
            Space.Add(compound2);
            Space.Add(compound3);


            int width = 3;
            int height = 3;
            int length = 10;
            Fix64 blockWidth = 1.ToFix();
            Fix64 blockHeight = 1.ToFix();
            Fix64 blockLength = 1.ToFix();



            for (int q = 0; q < 1; q++)
            {
                shapes.Clear();
                totalWeight = 0.ToFix();
                density = 1.ToFix();

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            weight = ((density.Mul(blockWidth)).Mul(blockLength)).Mul(blockHeight);
                            totalWeight += weight;
                            shapes.Add(new CompoundShapeEntry(
                                new BoxShape(blockWidth, blockHeight, blockLength),
                                new RigidTransform(
                                new Vector3((5 + q * 20).ToFix().Add(i.ToFix().Mul(blockWidth)), j.ToFix().Mul(blockHeight), k.ToFix().Mul(blockLength)),
                                //Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2))
                                Quaternion.Identity)
                                , weight));
                        }
                    }
                }

                compound = new CompoundBody(shapes, totalWeight);
                //compound.Orientation = Quaternion.CreateFromYawPitchRoll(1, 1, 1);
                //compound.AngularVelocity = new Vector3(0, 1, 0);
                CompoundHelper.SplitCompound(x => x.ShapeIndex >= shapes.Count / 2, compound, out compound2);
                CompoundHelper.SplitCompound(x => x.ShapeIndex >= 3 * shapes.Count / 4, compound2, out compound3);



                //compound.ActivityInformation.IsAlwaysActive = true;
                //compound.IsAffectedByGravity = false;
                //compound2.ActivityInformation.IsAlwaysActive = true;
                //compound2.IsAffectedByGravity = false;
                //compound3.ActivityInformation.IsAlwaysActive = true;
                //compound3.IsAffectedByGravity = false;
                //compound.Tag = "noDisplayObject";
                Space.Add(compound);
                Space.Add(compound2);
                Space.Add(compound3);

            }

            Box ground = new Box(new Vector3(0.ToFix(), (-4.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix());
            Space.Add(ground);
            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Mutable Compound Test"; }
        }
    }
}