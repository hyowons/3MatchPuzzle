using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
//using UniRx;
//using UniRx.Triggers;

public class GameManager : MonoBehaviour {

    public enum eGamePhase
    {
        NONE,
        INITIAL_STAGE,  //게임초기화
        UPDATE_BUBBLE,  //버블 정렬
        BUBBLE_BOMB,    //버블 제거
        PLAYER_TURN,    //사용자 입력
        CLEAR,
    }

    public enum eGameMode
    {
        GAME,
        OPTION,
    }
    
    const float TILE_SIZE = 45f;

    eGamePhase _gamePhase = eGamePhase.NONE;

    [SerializeField]
    Transform _tileMapParent = null;

    [SerializeField]
    UIPanel _bubblePanel = null;

    [SerializeField]
    UIRoot _root;

    [SerializeField]
    Transform _gameBoardTrans = null;

    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    Dictionary<int, MapTile> _tileDic = new Dictionary<int, MapTile>();
    List<MapTile> _tileList = new List<MapTile>();

    Queue<MapTile> _uselessMapTile = new Queue<MapTile>();

    List<BubbleScript> _liveBubbleList = new List<BubbleScript>();
    Queue<BubbleScript> _uselessBubbles = new Queue<BubbleScript>();

    readonly string TILEBG_PATH = "Prefabs/TileBG";
    readonly string BUBBLE_PATH = "Prefabs/BubbleIcon";

    GameObject _tileBGRes = null;
    GameObject _bubbleRes = null;
    
    [SerializeField]
    UIButton _startButton = null;

    [SerializeField]
    UIButton _optionButton = null;
    

    [CSharpCallLua]
    public int TileWidth { get { return _optionManager.WidthSize; } }
    public int TileHeight { get { return _optionManager.HeightSize; }}

    public int BubbleTypeCount { get { return _optionManager.BubbleCount; } }

    public int MatchBubbleCount { get { return _optionManager.MatchBubbleCount; } }

    BubbleScript _pressdBubble = null;
    public BubbleScript PressedBubble { get { return _pressdBubble; } }

    int _bubbleLayerMask = 0;

    [SerializeField]
    OptionManager _optionManager = null;

    public enum eMissAniPhase
    {
        None,
        Begin,
        Return,
    }
    eMissAniPhase _missSwap = eMissAniPhase.None;
    BubbleScript _swapSourceBubble = null;
    BubbleScript _swapTargetBubble = null;

    eGameMode _mode = eGameMode.OPTION;

    //ReactiveProperty<eGameMode> _mode = new ReactiveProperty<eGameMode>(eGameMode.OPTION);

    public string solution(string number, int k)
    {
        int sort = 0;

        while(string.IsNullOrEmpty(number) == false)
        {
            Debug.Log("@@@@");
            int max = 0;
            int maxNum = 0;
            for(int i = 0; i < number.Length; ++i)
            {
                int x = int.Parse(number.Substring(i, 1));
                if (maxNum < x)
                {
                    maxNum = x;
                    max = i;
                }
            }

            sort = (sort * 10) + maxNum;
            number = number.Remove(max, 1);
        }

        
        for(int i = 0; i < k; ++i)
        {
            sort = sort % 10;   
        }

        return sort.ToString();
    }

    void Start()
    {
        Debug.Log(solution("1924", 2));

        _gamePhase = eGamePhase.NONE;

        if (_instance == null)
            _instance = this;

        _tileBGRes = Resources.Load(TILEBG_PATH) as GameObject;
        _bubbleRes = Resources.Load(BUBBLE_PATH) as GameObject;

        _bubbleLayerMask = 1 << LayerMask.NameToLayer("Bubble");
         

        OnOptionOpen();

        _startButton.onClick.Add(new EventDelegate(()=>OnStartGame()));
        _optionButton.onClick.Add(new EventDelegate(() => OnOptionOpen()));

        //_startButton.OnClickAsObservable().Subscribe(_ => OnStartGame());
        //_optionButton.OnClickAsObservable().Subscribe(_ => OnOptionOpen(), ()=>Debug.Log("Complete"));

        //Observable.EveryUpdate()
        //  .Where(_ => _gamePhase == eGamePhase.INITIAL_STAGE)
        //  .Subscribe(_ => {
        //      MakeUselessBubbles();
        //      _gamePhase = eGamePhase.UPDATE_BUBBLE;
        //  });

        //Observable.EveryUpdate()
        //    .Where(_ => _gamePhase == eGamePhase.UPDATE_BUBBLE)
        //    .Where(_ => UpdateBubble())
        //    .Subscribe(_ =>
        //    {
        //        switch (_missSwap)
        //        {
        //            case eMissAniPhase.Begin:
        //                BubbleSwap(_swapSourceBubble, _swapTargetBubble);
        //                _missSwap = eMissAniPhase.Return;
        //                OnClickBubble(null, false);
        //                _swapTargetBubble = null;
        //                _swapSourceBubble = null;
        //                break;
        //            case eMissAniPhase.Return:
        //                _missSwap = eMissAniPhase.None;
        //                _gamePhase = eGamePhase.PLAYER_TURN;
        //                break;
        //            case eMissAniPhase.None:
        //                _gamePhase = eGamePhase.BUBBLE_BOMB;
        //                break;
        //        }
        //    });

        //Observable.EveryUpdate()
        //    .Where(_ => _gamePhase == eGamePhase.BUBBLE_BOMB)
        //    .Subscribe(_ => {
        //        if (CalcBubbleBomb() == true) _gamePhase = eGamePhase.UPDATE_BUBBLE;
        //        else _gamePhase = eGamePhase.PLAYER_TURN;
        //    });

        //Observable.EveryUpdate()
        //    .Where(_ => _gamePhase == eGamePhase.PLAYER_TURN)
        //    .Subscribe(_ => CheckSwapBubble());


    }    

    void InitScale()
    {
        float x = 0f;
        float y = 0f;

        float widthSize = TileWidth * TILE_SIZE;
        float heightSize = TileHeight * TILE_SIZE;

        float scaleW = (_root.manualWidth - 20) / widthSize;
        float scaleH = (_root.manualHeight - 300) / heightSize;

        if( scaleH > 1.5f && scaleW > 1.5f)
        {
            x = ((widthSize * 1.5f) / 2f);// - (TILE_SIZE / 2f);
            y = (heightSize * 1.5f) / 2f;

            _gameBoardTrans.localPosition = new Vector3(-x, y, 0f);
            _gameBoardTrans.localScale = new Vector3(1.5f, 1.5f, 1f);

            Debug.LogError("BlockSize = " + (widthSize * 1.5f).ToString() + " viewSize = " + _root.manualWidth.ToString() + " posX = " + (-x).ToString());
        }
        else
        {
            float min = Mathf.Min(scaleW, scaleH);

            _gameBoardTrans.localScale = new Vector3(min, min, 1f);

            if ((widthSize * min + 1) < ((float)_root.manualWidth - 20))
            {
                x = ((widthSize * min) / 2f) + ( (TILE_SIZE * min) / 2f);
            }
            else
            {
                x = (((float)_root.manualWidth - 20) / 2f);
            }

            if ((heightSize * min + 1) < ((float)_root.manualHeight - 300))
            {
                y = ((heightSize * min) / 2f) + ((TILE_SIZE * min) / 2f);
            }
            else
            {
                y = (((float)_root.manualHeight - 300) / 2f);
            }
            _gameBoardTrans.localPosition = new Vector3(-x, y, 0f);

        }
    }

    public void OnStartGame()
    {
        if (_mode == eGameMode.OPTION)
        {
            InitStage(TileWidth, TileHeight);
            
            MakeUselessBubbles();
            InitScale();
        }

        _gamePhase = eGamePhase.INITIAL_STAGE;
        _missSwap = eMissAniPhase.None;
        _swapTargetBubble = null;
        _swapSourceBubble = null;
        _optionManager.gameObject.SetActive(false);
        _mode = eGameMode.GAME;
    }
    public void OnOptionOpen()
    {
        _gameBoardTrans.localPosition = new Vector3(10000, 0, 0);
        _optionManager.gameObject.SetActive(true);
        _gamePhase = eGamePhase.NONE;
        _mode = eGameMode.OPTION;
    }

    void Update()
    {
        switch (_gamePhase)
        {
            case eGamePhase.NONE:
                break;
            case eGamePhase.INITIAL_STAGE:
                MakeUselessBubbles();
                _gamePhase = eGamePhase.UPDATE_BUBBLE;
                break;
            case eGamePhase.UPDATE_BUBBLE:
                if (UpdateBubble() == true)
                {
                    switch (_missSwap)
                    {
                        case eMissAniPhase.Begin:
                            BubbleSwap(_swapSourceBubble, _swapTargetBubble);
                            _missSwap = eMissAniPhase.Return;
                            OnClickBubble(null, false);
                            _swapTargetBubble = null;
                            _swapSourceBubble = null;
                            break;
                        case eMissAniPhase.Return:
                            _missSwap = eMissAniPhase.None;
                            _gamePhase = eGamePhase.PLAYER_TURN;
                            break;
                        case eMissAniPhase.None:
                            _gamePhase = eGamePhase.BUBBLE_BOMB;
                            break;
                    }
                }
                break;
            case eGamePhase.BUBBLE_BOMB:
                if (CalcBubbleBomb() == true) _gamePhase = eGamePhase.UPDATE_BUBBLE;
                else _gamePhase = eGamePhase.PLAYER_TURN;
                break;
            case eGamePhase.PLAYER_TURN:
                CheckSwapBubble();
                break;
            case eGamePhase.CLEAR:
                break;
        }
    }

    bool CalcBubbleBomb(bool clear = true)
    {
        for (int i = 0; i < _tileList.Count; ++i)
        {
            if (_tileList != null && _tileList[i].IsEmptyTile == false && _tileList[i].TargetBubble != null)
                _tileList[i].TargetBubble.CalcBubbleBomb();
        }

        bool bomb = false;
        for (int i = 0; i < _tileList.Count; ++i)
        {
            if (_tileList != null && _tileList[i].IsEmptyTile == false && _tileList[i].TargetBubble != null)
            {
                if (_tileList[i].TargetBubble.IsBomb == true)
                {
                    if( clear == true)
                    {
                        ChainBomb(_tileList[i].TargetBubble);
                    }
                    bomb = true;
                }
            }
        }
        
        for (int i = 0; i < _liveBubbleList.Count; ++i)
        {
            _liveBubbleList[i].ClearBombInfo();
            if (bomb == true)
            {
                _liveBubbleList[i].FindNextTile();
            }
        }

        return bomb;
    }


    void MakeUselessBubbles()
    {
        for( int i = 0; i < _liveBubbleList.Count; ++i)
        {
            _liveBubbleList[i].transform.localPosition = new Vector3(10000f, 0, 0);
            _uselessBubbles.Enqueue(_liveBubbleList[i]);
            _liveBubbleList[i].ClearBubble();
        }
        _liveBubbleList.Clear();

        int createItemCount = TileWidth * TileHeight - _uselessBubbles.Count;
        for (int i = 0; i < createItemCount; ++i)
        {
            GameObject bubbleObj = Instantiate(_bubbleRes) as GameObject;
            bubbleObj.transform.parent = _bubblePanel.transform;
            bubbleObj.transform.localScale = Vector3.one;
            bubbleObj.transform.localPosition = new Vector3(10000f, 0, 0);
            BubbleScript bubble = bubbleObj.GetComponent<BubbleScript>();
            _uselessBubbles.Enqueue(bubble);
            
            bubble.Init();
        }
    }

    void InitStage(int width, int height)
    {
        for( int i =0; i< _tileList.Count; ++i)
        {
            _uselessMapTile.Enqueue(_tileList[i]);
            _tileList[i].gameObject.transform.localPosition = new Vector3(10000, 0, 0);
        }

        _tileList.Clear();
        _tileDic.Clear();

        int rockTileCount = _optionManager.RockCount;
        HashSet<int> rockIDXSet = new HashSet<int>();

        while(rockTileCount > 0)
        {
            int rockIDX = Random.Range(0, height * width);
            if (rockIDXSet.Contains(rockIDX) == true) continue;
            rockIDXSet.Add(rockIDX);
            rockTileCount--;
        }

        for(int h = 0; h < height; ++h)
        {
            for(int w = 0; w < width; ++w)
            {
                MapTile tile = null;
                if( _uselessMapTile.Count == 0)
                {
                    GameObject tileBG = Instantiate(_tileBGRes) as GameObject;
                    tileBG.transform.parent = _tileMapParent;
                    tileBG.transform.localScale = Vector3.one;
                    tile = tileBG.GetComponent<MapTile>();
                }
                else
                {
                    tile = _uselessMapTile.Dequeue();
                }
                
                float tilePosY = (w % 2 == 0) ? h * TILE_SIZE : h * TILE_SIZE + (TILE_SIZE / 2f);
                tilePosY *= -1f;
                tile.gameObject.transform.localPosition = new Vector3(w * TILE_SIZE + (TILE_SIZE / 2f), tilePosY, 0f);

                int idx = h * width + w;
                bool rockTile = rockIDXSet.Contains(idx);

                tile.InitMapTile(idx, (h  == height - 1 && w % 2 == 1) || rockTile);
                tile.gameObject.name = tile.INDEX.ToString();
                _tileDic.Add(tile.INDEX, tile);
                _tileList.Add(tile);
            }
        }

        rockIDXSet.Clear();
        rockIDXSet = null;

        var itor = _tileDic.GetEnumerator();
        while (itor.MoveNext())
            itor.Current.Value.CalcNearbyTile();
    }

    public MapTile GetTile(int index) {
        MapTile result = null;
        _tileDic.TryGetValue(index, out result);
        if (result != null && result.IsEmptyTile == true) return null;
        return result;
    }

    bool UpdateBubble()
    {
        bool updateComplete = true;

        for(int i = _tileList.Count - 1; i >= 0; --i)
        {
            if (_tileList[i].IsEmptyTile == true) continue;

            if (_tileList[i].TargetBubble == null)
            {
                if (i >= TileWidth) continue;

                BubbleScript bubble = null;
                if (_uselessBubbles.Count == 0)
                {
                    //들어오면 안되는 코드
                    Debug.LogError("???????????");
                    GameObject bubbleObj = Instantiate(_bubbleRes) as GameObject;
                    bubbleObj.transform.parent = _bubblePanel.transform;
                    bubble.transform.localScale = Vector3.one;
                    bubble = bubbleObj.GetComponent<BubbleScript>();
                    bubble.Init();
                }
                else
                {
                    bubble = _uselessBubbles.Dequeue();
                    bubble.gameObject.SetActive(true);
                }

                _liveBubbleList.Add(bubble);

                bubble.transform.localPosition = new Vector3(TILE_SIZE * i + (TILE_SIZE/2f), TILE_SIZE - ((i % 2) * (TILE_SIZE / 2)), 0f);

                bubble.TargetMapTile = _tileList[i];
                _tileList[i].TargetBubble = bubble;
                bubble.GenerateBubble();
            }

            if (_tileList[i].TargetBubble.UpdateBubble(Time.deltaTime) == true)
                updateComplete = false;
        }

        return updateComplete;
    }

    public void OnClickBubble(BubbleScript bubble, bool isPress)
    {
        if (_gamePhase != eGamePhase.PLAYER_TURN && isPress == true) return;
        if (isPress == false)
        {
            _pressdBubble = null;
            return;
        }

        _pressdBubble = bubble;
    }

    public void CheckSwapBubble()
    {
        if (_gamePhase != eGamePhase.PLAYER_TURN || _pressdBubble == null) return;

        Ray ray = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if( Physics.Raycast(ray, out hit, 10f, _bubbleLayerMask) )
        {
            BubbleScript bubble = hit.collider.gameObject.GetComponent<BubbleScript>();
            if (bubble == null) return;

            if( bubble.TargetMapTile.IsNearbyMapTile(_pressdBubble.TargetMapTile) == true)
            {
                if( SwapBubble(_pressdBubble, bubble) == true)
                {
                    BubbleSwap(_pressdBubble, bubble);
                    OnClickBubble(null, false);
                }
                else
                {
                    _missSwap = eMissAniPhase.Begin;
                    BubbleSwap(_pressdBubble, bubble);
                    _swapTargetBubble = bubble;
                    _swapSourceBubble = _pressdBubble;
                }
                _gamePhase = eGamePhase.UPDATE_BUBBLE;
            }
        }
    }



    bool SwapBubble(BubbleScript bubble1, BubbleScript bubble2)
    {
        eBubbleType tempType = bubble1.BubbleType;
        bubble1.BubbleType = bubble2.BubbleType;
        bubble2.BubbleType = tempType;

        bool isBomb = CalcBubbleBomb(false);

        tempType = bubble1.BubbleType;
        bubble1.BubbleType = bubble2.BubbleType;
        bubble2.BubbleType = tempType;
      
        return isBomb;
    }

    void BubbleSwap(BubbleScript bubble1, BubbleScript bubble2)
    {
        MapTile temp = bubble1.TargetMapTile;
        bubble1.TargetMapTile = bubble2.TargetMapTile;
        bubble1.TargetMapTile.TargetBubble = bubble1;
        bubble2.TargetMapTile = temp;
        bubble2.TargetMapTile.TargetBubble = bubble2;
    }

    void ChainBomb(BubbleScript bubble)
    {
        MapTile t = null;
        MapTile rt = null;
        MapTile rb = null;
        MapTile b = null;
        MapTile lb = null;
        MapTile lt = null;

        if (bubble.GetChainBombCount(eTileDir.Top) >= MatchBubbleCount) t = bubble.TargetMapTile.GetNearbMapTile(eTileDir.Top);
        if (bubble.GetChainBombCount(eTileDir.RightTop) >= MatchBubbleCount) rt = bubble.TargetMapTile.GetNearbMapTile(eTileDir.RightTop);
        if (bubble.GetChainBombCount(eTileDir.RightBottom) >= MatchBubbleCount) rb = bubble.TargetMapTile.GetNearbMapTile(eTileDir.RightBottom);
        if (bubble.GetChainBombCount(eTileDir.Bottom) >= MatchBubbleCount) b = bubble.TargetMapTile.GetNearbMapTile(eTileDir.Bottom);
        if (bubble.GetChainBombCount(eTileDir.LeftBottom) >= MatchBubbleCount) lb = bubble.TargetMapTile.GetNearbMapTile(eTileDir.LeftBottom);
        if (bubble.GetChainBombCount(eTileDir.LeftTop) >= MatchBubbleCount) lt = bubble.TargetMapTile.GetNearbMapTile(eTileDir.LeftTop);

        if (t == null && rt == null && rb == null && b == null && lb == null && lt == null) return;
        
        bubble.ClearBubble();
        bubble.transform.localPosition = new Vector3(10000f, 0f, 0f);
        _liveBubbleList.Remove(bubble);
        _uselessBubbles.Enqueue(bubble);

        if (t != null && t.TargetBubble != null) ChainBomb(t.TargetBubble);
        if (rt != null && rt.TargetBubble != null) ChainBomb(rt.TargetBubble);
        if (rb != null && rb.TargetBubble != null) ChainBomb(rb.TargetBubble);
        if (b != null && b.TargetBubble != null) ChainBomb(b.TargetBubble);
        if (lb != null && lb.TargetBubble != null) ChainBomb(lb.TargetBubble);
        if (lt != null && lt.TargetBubble != null) ChainBomb(lt.TargetBubble);
    }

}


public class UTIL
{
    public static bool RayFunction(Ray ray, out RaycastHit hit, int layer)
    {
        return Physics.Raycast(ray, out hit, 1000, layer);
    }


    public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
    {
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
    {
        Debug.LogError("@@@@");
        return false;
    }

    public static bool Raycast(Vector3 origin, Vector3 direction)
    {
        Debug.LogError("@@@@");
        return false;
    }
    
    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
    {
        hitInfo = default(RaycastHit);
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Ray ray, float maxDistance)
    {
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Ray ray, float maxDistance, int layerMask)
    {
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
    {
        hitInfo = default(RaycastHit);
        Debug.LogError("@@@@");
        return false;
    }


    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
    {
        hitInfo = default(RaycastHit);
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Ray ray)
    {
        Debug.LogError("@@@@");
        return false;
    }
    
    public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
    {
        hitInfo = default(RaycastHit);
        Debug.LogError("@@@@");
        return false;
    }
    public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
    {
        hitInfo = default(RaycastHit);
        Debug.LogError("@@@@");
        return false;
    }


}
