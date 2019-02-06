using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using Microsoft.Xna.Framework.Graphics;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Checks the mobile mesh solidity sidedness calculation.
    /// </summary>
    public class MobileMeshSolidityTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public MobileMeshSolidityTestDemo(DemosGame game)
            : base(game)
        {
            Vector3[] vertices;
            int[] indices;

            //Hardcoded box
            vertices = new Vector3[] 
            {
                new Vector3(0.5m.ToFix(), 0.5m.ToFix(), 0.5m.ToFix()),
                new Vector3(0.5m.ToFix(),0.5m.ToFix(),(-0.5m).ToFix()),
                new Vector3((-0.5000001m).ToFix(),0.5m.ToFix(),(-0.4999999m).ToFix()),
                new Vector3((-0.4999998m).ToFix(),0.5m.ToFix(),0.5000002m.ToFix()),
                new Vector3((-0.4999998m).ToFix(),(-0.5m).ToFix(),0.5000002m.ToFix()),
                new Vector3((-0.5000001m).ToFix(),(-0.5m).ToFix(),(-0.4999999m).ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),(-0.5m).ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),0.5m.ToFix()),
                new Vector3(0.5m.ToFix(),0.5m.ToFix(),0.5m.ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),0.5m.ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),(-0.5m).ToFix()),
                new Vector3(0.5m.ToFix(),0.5m.ToFix(),(-0.5m).ToFix()),
                new Vector3(0.5m.ToFix(),0.5m.ToFix(),(-0.5m).ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),(-0.5m).ToFix()),
                new Vector3((-0.5000001m).ToFix(),(-0.5m).ToFix(),(-0.4999999m).ToFix()),
                new Vector3((-0.5000001m).ToFix(),0.5m.ToFix(),(-0.4999999m).ToFix()),
                new Vector3((-0.5000001m).ToFix(),0.5m.ToFix(),(-0.4999999m).ToFix()),
                new Vector3((-0.5000001m).ToFix(),(-0.5m).ToFix(),(-0.4999999m).ToFix()),
                new Vector3((-0.4999998m).ToFix(),(-0.5m).ToFix(),0.5000002m.ToFix()),
                new Vector3((-0.4999998m).ToFix(),0.5m.ToFix(),0.5000002m.ToFix()),
                new Vector3((-0.4999998m).ToFix(),0.5m.ToFix(),0.5000002m.ToFix()),
                new Vector3((-0.4999998m).ToFix(),(-0.5m).ToFix(),0.5000002m.ToFix()),
                new Vector3(0.5m.ToFix(),(-0.5m).ToFix(),0.5m.ToFix()) ,
                new Vector3(0.5m.ToFix(),0.5m.ToFix(),0.5m.ToFix())
            };

            indices = new[] 
            {
                2, 1, 0,
                3, 2, 0,
                6, 5 ,4,
                7, 6 ,4,
                10, 9, 8,
                11, 10, 8,
                14, 13, 12,
                15, 14, 12,
                18, 17, 16,
                19, 18, 16,
                22, 21, 20,
                23, 22, 20
            };

            var mesh = new MobileMesh(vertices, indices, AffineTransform.Identity, MobileMeshSolidity.Solid, 10.ToFix());
            Space.Add(mesh);

            //Tube
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("tube"), out vertices, out indices);
            mesh = new MobileMesh(vertices, indices, AffineTransform.Identity, MobileMeshSolidity.Solid, 10.ToFix());
            mesh.Position = new Vector3((-10).ToFix(), 10.ToFix(), 0.ToFix());
            Space.Add(mesh);

            //Cube
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("cube"), out vertices, out indices);
            mesh = new MobileMesh(vertices, indices, AffineTransform.Identity, MobileMeshSolidity.Solid, 10.ToFix());
            mesh.Position = new Vector3(10.ToFix(), 0.ToFix(), 0.ToFix());
            Space.Add(mesh);

            //Guy
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("guy"), out vertices, out indices);
            mesh = new MobileMesh(vertices, indices, AffineTransform.Identity, MobileMeshSolidity.Solid, 10.ToFix());
            mesh.Position = new Vector3(0.ToFix(), 0.ToFix(), 10.ToFix());
            Space.Add(mesh);

            //Barrel Platform
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("barrelandplatform"), out vertices, out indices);
            mesh = new MobileMesh(vertices, indices, AffineTransform.Identity, MobileMeshSolidity.Solid, 10.ToFix());
            mesh.Position = new Vector3(0.ToFix(), 0.ToFix(), (-10).ToFix());
            Space.Add(mesh);

            //FloaterTube
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("tube"), out vertices, out indices);
            mesh = new MobileMesh(vertices, indices, new AffineTransform(new Vector3(1.ToFix(), 1.ToFix(), 1.ToFix()), Quaternion.Identity, new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix())), MobileMeshSolidity.Solid);
            mesh.Position = new Vector3(5.ToFix(), 18.ToFix(), 0.ToFix());
            Space.Add(mesh);

            //Float a box through the last mesh to check contact generation controllably.
            var solidityTester = new Box(new Vector3(5.ToFix(), 8.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix());
            solidityTester.LinearVelocity = new Vector3(0.ToFix(), 1.ToFix(), 0.ToFix());
            CollisionRules.AddRule(solidityTester, mesh, CollisionRule.NoSolver);
            Space.Add(solidityTester);


            Space.Add(new Box(new Vector3(0.ToFix(), (-5).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 10.ToFix(), 20.ToFix());

        }





        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Solidity Test"; }
        }
    }
}