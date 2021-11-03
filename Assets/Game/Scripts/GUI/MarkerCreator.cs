using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MarkerCreator : MonoBehaviour
{
    public string markerText;

    [SerializeField]
    private GameObject _markerPrefab;
    private TMPro.TextMeshPro _textComponent;
    private PositionConstraint _positionConstraint;

    private void Awake()
    {
        GameObject marker = Instantiate(_markerPrefab);

        _positionConstraint = marker.GetComponent<PositionConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = this.transform;
        source.weight = 1;
        _positionConstraint.AddSource(source);
        _positionConstraint.constraintActive = true;

        _textComponent = marker.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _textComponent.text = markerText;
    }

    public void SetMarkerActive(bool active)
    {
        if(_positionConstraint != null)
            _positionConstraint.gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        SetMarkerActive(true);
    }

    private void OnDisable()
    {
        SetMarkerActive(false);
    }
}
