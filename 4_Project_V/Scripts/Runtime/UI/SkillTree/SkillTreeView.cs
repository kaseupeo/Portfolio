using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillTreeView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Slot UI들의 부모 Object
    [SerializeField] private Transform root;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Slot")]
    [SerializeField] private GameObject slotViewPrefab;
    [SerializeField] private Vector2 slotSize;
    // Slot 사이의 거리
    [SerializeField] private float spacing;

    [Header("Line")]
    [SerializeField] private GameObject linkLinePrefab;
    [SerializeField] private float lineWidth;

    private bool _isPointerOverUI;
    // 생성된 SlotView들
    private List<SkillTreeSlotView> _slotViewList = new();
    // 생성된 Line들
    private List<RectTransform> _linkLineList = new();

    // 각 SlotView별로 자신의 아래로 뻗어있는 Line들을 저장하는 변수
    private Dictionary<SkillTreeSlotView, List<RectTransform>> _lineBySlotDic = new();
    
    public bool IsPointerOverUI => _isPointerOverUI;
    public Creature RequesterCreature { get; private set; }
    public SkillTree ViewingSkillTree { get; private set; }

    private void Awake()
    {
        Managers.UI.SkillTreeView = this;
        
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Managers.UI.SkillTooltipUI?.Hide();
    }

    public void Show(Creature creature, SkillTree skillTree)
    {
        gameObject.SetActive(true);

        if (RequesterCreature == creature && ViewingSkillTree == skillTree)
            return;

        RequesterCreature = creature;
        ViewingSkillTree = skillTree;

        titleText.text = skillTree.DisplayName;

        var nodes = skillTree.GetSlotNodes();

        ChangeViewWidth(nodes);
        CreateSlotViews(nodes.Length);
        PlaceSlotViews(creature, nodes);
        LinkSlotViews(creature, nodes);
    }

    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }

    private void ChangeViewWidth(SkillTreeSlotNode[] nodes)
    {
        // SkillTreeView의 넓이를 Node 수에 맞게 넓힘
        var maxOverIndex = nodes.Max(x => x.Index) + 1;
        var width = ((slotSize.x + spacing) * maxOverIndex) + spacing;
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new(width, rectTransform.sizeDelta.y);
    }

    private void CreateSlotViews(int nodeCount)
    {
        // slotViews에서 부족한만큼 SlotView를 생성함
        int needSlotCount = nodeCount - _slotViewList.Count;
        for (int i = 0; i < needSlotCount; i++)
        {
            var slotView = Instantiate(slotViewPrefab, root).GetComponent<SkillTreeSlotView>();

            var rectTransform = slotView.GetComponent<RectTransform>();
            rectTransform.sizeDelta = slotSize;

            _slotViewList.Add(slotView);
        }
    }

    private RectTransform CreateLinkLine()
    {
        var linkLine = Instantiate(linkLinePrefab, root).GetComponent<RectTransform>();
        linkLine.transform.SetAsFirstSibling();

        _linkLineList.Add(linkLine);

        return linkLine;
    }

    private void PlaceSlotViews(Creature creature, SkillTreeSlotNode[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];
            var slotView = _slotViewList[i];
            slotView.gameObject.SetActive(true);
            slotView.SetViewTarget(creature, node);

            // Slot이 위치해야할 좌표를 구함
            float x = node.Index * slotSize.x;
            float y = node.Tier * -slotSize.y;
            // 다른 Node와 공간을 두기위해 필요한 spacing 값을 구함
            float xSpacing = spacing * (node.Index + 1);
            float ySpacing = spacing * (node.Tier + 1);
            Vector3 position = new Vector3(x + xSpacing, y - ySpacing);

            slotView.transform.localPosition = position;
        }

        // 쓰지 않는 SlotView들을 모두 꺼줌
        for (int i = nodes.Length; i < _slotViewList.Count; i++)
            _slotViewList[i].gameObject.SetActive(false);
    }

    private void LinkSlotViews(Creature creature, SkillTreeSlotNode[] nodes)
    {
        int nextLineIndex = 0;
        var halfSlotSize = slotSize * 0.5f;

        foreach ((var slotView, var lines) in _lineBySlotDic)
            slotView.OnSkillAcquired -= OnSkillAcquired;

        _lineBySlotDic.Clear();

        for (int i = 1; i < nodes.Length; i++)
        {
            var node = nodes[i];
            var slotView = _slotViewList[i];

            foreach (var precedingNode in node.GetPrecedingSlotNodes())
            {
                var linkTargetSlot = _slotViewList.Find(x => x.SlotNode == precedingNode);
                // Cach된 Line이 남았다면 가져오고, 없다면 새로 만들어서 가져옴
                var line = nextLineIndex < _linkLineList.Count ? _linkLineList[nextLineIndex] : CreateLinkLine();
                nextLineIndex++;

                // 선행 조건 Skill이 Entity의 SkillSystem에 등록되어 있다면 Line을 노란색로 표시해줌
                if (creature.SkillSystem.Contains(precedingNode.Skill))
                    line.GetComponent<Image>().color = Color.yellow;
                else
                {
                    if (!_lineBySlotDic.ContainsKey(linkTargetSlot))
                        _lineBySlotDic[linkTargetSlot] = new List<RectTransform>();

                    // Line을 선행 조건 Slot Viwe에 종속시킴
                    _lineBySlotDic[linkTargetSlot].Add(line);

                    linkTargetSlot.OnSkillAcquired += OnSkillAcquired;
                }

                // Line을 선행 조건 SlotView와 현재 SlotView의 사이 위치로 이동 시킴.
                var linePosition = (linkTargetSlot.transform.localPosition + slotView.transform.localPosition) * 0.5f;
                linePosition.x += halfSlotSize.x;
                linePosition.y -= halfSlotSize.y;

                line.localPosition = linePosition;

                // Line을 선행 조건 SlotView에서 현재 SlotView를 바라보는 각도로 회전시킴
                var direction = linkTargetSlot.transform.localPosition - slotView.transform.localPosition;
                float lineHeight = direction.y;

                line.transform.up = direction;
                line.sizeDelta = new(lineWidth, lineHeight);
            }
        }

        // 쓰지 않는 Line들을 모두 꺼줌
        for (int i = nextLineIndex; i < _linkLineList.Count; i++)
            _linkLineList[i].gameObject.SetActive(false);
    }

    private void OnSkillAcquired(SkillTreeSlotView slotView, Skill skill)
    {
        foreach (var line in _lineBySlotDic[slotView])
            line.GetComponent<Image>().color = Color.yellow;
    }

    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;

    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}
