using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionRuleManagement;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Some objects pass through each other, showing one way in which collision rules function.
    /// </summary>
    public class CollisionFilteringDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public CollisionFilteringDemo(DemosGame game)
            : base(game)
        {
            Entity toAdd;
            toAdd = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix());
            Space.Add(toAdd);

            //Set up two stacks which go through each other
            var firstStackGroup = new CollisionGroup();
            var secondStackGroup = new CollisionGroup();
            //Adding this rule to the space's collision group rules will prevent entities belong to these two groups from generating collision pairs with each other.
            groupPair = new CollisionGroupPair(firstStackGroup, secondStackGroup);
            CollisionRules.CollisionGroupRules.Add(groupPair, CollisionRule.NoBroadPhase);

            for (int k = 0; k < 10; k++)
            {
                toAdd = new Box(
                    new Vector3((-4 + .12m * k).ToFix(), (.5m + k).ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(),
10.ToFix());
                toAdd.CollisionInformation.CollisionRules.Group = firstStackGroup;
                Space.Add(toAdd);
                toAdd = new Box(new Vector3((4 - .12m * k).ToFix(), (.5m + k).ToFix(), 0.ToFix()),
1.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix());
                toAdd.CollisionInformation.CollisionRules.Group = secondStackGroup;
                Space.Add(toAdd);
            }
            //Add another two boxes which ignore each other using the specific entities method; they will still collide with the stacks since they will have the default dynamic collision group.
            toAdd = new Box(new Vector3(1.ToFix(), 3.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 2.ToFix(), 10.ToFix());
            var toAdd2 = new Box(new Vector3((-1).ToFix(), 3.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 2.ToFix(), 15.ToFix());
            CollisionRules.AddRule(toAdd, toAdd2, CollisionRule.NoBroadPhase);
            Space.Add(toAdd);
            Space.Add(toAdd2);
            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 20.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Collision Filtering"; }
        }

        CollisionGroupPair groupPair;
        public override void CleanUp()
        {
            base.CleanUp();
            //The CollisionGroupRules are static, so this dictionary would fill up
            //if we kept changing the simulation without cleaning it out.
            CollisionRules.CollisionGroupRules.Remove(groupPair);
        }
    }
}