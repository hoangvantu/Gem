﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gem.Network.Shooter.Client.Actors
{
    using Camera;
    using Gem.Network.Shooter.Client.Input;
    using Level;
    using Scene;

    public class Actor : ACollidable
    {
        private Vector2 fallSpeed = new Vector2(0, 20);
        private bool dead = false;
        private int livesRemaining = 999;
        private EffectsManager EffectsManager;

        #region Velocity Handler Declarations

        private Vector2 friction = Vector2.Zero;
        private Vector2 groundFriction = new Vector2(20f, 0);
        private Vector2 airFriction = new Vector2(15f, 0);
        private Vector2 accelerationAmount = new Vector2(50f, 0);
        private Vector2 sprintAccelerationAmount = new Vector2(40f, 0);
        private Vector2 currentMaxVelocity = Vector2.Zero;
        private Vector2 sprintMaxVelocity = new Vector2(250f, 0);
        private bool onAir;

        #endregion

        public bool Dead
        {
            get { return dead; }
        }

        public float Friction
        {
            get { return friction.X; }
        }

        public bool IsJumping
        {
            get { return onAir; }
        }

        public float VelocityY
        {
            get { return velocity.Y; }
        }

        public int LivesRemaining
        {
            get { return livesRemaining; }
            set { livesRemaining = value; }
        }

        #region Constructor

        public Actor(ContentManager content,Vector2 location)
        {
            frameWidth = 48;
            frameHeight = 48;
            texture = content.Load<Texture2D>(@"block");
            CollisionRectangle = new Rectangle(0, 0, 48, 48);
            this.color = Color.DarkBlue;
            drawDepth = 0.825f;

            enabled = true;


            tileMap = TileMap.GetInstance();
            Camera = CameraManager.GetInstance();
            livesRemaining = 999;
            worldLocation = location;
            EffectsManager = EffectsManager.GetInstance();
        }

        #endregion

        #region Public Methods

        public void HandleInput()
        {
            if (Dead) return;

            if (InputHandler.IsKeyDown(Keys.D))
            {
                velocity += accelerationAmount;
            }
            if (InputHandler.IsKeyDown(Keys.A))
            {
                velocity -= accelerationAmount;
            }
            if (InputHandler.IsKeyDown(Keys.W))
            {
                if (onGround)
                {

                    Jump();
                }
            }
            //   EffectsManager.AddBulletParticle(worldLocation + offSet, bulletDirection);   EffectsManager.AddBulletParticle(worldLocation + offSet, bulletDirection);
        }

        public override void Update(GameTime gameTime)
        {
            velocity += fallSpeed;

            HandleVelocity();

            CheckCollision(gameTime);

            Camera.Move((float)gameTime.ElapsedGameTime.TotalSeconds, WorldLocation, velocity, accelerationAmount.X);
        }

        public void HandleVelocity()
        {
            currentMaxVelocity = sprintMaxVelocity;
            accelerationAmount = sprintAccelerationAmount;

            if (onGround)
            {
                friction = groundFriction;
            }
            else if (onAir)
            {
                friction = airFriction;
            }
            if (velocity.X > 0)
            {
                velocity.X = MathHelper.Clamp(Velocity.X - friction.X / 2,
                    0, +currentMaxVelocity.X);
            }
            else
            {
                velocity.X = MathHelper.Clamp(Velocity.X + friction.X / 2,
                    -currentMaxVelocity.X, 0);
            }
        }

    

        public void Jump()
        {
            velocity.Y = -600;
            onAir = true;
        }

        public void Hit()
        {
            LivesRemaining--;
            velocity.X = 0;
            dead = true;
        }

        public void Revive()
        {
            dead = false;
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}