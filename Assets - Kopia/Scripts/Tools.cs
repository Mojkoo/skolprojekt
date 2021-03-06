﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum EquipSlot {
    HAT, ACCESSORY, FACE, WEAPON, SHIELD, TOP, GLOVES, PANTS, BOOTS
};

public class Tools
{
    public static readonly int ITEM_PROPERTY_SIZE = 15;

	public static GameObject findInactiveChild(GameObject parent, string name){
		Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in transforms){
			if(t.name.Equals(name)){
				return t.gameObject;
			}
		}
		return null;
	}
    public static GameObject loadObjectFromResources(e_Objects obj)
    {
        return (GameObject)Resources.Load(ResourceStructure.getPathForObject(obj));
    }

	public static byte[] objectToByteArray(object obj){
		if (obj == null)
			return null;

		BinaryFormatter bf = new BinaryFormatter ();
        
		using (MemoryStream ms = new MemoryStream ()) {
			bf.Serialize (ms,obj);
			return ms.ToArray();
		}
	}

	public static object byteArrayToObject(byte[] byteArray){
		MemoryStream memStream = new MemoryStream ();
		BinaryFormatter bf = new BinaryFormatter ();
		memStream.Write (byteArray, 0, byteArray.Length);
		memStream.Seek (0, SeekOrigin.Begin);
		object obj = (object)bf.Deserialize (memStream);
		return obj;
	}

    public static GameObject[] getChildrenByTag(GameObject parent, string tag)
    {
        List<GameObject> objects = new List<GameObject>();
        for(int i = 0; i < parent.transform.childCount; i++) {
            if (parent.transform.GetChild(i).CompareTag(tag)) {
                objects.Add(parent.transform.GetChild(i).gameObject);
            }
        }
        return objects.ToArray();
    }

    public static GameObject getChild(GameObject parent, string name)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).name.Equals(name))
                return parent.transform.GetChild(i).gameObject;
            if (parent.transform.GetChild(i).childCount > 0)
            {
                GameObject child = getChild(parent.transform.GetChild(i).gameObject, name);
                if (child == null)
                    continue;
                if (child.name.Equals(name))
                    return child.gameObject;
            }
        }
        return null;
    }
    public static Color hexColor(int hex)
    {
        int r = (hex >> 16) & 0xFF;
        int g = (hex >> 8) & 0xFF;
        int b = hex & 0xFF;
        Debug.Log(r + " + " + g + " + " + b);
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }
}

namespace GlobalTools
{
    class ToolsGlobal
    {
	    public Quaternion rotateTowards(Vector3 yourPosition, Vector3 targetPosition) {
		    float x = (targetPosition - yourPosition).normalized.x;
		    float y = 0;
		    float z = (targetPosition - yourPosition).normalized.z;

		    Quaternion r = Quaternion.LookRotation(new Vector3(x,y,z));
		    return r;
	    }
    }
}