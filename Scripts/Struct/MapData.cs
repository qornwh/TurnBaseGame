using System.Collections.Generic;

[System.Serializable]
public class MapTypeInfo
{
    public int code;
    public string name;
    public string path;
}

[System.Serializable]
public class MapData
{
    public int code;
    public int type;
    public int width;
    public int hight;
    public int[,] data;
}

[System.Serializable]
public class StructMapData
{
    List<int> data; 
}