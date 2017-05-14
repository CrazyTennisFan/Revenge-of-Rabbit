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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public enum GameState
        {
            Start,            //Set game state
            Story,
            Initial, Run, Pass, Fail,Pause,
            GameCleared
        };

        public enum Tags
        {
            GAME_CLEAR, RESTART_GAME, EXIT_GAME, CHAPTER_CLEAR, LEVEL_DISPLAY, BEARS_LEFT, CH1_TITLE,
            CH1_STORY, CH1_OBJECTIVE, CH2_TITLE, CH2_STORY, CH2_OBJECTIVE, GAME_OVER, RESTART_LEVEL,
            GAME_PAUSED, CONTINUE, CONTROLS, CREDITS, PLAY, PLAYER_SELECT
        }

        GameState currentGameState;
        int curLevel =1;
        int lastLevel = 2;

        SpriteFont bearCount;
        SpriteFont StartupScreenTips;

        // Model stuff
        ModelManager modelManager;

        // Crosshair
        Texture2D startupBackground;
        Texture2D crosshairTexture;
        Texture2D gameoverTexture;
        Texture2D skyTexture;
        Texture2D skyTexture2;
		Texture2D frontUIRabbitTexture;
		Texture2D mHealthBar;
		Texture2D questButton;
		Texture2D instructionButton;
		Texture2D miniMap;

        Texture2D backButton;

        Texture2D startupScreenCarrot;
        Texture2D gameContinuePic;
        Texture2D credits;
        Texture2D instructionMenu;
        Texture2D settingRabbitPanel;
        Texture2D storyScroll;

        Texture2D passChapterOneBackground;
        Texture2D passChapterTwoBackground;
        Texture2D frontUIBearTexture;

		int maxHealth = 5;
        int curHealth = 5;
        
        // To handle invincible mode, when the rabbit gets hit
        int invinciTimer = 0;
        int invinciThreshold = 3000;
        int alpha = 255;
        bool down = true;

        // Shot variables
        float shotSpeed = 1.5f;
        // To control firing speed
        int inputTimer = 0;
        int firingLimiter = 500;

        // Rabbit running speed
        float runSpeed = .3f;

        // Targeted bear's HP
        int targetHP=0;

        // Bear speed
        float baseBearSpeed = .1f;
        float bearSpeed;

        // Camera
        public Camera camera { get; protected set; }

        int startupSelect;
        float intervalOfSelect = 200f;
        float timerOfSelect = 0f;
        Boolean displayCredits;
        Boolean displayInstructionMenu;

        int gamePauseSelect;
        int baseNum = 100;

        float levelPassTimer = 0;
        float levelPassedDelay = 2000;
        //Music part
        //For general background music
        AudioEngine backgroundEngine;

        WaveBank backgroundWaveBank;
        SoundBank backgroundSoundBank;
        Cue backgroundSound;

        WaveBank storyMusicWaveBank;
        SoundBank storyMusicSoundBank;
        Cue storyMusicSoundCue;

        WaveBank levelOneMusicWaveBank;
        SoundBank levelOneMusicSoundBank;
        Cue levelMusicSoundCue;

        WaveBank winMusicWaveBank;
        SoundBank winMusicSoundBank;
        Cue winMusicSoundCue;

        WaveBank levelTwoMusicWaveBank;
        SoundBank levelTwoMusicSoundBank;

        SoundEffect bearHit;
        SoundEffect carrotHit;
        SoundEffect carrotThrow;
        Random random = new Random();
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = true;
            startupSelect = 0;
            gamePauseSelect = 0;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize Camera
            camera = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            // Initialize model manager
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            StartupScreenTips = Content.Load<SpriteFont>(@"Fonts\StartupScreenTips");
            startupScreenCarrot = Content.Load<Texture2D>(@"Textures\startupScreenCarrot");
            startupBackground = Content.Load<Texture2D>(@"Textures\startupBackground");
            credits = Content.Load<Texture2D>(@"Textures\credit");
            instructionMenu = Content.Load<Texture2D>(@"Textures\instructionMenu");
            backButton = Content.Load<Texture2D>(@"Textures\green_back");
            storyScroll = Content.Load<Texture2D>(@"Textures\stroyScroll");
            settingRabbitPanel = Content.Load<Texture2D>(@"Textures\settingPanel");

            passChapterOneBackground = Content.Load<Texture2D>(@"Textures\winChapterOne");
            passChapterTwoBackground = Content.Load<Texture2D>(@"Textures\winChapterTwo");
            frontUIBearTexture = Content.Load<Texture2D>(@"Textures\bear_prehealthbar");

            backgroundEngine = new AudioEngine(@"Content\Music\g.xgs"); 

            backgroundWaveBank = new WaveBank(backgroundEngine, @"Content\Music\g_wave_bank.xwb");
            backgroundSoundBank = new SoundBank(backgroundEngine, @"Content\Music\g_sound_bank.xsb"); 
            backgroundSound = backgroundSoundBank.GetCue("generalBackgroundMusic");

            storyMusicWaveBank = new WaveBank(backgroundEngine, @"Content\Music\story_wave_bank.xwb"); 
            storyMusicSoundBank = new SoundBank(backgroundEngine, @"Content\Music\story_sound_bank.xsb"); 
            storyMusicSoundCue = storyMusicSoundBank.GetCue("storyMusic");

            levelOneMusicWaveBank = new WaveBank(backgroundEngine, @"Content\Music\levelOne_wave_bank.xwb");
            levelOneMusicSoundBank = new SoundBank(backgroundEngine, @"Content\Music\levelOne_sound_bank.xsb");


            winMusicWaveBank = new WaveBank(backgroundEngine, @"Content\Music\win_wave_bank.xwb");
            winMusicSoundBank = new SoundBank(backgroundEngine, @"Content\Music\win_sound_bank.xsb");
            winMusicSoundCue = winMusicSoundBank.GetCue("win");


            levelTwoMusicWaveBank = new WaveBank(backgroundEngine, @"Content\Music\levelTwo_wave_bank.xwb");
            levelTwoMusicSoundBank = new SoundBank(backgroundEngine, @"Content\Music\levelTwo_sound_bank.xsb");

            bearHit = Content.Load<SoundEffect>("Effects/bearHit");
            carrotHit = Content.Load<SoundEffect>("Effects/carrotHit");
            carrotThrow = Content.Load<SoundEffect>("Effects/throw");   


            displayCredits = false;
            displayInstructionMenu = false;

            currentGameState = GameState.Start;
            timerOfSelect = 0;
            camera.setGameState(currentGameState);
            modelManager.setGameState(currentGameState);
            backgroundSound.Play();

            base.Initialize();
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
            loadTexture();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        private void loadTexture()
        {
            // Load the crosshair texture
            crosshairTexture = Content.Load<Texture2D>(@"Textures\crosshair");
            gameoverTexture = Content.Load<Texture2D>(@"Textures\gameover_scene");                  //second level will change 
            skyTexture = Content.Load<Texture2D>(@"Textures\sky");
            skyTexture2 = Content.Load<Texture2D>(@"Textures\sky2");
            frontUIRabbitTexture = Content.Load<Texture2D>(@"Textures\UIrabbit");
            mHealthBar = Content.Load<Texture2D>(@"Textures\HealthBar");
            questButton = Content.Load<Texture2D>(@"Textures\Quest");
            instructionButton = Content.Load<Texture2D>(@"Textures\Instruction");
            miniMap = Content.Load<Texture2D>(@"Textures\miniMap");
            gameContinuePic = Content.Load<Texture2D>(@"Textures\continue");

            bearCount = Content.Load<SpriteFont>(@"Fonts\ScoreFont");

            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            switch (currentGameState)
            {
                case GameState.Start:
                    updateGameStartScreen(gameTime);
                    break;

                case GameState.Story:
                    timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (storyMusicSoundCue.IsPaused == true)
                    {
                        storyMusicSoundCue.Resume();
                    }
                    if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) ||  GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                    {
                        if (timerOfSelect > intervalOfSelect)
                        {
                            timerOfSelect = 0;
                            currentGameState = GameState.Initial;
                        }
                        
                    }                    
                    break;

                case GameState.Initial:
                    camera.setGameState(currentGameState);
                    modelManager.setGameState(currentGameState);
                    curHealth = maxHealth;
                    currentGameState = GameState.Run;
                    bearSpeed = curLevel * baseBearSpeed;
                    if (storyMusicSoundCue.IsPlaying)
                        storyMusicSoundCue.Pause();

                    setLevelMusic(curLevel);                    

                    levelMusicSoundCue.Play();  
                    break;

                case GameState.Run:
                    updatePlayLevel(gameTime);
                    break;

                case GameState.Fail:
                    updatePlayFail(gameTime);
                    break;

                case GameState.Pause:
                    updatePause(gameTime);
                    break;

                case GameState.Pass:
                    timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    
                    if (timerOfSelect > intervalOfSelect)
                    {
                        if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                        {
                            if (curLevel == lastLevel)
                            {
                                currentGameState = GameState.GameCleared;
                                timerOfSelect = -200;
                            }

                            curLevel++;
                            currentGameState = GameState.Story; 
                            
                            timerOfSelect = -200;

                            if (winMusicSoundCue.IsPlaying)
                                winMusicSoundCue.Pause();

                        }
                    }
                    break;

                case GameState.GameCleared:
                    updateClearGame(gameTime);                    
                    break;
            }


            base.Update(gameTime);
        }


        // Sets the level music sound cue to the appropriate Cue
        private void setLevelMusic(int level)
        {
            switch (level)
            {
                case 1:
                    levelMusicSoundCue = levelOneMusicSoundBank.GetCue("levelone");
                    break;
                case 2:
                    levelMusicSoundCue = levelTwoMusicSoundBank.GetCue("leveltwo");
                    break;
            }
        }


        private void updateClearGame(GameTime gameTime)
        {
            timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            camera.setGameState(currentGameState);
            modelManager.setGameState(currentGameState);

           
            if (this.storyMusicSoundCue.IsPlaying)
            {
                storyMusicSoundCue.Pause();                
            }
            if (levelMusicSoundCue.IsPaused)
            {
                levelMusicSoundCue.Resume();
            }


            if ((Keyboard.GetState().IsKeyDown(Keys.W)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)// menu up
            {
                if (timerOfSelect > intervalOfSelect)
                {
                    gamePauseSelect = (gamePauseSelect + 1) % 2;
                    timerOfSelect = 0;
                }
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.S)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)         // menu down                             //Later change to DPad.Down
            {
                if (timerOfSelect > intervalOfSelect)
                {
                    gamePauseSelect = (gamePauseSelect + 1) % 2;
                    timerOfSelect = 0;
                }
            }

            if (timerOfSelect > intervalOfSelect)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                {
                    if (gamePauseSelect == 0)                           //restart the game
                    {
                        timerOfSelect = 0;
                        currentGameState = GameState.Start;
                        camera.setGameState(currentGameState);
                        modelManager.setGameState(currentGameState);

                        if (storyMusicSoundCue.IsPlaying == true)
                        {
                            storyMusicSoundCue.Pause();
                        }

                        backgroundSound.Pause();
                        curLevel = 1;

                    }                   
                    else if (gamePauseSelect == 1)                        //exit game
                    {
                        Exit();
                    }
                }
            }
        }

        private void updatePlayLevel(GameTime gameTime)
        {
            camera.setGameState(currentGameState);
            modelManager.setGameState(currentGameState);

            inputTimer += gameTime.ElapsedGameTime.Milliseconds;

            invinciTimer += gameTime.ElapsedGameTime.Milliseconds;

            // XBOX Pad controller
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                currentGameState = GameState.Pause;
                timerOfSelect = -200;
                camera.setGameState(currentGameState);
                modelManager.setGameState(currentGameState);

                if (levelMusicSoundCue.IsPlaying)
                    levelMusicSoundCue.Pause();

                if (backgroundSound.IsPaused)
                    backgroundSound.Resume();

                return;

            }

            // Easier use of camera position
            Vector3 camPos = camera.cameraPosition;

            // List of carrots that hit bears or moved too far and must be removed
            List<Projectile> toRemove = new List<Projectile>();

            foreach (Projectile carrot in modelManager.getShots())
            {
                carrot.Update();
                foreach (BasicModel target in modelManager.getStaticCollidables())
                {
                    if (Vector3.Distance(target.xzPos(), carrot.xzPos()) < 5)
                    {
                        if (carrot.CollidesWith(target))
                        {
                            toRemove.Add(carrot);
                            break;
                        }
                    }
                }

                foreach (Bear oneBear in modelManager.getBears())
                {
                    if (Vector3.Distance(oneBear.xzPos(), carrot.xzPos()) < 10)
                    if (!oneBear.isDying && oneBear.CollidesWith(carrot))
                    {
                        toRemove.Add(carrot);
                        oneBear.loseHP(1);
                        carrotHit.Play();
                        if (oneBear.curHealth <= 0)
                        {
                            oneBear.die();                            
                        }
                        break;
                    }
                }

                if (carrot.position.Y <= -2.5f)
                    toRemove.Add(carrot);

            }

            // Remove carrots that need to be removed
            foreach (Projectile carrot in toRemove)
                modelManager.getShots().Remove(carrot);


            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = this.GraphicsDevice.Viewport;

            Vector3 dir;
            List<Bear> toRem = new List<Bear>();
            // Make the bears chase the rabbit
            foreach (Bear bear in modelManager.getBears())
            {
                // Die without trapping any bears
                if (bear.isDying)
                {                    
                    bear.rotationX(-10);
                    foreach (Bear target in modelManager.getBears())
                    {
                        if (bear.GetWorld() != target.GetWorld())
                        {
                            if (bear.CollidesWith(target))
                            {
                                bear.rotationX(10);
                            }

                        }
                    }
                    if (MathHelper.ToDegrees(bear.pitchAngle) <= -90)
                    {
                        // Lie it flat
                        bear.rotationX(-90 - MathHelper.ToDegrees(bear.pitchAngle));
                        bear.translation(new Vector3(0, .7f, 0));

                        modelManager.getDeadBears().Add(bear);
                        toRem.Add(bear);
                    }
                    // Next bear
                    continue;
                    
                }

                // If the bear is in attacking range, stop chasing
                if (Vector3.Distance(camera.cameraPosition, bear.position) < 10)
                {
                    bear.attack();
                    bear.stopRandomWalk();
                }

                if (Vector3.Distance(camera.cameraPosition, bear.position) < 8)
                {
                    if (invinciTimer > invinciThreshold)
                    {
                        curHealth--;
                        bearHit.Play();
                        invinciTimer = 0;
                        if (curHealth <= 0)
                        {
                            currentGameState = GameState.Fail;
                            timerOfSelect = -200;
                            camera.setGameState(currentGameState);
                            modelManager.setGameState(currentGameState);
                        }

                    }
                    continue;
                }

                // Otherwise, head straight for the rabbit
                if (!bear.isRandomWalking())
                    dir = Vector3.Normalize(new Vector3(camera.cameraPosition.X - bear.position.X, 0, camera.cameraPosition.Z - bear.position.Z));
                else
                    dir = bear.randomWalk(gameTime);
                
                bear.translation(bearSpeed * dir);
                
                // Collision check
                foreach (BasicModel target in modelManager.getCollidables())
                {
                    if (bear.GetWorld() != target.GetWorld())
                    {

                        if (bear.CollidesWith(target))
                        {
                            bear.translation(-bearSpeed * dir);
                            if (bear.isRandomWalking())
                                bear.changeDir();
                            else
                            {
                                if (random.NextDouble() < .005d)
                                    bear.startRandomWalk();
                                else
                                {

                                    bear.translation(bearSpeed * Vector3.Normalize(new Vector3(dir.Z, 0, -dir.X)));
                                    if (bear.CollidesWith(target))
                                    {
                                        bear.translation(-bearSpeed * Vector3.Normalize(new Vector3(dir.Z, 0, -dir.X)));
                                        if (random.NextDouble() < .1d)
                                            bear.startRandomWalk();
                                    }
                                }
                            }
                            break;
                        }

                    }
                }                
                bear.moveLegs(20 * bearSpeed);

            }

            foreach(Bear bear in toRem)
                modelManager.getBears().Remove(bear);

            float? leastDist = 300;
            targetHP = 0;
            foreach (Bear oneBear in modelManager.getBears())
            {
                // Display the targeted bear's health bar
                float? dist = Intersects(new Vector2((graphics.PreferredBackBufferWidth / 2),
                    (graphics.PreferredBackBufferHeight / 2)),
                    oneBear.model, oneBear.GetWorld(), camera.view, camera.projection, viewport);
                if (dist != null)
                {
                    if (dist < leastDist)
                    {
                        targetHP = oneBear.curHealth;
                        leastDist = dist;
                    }
                }
            }

            // Check if user beat the chapter
            if (modelManager.getBears().Count == 0)
            {
                levelPassTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (levelMusicSoundCue.IsPlaying)
                    levelMusicSoundCue.Pause();

                if (winMusicSoundCue.IsPaused)
                {
                    winMusicSoundCue.Resume();
                }
                else if (!winMusicSoundCue.IsPlaying)
                {
                    winMusicSoundCue.Play();
                }
            }
            if (levelPassTimer >= levelPassedDelay)
            {
                currentGameState = GameState.Pass;
                timerOfSelect = -200;
                levelPassTimer = 0;
                ;
                camera.setGameState(currentGameState);
                modelManager.setGameState(currentGameState);


            }

            // Get a unit vector in the direction of the camera to govern movement
            dir = Vector3.Normalize(camera.GetCameraDirection);


            Vector3 movement = Vector3.Zero;
            if ((Keyboard.GetState().IsKeyDown(Keys.W)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)// up
            {
                movement = (dir - new Vector3(0, dir.Y, 0)) * runSpeed;
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.S)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)         // down                             //Later change to DPad.Down
            {
                movement = -(dir - new Vector3(0, dir.Y, 0)) * runSpeed;
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.A)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0 || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)                                           //Later change to DPad.Left
            {
                movement = new Vector3(dir.Z, 0, -dir.X) * runSpeed;
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.D)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0 || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)                                      //Later change to DPad.Right
            {
                movement += new Vector3(-dir.Z, 0, dir.X) * runSpeed;
            }


            camera.cameraPosition += movement;

            // Make sure movement is valid
            if (!checkMove())
                camera.cameraPosition -= movement;
            if (curLevel < 2)
                baseNum = 100;
            else baseNum = 150;
            if (camera.cameraPosition.X < -baseNum)
                camera.cameraPosition.X = -baseNum;
            else if (camera.cameraPosition.X > baseNum)
                camera.cameraPosition.X = baseNum;
            if (camera.cameraPosition.Z < -baseNum)
                camera.cameraPosition.Z = -baseNum;
            else if (camera.cameraPosition.Z > baseNum)
                camera.cameraPosition.Z = baseNum;


            if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Mouse.GetState().LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && inputTimer > firingLimiter)
            {
                // Add a shot to the model manager
                modelManager.AddShot(camera.cameraPosition+new Vector3(.5f,-1,0), camera.GetCameraDirection * shotSpeed);
                carrotThrow.Play();
                inputTimer = 0;
            }
        }
        


        private void updatePlayFail(GameTime gameTime)
        {
            timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
           
            camera.setGameState(currentGameState);
            modelManager.setGameState(currentGameState);

            if (levelMusicSoundCue.IsPlaying)
            {
                levelMusicSoundCue.Pause();
                storyMusicSoundCue.Resume();
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.W)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)// menu up
            {
                if (timerOfSelect > intervalOfSelect)
                {
                    gamePauseSelect = (gamePauseSelect + 2) % 3;
                    timerOfSelect = 0;
                }
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.S)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)         // menu down                             //Later change to DPad.Down
            {
                if (timerOfSelect > intervalOfSelect)
                {
                    gamePauseSelect = (gamePauseSelect + 1) % 3;
                    timerOfSelect = 0;
                }
            }

            if (timerOfSelect > intervalOfSelect)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                {
                    if (gamePauseSelect == 0)                           //restart the game
                    {
                        timerOfSelect = 0;
                        currentGameState = GameState.Start;
                        camera.setGameState(currentGameState);
                        modelManager.setGameState(currentGameState);
                        if(storyMusicSoundCue.IsPlaying == true)
                        {
                            storyMusicSoundCue.Pause();
                        }

                        curLevel = 1;
                        backgroundSound.Pause();

                    }
                    else if (gamePauseSelect == 1)                      //restart the level
                    {
                        timerOfSelect = -200;
                        currentGameState = GameState.Initial;
                        //camera.setGameState(currentGameState);
                        //modelManager.setGameState(currentGameState);
                        
                    }
                    else if (gamePauseSelect == 2)                        //exit game
                    {
                        Exit();
                    }
                }
            }

        }


        private void updatePause(GameTime gameTime)
        {
            timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

             if (displayInstructionMenu == false)
             {
                 if ((Keyboard.GetState().IsKeyDown(Keys.W)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)// menu up
                {
                    if (timerOfSelect > intervalOfSelect)
                    {                        
                        gamePauseSelect = (gamePauseSelect + 2) % 3;
                        timerOfSelect = 0;
                    }
                }
                 else if ((Keyboard.GetState().IsKeyDown(Keys.S)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)         // menu down                             //Later change to DPad.Down
                {
                    if (timerOfSelect > intervalOfSelect)
                    {
                        gamePauseSelect = (gamePauseSelect + 1) % 3;
                        timerOfSelect = 0;
                    }
                }
             }
             if ((Keyboard.GetState().IsKeyDown(Keys.Escape)) || (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed))
             {                
                 displayInstructionMenu = false;
             }

             if (timerOfSelect > intervalOfSelect)
             {
                 if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                 {
                     if (gamePauseSelect == 0)                           //go back and continue the game
                     {
                         timerOfSelect = 0;
                         currentGameState = GameState.Run;
                         if (levelMusicSoundCue.IsPaused)
                             levelMusicSoundCue.Resume();

                         backgroundSound.Pause();

                     }
                     else if (gamePauseSelect == 1)                      //display instruction menu
                     {
                         displayInstructionMenu = true;
                     }
                     else if (gamePauseSelect == 2)                        //exit game
                     {
                         Exit();
                     }
                 }
             }

        }


        private void updateGameStartScreen(GameTime gameTime)
        {
            timerOfSelect += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            if ((levelMusicSoundCue!=null) && (levelMusicSoundCue.IsPlaying == true))
            {
                levelMusicSoundCue.Pause();
            }

            if ((displayCredits == false) && (displayInstructionMenu == false))
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.W)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)// menu up
                {
                    if (timerOfSelect > intervalOfSelect)
                    {
                        startupSelect = (startupSelect + 3) % 4;
                        timerOfSelect = 0;
                    }
                }
                else if ((Keyboard.GetState().IsKeyDown(Keys.S)) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)         // menu down                             //Later change to DPad.Down
                {
                    if (timerOfSelect > intervalOfSelect)
                    {
                        startupSelect = (startupSelect + 1) % 4;
                        timerOfSelect = 0;
                    }
                }
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.Escape)) || (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed))
            {
                displayCredits = false;
                displayInstructionMenu = false;
            }

            if (backgroundSound.IsPaused == true)
            {
                backgroundSound.Resume();
            }
            else
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                {
                    if (timerOfSelect > intervalOfSelect)
                    {
                        timerOfSelect = -200;
                        if (startupSelect == 0)
                        {                            
                            curLevel = 1;
                            currentGameState = GameState.Story;
                            
                            if (backgroundSound.IsPlaying == true)
                            {
                                backgroundSound.Pause();
                            }
                            if (storyMusicSoundCue.IsPlaying == false)
                            {
                                storyMusicSoundCue.Play();
                            }

                        }

                        if (startupSelect == 1)
                        {
                            displayInstructionMenu = true;
                        }

                        if (startupSelect == 2)
                        {
                            displayCredits = true;
                        }


                        if (startupSelect == 3)
                        {
                            Exit();
                        }
                    }
                }
            }
        }


        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);

            switch (currentGameState)
            {
                case GameState.Start:
                    DisplayGameStartScreen();
                    break;
                case GameState.Initial:
                    DisplayStoryScreen();
                    break;
                case GameState.Run:
                    DisplayGameLevelRun();
                    break;
                case GameState.Pause:
                    DisplayGamePause();
                    break;
                case GameState.Pass:
                    DisplayGameLevelPass();
                    break;
                case GameState.Fail:
                    DisplayGameFail();
                    break;
                case GameState.Story:                
                    DisplayStoryScreen();
                    break;
                case GameState.GameCleared:
                    DisplayClearedScreen();
                    break;

            }

          
            base.Draw(gameTime);
        }


        private void DisplayClearedScreen()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(passChapterTwoBackground,
                             new Vector2(-20, -20),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.6f, 1.2f),                 //scale
                             SpriteEffects.None,
                             0);

            spriteBatch.Draw(storyScroll,
                         new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         new Vector2(0.63f, 0.6f),               //scale
                         SpriteEffects.None,
                         0);

            

            spriteBatch.DrawString(StartupScreenTips,
                                   getText(Tags.GAME_CLEAR),
                                   new Vector2(180, 220),
                                   Color.Red, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);

            spriteBatch.DrawString(StartupScreenTips,
                                    getText(Tags.RESTART_GAME),
                                    new Vector2(310, 280),
                                    Color.Black, 0,
                                    Vector2.Zero,
                                    1.2f,
                                    SpriteEffects.None,
                                    1);

               

            spriteBatch.DrawString(StartupScreenTips,
                                   getText(Tags.EXIT_GAME),
                                   new Vector2(310, 335),
                                   Color.Black, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);


            switch (gamePauseSelect)
            {
                 case 0:
                    spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(230, 270),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                    spriteBatch.DrawString(StartupScreenTips,
                                    getText(Tags.RESTART_GAME),
                                    new Vector2(310, 280),
                                    Color.White, 0,
                                    Vector2.Zero,
                                    1.2f,
                                    SpriteEffects.None,
                                    1);
                    break;


                case 1:
                    spriteBatch.Draw(startupScreenCarrot,
                            new Vector2(230, 320),            // The location (in screen coordinates) to draw the sprite. 
                            null,
                            Color.White,
                            0f,
                            new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                            (float)0.2f,                 //scale
                            SpriteEffects.None,
                            0);
                    spriteBatch.DrawString(StartupScreenTips,
                            getText(Tags.EXIT_GAME),
                            new Vector2(310, 335),
                            Color.White, 0,
                            Vector2.Zero,
                            1.2f,
                            SpriteEffects.None,
                            1);
                    break;
                    
                }
            
            spriteBatch.End();
        }


        private void DisplayGameLevelPass()
        {

            spriteBatch.Begin();

            switch (curLevel)
            {
                case 1:
                    spriteBatch.Draw(this.passChapterOneBackground,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.8f, 0.8f),                 //scale
                             SpriteEffects.None,
                             0);

                    break;
                case 2:
                    spriteBatch.Draw(this.passChapterTwoBackground,
                             new Vector2(-20, -20),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.6f, 1.2f),                 //scale
                             SpriteEffects.None,
                             0);
                    break;
            }

  
            spriteBatch.DrawString(StartupScreenTips,
                                   getText(Tags.CHAPTER_CLEAR),
                                   new Vector2(160, 200),
                                   Color.Yellow, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);
            
            
            spriteBatch.Draw(gameContinuePic,
                         new Vector2(320, 300),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         (float)1.0f,                 //scale
                         SpriteEffects.None,
                         0);
            
            spriteBatch.End();
        }


        private void DisplayGameLevelRun()
        {
            spriteBatch.Begin();
            switch (curLevel)
            {
                case 1:
                    spriteBatch.Draw(skyTexture, Vector2.Zero, Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(skyTexture2, Vector2.Zero, Color.White);
                    break;
                default:
                    spriteBatch.Draw(skyTexture, Vector2.Zero, Color.White);
                    break;
            }

            spriteBatch.End();

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            spriteBatch.Begin();

            foreach (BasicModel model in modelManager.getDrawables())
                model.Draw(camera);

			foreach (Bear bear in modelManager.getBears())
				bear.Draw(camera);

            spriteBatch.End();

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            spriteBatch.Begin();
            
           
            // Draw crosshair
            spriteBatch.Draw(crosshairTexture,
                new Vector2((graphics.PreferredBackBufferWidth / 2)	//(Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2),
					(graphics.PreferredBackBufferHeight/2) //(Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)),
                    Color.White);

            // If we're in invincible mode, make the rabbit picture blink
            if (invinciTimer < invinciThreshold)
            {
                if (alpha >= 255)
                    down = true;

                if (alpha <= 0)
                    down = false;

                if (down)
                    alpha -= 30;
                else
                    alpha += 30;
            }
            else if (alpha < 255)
            {
                alpha += 30;
            }

            spriteBatch.Draw(frontUIRabbitTexture, new Vector2(5, 470), new Color(255,255,255,alpha));
            //Draw the negative space for the health bar
            spriteBatch.Draw(mHealthBar, new Rectangle(120, 520, mHealthBar.Width, 44), new Rectangle(0, 45, mHealthBar.Width, 44), Color.Gray);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(mHealthBar, new Rectangle(120, 520, (int)(mHealthBar.Width * ((double)curHealth / maxHealth)), 44), new Rectangle(0, 45, mHealthBar.Width, 44), Color.Red);

            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, new Rectangle(120, 520, mHealthBar.Width, 44), new Rectangle(0, 0, mHealthBar.Width, 44), Color.White);

            spriteBatch.DrawString(bearCount,
                                          getText(Tags.LEVEL_DISPLAY) + curLevel,
                                          new Vector2(600, 520),
                                          Color.Red,
                                          0,
                                          Vector2.Zero,
                                          1.1f,
                                          SpriteEffects.None,
                                          1);

            if (targetHP != 0)
            {
                spriteBatch.Draw(frontUIBearTexture,
                         new Vector2(100, 10),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         (float)0.45f,                 //scale
                         SpriteEffects.None,
                         0);
                //Draw the negative space for the health bar
                spriteBatch.Draw(mHealthBar, new Rectangle(170, 10, mHealthBar.Width, 44), new Rectangle(0, 45, mHealthBar.Width, 44), Color.Gray);

                //Draw the current health level based on the current Health
                spriteBatch.Draw(mHealthBar, new Rectangle(170, 10, (int)(mHealthBar.Width * ((double)targetHP / Bear.maxHealth)), 44), new Rectangle(0, 45, mHealthBar.Width, 44), Color.Red);

                //Draw the box around the health bar
                spriteBatch.Draw(mHealthBar, new Rectangle(170, 10, mHealthBar.Width, 44), new Rectangle(0, 0, mHealthBar.Width, 44), Color.White);
            }

            spriteBatch.Draw(miniMap, new Rectangle(660, 10, 106, 106), new Rectangle(0, 0, 106, 106), Color.GreenYellow);
            float playerx = camera.cameraPosition.X;
            float playerz = camera.cameraPosition.Z;
            //spriteBatch.Draw(miniMap, new Rectangle(660 + (int)((100 + playerx) / 200.0 * 100.0), 20 + (int)((playerz + 100.0) / 200 * 100.0), 5, 5), new Rectangle(0, 0, 5, 5), Color.Black);
			if (curLevel < 2)
                baseNum = 100;
            else baseNum = 150;
            spriteBatch.Draw(miniMap, new Rectangle(660 + (int)((baseNum + playerx) / (baseNum * 2) * 100), 10 + (int)((playerz + baseNum) / (baseNum * 2) * 100), 5, 5), new Rectangle(0, 0, 5, 5), Color.Black);

            foreach (Bear bear in modelManager.getBears())
            {
                float bearx = bear.position.X;
                float bearz = bear.position.Z;
                //spriteBatch.Draw(miniMap, new Rectangle(660 + (int)((100 + bearx) / 200.0 * 100.0), 20 + (int)((bearz + 100.0) / 200 * 100.0), 5, 5), new Rectangle(0, 0, 5, 5), Color.Red);
                spriteBatch.Draw(miniMap, new Rectangle(660 + (int)((baseNum + bearx) / (baseNum * 2) * 100), 10 + (int)((bearz + baseNum) / (baseNum * 2) * 100), 5, 5), new Rectangle(0, 0, 5, 5), Color.Red);
            }

            spriteBatch.DrawString(bearCount,
                                          getText(Tags.BEARS_LEFT) + modelManager.getBears().Count + "/" + (modelManager.getBears().Count + modelManager.getDeadBears().Count),
                                          new Vector2(600, 550),
                                          Color.Red,
                                          0,
                                          Vector2.Zero,
                                          1.1f,
                                          SpriteEffects.None,
                                          1);
            
            spriteBatch.End();

        }


        private void DisplayStoryScreen()
        {
            spriteBatch.Begin();
            String title = "";
            String text = "";
            String mission = "";


            switch (curLevel)
            {
                case 1:
                    title = getText(Tags.CH1_TITLE);
                    text = getText(Tags.CH1_STORY);
                    mission = getText(Tags.CH1_OBJECTIVE);
                    spriteBatch.Draw(startupBackground,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.45f, 0.58f),                 //scale
                             SpriteEffects.None,
                             0);
                    break;
                case 2:
                    title = getText(Tags.CH2_TITLE);
                    text = getText(Tags.CH2_STORY);
                    mission = getText(Tags.CH2_OBJECTIVE);
                    spriteBatch.Draw(this.passChapterOneBackground,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.8f, 0.8f),                 //scale
                             SpriteEffects.None,
                             0);
                    break;
                default:
                    title = getText(Tags.GAME_OVER);
                    currentGameState = GameState.GameCleared;
                    timerOfSelect = 0;
                    break;

            }

            spriteBatch.Draw(storyScroll,
                         new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         new Vector2(0.63f, 0.6f),               //scale
                         SpriteEffects.None,
                         0);

            spriteBatch.Draw(instructionButton,
                         new Vector2(190, 160),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         new Vector2(0.55f, 0.55f),               //scale
                         SpriteEffects.None,
                         0);

            spriteBatch.Draw(questButton,
                         new Vector2(190, 400),            // The location (in screen coordinates) to draw the sprite. 
                         null,
                         Color.White,
                         0f,
                         new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                         new Vector2(0.6f, 0.6f),               //scale
                         SpriteEffects.None,
                         0);


            spriteBatch.DrawString(StartupScreenTips,
                                   title,
                                   new Vector2(280, 160),
                                   Color.Black, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);
            spriteBatch.DrawString(StartupScreenTips,
                                   text,
                                   new Vector2(180, 220),
                                   Color.Black, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);
            spriteBatch.DrawString(StartupScreenTips,
                                   mission,
                                   new Vector2(260, 400),
                                   Color.Red, 0,
                                   Vector2.Zero,
                                   1.2f,
                                   SpriteEffects.None,
                                   1);
            if (currentGameState != GameState.GameCleared)
            {
                spriteBatch.Draw(gameContinuePic,
                             new Vector2(320, 500),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             (float)1.0f,                 //scale
                             SpriteEffects.None,
                             0);
            }
            spriteBatch.End();
        }

 

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void DisplayGameFail()
        {
            spriteBatch.Begin();


			spriteBatch.Draw(gameoverTexture,
							 new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
							 null,
							 Color.White,
							 0f,
							 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
							 new Vector2(0.45f, 0.58f),                 //scale
							 SpriteEffects.None,
							 0);
                      
            {

                spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.RESTART_GAME),
                                       new Vector2(380, 335),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                spriteBatch.DrawString(StartupScreenTips,
                               getText(Tags.RESTART_LEVEL),
                               new Vector2(380, 385),
                               Color.Black, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);

                spriteBatch.DrawString(StartupScreenTips,
                               getText(Tags.EXIT_GAME),
                               new Vector2(380, 435),
                               Color.Black, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);


                switch (gamePauseSelect)
                {
                    case 0:
                        spriteBatch.Draw(startupScreenCarrot,
                                 new Vector2(300, 320),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.2f,                 //scale
                                 SpriteEffects.None,
                                 0);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.RESTART_GAME),
                                       new Vector2(380, 335),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        break;


                    case 1:
                        spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(300, 370),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                        spriteBatch.DrawString(StartupScreenTips,
                                getText(Tags.RESTART_LEVEL),
                                new Vector2(380, 385),
                                Color.White, 0,
                                Vector2.Zero,
                                1.2f,
                                SpriteEffects.None,
                                1);
                        break;
                    case 2:
                        spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(300, 420),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                        spriteBatch.DrawString(StartupScreenTips,
                                getText(Tags.EXIT_GAME),
                               new Vector2(380, 435),
                               Color.White, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);
                        break;
                }
            }
            spriteBatch.End();
        }


        private void DisplayGamePause()
        {
            spriteBatch.Begin();


            spriteBatch.Draw(this.startupBackground,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.45f, 0.58f),                 //scale
                             SpriteEffects.None,
                             0);
            
            if ((gamePauseSelect == 1) && (displayInstructionMenu == true))
            {
                spriteBatch.Draw(instructionMenu,
                              new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                              null,
                              Color.White,
                              0f,
                              new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                              0.785f,//new Vector2(0.465f, 0.5f),                 //scale
                              SpriteEffects.None,
                              0);
                spriteBatch.Draw(settingRabbitPanel,
                             new Vector2(15, 450),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.5f, 0.5f),                 //scale
                             SpriteEffects.None,
                             0);
            }            
            else
            {
                spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.GAME_PAUSED),
                                       new Vector2(300, 280),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.5f,
                                       SpriteEffects.None,
                                       1);

                spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTINUE),
                                       new Vector2(360, 380),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                
                spriteBatch.DrawString(StartupScreenTips,
                               getText(Tags.CONTROLS),
                               new Vector2(360, 430),
                               Color.Black, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);

                spriteBatch.DrawString(StartupScreenTips,
                               getText(Tags.EXIT_GAME),
                               new Vector2(360, 480),
                               Color.Black, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);


                switch (gamePauseSelect)
                {
                    case 0:
                        spriteBatch.Draw(startupScreenCarrot,
                                 new Vector2(280, 370),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.2f,                 //scale
                                 SpriteEffects.None,
                                 0);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTINUE),
                                       new Vector2(360, 380),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        
                        break;
                 
                  
                    case 1:
                        spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(280, 420),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                        spriteBatch.DrawString(StartupScreenTips,
                                getText(Tags.CONTROLS),
                                new Vector2(360, 430),
                                Color.White, 0,
                                Vector2.Zero,
                                1.2f,
                                SpriteEffects.None,
                                1);                              
                        break;
                    case 2:
                        spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(280, 470),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                        spriteBatch.DrawString(StartupScreenTips,
                                getText(Tags.EXIT_GAME),
                               new Vector2(360, 480),
                               Color.White, 0,
                               Vector2.Zero,
                               1.2f,
                               SpriteEffects.None,
                               1);
                        break;
                }
            }
            spriteBatch.End();
        }


        private void DisplayGameStartScreen()
        {
            spriteBatch.Begin();

   
            spriteBatch.Draw(this.startupBackground,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.45f,0.58f),                 //scale
                             SpriteEffects.None,
                             0);
            
            
            if ((startupSelect == 1) && (displayInstructionMenu == true))
            {
                spriteBatch.Draw(instructionMenu,
                             new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             0.785f, //new Vector2(0.465f,0.5f),                 //scale
                             SpriteEffects.None,
                             0);
                spriteBatch.Draw(settingRabbitPanel,
                             new Vector2(15, 450),            // The location (in screen coordinates) to draw the sprite. 
                             null,
                             Color.White,
                             0f,
                             new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                             new Vector2(0.5f,0.5f),                 //scale
                             SpriteEffects.None,
                             0);
                
            }
            else if ((startupSelect == 2) && (displayCredits == true))
            {
                spriteBatch.Draw(credits,
                                new Vector2(150, 0),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.6f,                 //scale
                                SpriteEffects.None,
                                0);

                spriteBatch.DrawString(StartupScreenTips,
                                       "Team 9",
                                       new Vector2(350, 150),
                                       Color.Blue, 0,
                                       Vector2.Zero,
                                       1.5f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.DrawString(StartupScreenTips,
                                       "Icaro Freitas",
                                       new Vector2(350, 200),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.DrawString(StartupScreenTips,
                                       "Fang Shen",
                                       new Vector2(350, 250),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.DrawString(StartupScreenTips,
                                       "Chang Wei",
                                       new Vector2(350, 300),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.DrawString(StartupScreenTips,
                                       "Guang Yang",
                                       new Vector2(350, 350),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.Draw(backButton,
                               new Vector2(370, 400),            // The location (in screen coordinates) to draw the sprite. 
                               null,
                               Color.White,
                               0f,
                               new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                               (float)0.4f,                 //scale
                               SpriteEffects.None,
                               0);

            }
            else
            {
                spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.PLAYER_SELECT),
                                       new Vector2(300, 280),
                                       Color.Red, 0,
                                       Vector2.Zero,
                                       1.5f,
                                       SpriteEffects.None,
                                       1);

                switch (startupSelect)
                {
                    case 0:
                        spriteBatch.Draw(startupScreenCarrot,
                                 new Vector2(280, 370),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.2f,                 //scale
                                 SpriteEffects.None,
                                 0);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.PLAY),
                                       new Vector2(360, 380),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTROLS),
                                       new Vector2(360, 435),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CREDITS),
                                       new Vector2(360, 485),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.EXIT_GAME),
                                       new Vector2(360, 535),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        break;
                    case 1:
                        spriteBatch.Draw(startupScreenCarrot,
                                new Vector2(280, 420),            // The location (in screen coordinates) to draw the sprite. 
                                null,
                                Color.White,
                                0f,
                                new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                (float)0.2f,                 //scale
                                SpriteEffects.None,
                                0);
                        spriteBatch.DrawString(StartupScreenTips,
                                      getText(Tags.PLAY),
                                      new Vector2(360, 380),
                                      Color.Black, 0,
                                      Vector2.Zero,
                                      1.2f,
                                      SpriteEffects.None,
                                      1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTROLS),
                                       new Vector2(360, 435),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CREDITS),
                                       new Vector2(360, 485),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.EXIT_GAME),
                                       new Vector2(360, 535),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        break;
                    case 2:
                        spriteBatch.Draw(startupScreenCarrot,
                                 new Vector2(280, 470),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.2f,                 //scale
                                 SpriteEffects.None,
                                 0);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.PLAY),
                                       new Vector2(360, 380),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTROLS),
                                       new Vector2(360, 435),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CREDITS),
                                       new Vector2(360, 485),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.EXIT_GAME),
                                       new Vector2(360, 535),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        break;
                    case 3:

                        spriteBatch.Draw(startupScreenCarrot,
                                 new Vector2(280, 520),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.2f,                 //scale
                                 SpriteEffects.None,
                                 0);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.PLAY),
                                       new Vector2(360, 380),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CONTROLS),
                                       new Vector2(360, 435),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);

                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.CREDITS),
                                       new Vector2(360, 485),
                                       Color.Black, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        spriteBatch.DrawString(StartupScreenTips,
                                       getText(Tags.EXIT_GAME),
                                       new Vector2(360, 535),
                                       Color.White, 0,
                                       Vector2.Zero,
                                       1.2f,
                                       SpriteEffects.None,
                                       1);
                        break;
                }
            }
            
            spriteBatch.End();
        }

        public bool checkMove()
        {
            Bear rabbit = modelManager.rabbit;
            rabbit.translation(-rabbit.position);
            rabbit.translation(camera.cameraPosition);

            List<BasicModel> collidables;
            if (invinciTimer > invinciThreshold)
                collidables = modelManager.getCollidables();
            else
                collidables = modelManager.getStaticCollidables();

            foreach (BasicModel target in collidables)
            {
                if (Vector3.Distance(camera.cameraPosition, target.xzPos()) < 10)
                    if(rabbit.CollidesWith(target))
                        return false;
            }
          
            return true;

        }

        public Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 1.0f),
                    projection,
                    view,
                    Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation,
                    Matrix view, Matrix projection, Viewport viewport)
        {
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(sphere);
        }

        public float? Intersects(Vector2 mouseLocation,
                    Model model, Matrix world,
                    Matrix view, Matrix projection,
                    Viewport viewport)
        {
            for (int index = 0; index < model.Meshes.Count; index++)
            {
                BoundingSphere sphere = model.Meshes[index].BoundingSphere;
                sphere = sphere.Transform(world);
                float? distance = IntersectDistance(sphere, mouseLocation, view, projection, viewport);

                if (distance != null)
                {
                    return distance;
                }
            }

            return null;
        }

        private String getText(Tags selection)
        {
            switch (selection)
            {
                case Tags.GAME_CLEAR:
                    return "You Win! You Beat All The Bears!";
                case Tags.RESTART_GAME:
                    return "RESTART GAME";
                case Tags.EXIT_GAME:
                    return "EXIT GAME";
                case Tags.CHAPTER_CLEAR:
                    return "Chapter Cleared! Congratulations!";
                case Tags.LEVEL_DISPLAY:
                    return "Level : ";
                case Tags.BEARS_LEFT:
                    return "Bears Left: ";
                case Tags.CH1_TITLE:
                    return "Chapter One";
                case Tags.CH1_STORY:
                    return " The bears invaded your homeland,\n and captured some of your brothers\n and sisters! You need to use your\n weapon to fight them!";
                case Tags.CH1_OBJECTIVE:
                    return "Beat Six Bears !!";
                case Tags.CH2_TITLE:
                    return "Chapter Two";
                case Tags.CH2_STORY:
                    return " Some bears still walk around your\n homeland. Kick the bears out of\n your homeland completely! Fight!!!";
                case Tags.CH2_OBJECTIVE:
                    return "Beat The Bears Around \nYour Homeland !!";
                case Tags.GAME_OVER:
                    return "Game over";
                case Tags.RESTART_LEVEL:
                    return "RESTART LEVEL";
                case Tags.GAME_PAUSED:
                    return "GAME PAUSED";
                case Tags.CONTINUE:
                    return "CONTINUE";
                case Tags.CONTROLS:
                    return "CONTROLS";
                case Tags.CREDITS:
                    return "CREDITS";
                case Tags.PLAY:
                    return "PLAY";
                case Tags.PLAYER_SELECT:
                    return "PLAYER SELECT";
                default:
                    return selection.ToString();
            }
        }
    }
   

}

