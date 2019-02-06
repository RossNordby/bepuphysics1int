using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Boxes slide and spheres bounce.
    /// </summary>
    public class CoefficientsDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public CoefficientsDemo(DemosGame game)
            : base(game)
        {
            //The material blender defines how the friction and bounciness values are computed between objects.
            //It defaults to multiplicative, but for the purposes of this demo, we'll switch it to use the smaller friction and the larger bounciness.
            MaterialManager.MaterialBlender = delegate(Material a, Material b, out InteractionProperties properties)
                {
                    properties.KineticFriction = MathHelper.Min(a.KineticFriction, b.KineticFriction);
                    properties.StaticFriction = MathHelper.Min(a.StaticFriction, b.StaticFriction);
                    properties.Bounciness = MathHelper.Max(a.Bounciness, b.Bounciness);
                };
            //Special blenders can be defined between specific Material instances by adding entries to the MaterialManager.MaterialInteractions dictionary.

            //Create the ground
            Entity toAdd = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 20.ToFix(), 1.ToFix(), 20.ToFix());
            Space.Add(toAdd);
            //Bouncy balls 
            toAdd = new Sphere(new Vector3((-8).ToFix(), 10.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.8m.ToFix(), .8m.ToFix(), .95m.ToFix());
            Space.Add(toAdd);
            toAdd = new Sphere(new Vector3((-5).ToFix(), 10.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.8m.ToFix(), .8m.ToFix(), .5m.ToFix());
            Space.Add(toAdd);
            toAdd = new Sphere(new Vector3((-2).ToFix(), 10.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.8m.ToFix(), .8m.ToFix(), .25m.ToFix());
            Space.Add(toAdd);
            //Slide-y boxes
            toAdd = new Box(new Vector3(9.ToFix(), 1.ToFix(), 9.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(0.ToFix(), 0.ToFix(), 0.ToFix());
            toAdd.LinearVelocity = new Vector3((-5).ToFix(), 0.ToFix(), 0.ToFix());
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(9.ToFix(), 1.ToFix(), 5.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.1m.ToFix(), .1m.ToFix(), 0.ToFix());
            toAdd.LinearVelocity = new Vector3((-5).ToFix(), 0.ToFix(), 0.ToFix());
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(9.ToFix(), 1.ToFix(), (-5).ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.2m.ToFix(), .2m.ToFix(), 0.ToFix());
            toAdd.LinearVelocity = new Vector3((-5).ToFix(), 0.ToFix(), 0.ToFix());
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(9.ToFix(), 1.ToFix(), (-9).ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Material = new Material(.5m.ToFix(), .5m.ToFix(), 0.ToFix());
            toAdd.LinearVelocity = new Vector3((-5).ToFix(), 0.ToFix(), 0.ToFix());
            Space.Add(toAdd);
            game.Camera.Position = new Vector3(0.ToFix(), 2.ToFix(), 30.ToFix());

        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Bounciness and Friction"; }
        }

        public override void CleanUp()
        {
            MaterialManager.MaterialBlender = MaterialManager.DefaultMaterialBlender;
            base.CleanUp();
        }
    }
}