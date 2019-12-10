using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestEntity
{
    public int id;
    public string name;
    public int price;
}

[ExcelAsset]
public class test : ScriptableObject
{
	public List<TestEntity> Table1; // Replace 'EntityType' to an actual type that is serializable.
}
