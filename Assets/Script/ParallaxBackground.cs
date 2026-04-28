using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        [Tooltip("Tiles in one layer (top to bottom order is auto-fixed at runtime).")]
        public Transform[] tiles;
        [Tooltip("Relative speed. 1 = base, 0.5 = slower, 1.5 = faster.")]
        public float parallax = 1f;
        [Tooltip("Vertical spacing between tiles. 0 = auto by sprite height.")]
        public float tileSpacingY = 0f;
    }

    [Header("Group Speed")]
    [Tooltip("Base downward speed for the background group.")]
    public float baseSpeed = 1.5f;
    [Tooltip("If camera does not move, auto-scroll downward at this speed.")]
    public bool autoScrollWhenCameraStatic = true;

    [Header("Seam Fix")]
    [Tooltip("Tiny overlap to hide seams between recycled tiles.")]
    public float seamOverlapY = 0.005f;

    [Header("Layers")]
    public List<Layer> layers = new List<Layer>();
    [Tooltip("If layer tiles are missing, fallback to this object's direct children.")]
    public bool autoFillMissingTilesFromChildren = true;
    public bool verboseLog = false;

    private class RuntimeLayer
    {
        public Transform[] tiles;
        public float speed;
        public float spacing;
    }

    private readonly List<RuntimeLayer> _runtime = new List<RuntimeLayer>();
    private Camera _cam;
    private bool _initialized;
    private Vector3 _lastCamPos;
    private bool _hasLastCamPos;

    private void Awake()
    {
        _cam = Camera.main;
        TryInitializeRuntime();
    }

    private void OnEnable()
    {
        TryInitializeRuntime();
    }

    private void LateUpdate()
    {
        if (_runtime.Count == 0)
        {
            return;
        }

        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null)
            {
                if (verboseLog)
                {
                    Debug.LogWarning($"[{name}] ParallaxBackground: MainCamera not found.");
                }
                return;
            }
        }

        Vector3 currentCamPos = _cam.transform.position;
        if (!_hasLastCamPos)
        {
            _lastCamPos = currentCamPos;
            _hasLastCamPos = true;
            return;
        }

        Vector3 camDelta = currentCamPos - _lastCamPos;
        _lastCamPos = currentCamPos;
        float cameraDrivenDeltaY = camDelta.y;
        float fallbackAutoScrollDeltaY = autoScrollWhenCameraStatic ? (-baseSpeed * Time.deltaTime) : 0f;
        float appliedDeltaY = Mathf.Abs(cameraDrivenDeltaY) > 0.00001f ? cameraDrivenDeltaY : fallbackAutoScrollDeltaY;
        float camBottom = _cam.transform.position.y - _cam.orthographicSize;

        for (int i = 0; i < _runtime.Count; i++)
        {
            RuntimeLayer layer = _runtime[i];
            if (layer.tiles == null || layer.tiles.Length == 0)
            {
                continue;
            }

            float topY = float.NegativeInfinity;
            for (int t = 0; t < layer.tiles.Length; t++)
            {
                Transform tile = layer.tiles[t];
                if (tile == null)
                {
                    continue;
                }

                tile.position += Vector3.up * (appliedDeltaY * layer.speed);
                if (tile.position.y > topY)
                {
                    topY = tile.position.y;
                }
            }

            float recycleLine = camBottom - (layer.spacing * 0.5f);
            float spanY = layer.spacing * layer.tiles.Length;
            for (int t = 0; t < layer.tiles.Length; t++)
            {
                Transform tile = layer.tiles[t];
                if (tile == null)
                {
                    continue;
                }

                while (tile.position.y < recycleLine)
                {
                    Vector3 p = tile.position;
                    p.y += spanY - seamOverlapY;
                    tile.position = p;

                    if (p.y > topY)
                    {
                        topY = p.y;
                    }
                }
            }
        }
    }

    private void TryInitializeRuntime()
    {
        if (_initialized)
        {
            return;
        }

        BuildRuntime();
        _initialized = _runtime.Count > 0;

        if (_cam == null)
        {
            _cam = Camera.main;
        }

        if (_cam != null)
        {
            _lastCamPos = _cam.transform.position;
            _hasLastCamPos = true;
        }
    }

    private void BuildRuntime()
    {
        _runtime.Clear();
        Transform[] directChildren = GetDirectChildren();
        List<Layer> sourceLayers = layers;
        if (sourceLayers == null || sourceLayers.Count == 0)
        {
            sourceLayers = new List<Layer>
            {
                new Layer
                {
                    tiles = directChildren,
                    parallax = 1f,
                    tileSpacingY = 0f
                }
            };
        }

        for (int i = 0; i < sourceLayers.Count; i++)
        {
            Layer src = sourceLayers[i];
            if (src == null || src.tiles == null)
            {
                continue;
            }

            List<Transform> valid = new List<Transform>();
            for (int t = 0; t < src.tiles.Length; t++)
            {
                if (src.tiles[t] != null)
                {
                    valid.Add(src.tiles[t]);
                }
            }

            if (valid.Count == 0)
            {
                continue;
            }

            // Do not create runtime clones automatically.
            // Unexpected clones can make the background look like "new tiles" are falling.
            if (valid.Count < 2 && autoFillMissingTilesFromChildren)
            {
                for (int c = 0; c < directChildren.Length; c++)
                {
                    Transform child = directChildren[c];
                    if (child == null || valid.Contains(child))
                    {
                        continue;
                    }

                    valid.Add(child);
                    if (valid.Count >= 2)
                    {
                        break;
                    }
                }
            }

            if (valid.Count == 1 && verboseLog)
            {
                Debug.LogWarning($"[{name}] Layer {i} has only one tile. Add at least 2 tiles for a seamless loop.");
            }

            float spacing = src.tileSpacingY > 0f ? src.tileSpacingY : GetTileHeight(valid[0]);
            spacing = Mathf.Max(0.01f, spacing);

            Transform[] arranged = valid.ToArray();
            System.Array.Sort(arranged, (a, b) => b.position.y.CompareTo(a.position.y));

            // Force exact initial spacing to prevent drift from bad placement.
            float topY = arranged[0].position.y;
            for (int t = 0; t < arranged.Length; t++)
            {
                Vector3 p = arranged[t].position;
                p.y = topY - (t * spacing);
                arranged[t].position = p;
            }

            _runtime.Add(new RuntimeLayer
            {
                tiles = arranged,
                speed = Mathf.Max(0f, baseSpeed * (src.parallax <= 0f ? 1f : src.parallax)),
                spacing = spacing
            });
        }

        if (_runtime.Count == 0)
        {
            Debug.LogWarning($"[{name}] ParallaxBackground: no runtime layers. Check child tiles or layer settings.");
        }
    }

    private Transform[] GetDirectChildren()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        return children;
    }

    private static float GetTileHeight(Transform tile)
    {
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            return Mathf.Max(0.01f, sr.bounds.size.y);
        }

        Renderer r = tile.GetComponent<Renderer>();
        if (r != null)
        {
            return Mathf.Max(0.01f, r.bounds.size.y);
        }

        return 10f;
    }
}
