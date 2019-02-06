using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Checks various corner cases related to kinematic entities and the deactivation system.
    /// </summary>
    public class KinematicActivityTestDemo : StandardDemo
    {
        Box teleportingBox;
        Box movingBox;
        Box boxToDelete;
        Box alwaysActiveBox;

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public KinematicActivityTestDemo(DemosGame game)
            : base(game)
        {

            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix()));

            //Kinematic objects, by default, rely on velocity to define activity.
            //If a kinematic object teleports every frame, it never has velocity.
            //However, it should still wake up anything that it collides with, including sleeping dynamic objects.
            teleportingBox = new Box(new Vector3((-12).ToFix(), 3.ToFix(), 10.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix());
            Space.Add(teleportingBox);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Space.Add(new Box(new Vector3((i * 2 - 8).ToFix(), (j * 2 + 4).ToFix(), (k * 2 + 10).ToFix()), 2.ToFix(), 2.ToFix(), 2.ToFix(), 4.ToFix()));
                    }
                }
            }

            //Create a stack for the 'add kinematic to space' test.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Space.Add(new Box(new Vector3((i * 2 - 8).ToFix(), (j * 2 + 4).ToFix(), (k * 2 - 10).ToFix()), 2.ToFix(), 2.ToFix(), 2.ToFix(), 4.ToFix()));
                    }
                }
            }

            //Create a stack for carrying with a moving kinematic.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Space.Add(new Box(new Vector3((i * 2 + 8).ToFix(), (j * 2 + 6).ToFix(), (k * 2 + 10).ToFix()), 2.ToFix(), 2.ToFix(), 2.ToFix(), 4.ToFix()));
                    }
                }
            }
            movingBox = new Box(new Vector3(10.ToFix(), 4.ToFix(), 12.ToFix()), 10.ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(movingBox);

            //Create a stack for the deletion test.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Space.Add(new Box(new Vector3((i * 2 + 8).ToFix(), (j * 2 + 6).ToFix(), (k * 2 - 10).ToFix()), 2.ToFix(), 2.ToFix(), 2.ToFix(), 4.ToFix()));
                    }
                }
            }
            boxToDelete = new Box(new Vector3(10.ToFix(), 4.ToFix(), (-8).ToFix()), 10.ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(boxToDelete);

            //Create a stack for the always-active test.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Space.Add(new Box(new Vector3((i * 2 - 8).ToFix(), (j * 2 + 15).ToFix(), (k * 2 - 10).ToFix()), 2.ToFix(), 2.ToFix(), 2.ToFix(), 4.ToFix()));
                    }
                }
            }
            alwaysActiveBox = new Box(new Vector3((-6).ToFix(), 12.5m.ToFix(), (-8).ToFix()), 10.ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(alwaysActiveBox);


            game.Camera.Position = new Vector3(0.ToFix(), 7.ToFix(), 30.ToFix());

        }


        public override void Update(Fix32 dt)
        {
            base.Update(dt);
            //Teleport the box sideways into the sleeping dynamic objects.
            if (Game.KeyboardInput.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad0))
                teleportingBox.Position += new Vector3(.1m.ToFix(), 0.ToFix(), 0.ToFix());
            //Moving the object after things relying on it have gone to sleep.
            if (Game.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
            {
                if (movingBox.LinearVelocity.LengthSquared() == 0.ToFix())
                    movingBox.LinearVelocity = new Vector3(1.ToFix(), 0.ToFix(), 0.ToFix());
                else
                    movingBox.LinearVelocity = new Vector3();
            }

            //Adding a new kinematic after the stack of objects goes to sleep.
            if (Game.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
            {
                Box box = new Box(new Vector3((-6).ToFix(), 1.ToFix(), (-8).ToFix()), 10.ToFix(), .1m.ToFix(), 10.ToFix());
                Space.Add(box);
                Game.ModelDrawer.Add(box);
            }

            //Removing a kinematic object from below a sleeping stack of objects.
            if (Game.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3) && boxToDelete.Space != null)
            {
                Space.Remove(boxToDelete);
                Game.ModelDrawer.Remove(boxToDelete);
            }

            //Switching the always-active state of a kinematic.
            if (Game.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad4))
            {
                alwaysActiveBox.ActivityInformation.IsAlwaysActive = !alwaysActiveBox.ActivityInformation.IsAlwaysActive;
            }
        }

        public override void DrawUI()
        {
            base.DrawUI();
            if (teleportingBox.ActivityInformation.IsActive)
                Game.DataTextDrawer.Draw("Teleporting box is active.", new Microsoft.Xna.Framework.Vector2(10, 10));
            else
                Game.DataTextDrawer.Draw("Teleporting box is NOT active.", new Microsoft.Xna.Framework.Vector2(10, 10));
        }
        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Kinematic Activity Tests"; }
        }
    }
}