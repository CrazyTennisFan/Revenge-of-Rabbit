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


namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 
 
    public class ModelManager : DrawableGameComponent
    {


        Game1.GameState currentGameState;
        Game1.GameState previousGameState;

        // Lists of models
        List<Bear> bears = new List<Bear>();
        List<Projectile> shots = new List<Projectile>();
        List<Bear> deadBears = new List<Bear>();
        List<BasicModel> trees = new List<BasicModel>();
        List<BasicModel> mushrooms = new List<BasicModel>();
		List<BasicModel> outOfBoundTrees = new List<BasicModel>();
        List<BasicModel> ground = new List<BasicModel>();
        List<BasicModel> houses = new List<BasicModel>();
        public Bear rabbit { get; protected set; }
    
        Random random = new Random();

		//Model bearModel;
		Model blueBearModel;
		Model brownBearModel;
		Model cyanBearModel;
		Model greenBearModel;
		Model purpleBearModel;
		Model redBearModel;
		Model whiteBearModel;
		Model yellowBearModel;

		Model carrotModel;

		Model greenTreeModel;
		Model yellowTreeModel;
		Model redTreeModel;

		Model blueHouseModel;
		Model greenHouseModel;
		Model purpleHouseModel;
		Model redHouseModel;
		Model yellowHouseModel;

		Model blueMushroomModel;
		Model greenMushroomModel;
		Model redMushroomModel;

        Model groundModel;

		int level = 0;
        int baseNum = 100;

        public ModelManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

                
        }

        public void setGameState(Game1.GameState state)
        {
            currentGameState = state;            
        }

        public List<Projectile> getShots()
        {
            return shots;
        }

        public List<Bear> getBears()
        {
            return bears;
        }

        public List<Bear> getDeadBears()
        {
            return deadBears;
        }

        public List<BasicModel> getStaticCollidables()
        {
			List<BasicModel> co = new List<BasicModel>();
			foreach (BasicModel tree in trees)
			{
				co.Add(tree);
			}

			foreach (BasicModel house in houses)
			{
				co.Add(house);
			}


			foreach (Bear bear in deadBears)
			{
				co.Add(bear);
			}
			
			return co;
        }

        public List<BasicModel> getCollidables()
        {
            List<BasicModel> co = getStaticCollidables();
            foreach (Bear bear in bears)
            {
                co.Add(bear);
            }

            return co;
        }
        public List<BasicModel> getDrawables()
        {
			List<BasicModel> drawable = getStaticCollidables();
			foreach (BasicModel mushroom in mushrooms)
			{
				drawable.Add(mushroom);
			}

			foreach (Projectile shot in shots)
			{
				drawable.Add(shot);
			}

			foreach (BasicModel tree in outOfBoundTrees)
			{
				drawable.Add(tree);
			}

            foreach (BasicModel patch in ground)
            {
                drawable.Add(patch);
            }

			return drawable;
			
        }

        public int getBearNumber()
        {
            return bears.Count;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //bearModel = Game.Content.Load<Model>(@"models\bear");
			blueBearModel = Game.Content.Load<Model>(@"models\blue_bear");
			brownBearModel = Game.Content.Load<Model>(@"models\brown_bear");
			cyanBearModel = Game.Content.Load<Model>(@"models\cyan_bear");
			greenBearModel = Game.Content.Load<Model>(@"models\green_bear");
			purpleBearModel = Game.Content.Load<Model>(@"models\purple_bear");
			redBearModel = Game.Content.Load<Model>(@"models\red_bear");
			whiteBearModel = Game.Content.Load<Model>(@"models\white_bear");
			yellowBearModel = Game.Content.Load<Model>(@"models\yellow_bear");

            carrotModel = Game.Content.Load<Model>(@"models\carrot");

            greenTreeModel = Game.Content.Load<Model>(@"models\green_tree");
            yellowTreeModel = Game.Content.Load<Model>(@"models\yellow_tree");
            redTreeModel = Game.Content.Load<Model>(@"models\red_tree");

            blueHouseModel = Game.Content.Load<Model>(@"models\blue_house");
            greenHouseModel = Game.Content.Load<Model>(@"models\green_house");
            purpleHouseModel = Game.Content.Load<Model>(@"models\purple_house");
            redHouseModel = Game.Content.Load<Model>(@"models\red_house");
            yellowHouseModel = Game.Content.Load<Model>(@"models\yellow_house");

            blueMushroomModel = Game.Content.Load<Model>(@"models\blue_mushroom");
            greenMushroomModel = Game.Content.Load<Model>(@"models\green_mushroom");
            redMushroomModel = Game.Content.Load<Model>(@"models\red_mushroom");

            groundModel = Game.Content.Load<Model>(@"models\ground");
            base.LoadContent();
        }

        public void AddShot(Vector3 position, Vector3 direction)
        {
            Projectile carrot = new Projectile(carrotModel, position, direction, 0, 0, 0);

            carrot.scale(8);
			shots.Add(carrot);
        }


        private void loadNewLevel()
        {
           
            if (previousGameState == Game1.GameState.Fail)
            {
                level--;
                previousGameState = Game1.GameState.Start;
            }
            
			level++;

            // Clear current lists
            bears.Clear();
            shots.Clear();
            deadBears.Clear();            
            trees.Clear();
            mushrooms.Clear();
            houses.Clear();
			outOfBoundTrees.Clear();
            ground.Clear();
           
            rabbit = new Bear(carrotModel, ((Game1)Game).camera.cameraPosition - new Vector3(0, 0, -5), Vector3.Zero);

			this.addMushrooms(50); 
            this.addTrees(4 * level);
            this.addHouses(5);
			this.addBears(6 * level);
            this.addGround();

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            if (currentGameState == Game1.GameState.Start)
            {
                level = 0;
                previousGameState = Game1.GameState.Start;
            }
           
            if (currentGameState == Game1.GameState.Fail)
            {
                previousGameState = Game1.GameState.Fail;
            }
            
            if (currentGameState == Game1.GameState.Initial)
            {
                loadNewLevel();
            }               

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void addGround()
        {
            for (int i = -350; i <= 350; i += 40)
            {
                for (int j = -350; j <= 350; j += 40)
                {
                    ground.Add(new BasicModel(groundModel, new Vector3(i, -2.5f, j)));
                }
            }
            
            foreach (BasicModel patch in ground)
            {
                patch.rotationX(-90);
                patch.scale(20);
            }
            
            
        }
		public void addBears(int numBears)																	
		{
            Bear bear;
            for(int i=0; i<numBears; i++)
            {
                Vector3 pos = trees[random.Next(trees.Count())].position;
                pos = pos + 10*Vector3.Normalize(pos-rabbit.position);

				switch (random.Next(8))
				{
					case 1:
						bear = new Bear(blueBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 2:
						bear = new Bear(cyanBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 3:
						bear = new Bear(greenBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 4:
						bear = new Bear(purpleBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 5:
						bear = new Bear(redBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 6:
						bear = new Bear(whiteBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					case 7:
						bear = new Bear(yellowBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
					default:
						bear = new Bear(brownBearModel, new Vector3(pos.X, -2.5f, pos.Z), new Vector3(0, 0, 1));
						break;
				}

                bool isValid = true;
                foreach (BasicModel target in getCollidables())
                {                    
                    if (bear.CollidesWith(target) || Vector3.Distance(bear.position, rabbit.position) < 80)
                    {
                        isValid = false;
                        i--;
                        break;
                    }
                }
                if (isValid)
                {
                    bears.Add(bear);
                }
            }
		}


        void addHouses(int numHouses)																
        {
            BasicModel tentativeHouse;
            for (int i = 0; i < numHouses; i++)
            {
                switch (random.Next(5))
                {
                    case 1:
                        tentativeHouse = new BasicModel(blueHouseModel, new Vector3(random.Next(100 * level * 2) - 100 * level, -2.5f, random.Next(100 * level * 2) - 100 * level));
                        break;
                    case 2:
                        tentativeHouse = new BasicModel(yellowHouseModel, new Vector3(random.Next(100 * level * 2) - 100 * level, -2.5f, random.Next(100 * level * 2) - 100 * level));
                        break;
                    case 3:
                        tentativeHouse = new BasicModel(greenHouseModel, new Vector3(random.Next(100 * level * 2) - 100 * level, -2.5f, random.Next(100 * level * 2) - 100 * level));
                        break;
                    case 4:
                        tentativeHouse = new BasicModel(purpleHouseModel, new Vector3(random.Next(100 * level * 2) - 100 * level, -2.5f, random.Next(100 * level * 2) - 100 * level));
                        break;
                    default:
                        tentativeHouse = new BasicModel(redHouseModel, new Vector3(random.Next(100 * level * 2) - 100 * level, -2.5f, random.Next(100 * level * 2) - 100 * level));
                        break;
                }

                bool isValid = true;
                tentativeHouse.scale(8);
                foreach (BasicModel target in getCollidables())
                {

                    if (Vector3.Distance(tentativeHouse.position, target.position) < 30 || Vector3.Distance(tentativeHouse.position, rabbit.position) < 20)
                    {
                        isValid = false;
                        i--;
                        break;
                    }
                }
                if (isValid)
                {       
                    houses.Add(tentativeHouse);
                }
            }
        }


        void addTrees(int numTrees)
        {
			int treeChoice = 0; // choose green tree or yellow tree
            
			// Randomly create trees for the border of the playing area
            if (level < 2)
                baseNum = 100;
            else
                baseNum = 150;

            for (int i = -baseNum; i <= baseNum; i = i + 40)
			{
				treeChoice = random.Next(3);

                if (treeChoice == 0)
				{
                    trees.Add(new BasicModel(greenTreeModel, new Vector3(i, -3, -baseNum)));
				}
                else if (treeChoice == 1)
				{
                    trees.Add(new BasicModel(yellowTreeModel, new Vector3(i, -3, -baseNum)));
				}
                else if (treeChoice == 2)
				{
                    trees.Add(new BasicModel(redTreeModel, new Vector3(i, -3, -baseNum)));
			}
            }

            for (int i = -baseNum + 40; i < baseNum; i = i + 40)
			{
				treeChoice = random.Next(3);
				if (treeChoice == 0)
				{
                    trees.Add(new BasicModel(greenTreeModel, new Vector3(-baseNum, -3, i)));
				}
				else if (treeChoice == 1)
				{
                    trees.Add(new BasicModel(yellowTreeModel, new Vector3(-baseNum, -3, i)));
				}
				else if (treeChoice == 2)
				{
                    trees.Add(new BasicModel(redTreeModel, new Vector3(-baseNum, -3, i)));
				}
			}

            for (int i = -baseNum; i <= baseNum; i = i + 40)
			{
				treeChoice = random.Next(3);
				if (treeChoice == 0)
				{
                    trees.Add(new BasicModel(greenTreeModel, new Vector3(i, -3, baseNum)));
				}
				else if (treeChoice == 1)
				{
                    trees.Add(new BasicModel(yellowTreeModel, new Vector3(i, -3, baseNum)));
				}
				else if (treeChoice == 2)
				{
                    trees.Add(new BasicModel(redTreeModel, new Vector3(i, -3, baseNum)));
				}
			}

            for (int i = -baseNum + 40; i < baseNum; i = i + 40)
			{
				treeChoice = random.Next(3);
				if (treeChoice == 1)
				{
                    trees.Add(new BasicModel(greenTreeModel, new Vector3(baseNum, -3, i)));
				}
				else if (treeChoice == 2)
				{
                    trees.Add(new BasicModel(yellowTreeModel, new Vector3(baseNum, -3, i)));
				}
				else if (treeChoice == 0)
				{
                    trees.Add(new BasicModel(redTreeModel, new Vector3(baseNum, -3, i)));
				}
			}
            
			//add some trees in the fight area		
            BasicModel tentativeTree;
            for (int i = 0; i < numTrees; i++)
            {
                switch (random.Next(3))
                {
                    case 1:
                        tentativeTree = new BasicModel(greenTreeModel, new Vector3(random.Next(baseNum * 2 - 30) - baseNum, -3, random.Next(baseNum * 2 - 30) - baseNum));
                        break;
                    case 2:
                        tentativeTree = new BasicModel(yellowTreeModel, new Vector3(random.Next(baseNum * 2 - 30) - baseNum, -3, random.Next(baseNum * 2 - 30) - baseNum));
                        break;
                    default:
                        tentativeTree = new BasicModel(redTreeModel, new Vector3(random.Next(baseNum * 2 - 30) - baseNum, -3, random.Next(baseNum * 2 - 30) - baseNum));
                        break;
                }

                bool isValid = true;
                foreach (BasicModel target in getCollidables())
                {

                    if (Vector3.Distance(tentativeTree.position, target.position) < 30 || Vector3.Distance(tentativeTree.position, rabbit.position) < 20)
                    {
                        isValid = false;
                        i--;
                        break;
                    }
                }
                if (isValid)
                    trees.Add(tentativeTree);
            }
			
			// add trees outside boundary
			for (int i = 0; i < numTrees; i++)
			{
                Model modelChoice;
				switch (random.Next(3))
				{
					case 1:
                        modelChoice = greenTreeModel;						
						break;
					case 2:
                        modelChoice = yellowTreeModel;						
						break;
					default:
                        modelChoice = redTreeModel;
						break;
				}

                List<BasicModel> tentativeTrees = new List<BasicModel>();
                tentativeTrees.Add(new BasicModel(modelChoice, new Vector3(random.Next(baseNum) - baseNum * 2 - 30, -3, random.Next(baseNum * 3) - baseNum)));
                tentativeTrees.Add(new BasicModel(modelChoice, new Vector3(random.Next(baseNum * 3) - baseNum, -3, random.Next(baseNum) + baseNum + 30)));
                tentativeTrees.Add(new BasicModel(modelChoice, new Vector3(random.Next(baseNum) + baseNum + 30, -3, random.Next(baseNum * 3) - baseNum * 2)));
                tentativeTrees.Add(new BasicModel(modelChoice, new Vector3(random.Next(baseNum * 3) - baseNum * 2, -3, random.Next(baseNum) - baseNum * 2 - 30)));

                bool isValid = true;
                foreach (BasicModel tree in tentativeTrees)
                {
                    foreach (BasicModel target in outOfBoundTrees)
                    {
                        if (Vector3.Distance(tree.position, target.position) < 15)
                        {
                            isValid = false;
                            i--;
                            break;
                        }
                    }
                }
                if (isValid)
                {
                    foreach(BasicModel tree in tentativeTrees)
                        outOfBoundTrees.Add(tree);
                }
               
			}

            foreach (BasicModel tree in trees)
            {
                tree.scale(10, 13, 10);
            }
			foreach (BasicModel tree in outOfBoundTrees)
				tree.scale(20, 25, 20);
        }



        void addMushrooms(int numMushrooms)
		{		
																			
            BasicModel mushroom;

            if (level < 2)
                baseNum = 100;
            else
                baseNum = 150;

            for (int i = 0; i < numMushrooms; i++)
            {
                switch (random.Next(3))
                {
                    case 1:
                        mushroom = new BasicModel(redMushroomModel, new Vector3(random.Next(baseNum * 2) - baseNum, -2.6f, random.Next(baseNum * 2) - baseNum));
                        break;
                    case 2:
                        mushroom = new BasicModel(blueMushroomModel, new Vector3(random.Next(baseNum * 2) - baseNum, -3f, random.Next(baseNum * 2) - baseNum));
                        break;
                    default:
                        mushroom = new BasicModel(greenMushroomModel, new Vector3(random.Next(baseNum * 2) - baseNum, -3.5f, random.Next(baseNum * 2) - baseNum));
                        break;
                }

                bool isValid = true;
                foreach (BasicModel target in getCollidables())
                {

                    if (mushroom.CollidesWith(target))
                    {
                        isValid = false;
                        i--;
                        break;
                    }
                }
                if (isValid)
                    mushrooms.Add(mushroom);
            }

			// add mushrooms outside boundary
			for (int i = 0; i < (int)(numMushrooms * 3 / 4); i++)
			{
				switch (random.Next(3))
				{
					case 1:
                        mushrooms.Add(new BasicModel(redMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -2.6f, random.Next(baseNum * 4) - baseNum * 5)));
                        mushrooms.Add(new BasicModel(redMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -2.6f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(redMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -2.6f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(redMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -2.6f, random.Next(baseNum * 4) - baseNum * 5)));
						break;
					case 2:
                        mushrooms.Add(new BasicModel(blueMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -3f, random.Next(baseNum * 4) - baseNum * 5)));
                        mushrooms.Add(new BasicModel(blueMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -3f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(blueMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -3f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(blueMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -3f, random.Next(baseNum * 4) - baseNum * 5)));
						break;
					default:
                        mushrooms.Add(new BasicModel(greenMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -3.5f, random.Next(baseNum * 4) - baseNum * 5)));
                        mushrooms.Add(new BasicModel(greenMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -3.5f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(greenMushroomModel, new Vector3(random.Next(baseNum * 4) - baseNum * 5, -3.5f, random.Next(baseNum * 4) + baseNum)));
                        mushrooms.Add(new BasicModel(greenMushroomModel, new Vector3(random.Next(baseNum * 4) + baseNum, -3.5f, random.Next(baseNum * 4) - baseNum * 5)));
						break;
				}
			}
           
            foreach (BasicModel mushroomObject in mushrooms)
            {

                mushroomObject.scale(.5f, .5f, .5f);
            }           
        }
    }
}
