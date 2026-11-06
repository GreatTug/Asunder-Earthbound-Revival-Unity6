using UnityEngine;

/// <summary>
/// High-performance coroutine yield cache to eliminate per-frame GC heap allocations
/// across all game state machines and controllers.
/// </summary>
public static class AsunderCoroutines
{
	public static readonly WaitForSeconds FsmWait = new WaitForSeconds(0.01f);
	public static readonly WaitForSeconds HalfSecond = new WaitForSeconds(0.5f);
	public static readonly WaitForSeconds OneSecond = new WaitForSeconds(1.0f);
	public static readonly WaitForSeconds TwoSeconds = new WaitForSeconds(2.0f);
}
