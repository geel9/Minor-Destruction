using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiningGameServer.ItemAttributes
{
    public class ItemAttribute
    {
        public double AttributeModifier;

        //Instead of having to have attributes manage stat changes that are simple,
        //have a list of all buffs and manage it for them.
        public List<PlayerStatBuff> EquippedStatBuffs = new List<PlayerStatBuff>();

        //Likewise for the in-inventory buffs.
        public List<PlayerStatBuff> InventoryStatBuffs = new List<PlayerStatBuff>();

        //We store a Dictionary of the types, and return new instances when requested.
        private static Dictionary<int, Type> _itemAttributes = null;
        public static Dictionary<int, Type> ItemAttributes
        {
            get
            {
                if (_itemAttributes == null)
                {
                    GenerateList();
                }
                return _itemAttributes;
            }
        }

        private static void GenerateList()
        {
            _itemAttributes = new Dictionary<int, Type>();
            Assembly a = Assembly.GetExecutingAssembly();
            foreach (Type t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(ItemAttribute)) || t == typeof(ItemAttribute))
                {
                    ItemAttribute i = (ItemAttribute)Activator.CreateInstance(t);
                    _itemAttributes.Add(i.GetID(), t);
                }
            }
        }

        public static ItemAttribute GetAttributeOfID(int id, double modifier = 0)
        {
            Type ret = typeof(ItemAttribute);
            if (ItemAttributes.ContainsKey(id))
                ItemAttributes.TryGetValue(id, out ret);
            ItemAttribute i = (ItemAttribute)Activator.CreateInstance(ret);
            i.AttributeModifier = modifier;
            i.EquippedStatBuffs = i.GetEquippedBuffs().ToList();
            i.InventoryStatBuffs = i.GetInventoryBuffs().ToList();
            return i;
        }

        //For reflection. Helps with generating a list of Attributes.
        public virtual int GetID()
        {
            return 0;
        }

        public virtual string GetName()
        {
            return "Null";
        }

        public virtual AttributeAffinity GetAffinity()
        {
            return AttributeAffinity.Positive;
        }

        public virtual string GetDescription()
        {
            return GetName() + " (" + GetID() + "): " + AttributeModifier;
        }

        public virtual PlayerStatBuff[] GetEquippedBuffs()
        {
            return new PlayerStatBuff[0];
        }

        public virtual PlayerStatBuff[] GetInventoryBuffs()
        {
            return new PlayerStatBuff[0];
        }

        #region Game-related code

        public virtual void OnItemPickedUp(NetworkPlayer player, int inventorySlot)
        {
            player.ActiveBuffs.AddRange(InventoryStatBuffs);
        }

        public virtual void OnItemEquipped(NetworkPlayer player, int inventorySlot)
        {
            player.ActiveBuffs.AddRange(EquippedStatBuffs);
        }

        public virtual void OnItemDequipped(NetworkPlayer player, int inventorySlot)
        {
            List<PlayerStatBuff> toRemove = new List<PlayerStatBuff>();
            foreach (PlayerStatBuff buff in EquippedStatBuffs)
            {
                foreach (PlayerStatBuff buff2 in player.ActiveBuffs)
                {
                    if (buff2.EqualsBuff(buff))
                    {
                        toRemove.Add(buff2);
                        break;
                    }
                }
            }
            foreach (var playerStatBuff in toRemove)
            {
                player.ActiveBuffs.Remove(playerStatBuff);
            }
        }

        public virtual void OnItemDropped(NetworkPlayer player, int inventorySlot)
        {
            foreach (PlayerStatBuff buff in InventoryStatBuffs)
            {
                player.ActiveBuffs.Remove(buff);
            }
        }

        #endregion
    }

    public enum AttributeAffinity
    {
        Positive,
        Negative,
        Neutral,
        Invisible
    }
}
