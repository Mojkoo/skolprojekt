﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
public class PlayerServer {
    public List<Item> items = new List<Item>();
    public List<Equip> equips = new List<Equip>();
    public int databaseID;
    public int connectionID;
    public int playerID;
	public int level = 0;
	public string playerName = "";
	public List<Quest> questList = new List<Quest>();
    //public Quest[] quests;
    int[] stats = new int[5];

    int[] skills;
    public PlayerServer( int databaseID, int connectionID) {
        this.databaseID = databaseID;
        this.connectionID = connectionID;
        for (int i = 0; i < 9; i++) {
            equips.Add(null);
        }
    }
    public void setPlayerID(int id) {
        this.playerID = id;
    }
    public int getPlayerID() {
        return playerID;
    }
    public List<Item> getItems() {
        return this.items;
    }
    public bool hasItem(Item item) {
        for (int i = 0; i < this.items.Count; i++) {
            if (this.items[i].compareTo(item))
                return true;
        }
        return false;
    }
    /*
=======

    public int[] getItem(int itemId)
    {
        int[] value;
        if(itemsDictionary.TryGetValue(itemId, out value)){
            return value;
        }
        return null;
    }

>>>>>>> fa8b4ad6a725554923f1eeb44f5353e2fb6bd69f
    public void addItems(int[] items) {
        this.items = items;
        for(int i=0;i<items.Length;i+=Tools.ITEM_PROPERTY_SIZE){
            int[] tempItem = new int[Tools.ITEM_PROPERTY_SIZE];
            for(int j=0;j<Tools.ITEM_PROPERTY_SIZE;j++){
               tempItem[j] = items[i+j];
            }
            itemsDictionary.Add(items[i], tempItem);
        }
    }
    */
    public void replaceItems(Item item1, Item item2) {
        int posItem1 = items.IndexOf(item1);
        int posItem2 = items.IndexOf(item2);
        items[posItem1] = item1;
        items[posItem2] = item1;
    }
    public void addItem(Item item) {
        items.Add(item);
    }
    /*
    public void addItem(int[] item)
    {
        int[] tempItem = new int[this.items.Length+item.Length];
        for(int i = 0; i < this.items.Length; i++) {
            tempItem[i] = this.items[i];
        }
        for(int i = this.items.Length; i < item.Length; i++) {
            tempItem[i] = item[i];
        }
    }
    */
    public void addEquip(Equip equip) {
        equips.Add(equip);
    }
    public void setSkills(int[] skills) {
        this.skills = skills;
    }
    public void saveInventory(Server server) {
        MySqlConnection mysqlConn;
        server.mysqlNonQuerySelector(out mysqlConn, "");
        mysqlConn.Close();
    }
    public bool isEquipOccupied(int equipSlot) {
        if (equips[equipSlot] == null) return false;
        return true;
    }
    public int[] getEquips() {
        /*
        int[] equips = new int[this.equips.Length * Tools.ITEM_PROPERTY_SIZE];
        for (int i = 0; i < this.equips.Length; i++) {
            for (int j = 0; j < Tools.ITEM_PROPERTY_SIZE; j++)
            {
                equips[i + j] = this.equips[i][j];
            }
        }
        return equips;
        */
        return null;
    }
    public byte[] GetEquipBytes()
    {
        return Tools.objectToByteArray(equips);
    }
    public void SetEquipsFromBytes(byte[] bytes) {
        this.equips = (List<Equip>)Tools.byteArrayToObject(bytes);
    }
    public static PlayerServer GetDefaultCharacter(int connectionID)
    {
        return new PlayerServer(-1, connectionID);
        
    }
}
