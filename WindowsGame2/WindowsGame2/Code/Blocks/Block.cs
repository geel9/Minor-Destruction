using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
using MiningGameServer;

namespace MiningGame.Code.Blocks
{
    public class Block
    {
        public static List<Block> AllBlocks = new List<Block>();

        private string _blockName;
        private short _blockID;
        private int _blockHardness;
        private bool _blockRenderSpecial;
        private bool _blockWalkThrough = false;
        private int _lightLevel = 0;
        private int _numConnectionsAllowed = 0;
        private bool _blockOpaque;
        private Color _blockColor;
        private bool _hideBlock;
        private static Random _random = new Random();

        public static void GenerateBlocks()
        {
            Type[] types = ReflectionManager.GetAllSubClassesOf<Block>(true);
            foreach (Type t in types)
            {
                Block b = (Block)Activator.CreateInstance(t);
                b.FinalizeBlock();
            }
        }

        public Block()
        {
            _blockName = "block";
            _blockID = 0;
            _blockHardness = 1;
            _blockRenderSpecial = false;
            _blockWalkThrough = false;
            _lightLevel = 0;
            _blockOpaque = false;
            _blockColor = Color.White;
            _numConnectionsAllowed = 0;

            _hideBlock = false;
            /*_renderBlock = (x, y, sb) => { return null; };
            _onBlockUpdated = (x, y) => { return -1; };
            _onBlockUsed = (x, y) => { };
            _onBlockPlaced = (x, y, notify) => { };
            _onBlockRemoved = (x, y) => { };
            _onBlockTouched = (x, y, side, entity) => { };
            _getBlockDrop = (int x, int y) => { return (byte)0; };
            _getBlockDropNum = (x, y) => { return 0; };
            _getBlockBB = (x, y) => { Vector2 pos = new Vector2(x * GameWorld.BlockSize, y * GameWorld.BlockSize); return new Rectangle((int)pos.X, (int)pos.Y, GameWorld.BlockSize, GameWorld.BlockSize); };
*/
        }

        public Block FinalizeBlock()
        {
            if (!AllBlocks.Contains(this))
            {
                AllBlocks.Add(this);
                return this;
            }
            else
            {
                Managers.ConsoleManager.Log("Block " + GetBlockName() + " (" + GetBlockID() + ") is already in list", Color.Red);
                return this;
            }
        }

        public static Block GetBlock(short blockID)
        {
            IEnumerable<Block> query = AllBlocks.Where(x => x._blockID == blockID);
            return (query.Count() > 0) ? query.First() : new Block();
        }

        private void BlockPlaced(int x, int y, bool notify = true)
        {
            /*if (GetBlockLightLevel() >= 1)
            {
                GameWorld.lightUpAroundRadius(x, y, GetBlockLightLevel(), 0);
            }*/
        }

        private void BlockRemoved(int x, int y)
        {
            //GameServer.UnscheduleUpdate(x, y);
            /*if (GetBlockLightLevel() >= 1)
            {
                GameWorld.lightUpAroundRadius(x, y, GetBlockLightLevel(), 0, -1);
            }*/
        }


        public Block SetBlockID(short id)
        {
            this._blockID = id;
            return this;
        }

        public Block SetBlockHide(bool hide)
        {
            this._hideBlock = hide;
            return this;
        }

        public Block SetBlockOpaque(bool opaque)
        {
            _blockOpaque = opaque;
            return this;
        }

        public Block SetBlockLightLevel(int l)
        {
            this._lightLevel = l;
            return this;
        }

        public Block SetBlockName(string name)
        {
            _blockName = name;
            return this;
        }

        public Block SetBlockHardness(int hardness)
        {
            this._blockHardness = hardness;
            return this;
        }

        public Block SetBlockRenderSpecial(bool special)
        {
            _blockRenderSpecial = special;
            return this;
        }

        public Block SetBlockColor(Color c)
        {
            SetBlockRenderSpecial(false);
            _blockColor = c;
            return this;
        }

        public Block SetBlockColorRGBA(int r, int g, int b, int a = 255)
        {
            SetBlockRenderSpecial(false);
            _blockColor = new Color(r, g, b, a);
            return this;
        }

        public Block SetBlockWalkThrough(bool b)
        {
            _blockWalkThrough = b;
            return this;
        }

        public virtual Rectangle GetBlockBoundBox(int X, int Y)
        {
            return new Rectangle(X * GameWorld.BlockSize, Y * GameWorld.BlockSize, GameWorld.BlockSize, GameWorld.BlockSize);
        }

        public virtual void OnBlockTouched(int X, int Y, int side, Entities.EntityMoveable toucher)
        {

        }

        public virtual BlockRenderer RenderBlock(int x, int y, SpriteBatch sb)
        {
            return null;
        }

        public virtual void OnBlockPlaced(int x, int y, bool notify)
        {
        }

        public virtual void OnBlockRemoved(int x, int y)
        {

        }

        public virtual void OnBlockUsed(int x, int y)
        {

        }

        public int OnBlockUpdate(int x, int y)
        {
            return -1;
        }

        public byte GetItemDrop(int x, int y)
        {
            return 0;
        }

        public int GetItemDropNum(int x, int y)
        {
            return 0;
        }

        public bool GetBlockWalkThrough()
        {
            return _blockWalkThrough;
        }

        public bool GetBlockHide()
        {
            return _hideBlock;
        }

        public int GetBlockNumConnectionsAllowed()
        {
            return _numConnectionsAllowed;
        }

        public short GetBlockID()
        {
            return _blockID;
        }

        public int GetBlockLightLevel()
        {
            return _lightLevel;
        }

        public string GetBlockName()
        {
            return _blockName;
        }

        public bool GetBlockOpaque()
        {
            return _blockOpaque;
        }

        public int GetBlockHardness()
        {
            return _blockHardness;
        }

        public bool GetBlockRenderSpecial()
        {
            return _blockRenderSpecial;
        }

        public Color GetBlockRenderColor()
        {
            return _blockColor;
        }


        public override string ToString()
        {
            return _blockID + " " + _blockName;
        }
    }

    public class BlockRenderer
    {
        public Texture2D Texture;
        public SpriteEffects Effects = SpriteEffects.None;
        public float Rotation = 0f;
        public Vector2 Origin = Vector2.Zero;
        public bool Transparent = false;

        public BlockRenderer(Texture2D texture)
        {
            this.Texture = texture;
        }

        public BlockRenderer(Texture2D texture, SpriteEffects effects)
            : this(texture)
        {
            this.Effects = effects;
        }

        public BlockRenderer(Texture2D texture, float rotation)
            : this(texture)
        {
            this.Rotation = rotation;
        }
        public BlockRenderer(Texture2D texture, SpriteEffects effects, float rotation)
            : this(texture, effects)
        {
            this.Rotation = rotation;
        }
        public BlockRenderer(Texture2D texture, SpriteEffects effects, float rotation, bool transparent)
            : this(texture, effects)
        {
            this.Rotation = rotation;
            this.Transparent = transparent;
        }
    }

    public struct BlockData
    {
        public short ID;
        public byte MetaData;
        public Block Block
        {
            get { return Block.GetBlock(ID); }
        }
    }
}
