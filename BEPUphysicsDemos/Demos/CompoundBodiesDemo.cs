using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Collections.Generic;
using System.Diagnostics;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Compound bodies are created from other entities to make concave shapes.
    /// </summary>
    public class CompoundBodiesDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public CompoundBodiesDemo(DemosGame game)
            : base(game)
        {

            //Build the first body
            var bodies = new List<CompoundShapeEntry>
            {
                new CompoundShapeEntry(new SphereShape(.5m.ToFix()), new Vector3(0.ToFix(), 1.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new ConeShape(2.ToFix(), .5m.ToFix()), new Vector3(1.ToFix(), 1.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new SphereShape(.5m.ToFix()), new Vector3((-1).ToFix(), 1.ToFix(), 0.ToFix()), 1.ToFix())
            };
            var cb1 = new CompoundBody(bodies, 45.ToFix());



            //Build the second body
            bodies = new List<CompoundShapeEntry>
            {
                new CompoundShapeEntry(new BoxShape(1.ToFix(),1.ToFix(),1.ToFix()), new Vector3(0.ToFix(), 3.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new BoxShape(1.ToFix(),1.ToFix(),1.ToFix()), new Vector3(1.ToFix(), 3.5m.ToFix(), 0.ToFix()), 1.ToFix()),
            };
            var cb2 = new CompoundBody(bodies, 4.ToFix());

            bodies = new List<CompoundShapeEntry>();
            //Build the third Braum's-fry style body
            for (int k = 0; k < 7; k++)
            {
                bodies.Add(new CompoundShapeEntry(new BoxShape(1.ToFix(), 1.ToFix(), 1.ToFix()), new Vector3((-4 + k * .7m).ToFix(), (2 + .7m * k).ToFix(), (2 + k * .2m).ToFix()), 1.ToFix()));
            }
            var cb3 = new CompoundBody(bodies, 7.ToFix());

            //Add them all to the space
            Space.Add(cb3);
            Space.Add(cb2);
            Space.Add(cb1);



            Space.Add(new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 10.ToFix(), 1.ToFix(), 10.ToFix()));
            game.Camera.Position = new Vector3(0.ToFix(), 3.ToFix(), 15.ToFix());

        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Compound Bodies"; }
        }
    }
}