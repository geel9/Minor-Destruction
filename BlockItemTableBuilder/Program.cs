using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockItemTableBuilder
{
    //Create a class that can be used for both Client and Server
    struct Block
    {
        public string Name;
        public int ID;
        public int ItemDrop;
        public int ItemDropNum;
    }

    struct Item
    {
        public string Name;
        public int ID;
        public int BlockID;
    }
    class Program
    {
        public static Block ClientToLocal(MiningGame.Code.Blocks.Block block)
        {
            return new Block{ Name = block.GetBlockName(), ID = block.GetBlockID(), ItemDrop = block.GetItemDrop(0, 0), ItemDropNum = block.GetItemDropNum(0, 0)};
        }
        public static Block ServerToLocal(MiningGameServer.Blocks.Block block)
        {
            return new Block { Name = block.GetBlockName(), ID = block.GetBlockID(), ItemDrop = block.GetItemDrop(0, 0), ItemDropNum = block.GetItemDropNum(0, 0) };
        }

        public static Item ClientToLocal(MiningGame.Code.Items.Item item)
        {
            return new Item() { Name = item.GetName(), ID = item.GetItemID(), BlockID = item.GetBlockID()};
        }
        public static Item ServerToLocal(MiningGameServer.Items.ServerItem item)
        {
            return new Item() { Name = item.GetName(), ID = item.GetItemID(), BlockID = item.GetBlockID() };
        }

        public static Block[] GetServerBlocks()
        {
            List<Block> ret = new List<Block>();
            foreach (MiningGameServer.Blocks.Block b in MiningGameServer.Blocks.Block.AllBlocks)
            {
                ret.Add(ServerToLocal(b));
            }
            return ret.ToArray();
        }
        public static Block[] GetClientBlocks()
        {
            List<Block> ret = new List<Block>();
            foreach (MiningGame.Code.Blocks.Block b in MiningGame.Code.Blocks.Block.AllBlocks)
            {
                ret.Add(ClientToLocal(b));
            }
            return ret.ToArray();
        }

        public static Item[] GetServerItems()
        {
            List<Item> ret = new List<Item>();
            foreach (MiningGameServer.Items.ServerItem i in MiningGameServer.Items.ServerItem.Items)
            {
                ret.Add(ServerToLocal(i));
            }
            return ret.ToArray();
        }
        public static Item[] GetClientItems()
        {
            List<Item> ret = new List<Item>();
            foreach (MiningGame.Code.Items.Item i in MiningGame.Code.Items.Item.Items)
            {
                ret.Add(ClientToLocal(i));
            }
            return ret.ToArray();
        }

        public static string GenerateBlockHTMLTable(Block[] blocks)
        {
            string pre = "<table border='1'>" +
                         "<tr><th>ID</th>" +
                         "<th>Name</th>" +
                         "<th>Item drop:</th>" +
                         "<th>Num drop: </th></tr>";
            string end = "</table>";
            string mid = "";
            Array.Sort(blocks, (x, y) => x.ID - y.ID);

            foreach (Block b in blocks)
            {
                if (b.ID == 0) continue;
                mid += "<tr><td>" +
                       b.ID +
                       "</td><td>" +
                       b.Name +
                       "</td><td>" +
                       b.ItemDrop +
                       "</td><td>" +
                       b.ItemDropNum +
                       "</td></tr>";
            }

            return pre + mid + end;
        }

        public static string GenerateItemHTMLTable(Item[] items)
        {
            string pre = "<table border='1'>" +
                         "<tr><th>ID</th>" +
                         "<th>Name</th>" +
                         "<th>Places block:</th></tr>";
            string end = "</table>";
            string mid = "";
            Array.Sort(items, (x, y) => x.ID - y.ID);

            foreach (Item i in items)
            {
                if (i.ID == 0) continue;
                mid += "<tr><td>" +
                       i.ID +
                       "</td><td>" +
                       i.Name +
                       "</td><td>" +
                       i.BlockID +
                       "</td></tr>";
            }

            return pre + mid + end;
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expected an argument specifying the path to place the folder");
                return;
            }
            MiningGameServer.Blocks.Block.GenerateBlocks();
            MiningGame.Code.Blocks.Block.GenerateBlocks();
            MiningGame.Code.Items.Item.MakeItems();
            MiningGameServer.Items.ServerItem.MakeItems();

            string HTML = "<html>" +
                          "<head>" +
                          "<title>Minor Destruction Item/Block IDs</title>" +
                          "</head>" +
                          "<body>" +
                          "<h3>Client blocks:</h3>";
            HTML += GenerateBlockHTMLTable(GetClientBlocks());
            HTML += "<br/><h3>Server blocks:</h3>";
            HTML += GenerateBlockHTMLTable(GetServerBlocks());
            HTML += "<br/><h3>Client items:</h3>";
            HTML += GenerateItemHTMLTable(GetClientItems());
            HTML += "<br/><h3>Server items:</h3>";
            HTML += GenerateItemHTMLTable(GetServerItems());
            HTML += "</body></html>";
            string path = args[0] + "BlockItemTables\\";
            Directory.CreateDirectory(path);
            path += "Tables.html";

            TextWriter tw = new StreamWriter(path);
            tw.Write(HTML);
            tw.Close();
        }
    }
}
