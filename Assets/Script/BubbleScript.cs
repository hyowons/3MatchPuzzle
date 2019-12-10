using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;


public enum eBubbleType
{
    None,
    white,
    black,
    blue,
    green,
    orange,
    pink,
    purple,
    yellow,
}


[Hotfix]
public class BubbleScript : MonoBehaviour {

    //const float DropSpeed = 800f;
    
    public static float DropSpeed = 250f;

    //const float DropSpeed = 20f;

    [SerializeField]
    private MapTile _targetMapTile;

    Vector3 _targetMapTilePos;

    public Vector3 TargetMapTilePos
    {
        get { return _targetMapTilePos; }
    }

    public MapTile TargetMapTile {
        get { return _targetMapTile; }
        set {
            _targetMapTile = value;
            if( _targetMapTile!= null)
                _targetMapTilePos = _targetMapTile.transform.localPosition;
        }
    }

    int[] _chainBombArry = new int[6] { 0, 0, 0, 0, 0, 0 };
    public int GetChainBombCount(eTileDir dir)
    {
        return _chainBombArry[(int)dir];
    }


    public bool IsBomb = false;

    UISprite _bubbleSprite;
    eBubbleType _bubbleType = eBubbleType.None;
    public eBubbleType BubbleType { get { return _bubbleType; } set { _bubbleType = value; } }

    UIButton _bubbleButton = null;


    [CSharpCallLua]
    public delegate bool GetLuaUpdateCB(BubbleScript bubble, float deltaTime);
    GetLuaUpdateCB GetLuaUpdate = null;



    LuaFunction luaFunc_BubbleTypeCount = null;
    int BubbleTypeCount
    {
        get
        {
            if( GameManager.Instance != null)
            {
                return GameManager.Instance.BubbleTypeCount;
            }
            else
            {
                if (luaFunc_BubbleTypeCount == null)
                {
                    var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
                    table.Get("BubbleTypeCount", out luaFunc_BubbleTypeCount);
                    if (luaFunc_BubbleTypeCount == null)
                    {
                        Debug.LogError("Lua Function is NotFound - BubbleTypeCount");
                        return 0;
                    }
                }

                return luaFunc_BubbleTypeCount.Func<int, int>(0);
            }
            
        }
    }

    LuaFunction luaFunc_MatchBubbleCount = null;
    int MatchBubbleCount
    {
        get
        {
            if( GameManager.Instance != null)
            {
                return GameManager.Instance.MatchBubbleCount;
            }
            else
            {
                if (luaFunc_MatchBubbleCount == null)
                {
                    var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
                    table.Get("MatchBubbleCount", out luaFunc_MatchBubbleCount);
                    if (luaFunc_MatchBubbleCount == null)
                    {
                        Debug.LogError("Lua Function is NotFound - MatchBubbleCount");
                        return 0;
                    }
                }

                return luaFunc_MatchBubbleCount.Func<int, int>(0);
            }
  
        }
    }


    // Use this for initialization
    public void Init () {
        _bubbleSprite = GetComponentInChildren<UISprite>();
        TargetMapTile = null;
        _bubbleButton = GetComponent<UIButton>();

        _targetMapTilePos = Vector3.one;
        //GetLuaUpdate = luaenv.Global.Get<GetLuaUpdateCB>("LuaUpdateBubble");
            
    }

    void OnPress(bool isDown)
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnClickBubble(this, isDown);
        }
        else
        {
            var table = XLuaTest.LuaBehaviour.LuaBehaviourDic["GameManager"];
            LuaFunction func;
            table.Get("OnClickBubble", out func);
            func.Action(this, isDown);
        }

        
    }

    public void ClearBubble()
    {
        if (_targetMapTile != null)
        {
            _targetMapTile.TargetBubble = null;
            _targetMapTile = null;
        }

        ClearBombInfo();

        transform.localScale = Vector3.one;
    }

    public void ClearBombInfo()
    {
        for (int i = 0; i < _chainBombArry.Length; ++i)
            _chainBombArry[i] = 0;

        IsBomb = false;
    }

    public void GenerateBubble()
    {
        //_bubbleType = (eBubbleType)(UnityEngine.Random.Range((int)eBubbleType.None, GameManager.Instance.BubbleTypeCount))+1;
        _bubbleType = (eBubbleType)(UnityEngine.Random.Range((int)eBubbleType.None, BubbleTypeCount)) + 1;
        SetSprite();
    }

    public void FindNextTile()
    {
        MapTile nextTile = TargetMapTile.NextTile();
        if( nextTile != null)
        {
            TargetMapTile.TargetBubble = null;
            TargetMapTile = nextTile;
            nextTile.TargetBubble = this;
        }
    }

    public bool UpdateBubble(float deltaTime)
    {
        //return GetLuaUpdate(this, deltaTime);
        Vector3 targetVec = _targetMapTilePos - transform.localPosition;
        Vector3 moveVec = ((targetVec.normalized * DropSpeed) * deltaTime);
        if (moveVec.magnitude >= targetVec.magnitude)
        {
            MapTile nextTile = TargetMapTile.NextTile();
            if (nextTile == null)
            {
                transform.localPosition = _targetMapTilePos;
                return false;
            }
            else
            {
                transform.localPosition = _targetMapTilePos + (nextTile.transform.localPosition - _targetMapTilePos).normalized * (moveVec.magnitude - targetVec.magnitude);
                TargetMapTile.TargetBubble = null;
                TargetMapTile = nextTile;
                nextTile.TargetBubble = this;
                return true;
            }
        }
        else
        {
            transform.localPosition += moveVec;
        }

        return true;
    }

    public void CalcBubbleBomb()
    {
        CalcBubbleBomb(eTileDir.LeftBottom);
        CalcBubbleBomb(eTileDir.RightBottom);
        CalcBubbleBomb(eTileDir.Bottom);

        //if( _chainBombArry[(int)eTileDir.LeftBottom] >= GameManager.Instance.MatchBubbleCount ||
        //    _chainBombArry[(int)eTileDir.RightBottom] >= GameManager.Instance.MatchBubbleCount  ||
        //    _chainBombArry[(int)eTileDir.Bottom] >= GameManager.Instance.MatchBubbleCount)
        if (_chainBombArry[(int)eTileDir.LeftBottom] >= MatchBubbleCount ||
            _chainBombArry[(int)eTileDir.RightBottom] >= MatchBubbleCount ||
            _chainBombArry[(int)eTileDir.Bottom] >= MatchBubbleCount)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            IsBomb = true;
        }
        else
        {
            transform.localScale = Vector3.one;
            IsBomb = false;
        }
    }

    void CalcBubbleBomb(eTileDir dir)
    {
        if( _chainBombArry[(int)dir] == 0)
        {
            MapTile lbTile = _targetMapTile.GetNearbMapTile(dir);
            if (lbTile != null && lbTile.TargetBubble != null)
            {
                _chainBombArry[(int)dir]++;
                lbTile.TargetBubble.CheckChainBomb(BubbleType, dir, ref _chainBombArry[(int)dir]);
            }
        }
    }

    public void CheckChainBomb(eBubbleType bubbleType, eTileDir dir, ref int chainCount)
    {
        if (bubbleType == BubbleType)
        {
            chainCount++;
            MapTile tile = _targetMapTile.GetNearbMapTile(dir);
            if (tile != null && tile.TargetBubble != null)
            {
                tile.TargetBubble.CheckChainBomb(bubbleType, dir, ref chainCount);
            }
            _chainBombArry[(int)dir] = chainCount;
            _chainBombArry[((int)dir + (int)eTileDir.Bottom) % (int)(eTileDir.LeftTop + 1)] = chainCount;
        }

        //if (_chainBombArry[(int)eTileDir.LeftBottom] >= GameManager.Instance.MatchBubbleCount ||
        //   _chainBombArry[(int)eTileDir.RightBottom] >= GameManager.Instance.MatchBubbleCount ||
        //   _chainBombArry[(int)eTileDir.Bottom] >= GameManager.Instance.MatchBubbleCount)
        if (_chainBombArry[(int)eTileDir.LeftBottom] >= MatchBubbleCount ||
           _chainBombArry[(int)eTileDir.RightBottom] >= MatchBubbleCount ||
           _chainBombArry[(int)eTileDir.Bottom] >= MatchBubbleCount)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            IsBomb = true;
        }
        else
        {
            transform.localScale = Vector3.one;
            IsBomb = false;
        }
            
    }
    
    void SetSprite()
    {
        if (_bubbleSprite == null)
        {
            Debug.LogError("bubbleSprite is null");
            return;
        }

        switch(_bubbleType)
        {
        case eBubbleType.None:
            _bubbleSprite.spriteName = string.Empty;    break;
        case eBubbleType.white:
            _bubbleSprite.spriteName = "bubble_white"; break;
        case eBubbleType.black:
            _bubbleSprite.spriteName = "bubble_black"; break;
        case eBubbleType.blue:
            _bubbleSprite.spriteName = "bubble_blue"; break;
        case eBubbleType.green:
            _bubbleSprite.spriteName = "bubble_green"; break;
        case eBubbleType.orange:
            _bubbleSprite.spriteName = "bubble_orange"; break;
        case eBubbleType.pink:
            _bubbleSprite.spriteName = "bubble_pink"; break;
        case eBubbleType.purple:
            _bubbleSprite.spriteName = "bubble_purple"; break;
        case eBubbleType.yellow:
            _bubbleSprite.spriteName = "bubble_yellow"; break;
        }
    }
	
}
