using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A large number of blocks fall from the sky into stacks and deactivate.
    /// </summary>
    public class SleepModeDemo : StandardDemo
    {
        private int rowIndex;
        private int updatesSinceLastRow;

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SleepModeDemo(DemosGame game)
            : base(game)
        {
            SpawnRow();

            game.Camera.Position = new Vector3((-30).ToFix(), 30.ToFix(), (-30).ToFix());
            game.Camera.Yaw((-3 * Math.PI / 4).ToFix());
            game.Camera.Pitch(((-Math.PI).ToFix()).Div(12.ToFix()));
        }

        void SpawnRow()
        {
            //Create a bunch of blocks.
            int zOffset = 5;

            int numColumns = 20;

            Fix32 damping = 0.3m.ToFix();
            Fix32 verticalOffsetPerColumn = 0.5m.ToFix();
            Fix32 verticalSpacing = 1.5m.ToFix();

            Entity toAdd;
            for (int j = 0; j < numColumns; j++)
            {
                for (int k = 1; k <= 7; k++)
                {
                    if (k % 2 == 1)
                    {
                        toAdd = new Box(new Vector3((j * 10 + -3).ToFix(), ((-0.5m).ToFix().Add(j.ToFix().Mul(verticalOffsetPerColumn))).Add(verticalSpacing.Mul(k.ToFix())), (rowIndex * 10 + zOffset).ToFix()), 1.ToFix(), 1.ToFix(), 7.ToFix(), 20.ToFix());
                        toAdd.LinearDamping = damping;
                        toAdd.AngularDamping = damping;
                        Space.Add(toAdd);
                        Game.ModelDrawer.Add(toAdd);
                        toAdd.Tag = "noDisplayObject";
                        toAdd = new Box(new Vector3((j * 10 + 3).ToFix(), ((-0.5m).ToFix().Add(j.ToFix().Mul(verticalOffsetPerColumn))).Add(verticalSpacing.Mul(k.ToFix())), (rowIndex * 10 + zOffset).ToFix()), 1.ToFix(), 1.ToFix(), 7.ToFix(), 20.ToFix());
                        toAdd.LinearDamping = damping;
                        toAdd.AngularDamping = damping;
                        Space.Add(toAdd);
                        Game.ModelDrawer.Add(toAdd);
                        toAdd.Tag = "noDisplayObject";
                    }
                    else
                    {
                        toAdd = new Box(new Vector3((j * 10 + 0).ToFix(), ((-0.5m).ToFix().Add(j.ToFix().Mul(verticalOffsetPerColumn))).Add(verticalSpacing.Mul(k.ToFix())), (rowIndex * 10 + zOffset - 3).ToFix()), 7.ToFix(), 1.ToFix(), 1.ToFix(), 20.ToFix());
                        toAdd.LinearDamping = damping;
                        toAdd.AngularDamping = damping;
                        Space.Add(toAdd);
                        Game.ModelDrawer.Add(toAdd);
                        toAdd.Tag = "noDisplayObject";
                        toAdd = new Box(new Vector3((j * 10 + 0).ToFix(), ((-0.5m).ToFix().Add(j.ToFix().Mul(verticalOffsetPerColumn))).Add(verticalSpacing.Mul(k.ToFix())), (rowIndex * 10 + zOffset + 3).ToFix()), 7.ToFix(), 1.ToFix(), 1.ToFix(), 20.ToFix());
                        toAdd.LinearDamping = damping;
                        toAdd.AngularDamping = damping;
                        Space.Add(toAdd);
                        Game.ModelDrawer.Add(toAdd);
                        toAdd.Tag = "noDisplayObject";
                    }
                }
            }
            var ground = new Box(new Vector3((10 * numColumns * 0.5m - 5).ToFix(), (-.5m).ToFix(), (rowIndex * 10 + zOffset).ToFix()), (10 * numColumns).ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(ground);
            Game.ModelDrawer.Add(ground);
            ground.Tag = "noDisplayObject";
            ++rowIndex;
        }

        public override void Update(Fix32 dt)
        {
            if (rowIndex < 20)
            {
                updatesSinceLastRow++;
                if (updatesSinceLastRow > 120)
                {
                    updatesSinceLastRow = 0;
                    SpawnRow();
                }
            }
            base.Update(dt);
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Sleep Mode Stress Test"; }
        }
    }
}