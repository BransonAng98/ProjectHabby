using System.Collections;
using UnityEngine;

public class HittableSpriteGroup : MonoBehaviour {
	public Material material;
	public Color hitColor = Color.white;

	private SpriteRenderer[] _renderers;
	private MaterialPropertyBlock _mpb;
	private static readonly int HIT_TIME_KEY = Shader.PropertyToID("_HitTime");
	private static readonly int HIT_COLOR_KEY = Shader.PropertyToID("_HitColor");

	private void Awake() {
		_renderers = GetComponentsInChildren<SpriteRenderer>();
		foreach (var renderer in _renderers) {
			renderer.sharedMaterial = material;
		}
		SetHitColor(hitColor);
	}

	public void Blink() {
		_mpb ??= new MaterialPropertyBlock();
		foreach (var spriteRenderer in _renderers) {
			spriteRenderer.GetPropertyBlock(_mpb);
			_mpb.SetFloat(HIT_TIME_KEY, Time.time);
			spriteRenderer.SetPropertyBlock(_mpb);
		}
	}

	public void SetHitColor(Color color) {
		_mpb ??= new MaterialPropertyBlock();
		foreach (var spriteRenderer in _renderers) {
			spriteRenderer.GetPropertyBlock(_mpb);
			_mpb.SetColor(HIT_COLOR_KEY, color);
			spriteRenderer.SetPropertyBlock(_mpb);
		}
	}

	public void ResetColor() {
		foreach (var spriteRenderer in _renderers) {
			spriteRenderer.color = Color.white;
		}
	}

	public void FadeOut() {
		StopCoroutine(nameof(FadeCoroutine));
		StartCoroutine(nameof(FadeCoroutine));
	}

	private IEnumerator FadeCoroutine() {
		var t = 0f;
		var startTime = Time.time;
		var color = Color.white;
		while (t < 1f) {
			color.a = Mathf.Lerp(1f, 0f, t);
			foreach (var spriteRenderer in _renderers) {
				spriteRenderer.color = color;
			}
			t = (Time.time - startTime) / 0.2f;
			yield return null;
		}
		color.a = 0f;
		foreach (var spriteRenderer in _renderers) {
			spriteRenderer.color = color;
		}
	}

#if UNITY_EDITOR
	private void Reset() {
		_renderers = GetComponentsInChildren<SpriteRenderer>(true);
	}
#endif
}