using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class SpriteConfigurator : MonoBehaviour
{
    [SerializeField]
    private int _id = 0;

    [SerializeField]
    private List<int> _interactWith;

    [SerializeField]
    private float _distToNeighbour = 1f;

    [SerializeField]
    private bool _updateOnTransformChange = true;

    [SerializeField]
    private bool _updateOnEnable = true;

    [SerializeField]
    private bool _updateOnDisable = true;

    [SerializeField]
    private GameObject _edgeLeft;

    [SerializeField]
    private GameObject _edgeRight;

    [SerializeField]
    private GameObject _edgeTop;

    [SerializeField]
    private GameObject _edgeBottom;

    [SerializeField]
    private GameObject _outerTopLeft;

    [SerializeField]
    private GameObject _outerTopRight;

    [SerializeField]
    private GameObject _outerBottomRight;

    [SerializeField]
    private GameObject _outerBottomLeft;

    [SerializeField]
    private GameObject _innerTopLeft;

    [SerializeField]
    private GameObject _innerTopRight;

    [SerializeField]
    private GameObject _innerBottomRight;

    [SerializeField]
    private GameObject _innerBottomLeft;

    private List<SpriteConfigurator> _lastNeighbours = new List<SpriteConfigurator>();

    private struct ConfiguratorNeighbours
    {
        public SpriteConfigurator topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;
    }

    private void Update()
    {
        if (!_updateOnTransformChange)
            return;

        if (transform.hasChanged)
        {
            UpdateSprite();
            transform.hasChanged = false;
        }
    }

    private void OnEnable()
    {
        if (!_updateOnEnable) 
            return;

        UpdateSprite();
    }

    private void OnDisable()
    {
        if (!_updateOnDisable) 
            return;

        UpdateSprite();
    }

    public void InteractWithId(int newId)
    {
        _interactWith.Add(newId);
    }

    public void SetId(int newId)
    {
        _id = newId;
    }

    public void UpdateSprite()
    {
        UpdateMyself(true);
    }

    private void UpdateMyself(bool updateNeighbours)
    {
        var n = GetNeighbours();
        ConfigureByNeighbourBools(
            n.topLeft != null,
            n.topRight != null,
            n.bottomLeft != null,
            n.bottomRight != null,

            n.left != null,
            n.top != null,
            n.right != null,
            n.bottom != null);

        foreach (var lastNeighbour in _lastNeighbours)
        {
            if (lastNeighbour != null)
                lastNeighbour.UpdateMyself(false);
        }

        _lastNeighbours = new List<SpriteConfigurator>();

        if (updateNeighbours)
        {
            SpriteConfigurator[] confs = { n.topLeft,
            n.topRight,
            n.bottomLeft,
            n.bottomRight,

            n.left,
            n.top,
            n.right,
            n.bottom };

            foreach (var conf in confs)
            {
                if (conf != null)
                {
                    conf.UpdateMyself(false);
                    _lastNeighbours.Add(conf);
                }
            }
        }
    }

    private void ConfigureByNeighbourBools(bool hasTopLeft, bool hasTopRight, bool hasBottomLeft, bool hasBottomRight,
        bool hasLeft, bool hasTop, bool hasRight, bool hasBottom)
    {
        //Edges
        _edgeLeft.SetActive(!hasLeft);
        _edgeRight.SetActive(!hasRight);
        _edgeTop.SetActive(!hasTop);
        _edgeBottom.SetActive(!hasBottom);

        //Outer corners
        _outerTopLeft.SetActive(!hasTop && !hasLeft);// && !hasTopLeft);
        _outerTopRight.SetActive(!hasTop && !hasRight);// && !hasTopRight);
        _outerBottomRight.SetActive(!hasBottom && !hasRight);// && !hasBottomRight);
        _outerBottomLeft.SetActive(!hasBottom && !hasLeft);// && !hasBottomLeft);

        //Inner corners
        _innerTopLeft.SetActive(hasTop && hasLeft && !hasTopLeft);
        _innerTopRight.SetActive(hasTop && hasRight && !hasTopRight);
        _innerBottomRight.SetActive(hasBottom && hasRight && !hasBottomRight);
        _innerBottomLeft.SetActive(hasBottom && hasLeft && !hasBottomLeft);
    }

    private ConfiguratorNeighbours GetNeighbours()
    {
        Vector3 center = transform.position;
        float dist = _distToNeighbour;

        ConfiguratorNeighbours neighbours = new ConfiguratorNeighbours();
        neighbours.top = GetConfiguratorAt(center + dist * Vector3.up);
        neighbours.bottom = GetConfiguratorAt(center + dist * Vector3.down);
        neighbours.left = GetConfiguratorAt(center + dist * Vector3.left);
        neighbours.right = GetConfiguratorAt(center + dist * Vector3.right);

        neighbours.topLeft = GetConfiguratorAt(center + dist * (Vector3.up + Vector3.left).normalized);
        neighbours.topRight = GetConfiguratorAt(center + dist * (Vector3.up + Vector3.right).normalized);
        neighbours.bottomLeft = GetConfiguratorAt(center + dist * (Vector3.down + Vector3.left).normalized);
        neighbours.bottomRight = GetConfiguratorAt(center + dist * (Vector3.down + Vector3.right).normalized);

        return neighbours;
    }

    private SpriteConfigurator GetConfiguratorAt(Vector2 pos)
    {
        var colliders = Physics2D.OverlapPointAll(pos);
        foreach (var collider in colliders)
        {
            var configurator = collider.gameObject.GetComponent<SpriteConfigurator>();
            if (configurator != null && ShouldInteractWith(configurator))
                return configurator;
        }
        return null;
    }

    private bool ShouldInteractWith(SpriteConfigurator otherConf)
    {
        return _id == otherConf._id || _interactWith.Intersect(otherConf._interactWith).Any();
    }
}

