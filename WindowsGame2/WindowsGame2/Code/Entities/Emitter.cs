using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Structs;
using MiningGame.Code.CInterfaces;
namespace MiningGame.Code.Entities
{
    public class Emitter : Updatable
    {

        private EmitRules rules;
        private string particleName = "A_on";
        private int interval = 0;
        private int amount = -1;
        private Vector2 Position;

        public Emitter(EmitRules rules, string particle, Vector2 Position, int amount = 100)
        {
            this.rules = rules;
            this.particleName = particle;
            this.Position = Position;
            this.amount = amount;
        }


        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
            if (!Main.PauseManager.Paused)
            {
                interval++;
                if (amount <= 0)
                {
                    removeFromUpdateList();
                }
                if (interval >= rules.interval)
                {
                    amount--;
                    EmitDirectionMode mode = rules.mode;
                    if (mode == EmitDirectionMode.Fixed)
                    {
                        Particle p = new Particle(this.Position, rules.emitDegrees, particleName, rules.speed, rules.scale, rules.particleLifetime);
                    }
                    else if (mode == EmitDirectionMode.Random)
                    {
                        Random r = new Random();
                        double degrees = r.Next(0, 360);
                        Particle p = new Particle(this.Position, degrees, particleName, rules.speed, rules.scale, rules.particleLifetime);
                    }
                    else if (mode == EmitDirectionMode.Range)
                    {
                        Random r = new Random();
                        double degrees = r.Next((int)rules.range.X, (int)rules.range.Y);
                        Particle p = new Particle(this.Position, degrees, particleName, rules.speed, rules.scale, rules.particleLifetime);
                    }
                    interval = 0;
                }
            }
        }

        public void addToUpdateList()
        {
            if (!Main.updatables.Contains(this)) Main.updatables.Add(this);
        }

        public void removeFromUpdateList()
        {
            Main.updatables.Remove(this);
        }
    }
}
