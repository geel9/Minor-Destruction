using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework;
using MiningGame.Code.Server;

namespace MiningGame.Code.Blocks
{
    public class Block
    {
        public static List<Block> AllBlocks = new List<Block>();

        private string _blockName;
        private byte _blockID;
        private int _blockHardness;
        private bool _blockRenderSpecial;
        private bool _blockWalkThrough = false;
        private int _lightLevel = 0;
        private int _numConnectionsAllowed = 0;
        private bool _blockOpaque;
        private Color _blockColor;
        private bool _hideBlock;
        private static Random _random = new Random();

        private Func<int, int, SpriteBatch, Texture2D> _renderBlock;
        private Func<int, int, int> _onBlockUpdated;
        private Action<int, int> _onBlockUsed;
        private Action<int, int, bool> _onBlockPlaced;
        private Action<int, int> _onBlockRemoved;
        private Action<int, int, int, Entities.EntityMoveable> _onBlockTouched;
        private Action<int, int, int, int> _onBlockConnected;
        private Func<int, int, byte> _getBlockDrop;
        private Func<int, int, int> _getBlockDropNum;
        private Func<int, int, Rectangle> _getBlockBB;

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
            _renderBlock = (x, y, sb) => { return null; };
            _onBlockUpdated = (x, y) => { return -1; };
            _onBlockUsed = (x, y) => { };
            _onBlockPlaced = (x, y, notify) => { };
            _onBlockRemoved = (x, y) => { };
            _onBlockTouched = (x, y, side, entity) => { };
            _onBlockConnected = (x1, y1, x2, y2) => { };
            _getBlockDrop = (int x, int y) => { return (byte)0; };
            _getBlockDropNum = (x, y) => { return 0; };
            _getBlockBB = (x, y) => { Vector2 pos = new Vector2(x * GameWorld.BlockWidth, y * GameWorld.BlockHeight); return new Rectangle((int)pos.X, (int)pos.Y, GameWorld.BlockWidth, GameWorld.BlockHeight); };

        }

        public Block finalizeBlock()
        {
            if (!AllBlocks.Contains(this))
            {
                AllBlocks.Add(this);
                return this;
            }
            else
            {
                Managers.ConsoleManager.Log("Block " + getBlockName() + " (" + getBlockID() + ") is already in list", Color.Red);
                return this;
            }
        }

    

        public static Block getBlock(byte blockID)
        {
            IEnumerable<Block> query = AllBlocks.Where(x => x._blockID == blockID);
            return (query.Count() > 0) ? query.First() : new Block();
        }

        private void blockPlaced(int x, int y, bool notify = true)
        {
            if (getBlockLightLevel() >= 1)
            {
                GameWorld.lightUpAroundRadius(x, y, getBlockLightLevel(), 0);
            }
        }

        private void blockRemoved(int x, int y)
        {
            GameServer.UnscheduleUpdate(x, y);
            if (getBlockLightLevel() >= 1)
            {
                GameWorld.lightUpAroundRadius(x, y, getBlockLightLevel(), 0, -1);
            }
        }

        public Block setBlockOnPlaced(Action<int, int, bool> onBlockPlaced)
        {
            this._onBlockPlaced = onBlockPlaced;
            return this;
        }

        public Block setBlockOnRemoved(Action<int, int> onBlockRemoved)
        {
            this._onBlockRemoved = onBlockRemoved;
            return this;
        }

        public Block setBlockOnTouched(Action<int, int, int, Entities.EntityMoveable> onBlockTouched)
        {
            this._onBlockTouched = onBlockTouched;
            return this;
        }

        public Block setBlockOnUsed(Action<int, int> onBlockUsed)
        {
            this._onBlockUsed = onBlockUsed;
            return this;
        }

        public Block setBlockGetBB(Func<int, int, Rectangle> getBB)
        {
            this._getBlockBB = getBB;
            return this;
        }

        public Block setBlockGetRender(Func<int, int, SpriteBatch, Texture2D> onBlockRender)
        {
            this._renderBlock = onBlockRender;
            return this;
        }

        public Block setBlockGetDrop(Func<int, int, byte> onBlockGetDrop)
        {
            this._getBlockDrop = onBlockGetDrop;
            return this;
        }

        public Block setBlockOnUpdated(Func<int, int, int> onBlockUpdated)
        {
            this._onBlockUpdated = onBlockUpdated;
            return this;
        }

        public Block setBlockGetDropNum(Func<int, int, int> onBlockGetDropNum)
        {
            this._getBlockDropNum = onBlockGetDropNum;
            return this;
        }

        public Block setBlockOnConnected(Action<int, int, int, int> act)
        {
            _onBlockConnected = act;
            return this;
        }

        public Block setBlockID(byte id)
        {
            this._blockID = id;
            return this;
        }

        public Block setBlockNumConnectionsAllowed(int a)
        {
            _numConnectionsAllowed = a;
            return this;
        }

        public Block setBlockHide(bool hide)
        {
            this._hideBlock = hide;
            return this;
        }

        public Block setBlockOpaque(bool opaque)
        {
            _blockOpaque = opaque;
            return this;
        }

        public Block setBlockLightLevel(int l)
        {
            this._lightLevel = l;
            return this;
        }

        public Block setBlockName(string name)
        {
            this._blockName = name;
            return this;
        }

        public Block setBlockHardness(int hardness)
        {
            this._blockHardness = hardness;
            return this;
        }

        public Block setBlockRenderSpecial(bool special)
        {
            _blockRenderSpecial = special;
            return this;
        }

        public Block setBlockColor(Color c)
        {
            setBlockRenderSpecial(false);
            _blockColor = c;
            return this;
        }

        public Block setBlockColorRGBA(int r, int g, int b, int a = 255)
        {
            setBlockRenderSpecial(false);
            _blockColor = new Color(r, g, b, a);
            return this;
        }

        public Block setBlockWalkThrough(bool b)
        {
            _blockWalkThrough = b;
            return this;
        }

        public Func<int, int, Rectangle> getBlockBoundBox()
        {
            return _getBlockBB;
        }

        public Action<int, int, int, Entities.EntityMoveable> getBlockOnTouched()
        {
            return _onBlockTouched;
        }

        public Func<int, int, SpriteBatch, Texture2D> getRenderBlock()
        {
            return _renderBlock;
        }

        public Action<int, int, bool> getBlockPlaced(int x, int y, bool notify)
        {
            blockPlaced(x, y, notify);
            return _onBlockPlaced;
        }

        public Action<int, int> getBlockRemoved(int x, int y)
        {
            blockRemoved(x, y);
            return _onBlockRemoved;
        }

        public Action<int, int> getBlockUsed()
        {
            return _onBlockUsed;
        }

        public Action<int, int, int, int> getBlockOnConnected(){
            return _onBlockConnected;
        }

        public Func<int, int, int> getBlockOnUpdate()
        {
            return _onBlockUpdated;
        }

        public Func<int, int, byte> getItemDrop()
        {
            return _getBlockDrop;
        }

        public Func<int, int, int> getItemDropNum()
        {
            return _getBlockDropNum;
        }

        public bool getBlockWalkThrough()
        {
            return _blockWalkThrough;
        }

        public bool getBlockHide()
        {
            return _hideBlock;
        }

        public int getBlockNumConnectionsAllowed()
        {
            return _numConnectionsAllowed;
        }

        public byte getBlockID()
        {
            return _blockID;
        }

        public int getBlockLightLevel()
        {
            return _lightLevel;
        }

        public string getBlockName()
        {
            return _blockName;
        }

        public bool getBlockOpaque()
        {
            return _blockOpaque;
        }

        public int getBlockHardness()
        {
            return _blockHardness;
        }

        public bool getBlockRenderSpecial()
        {
            return _blockRenderSpecial;
        }

        public Color getBlockRenderColor()
        {
            return _blockColor;
        }

        public override string ToString()
        {
            return _blockID + " " + _blockName;
        }
    }
}
