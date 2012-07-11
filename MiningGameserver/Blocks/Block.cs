using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using MiningGameServer;
using MiningGameServer.Managers;
using MiningGameServer.Packets;
using MiningGameServer.Entities;
using MiningGameServer.Interfaces;

namespace MiningGameServer.Blocks
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
        private bool _pistonable;
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
            _pistonable = true;

            _hideBlock = false;
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
                //Managers.ConsoleManager.Log("Block " + GetBlockName() + " (" + GetBlockID() + ") is already in list", Color.Red);
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
        }

        private void BlockRemoved(int x, int y)
        {

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

        public Block SetBlockPistonable(bool pistonable)
        {
            _pistonable = pistonable;
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
            return new Rectangle(X * GameServer.BlockSize, Y * GameServer.BlockSize, GameServer.BlockSize, GameServer.BlockSize);
        }

        public virtual void OnBlockTouched(int X, int Y, int side, ServerEntityMoveable toucher)
        {

        }

        public virtual void OnBlockPlaced(int x, int y, bool notify, NetworkPlayer placer = null)
        {
        }

        public virtual void OnBlockRemoved(int x, int y)
        {
            GameServer.UnscheduleUpdate(x, y);
        }

        public virtual void OnBlockUsed(int x, int y)
        {

        }

        public int OnBlockUpdate(int x, int y)
        {
            return -1;
        }

        public virtual byte GetItemDrop(int x, int y)
        {
            return 0;
        }

        public virtual int GetItemDropNum(int x, int y)
        {
            return 0;
        }

        public bool GetBlockPistonable()
        {
            return _pistonable;
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

    public class BlockData : INetTransferable<BlockData>
    {
        public short ID;
        public byte MetaData;
        public Block Block
        {
            get
            {
                return Block.GetBlock(ID);
            }
        }

        public void Write(Packet p)
        {
            p.WriteShort(ID);
            p.WriteByte(MetaData);
        }

        public BlockData Read(Packet p)
        {
            return new BlockData { ID = p.ReadShort(), MetaData = p.ReadByte() };
        }
    }
}
