using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour {

    [SerializeField]
    UILabel _widthSizeLabel = null;
    [SerializeField]
    UIButton _widthSizeDnButton = null;
    [SerializeField]
    UIButton _widthSizeUpButton = null;

    [SerializeField]
    UILabel _heightSizeLabel = null;
    [SerializeField]
    UIButton _heightSizeDnButton = null;
    [SerializeField]
    UIButton _heightSizeUpButton = null;

    [SerializeField]
    UILabel _bubbleTypeLabel = null;
    [SerializeField]
    UIButton _bubbleTypeDnButton = null;
    [SerializeField]
    UIButton _bubbleTypeUpButton = null;

    [SerializeField]
    UILabel _rockCountLabel = null;
    [SerializeField]
    UIButton _rockCountDnButton = null;
    [SerializeField]
    UIButton _rockCountUpButton = null;

    [SerializeField]
    UILabel _matchCountLabel = null;
    [SerializeField]
    UIButton _matchCountDnButton = null;
    [SerializeField]
    UIButton _matchCountUpButton = null;

    int _widthSize = 5;
    int _heightSize = 5;
    int _bubbleCount = 5;
    int _rockCount = 0;
    int _matchBubbleCount = 3;

    public int WidthSize { get { return _widthSize; } }
    public int HeightSize { get { return _heightSize; } }
    public int BubbleCount { get { return _bubbleCount; } }
    public int RockCount { get { return _rockCount; } }

    public int MatchBubbleCount { get { return _matchBubbleCount; } }

    // Use this for initialization
    void Start () {
        UpdateLabel();

        _widthSizeUpButton.onClick.Add(new EventDelegate(OnClickWidthUp));
        _widthSizeDnButton.onClick.Add(new EventDelegate(OnClickWidthDn));
        _heightSizeUpButton.onClick.Add(new EventDelegate(OnClickHeightUp));
        _heightSizeDnButton.onClick.Add(new EventDelegate(OnClickHeightDn));
        _bubbleTypeUpButton.onClick.Add(new EventDelegate(OnClickBubbleTypeUp));
        _bubbleTypeDnButton.onClick.Add(new EventDelegate(OnClickBubbleTypeDn));
        _rockCountUpButton.onClick.Add(new EventDelegate(OnClickRockCountUp));
        _rockCountDnButton.onClick.Add(new EventDelegate(OnClickRockCountDn));
        _matchCountUpButton.onClick.Add(new EventDelegate(OnClickMatchBubbleCountUp));
        _matchCountDnButton.onClick.Add(new EventDelegate(OnClickMatchBubbleCountDn));

    }

    void UpdateLabel()
    {
        _widthSizeLabel.text = _widthSize.ToString();
        _heightSizeLabel.text = _heightSize.ToString();
        _bubbleTypeLabel.text = _bubbleCount.ToString();
        _rockCountLabel.text = _rockCount.ToString();
        _matchCountLabel.text = _matchBubbleCount.ToString();
    }

    void OnClickWidthUp()
    {
        _widthSize++;
        _widthSizeLabel.text = _widthSize.ToString();
    }

    void OnClickWidthDn()
    {
        _widthSize--;
        if (_widthSize <= 0)
            _widthSize = 1;
        _widthSizeLabel.text = _widthSize.ToString();

        if (_rockCount > (_heightSize * _widthSize) / 2)
        {
            _rockCount = (_heightSize * _widthSize) / 2;
            _rockCountLabel.text = _rockCount.ToString();
        }
    }

    void OnClickHeightUp()
    {
        _heightSize++;
        _heightSizeLabel.text = _heightSize.ToString();
    }

    void OnClickHeightDn()
    {
        _heightSize--;
        if (_heightSize <= 0)
            _heightSize = 1;
            
        _heightSizeLabel.text = _heightSize.ToString();

        if (_rockCount > (_heightSize * _widthSize) / 2)
        {
            _rockCount = (_heightSize * _widthSize) / 2;
            _rockCountLabel.text = _rockCount.ToString();
        }
    }

    void OnClickBubbleTypeUp()
    {
        _bubbleCount++;
        if (_bubbleCount > (int)eBubbleType.yellow)
            _bubbleCount = (int)eBubbleType.yellow;

        _bubbleTypeLabel.text = _bubbleCount.ToString();
    }

    void OnClickBubbleTypeDn()
    {
        _bubbleCount--;
        if (_bubbleCount <= 3)
            _bubbleCount = 3;

        _bubbleTypeLabel.text = _bubbleCount.ToString();
    }

    void OnClickRockCountUp()
    {
        _rockCount++;

        if (_rockCount > (_heightSize * _widthSize) / 2)
            _rockCount = (_heightSize * _widthSize) / 2;

        _rockCountLabel.text = _rockCount.ToString();
    }

    void OnClickRockCountDn()
    {
        _rockCount--;
        if (_rockCount < 0)
            _rockCount = 0;

        _rockCountLabel.text = _rockCount.ToString();
    }

    void OnClickMatchBubbleCountUp()
    {
        _matchBubbleCount++;
        _matchCountLabel.text = _matchBubbleCount.ToString();
    }

    void OnClickMatchBubbleCountDn()
    {
        _matchBubbleCount--;
        if (_matchBubbleCount < 2)
            _matchBubbleCount = 2;

        _matchCountLabel.text = _matchBubbleCount.ToString();
    }

}
