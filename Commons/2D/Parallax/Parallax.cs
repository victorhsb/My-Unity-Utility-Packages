using System;
using UnityEngine;
using System.Collections.Generic;

namespace voidling {
	public class Parallax : MonoBehaviour {
		[SerializeField] private List<ScrollingGameObject> items;

		private event Action OnUpdate;

		private bool scrolling() => gameObject.activeInHierarchy;

		private void Start() {
			foreach (var item in items) {
				item.Start(transform);
				OnUpdate += item.OnUpdate;
			}
			foreach (var item in items)
				StartCoroutine(item.WatchProcess(scrolling));
		}

		// just call all of the update functions
		private void FixedUpdate() => OnUpdate?.Invoke();

		private void OnValidate() {
			foreach (var item in items) {
				item.Validate(transform);	
			}
		}
	}
}