using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class FloatingTextView : MonoBehaviour
{
    private class FloatingTextData
    {
        public TextMeshProUGUI TextMesh { get; private set; }
        public float CurrentDuration { get; set; }

        public FloatingTextData(TextMeshProUGUI textMesh)
            => TextMesh = textMesh;
    }

    private class FloatingTextGroup
    {
        public List<FloatingTextData> textDatas = new();

        public Transform TraceTarget { get; private set; }
        public RectTransform GroupTransform { get; private set; }
        public IReadOnlyList<FloatingTextData> TextDatas => textDatas;

        public FloatingTextGroup(Transform traceTarget, RectTransform groupTransform)
            => (TraceTarget, GroupTransform) = (traceTarget, groupTransform);

        public void AddData(FloatingTextData textData)
            => textDatas.Add(textData);

        public void RemoveData(FloatingTextData textData)
            => textDatas.Remove(textData);
    }

    [SerializeField] private RectTransform canvasTransform;

    [Space]
    [SerializeField] private GameObject textGroupPrefab;
    [SerializeField] private GameObject floatingTextPrefab;

    [Space]
    [SerializeField] private float floatingDuration;

    private readonly Dictionary<Transform, FloatingTextGroup> _textGroupsByTarget = new();
    private readonly Queue<Transform> _removeTargetQueue = new();
    private readonly Queue<FloatingTextData> _removeTextDataQueue = new();

    private void Awake()
    {
        Managers.UI.FloatingTextView = this;
    }

    private void LateUpdate()
    {
        foreach ((var traceTarget, var textGroup) in _textGroupsByTarget)
        {
            UpdatePosition(textGroup);

            foreach (var textData in textGroup.TextDatas)
            {
                textData.CurrentDuration += Time.deltaTime;

                var color = textData.TextMesh.color;
                color.a = Mathf.Lerp(1f, 0f, textData.CurrentDuration / floatingDuration);
                textData.TextMesh.color = color;

                if (textData.CurrentDuration >= floatingDuration)
                    _removeTextDataQueue.Enqueue(textData);
            }

            while (_removeTextDataQueue.Count > 0)
            {
                var targetTextData  = _removeTextDataQueue.Dequeue();

                Destroy(targetTextData.TextMesh.gameObject);

                textGroup.RemoveData(targetTextData);
            }

            if (textGroup.textDatas.Count == 0)
                _removeTargetQueue.Enqueue(traceTarget);
        }

        while (_removeTargetQueue.Count > 0)
        {
            var removeTarget = _removeTargetQueue.Dequeue();

            Destroy(_textGroupsByTarget[removeTarget].GroupTransform.gameObject);

            _textGroupsByTarget.Remove(removeTarget);
        }
    }

    private void UpdatePosition(FloatingTextGroup group)
    {
        if (!group.TraceTarget)
            return;
        
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(group.TraceTarget.position);
        Vector2 uiPosition = (viewportPosition * canvasTransform.sizeDelta) - (canvasTransform.sizeDelta * 0.5f);

        group.GroupTransform.anchoredPosition = uiPosition;
    }

    public void Show(Transform traceTarget, string text, Color color)
    {
        var textGroup = CreateCachedGroup(traceTarget);

        var textMesh = Instantiate(floatingTextPrefab, textGroup.GroupTransform).GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color;

        var newTextData = new FloatingTextData(textMesh);
        textGroup.AddData(newTextData);
    }

    public void Show(Transform traceTarget, string text)
        => Show(traceTarget, text, Color.white);

    private FloatingTextGroup CreateCachedGroup(Transform traceTarget)
    {
        if (!_textGroupsByTarget.ContainsKey(traceTarget))
        {
            var group = Instantiate(textGroupPrefab, transform);
            var newTextGroup = new FloatingTextGroup(traceTarget, group.GetComponent<RectTransform>());
            _textGroupsByTarget[traceTarget] = newTextGroup;

            UpdatePosition(newTextGroup);
        }

        return _textGroupsByTarget[traceTarget];
    }
}
