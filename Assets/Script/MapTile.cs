using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTileDir
{
    Top = 0,
    RightTop = 1,
    RightBottom = 2,
    Bottom = 3,
    LeftBottom = 4,
    LeftTop = 5,
}



public class MapTile : MonoBehaviour {

    [SerializeField]
    MapTile _tileLT;
    [SerializeField]
    MapTile _tileT;
    [SerializeField]
    MapTile _tileRT;
    [SerializeField]
    MapTile _tileLB;
    [SerializeField]
    MapTile _tileB;
    [SerializeField]
    MapTile _tileRB;

    [SerializeField]
    int _index;

    UISprite _bgSprite = null;

    public int INDEX { get { return _index; } }
    bool _emptyTile = false;
    public bool IsEmptyTile{ get { return _emptyTile; } }

    [SerializeField]
    public BubbleScript TargetBubble;

    

	// Use this for initialization
	void Start () {
        _bgSprite = GetComponent<UISprite>();
        _bgSprite.enabled = !_emptyTile;

     


    }

    public MapTile GetNearbMapTile(eTileDir dir )
    {
        switch(dir)
        {
        case eTileDir.Top:
            return _tileT;
        case eTileDir.LeftTop:
            return _tileLT;
        case eTileDir.RightTop:
            return _tileRT;
        case eTileDir.LeftBottom:
            return _tileLB;
        case eTileDir.RightBottom:
            return _tileRB;
        case eTileDir.Bottom:
            return _tileB;
        default:
            return null;
        }
    }

    public bool IsNearbyMapTile(MapTile tile)
    {
        if (tile == null) return false;
        if (tile == _tileT) return true;
        if (tile == _tileLT) return true;
        if (tile == _tileRT) return true;
        if (tile == _tileLB) return true;
        if (tile == _tileRB) return true;
        if (tile == _tileB) return true;
        return false;
    }

    public void InitMapTile(int index, bool emptyTile = false)
    {
        _tileLT = null;
        _tileT = null;
        _tileRT = null;
        _tileLB = null;
        _tileB = null;
        _tileRB = null;
        TargetBubble = null;

        _index = index;
        _emptyTile = emptyTile;
        if(_bgSprite != null)
            _bgSprite.enabled = !_emptyTile;

    }


    XLua.LuaFunction luaFunc_TileWidth = null;
    int TileWidth
    {
        get {
            if( GameManager.Instance != null)
            {
                return GameManager.Instance.TileWidth;
            }
            else
            {
                if (luaFunc_TileWidth == null)
                {
                    var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
                    table.Get("TileWidth", out luaFunc_TileWidth);
                    if (luaFunc_TileWidth == null)
                    {
                        Debug.LogError("Lua Function is NotFound - TileWidth");
                        return 0;
                    }
                }

                return luaFunc_TileWidth.Func<int, int>(0);
            }
        }
    }

    XLua.LuaFunction luaFunc_TileHeight = null;
    int TileHeight
    {
        get {
            if( GameManager.Instance != null)
            {
                return GameManager.Instance.TileHeight;
            }
            else
            {
                if (luaFunc_TileHeight == null)
                {
                    var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
                    table.Get("TileHeight", out luaFunc_TileHeight);
                    if (luaFunc_TileHeight == null)
                    {
                        Debug.LogError("Lua Function is NotFound - TileHeight");
                        return 0;
                    }
                }

                return luaFunc_TileHeight.Func<int, int>(0);
            }
        }
    }


   XLua.LuaFunction luaFunc_GetTile = null;
   MapTile GetTile(int index)
    {
        if( GameManager.Instance != null)
        {
            return GameManager.Instance.GetTile(index);
        }
        else
        {
            if (luaFunc_GetTile == null)
            {
                var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
                table.Get("GetTile", out luaFunc_GetTile);
                if (luaFunc_GetTile == null)
                {
                    Debug.LogError("Lua Function is NotFound - GetTile");
                    return null;
                }
            }

            MapTile result = luaFunc_GetTile.Func<int, MapTile>(index);
            return result;
        }
        
    }

    //근접한 타일 검색
    public void CalcNearbyTile()
    {
        if (_emptyTile == true) return;

        int width = TileWidth;
        int height = TileHeight;

        //int width = GameManager.Instance.TileWidth;
        //int height = GameManager.Instance.TileHeight;

        //_tileT = GameManager.Instance.GetTile(_index - width);
        //_tileB = GameManager.Instance.GetTile(_index + width);
        _tileT = GetTile(_index - width);
        _tileB = GetTile(_index + width);



        //if( _index % 2 == 0)
        int widthIDX = _index % width;
        if (widthIDX % 2 == 0 )
        {
            if(widthIDX != 0)
            {
                //_tileLT = GameManager.Instance.GetTile(_index - width - 1);
                //_tileLB = GameManager.Instance.GetTile(_index - 1);

                _tileLT = GetTile(_index - width - 1);
                _tileLB = GetTile(_index - 1);
            }
            else
            {
                _tileLT = null;
                _tileLB = null;
            }
            
            if( widthIDX < width - 1)
            {
                //_tileRT = GameManager.Instance.GetTile(_index - width + 1);
                //_tileRB = GameManager.Instance.GetTile(_index + 1);

                _tileRT = GetTile(_index - width + 1);
                _tileRB = GetTile(_index + 1);
            }
            else
            {
                _tileRT = null;
                _tileRB = null;
            }
            
        }
        else
        {
            if (widthIDX != 0)
            {
                //_tileLT = GameManager.Instance.GetTile(_index - 1);
                //_tileLB = GameManager.Instance.GetTile(_index + width - 1);

                _tileLT = GetTile(_index - 1);
                _tileLB = GetTile(_index + width - 1);
            }
            else
            {
                _tileLT = null;
                _tileLB = null;
            }

            if (widthIDX < width - 1)
            {
                //_tileRT = GameManager.Instance.GetTile(_index + 1);
                //_tileRB = GameManager.Instance.GetTile(_index + width + 1);

                _tileRT = GetTile(_index + 1);
                _tileRB = GetTile(_index + width + 1);
            }
            else
            {
                _tileRT = null;
                _tileRB = null;
            }

        }
    }

    public MapTile NextTile()
    {
        if (_tileB != null && _tileB.IsEmptyTile == false && _tileB.TargetBubble == null) return _tileB;
        if (_tileLB != null && _tileLB.IsEmptyTile == false
            //&& _tileLB.TargetBubble == null && _tileLB.INDEX >= GameManager.Instance.TileWidth
            && _tileLB.TargetBubble == null && _tileLB.INDEX >= TileWidth
            && (_tileLT == null /*|| _tileLT.TargetBubble == null*/) ) return _tileLB;
        if (_tileRB != null && _tileRB.IsEmptyTile == false
            //&& _tileRB.TargetBubble == null && _tileRB.INDEX >= GameManager.Instance.TileWidth
            && _tileRB.TargetBubble == null && _tileRB.INDEX >= TileWidth
            && (_tileRT == null /*|| _tileRT.TargetBubble == null*/) ) return _tileRB;
        return null;
    }

}
