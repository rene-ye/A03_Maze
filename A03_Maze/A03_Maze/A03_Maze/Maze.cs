using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace A03_Maze
{
    class Maze
    {
        public const int MAZE_WIDTH = 10;
        public const int MAZE_HEIGHT = 10;

        private Random rand = new Random();
        public Cell[,] Grid = new Cell[MAZE_WIDTH, MAZE_HEIGHT];

        GraphicsDevice graphics;

        // Floor Variables
        VertexBuffer floorBuffer;
        Color[] floorColors = new Color[2] { Color.White, Color.Black };

        // Wall Variables
        VertexBuffer wallBuffer;
        Vector3[] wallPoints = new Vector3[8];
        Color[] wallColors = new Color[4] {Color.Red, Color.Blue, Color.Green, Color.Purple };

        // Constructor
        public Maze(GraphicsDevice g)
        {
            graphics = g;

            CreateFloor();

            for (int x = 0; x < MAZE_WIDTH; x++)
                for (int z = 0; z < MAZE_HEIGHT; z++)
                {
                    Grid[x, z] = new Cell();
                }
            GenerateMaze();

            wallPoints[0] = new Vector3(0, 1, 0);
            wallPoints[1] = new Vector3(0, 1, 1);
            wallPoints[2] = new Vector3(0, 0, 0);
            wallPoints[3] = new Vector3(0, 0, 1);
            wallPoints[4] = new Vector3(1, 1, 0);
            wallPoints[5] = new Vector3(1, 1, 1);
            wallPoints[6] = new Vector3(1, 0, 0);
            wallPoints[7] = new Vector3(1, 0, 1);

            CreateWall();
        }

        // Draw Method
        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // Draw the floor
                graphics.SetVertexBuffer(floorBuffer);
                graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);

                // Draw the walls
                graphics.SetVertexBuffer(wallBuffer);
                graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, wallBuffer.VertexCount / 3);
            }
        }

        // Method to create the floor
        private void CreateFloor()
        {
            List<VertexPositionColor> vertexList =
            new List<VertexPositionColor>();
            int counter = 0;
            for (int x = 0; x < MAZE_WIDTH; x++)
            {
                counter++;
                for (int z = 0; z < MAZE_HEIGHT; z++)
                {
                    counter++;
                    foreach (VertexPositionColor vertex in
                    FloorTile(x, z, floorColors[counter % 2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }
            floorBuffer = new VertexBuffer(graphics, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
            floorBuffer.SetData<VertexPositionColor>(vertexList.ToArray());
        }

        // Method to create the walls
        private void CreateWall()
        {
            List<VertexPositionColor> wallVertexList = new
            List<VertexPositionColor>();
            for (int x = 0; x < MAZE_WIDTH; x++)
            {
                for (int z = 0; z < MAZE_HEIGHT; z++)
                {
                    foreach (VertexPositionColor vertex
                    in WallTile(x, z))
                    {
                        wallVertexList.Add(vertex);
                    }
                }
            }
            wallBuffer = new VertexBuffer(graphics, VertexPositionColor.VertexDeclaration, wallVertexList.Count, BufferUsage.WriteOnly);
            wallBuffer.SetData<VertexPositionColor>(wallVertexList.ToArray());
        }

        // Vertices required to draw the floors
        private List<VertexPositionColor> FloorTile(int xOffset, int zOffset, Color tileColor)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            vertices.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 0 + zOffset), tileColor));
            vertices.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vertices.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));
            vertices.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vertices.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 1 + zOffset), tileColor));
            vertices.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));

            return vertices;
        }

        // Vertices required to draw the walls
        private List<VertexPositionColor> WallTile(int x, int z)
        {
            List<VertexPositionColor> triangles = new List<VertexPositionColor>();

            if (Grid[x, z].Walls[0])
            {
                triangles.Add(CalculateWallPoint(0, x, z, wallColors[0]));
                triangles.Add(CalculateWallPoint(4, x, z, wallColors[0]));
                triangles.Add(CalculateWallPoint(2, x, z, wallColors[0]));
                triangles.Add(CalculateWallPoint(4, x, z, wallColors[0]));
                triangles.Add(CalculateWallPoint(6, x, z, wallColors[0]));
                triangles.Add(CalculateWallPoint(2, x, z, wallColors[0]));
            }
            if (Grid[x, z].Walls[1])
            {
                triangles.Add(CalculateWallPoint(4, x, z, wallColors[1]));
                triangles.Add(CalculateWallPoint(5, x, z, wallColors[1]));
                triangles.Add(CalculateWallPoint(6, x, z, wallColors[1]));
                triangles.Add(CalculateWallPoint(5, x, z, wallColors[1]));
                triangles.Add(CalculateWallPoint(7, x, z, wallColors[1]));
                triangles.Add(CalculateWallPoint(6, x, z, wallColors[1]));
            }
            if (Grid[x, z].Walls[2])
            {
                triangles.Add(CalculateWallPoint(5, x, z, wallColors[2]));
                triangles.Add(CalculateWallPoint(1, x, z, wallColors[2]));
                triangles.Add(CalculateWallPoint(7, x, z, wallColors[2]));
                triangles.Add(CalculateWallPoint(1, x, z, wallColors[2]));
                triangles.Add(CalculateWallPoint(3, x, z, wallColors[2]));
                triangles.Add(CalculateWallPoint(7, x, z, wallColors[2]));
            }
            if (Grid[x, z].Walls[3])
            {
                triangles.Add(CalculateWallPoint(1, x, z, wallColors[3]));
                triangles.Add(CalculateWallPoint(0, x, z, wallColors[3]));
                triangles.Add(CalculateWallPoint(3, x, z, wallColors[3]));
                triangles.Add(CalculateWallPoint(0, x, z, wallColors[3]));
                triangles.Add(CalculateWallPoint(2, x, z, wallColors[3]));
                triangles.Add(CalculateWallPoint(3, x, z, wallColors[3]));
            }
            return triangles;
        }

        // Assigns the specified color to a wall point
        private VertexPositionColor CalculateWallPoint(int wallPoint, int xOffset, int zOffset, Color color)
        {
            return new VertexPositionColor(wallPoints[wallPoint] + new Vector3(xOffset, 0, zOffset),color);
        }

        // Create a bounding box; used for collisions
        private BoundingBox BuildBoundingBox(int x, int z, int p1, int p2)
        {
            BoundingBox theBox = new BoundingBox(wallPoints[p1], wallPoints[p2]);

            theBox.Min.X += x;
            theBox.Min.Z += z;
            theBox.Max.X += x;
            theBox.Max.Z += z;

            theBox.Min.X -= 0.1f;
            theBox.Min.Z -= 0.1f;
            theBox.Max.X += 0.1f;
            theBox.Max.Z += 0.1f;

            return theBox;
        }

        // Gets the boundaries from a maze cell; used for collisions
        public List<BoundingBox> GetBoundsForCell(int x, int z)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();
            if (Grid[x, z].Walls[0])
                boxes.Add(BuildBoundingBox(x, z, 2, 4));
            if (Grid[x, z].Walls[1])
                boxes.Add(BuildBoundingBox(x, z, 6, 5));
            if (Grid[x, z].Walls[2])
                boxes.Add(BuildBoundingBox(x, z, 3, 5));
            if (Grid[x, z].Walls[3])
                boxes.Add(BuildBoundingBox(x, z, 2, 1));

            return boxes;
        }

        // Create the grid containing the maze cells
        public void GenerateMaze()
        {
            for (int x = 0; x < MAZE_WIDTH; x++)
                for (int z = 0; z < MAZE_HEIGHT; z++)
                {
                    Grid[x, z].Walls[0] = true;
                    Grid[x, z].Walls[1] = true;
                    Grid[x, z].Walls[2] = true;
                    Grid[x, z].Walls[3] = true;
                    Grid[x, z].Visited = false;
                }
            Grid[0, 0].Visited = true;
            DFS_Cell(new Vector2(0, 0));
        }

        // Perform a randomized DFS algorithm on a cell to create the maze path
        private void DFS_Cell(Vector2 cell)
        {
            List<int> neighborCells = new List<int>();
            neighborCells.Add(0);
            neighborCells.Add(1);
            neighborCells.Add(2);
            neighborCells.Add(3);
            while (neighborCells.Count > 0)
            {
                int pick = rand.Next(0, neighborCells.Count);
                int selectedNeighbor = neighborCells[pick];
                neighborCells.RemoveAt(pick);
                Vector2 neighbor = cell;
                switch (selectedNeighbor)
                {
                    case 0:
                        neighbor += new Vector2(0, -1);
                        break;
                    case 1:
                        neighbor += new Vector2(1, 0);
                        break;
                    case 2:
                        neighbor += new Vector2(0, 1);
                        break;
                    case 3:
                        neighbor += new Vector2(-1, 0);
                        break;
                }
                if (
                (neighbor.X >= 0) &&
                (neighbor.X < MAZE_WIDTH) &&
                (neighbor.Y >= 0) &&
                (neighbor.Y < MAZE_HEIGHT)
                )
                {
                    if (!Grid[(int)neighbor.X, (int)neighbor.Y].
                    Visited)
                    {
                        Grid[(int)neighbor.X, (int)neighbor.Y].Visited = true;
                        Grid[(int)cell.X, (int)cell.Y].Walls[selectedNeighbor] = false;
                        Grid[(int)neighbor.X, (int)neighbor.Y].Walls[(selectedNeighbor + 2) % 4] = false;
                        DFS_Cell(neighbor);
                    }
                }
            }
        }
    }
}
