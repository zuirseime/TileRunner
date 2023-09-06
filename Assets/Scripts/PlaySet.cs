using UnityEngine;

[CreateAssetMenu(menuName = "Play Set")]
public class PlaySet : ScriptableObject
{
	[field: SerializeField] public PlaySetName Name {get; set; }

	[field: SerializeField] public GameObject Tile { get; set; }
	[field: SerializeField] public GameObject Player { get; set; }
	[field: SerializeField] public GameObject Coin { get; set; }
	[field: SerializeField] public Sprite Icon { get; set; }

	[field: SerializeField] public int Price { get; set; }
	public bool IsPurchased { get; set; } = false;
}
