using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras
{
    /// <summary>
    /// Demonstrates tuning settings for handling different world scales.
    /// </summary>
    public class ScaleDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ScaleDemo(DemosGame game)
            : base(game)
        {
            //Pick a scale!
            //Beware: If you go too far (particularly 0.01 and lower) issues could start to crop up.
            Fix64 scale = 1.ToFix();

            //Load in mesh data and create the collision mesh.
            //The 'mesh' will be a supergiant triangle.
            //Triangles in meshes have a collision detection system which bypasses most numerical issues for collisions on the face of the triangle.
            //Edge collisions fall back to the general case collision detection system which is susceptible to numerical issues at extreme scales.
            //For our simulation, the edges will be too far away to worry about!
            Vector3[] vertices;
            int[] indices;
            vertices = new Vector3[] { new Vector3((-10000).ToFix(), 0.ToFix(), (-10000).ToFix()), new Vector3((-10000).ToFix(), 0.ToFix(), 20000.ToFix()), new Vector3(20000.ToFix(), 0.ToFix(), (-10000).ToFix()) };
            indices = new int[] { 2, 1, 0 };
            var staticMesh = new StaticMesh(vertices, indices, new AffineTransform(Matrix3x3.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix())));
            staticMesh.Sidedness = TriangleSidedness.Counterclockwise;

            Space.Add(staticMesh);
            game.ModelDrawer.Add(staticMesh);

            //Since everything's pretty large, increase the gravity a whole bunch to make things fall fast.
            Space.ForceUpdater.Gravity *= scale;

            //Change the various engine tuning factors so that collision detection and collision response handle the changed scale better.
            ConfigurationHelper.ApplyScale(Space, scale);

            //When dealing with objects that generally have high velocities and accelerations relative to their size, having a shorter time step duration can boost quality
            //a whole lot.  Once the configuration is set properly, most of any remaining 'unsmoothness' in the simulation is due to a lack of temporal resolution; 
            //one discrete step can take an object from a valid state to an unpleasing state due to the high rates of motion.  
            //To simulate the same amount of time with a smaller time step duration requires taking more time steps.
            //This is a quality-performance tradeoff.  If you want to do this, set the time step duration like so:

            //Space.TimeStepSettings.TimeStepDuration = 1 / 120f;

            //And then, in the update, either call the Space.Update() method proportionally more often or use the Space.Update(dt) version, which takes as many timesteps are necessary to simulate dt time.
            //Watch out: when using the internal timestepping method, you may notice slight motion jitter since the number of updates per frame isn't fixed.  Interpolation buffers can be used
            //to address this; check the Asynchronous Update documentation for more information on using internal time stepping.  
            //[Asynchronously updating isn't required to use internal timestepping, but it is a common use case.]

            //Dump some boxes on top of it for fun.
            int numColumns = 8;
            int numRows = 8;
            int numHigh = 1;
            Fix64 separation = 2.ToFix().Mul(scale);
            Fix64 baseWidth = 0.5m.ToFix();
            Fix64 baseHeight = 1.ToFix();
            Fix64 baseLength = 1.5m.ToFix();
            Entity toAdd;
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Box(
                            new Vector3(
(separation.Mul(i.ToFix())).Sub((numRows.ToFix().Mul(separation)).Div(2.ToFix())),
(2.ToFix().Mul(scale)).Add(k.ToFix().Mul(separation)),
(separation.Mul(j.ToFix())).Sub((numColumns.ToFix().Mul(separation)).Div(2.ToFix()))),
baseWidth.Mul(scale), baseHeight.Mul(scale), baseLength.Mul(scale), 15.ToFix());

                        Space.Add(toAdd);
                    }

            //Dump some stuff on top of it that use general case collision detection when they collide with boxes.
            numColumns = 3;
            numRows = 3;
            numHigh = 4;
            separation = 2.ToFix().Mul(scale);
            baseWidth = 1.ToFix();
            baseHeight = 1.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Cylinder(
                            new Vector3(
(separation.Mul(i.ToFix())).Sub((numRows.ToFix().Mul(separation)).Div(2.ToFix())),
(8.ToFix().Mul(scale)).Add(k.ToFix().Mul(separation)),
(separation.Mul(j.ToFix())).Sub((numColumns.ToFix().Mul(separation)).Div(2.ToFix()))),
baseHeight.Mul(scale), (0.5m.ToFix().Mul(baseWidth)).Mul(scale),  15.ToFix());

                        Space.Add(toAdd);
                    }



            game.Camera.Position = scale * new Vector3(0.ToFix(), 4.ToFix(), 10.ToFix());
            originalCameraSpeed = freeCameraControlScheme.Speed;
            freeCameraControlScheme.Speed *= scale;


        }


        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Scale Demo"; }
        }

        private Fix64 originalCameraSpeed;
        public override void CleanUp()
        {
            freeCameraControlScheme.Speed = originalCameraSpeed;
            base.CleanUp();
        }

    }
}