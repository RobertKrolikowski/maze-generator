using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace maze___labirynt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /*
        BasicEffect basicEffect;
        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        */
        Texture2D dot;

        Cell[] cells;
        int mapColumns = 40, mapRows = 40;
        int maxspeed = 1, speed = 0, size = 20;

        Cell current;
        Stack<Cell> stack = new Stack<Cell>();

        class Cell
        {
            int column;
            int row;
            int size;
            bool[] existLine; //top right botom left
            bool visited;
            public Cell()
            {
                column = 0;
                row = 0;
                size = 40;
                existLine = new bool[] { true, true, true, true };
                visited = false;
            }

            public Cell(int _column, int _row, int _size)
            {
                column = _column;
                row = _row;
                size = _size;
                existLine = new bool[] { true, true, true, true };
                visited = false;
            }

            public void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 begin, Vector2 end, Color color, int width = 1)
            {
                Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
                Vector2 v = Vector2.Normalize(begin - end);
                float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
                if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
                spriteBatch.Draw(texture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
            }

            public void DrawRectangle(SpriteBatch spriteBatch, Texture2D texture, Color color)
            {
                Vector2 v1, v2, v3, v4;

                v1 = new Vector2(column * size, row * size);
                v2 = new Vector2((column + 1) * size, row * size);
                v3 = new Vector2((column + 1) * size, (row + 1) * size);
                v4 = new Vector2(column * size, (row + 1) * size);

                if (visited)
                    spriteBatch.Draw(texture, new Rectangle(column * size, row * size, size, size), color);
                
                if (existLine[0])
                    DrawLine(spriteBatch, texture, v1, v2, Color.Black);
                if (existLine[1])
                    DrawLine(spriteBatch, texture, v2, v3, Color.Black);
                if (existLine[2])
                    DrawLine(spriteBatch, texture, v3, v4, Color.Black);
                if (existLine[3])
                    DrawLine(spriteBatch, texture, v4, v1, Color.Black);
               
            }

            public Cell CheckNeigbors(Cell[] cells, int _columns, int _row)
            {
                List<Cell> neigbors = new List<Cell>();
                //Cell[] neigbors;
                if (row > 0)
                {
                    Cell top = cells[column + ((row - 1) * _columns)];
                    if (!top.visited)
                        neigbors.Add(top);
                }
                if ((column + 1) < _columns)
                {
                    Cell right = cells[column + 1 + (row * _columns)];
                    if (!right.visited)
                        neigbors.Add(right);
                }
                if ((row + 1) < _row )
                {
                    Cell bottom = cells[column + ((row + 1) * _columns)];
                    if (!bottom.visited)
                        neigbors.Add(bottom);
                }
                if (column > 0)
                {
                    Cell left = cells[column - 1 + (row * _columns)];
                    if (!left.visited)
                        neigbors.Add(left);
                }
                if (neigbors.Count > 0)
                {
                    Random r = new Random();
                    int number = r.Next(0, neigbors.Count);
                    return neigbors[number];
                }
                return null;
                
            }

            public void RemoveWalls(Cell next)
            {
                int i = row - next.GetRows();
                if (i == 1)
                {
                    existLine[0] = false;
                    bool[] b = next.GetExistLine();
                    b[2] = false;
                    next.SetExistLine(b);
                }
                if (i == -1)
                {
                    existLine[2] = false;
                    bool[] b = next.GetExistLine();
                    b[0] = false;
                    next.SetExistLine(b);
                }
                i = column - next.GetColumn();
                
                if (i == 1)
                {
                    existLine[3] = false;
                    bool[] b = next.GetExistLine();
                    b[1] = false;
                    next.SetExistLine(b);
                }
                if (i == -1)
                {
                    existLine[1] = false;
                    bool[] b = next.GetExistLine();
                    b[3] = false;
                    next.SetExistLine(b);
                }

            }

            public void SetVisited(bool _visited)
            {
                visited = _visited;
            }

            public void SetExistLine(bool[] b)
            {
                existLine = b;
            }
            
            public bool GetVisited()
            {
                return visited;
            }

            public int GetColumn()
            {
                return column;
            }

            public int GetRows()
            {
                return row;
            }

            public bool[] GetExistLine()
            {
                return existLine;
            }
            
        }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            this.graphics.PreferredBackBufferHeight = 800;
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            /*
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            */
            dot = new Texture2D(GraphicsDevice, 1, 1);
            dot.SetData<Color>(new Color[] { Color.White });

            cells = new Cell[mapColumns* mapRows];
            for (int i = 0; i < mapRows; i++)
            {
                for (int j = 0; j < mapColumns; j++)
                {
                    cells[(mapColumns * i) + j] = new Cell(j, i, size);
                }
            }
            current = cells[0];
            current.SetVisited(true);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            /*
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1000), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000);
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            */
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            //Reset
            if (keyboard.IsKeyDown(Keys.R))
            {
                cells = new Cell[mapColumns * mapRows];
                for (int i = 0; i < mapRows; i++)
                {
                    for (int j = 0; j < mapColumns; j++)
                    {
                        cells[(mapColumns * i) + j] = new Cell(j, i, size);
                    }
                }
                current = cells[0];
                current.SetVisited(true);
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                if(maxspeed < 10)
                    maxspeed++;
            }
            else if (keyboard.IsKeyDown(Keys.S))
            {
                if (maxspeed > 1)
                    maxspeed--;
            }

            if(maxspeed <= speed)
            {
                speed = 0;
                current.SetVisited(true);
                //step 1 
                Cell next = current.CheckNeigbors(cells, mapColumns, mapRows);               
                if (next != null)
                {
                    next.SetVisited(true);
                    //step 2
                    stack.Push(current);
                    
                    //step 3
                    current.RemoveWalls(next);

                    //step 4
                    current = next;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                }
            }
            speed++;


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, cell.GetVertex(), 0, 4); 
            spriteBatch.Begin();
            for(int i = 0; i < cells.Length; i++)
                cells[i].DrawRectangle(spriteBatch, dot, Color.Purple);

            current.DrawRectangle(spriteBatch, dot, Color.Green);
            spriteBatch.End();      
            
            base.Draw(gameTime);
        }

    }
}
