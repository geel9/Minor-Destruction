using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;

namespace MiningGameServer.GameModes
{
    public class CPGameMode : ServerGameMode
    {
        public ShapeAABB RedCapPoint;
        public ShapeAABB BluCapPoint;

        public float RedProgress;
        public float BlueProgress;

        public override string GetName()
        {
            return "Control Points";
        }

        public CPGameMode()
        {
            RedCapPoint = new ShapeAABB(2520, 384, 168, 104);
            BluCapPoint = new ShapeAABB(120, 384, 168, 104);
        }

        public override void Update_PostAll(Microsoft.Xna.Framework.GameTime time)
        {
            int curProgressR = (int)RedProgress;
            int curProgressB = (int)BlueProgress;

            int enemiesOnRed = 0;
            int enemiesOnBlue = 0;
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                bool touchingRed = p.BoundBox.Intersects(RedCapPoint);
                bool touchingBlue = p.BoundBox.Intersects(BluCapPoint);
                if (!touchingRed && !touchingBlue) continue;

                if (p.PlayerTeam == 0 && touchingRed) enemiesOnRed++;
                if (p.PlayerTeam == 1 && touchingBlue) enemiesOnBlue++;

                //Standing on a CP slows progress/reverses it.
                if (p.PlayerTeam == 1 && touchingRed) enemiesOnRed--;
                if (p.PlayerTeam == 0 && touchingBlue) enemiesOnBlue--;
            }
            float increaseByR = (enemiesOnRed / 10f);
            float increaseByB = (enemiesOnBlue / 10f);
            if (enemiesOnRed == 0) increaseByR = -0.05f;
            if (enemiesOnBlue == 0) increaseByB = -0.05f;

            RedProgress += increaseByR;
            BlueProgress += increaseByB;

            if (RedProgress < 0) RedProgress = 0;
            if (BlueProgress < 0) BlueProgress = 0;

            if (RedProgress != curProgressR)
            {
                SendGameModeEvent("pr", new Packet((byte)RedProgress));
            }
            if (BlueProgress != curProgressB)
            {
                SendGameModeEvent("pb", new Packet((byte)BlueProgress));
            }

            if (RedProgress >= 100 && BlueProgress < 100) //Blue wins
            {
                GameServer.SendMessageToAll("Blue team wins!");
                SendGameModeEvent("w", new Packet(0));
                ResetGame();
            }
            else if (BlueProgress >= 100 && RedProgress < 100) //Red wins
            {
                GameServer.SendMessageToAll("Red team wins!");
                SendGameModeEvent("w", new Packet(1));
                ResetGame();
            }
            else if (RedProgress >= 100 && BlueProgress >= 100) //Tie
            {
                if (enemiesOnBlue == enemiesOnRed)
                {
                    SendGameModeEvent("t", new Packet());
                }
                else
                {
                    SendGameModeEvent("w", new Packet(enemiesOnBlue > enemiesOnRed ? (byte)1 : (byte)0));
                    GameServer.SendMessageToAll((enemiesOnBlue > enemiesOnRed ? "Red" : "Blue") + " team wins!");
                }
                ResetGame();
            }
            base.Update_PostAll(time);
        }

        public override void OnPlayerSpawn(NetworkPlayer player)
        {
            int spawnX = 0;
            if (player.PlayerTeam == 0)
                spawnX = 1060;
            if (player.PlayerTeam == 1)
                spawnX = 1460;
            player.EntityPosition = new Vector2(spawnX, 100);
            base.OnPlayerSpawn(player);
        }

        public void ResetGame()
        {
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                p.HurtPlayer(1000);
                RedProgress = 0;
                BlueProgress = 0;
            }
            MakeBases();
        }

        public void MakeBases()
        {
            //Blu base
            for (int x = 5; x < 13; x++) GameServer.SetBlock(x, 20, 3);
            for (int x = 4; x < 13; x++) GameServer.SetBlock(x, 15, 3);
            for (int y = 15; y < 21; y++) GameServer.SetBlock(4, y, 3);
            GameServer.SetBlock(12, 16, 3);
            GameServer.SetBlock(12, 17, 3);
            GameServer.SetBlock(12, 19, 4);
            GameServer.SetBlock(12, 18, 4);

            //Red base
            for (int x = 104; x < 113; x++) GameServer.SetBlock(x, 20, 3);
            for (int x = 104; x < 113; x++) GameServer.SetBlock(x, 15, 3);
            for (int y = 15; y < 21; y++) GameServer.SetBlock(112, y, 3);
            GameServer.SetBlock(104, 16, 3);
            GameServer.SetBlock(104, 17, 3);
            GameServer.SetBlock(104, 19, 4, true, 1);
            GameServer.SetBlock(104, 18, 4, true, 1);
        }

        public override void OnGameModeChosen()
        {
            MakeBases();
            base.OnGameModeChosen();
        }
    }
}
