﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Managers;
using MiningGame.Code.Server;
using MiningGame.Code.Structs;
using YogUILibrary.Managers;

namespace MiningGame.Code.Entities
{
    public class ProjectileArrow : EntityProjectile
    {
        public byte PlayerOwner = 0;

        public override byte GetProjectileType()
        {
            return 1;
        }

        public void BlockCollision(bool serverContext)
        {
            List<Vector2> tilesHitting = RectangleHitsTiles(BoundBox);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            AABB newRectTest = BoundBox;
            List<Vector2> newTilesHitting = RectangleHitsTiles(newRectTest);
            EntityPosition -= EntityVelocity;

            //The amount of tiles the player will be entering with his new position.
            List<Vector2> newTiles = newTilesHitting;

            int highestY = 0;
            for (int i = 0; i < newTiles.Count; i++)
            {
                if (newTiles[i].Y > highestY) highestY = (int)newTiles[i].Y;
            }

            foreach (Vector2 newTile in newTiles)
            {
                byte blockID = GameWorld.GetBlockIDAt(newTile.X, newTile.Y);

                if (blockID == 0) continue;

                Block block = Block.GetBlock(blockID);
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                AABB thisAABB = newRectTest;
                AABB blockAABB = new AABB(blockBB);
                Vector2 collide = thisAABB.AxisCollide(blockAABB);
                if (collide.X != 0)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X = (newEntityPosition.X + collide.X);
                        ShouldDestroy = true;
                        if (serverContext)
                            GameServer.SetBlock((int)newTile.X, (int)newTile.Y, 0);
                        break;
                    }
                }
                if (collide.Y != 0 && newTile.Y >= highestY)
                {
                    bool up = (collide.Y > 0);

                    if (!walkThrough)
                    {
                        EntityVelocity.Y = 0;
                        EntityPosition.Y = (newEntityPosition.Y + collide.Y);
                        ShouldDestroy = true;
                        if (serverContext)
                            GameServer.SetBlock((int)newTile.X, (int)newTile.Y, 0);
                        break;
                    }
                }
            }
        }

        public void PlayerCollision(bool serverContext)
        {
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            AABB newRectTest = BoundBox;
            EntityPosition -= EntityVelocity;

            NetworkPlayer playerOwner = new NetworkPlayer(PlayerOwner, null, Vector2.Zero, "PLAYER");
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerEntity.PlayerID == PlayerOwner)
                {
                    playerOwner = p;
                    break;
                }
            }

            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerEntity.PlayerID == PlayerOwner) continue;
                if (p.PlayerEntity.BoundBox.Intersects(newRectTest))
                {
                    ShouldDestroy = true;
                    p.PlayerHealth--;
                    if (p.PlayerHealth <= 0)
                    {
                        p.PlayerHealth = 5;
                        p.PlayerEntity.EntityPosition = new Vector2(50, 50);
                        GameServer.SendMessageToAll(playerOwner.PlayerEntity.PlayerName + " killed " + p.PlayerEntity.PlayerName + ".");
                    }
                    else
                        GameServer.SendMessageToAll(playerOwner.PlayerEntity.PlayerName + " hit " + p.PlayerEntity.PlayerName + ". Their new health: " + p.PlayerHealth);
                    break;
                }
            }
        }

        public override void EntityMovement(bool serverContext)
        {
            if (ShouldDestroy) return;
            if (BoundBox.Left < 0 || BoundBox.Top < 0 || BoundBox.Right > GameWorld.BlockWidth * GameWorld.WorldSizeX || BoundBox.Bottom > GameWorld.BlockHeight * GameWorld.WorldSizeY)
            {
                ShouldDestroy = true;
                return;
            }
            if (serverContext)
                PlayerCollision(serverContext);
            if (!ShouldDestroy)
                BlockCollision(serverContext);

            EntityPosition += EntityVelocity;

            if (EntityVelocity.Y < 6)
                EntityVelocity.Y += EffectOfGravity;


            if (EntityVelocity.X < 0) EntityVelocity.X += AirFriction;
            if (EntityVelocity.X > 0) EntityVelocity.X -= AirFriction;
        }


        public ProjectileArrow(Vector2 Position, float angle, byte owner, int strength = 10)
            : base()
        {
            EntityPosition = Position;
            float strengthX = (float)(strength * Math.Cos(angle));
            float strengthY = (float)(strength * Math.Sin(angle));

            EntityVelocity = new Vector2(strengthX, strengthY);
            LastPosition = Position - EntityVelocity;

            EffectOfGravity = 0.13f;

            SpriteTexture = AssetManager.GetTexture("ladder");
            Alpha = 255;
            PlayerOwner = owner;
        }
    }
}
