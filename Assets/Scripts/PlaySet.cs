using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PlaySet
{
	[field: SerializeField] public PlaySetName Name {get; set; }
	[field: SerializeField] public GameObject Tile { get; set; }
	[field: SerializeField] public GameObject Player { get; set; }
	[field: SerializeField] public GameObject Coin { get; set; }
	[field: SerializeField] public Sprite Icon { get; set; }
}
