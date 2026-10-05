using System.Collections.Generic;
using UnityEngine;

namespace NunAddon.Gameplay;

public static class SealVisual
{
    private sealed class SealFx
    {
        public GameObject Root = null!;
        public SpriteRenderer Ground = null!;
        public SpriteRenderer ScanA = null!;
        public SpriteRenderer ScanB = null!;
        public float Started;
    }

    private static readonly Dictionary<byte, SealFx> Rings = new();
    private static Sprite? _ringSprite;

    private static Sprite RingSprite
    {
        get
        {
            if (_ringSprite != null) return _ringSprite;

            const int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.name = "NunSealRingTexture";
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            var pixels = new Color32[size * size];
            var c = (size - 1) * .5f;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var dx = (x - c) / c;
                var dy = (y - c) / c;
                var d = Mathf.Sqrt(dx * dx + dy * dy);
                var a = Mathf.Clamp01(1f - Mathf.Abs(d - .82f) / .055f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255));
            }

            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            _ringSprite = Sprite.Create(tex, new Rect(0, 0, size, size),
                new Vector2(.5f, .5f), 100f);
            _ringSprite.name = "NunSealRingSprite";
            return _ringSprite;
        }
    }

    private static SpriteRenderer MakeRing(Transform parent, string name, float alpha)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = RingSprite;
        sr.color = new Color(1f, .08f, .08f, alpha);
        sr.sortingOrder = 20;
        return sr;
    }

    public static void Show(PlayerControl target)
    {
        if (Rings.ContainsKey(target.PlayerId)) return;

        var root = new GameObject("NunSealFx");
        root.transform.position = target.transform.position + new Vector3(0f, -.18f, -.25f);

        var ground = MakeRing(root.transform, "NunSealGround", .92f);
        ground.transform.localScale = new Vector3(1.05f, .42f, 1f);

        var a = MakeRing(root.transform, "NunSealScanA", .78f);
        var b = MakeRing(root.transform, "NunSealScanB", .55f);

        Rings[target.PlayerId] = new SealFx
        {
            Root = root, Ground = ground, ScanA = a, ScanB = b, Started = Time.time
        };
        Tick(target);
    }

    public static void Tick(PlayerControl target)
    {
        if (!Rings.TryGetValue(target.PlayerId, out var fx) || fx.Root == null) return;

        fx.Root.transform.position = target.transform.position + new Vector3(0f, -.18f, -.25f);

        fx.Ground.transform.localScale = new Vector3(1.05f, .42f, 1f);

        var t = (Time.time - fx.Started) * 2.8f;
        AnimateScan(fx.ScanA, t - Mathf.Floor(t));
        AnimateScan(fx.ScanB, (t + .5f) - Mathf.Floor(t + .5f));
    }

    private static void AnimateScan(SpriteRenderer sr, float phase)
    {
        var s = Mathf.Lerp(1.45f, .72f, phase);
        sr.transform.localScale = new Vector3(s, s * .40f, 1f);
        var c = sr.color;
        c.a = Mathf.Lerp(.82f, .12f, phase);
        sr.color = c;
    }

    public static void Hide(byte id)
    {
        if (!Rings.Remove(id, out var fx)) return;
        if (fx.Root != null) UnityEngine.Object.Destroy(fx.Root);
    }

    public static void Clear()
    {
        foreach (var fx in Rings.Values)
            if (fx.Root != null) UnityEngine.Object.Destroy(fx.Root);
        Rings.Clear();
    }
}
