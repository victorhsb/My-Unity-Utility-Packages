using System;
using System.Collections;
using UnityEngine;

namespace voidling {
	[Serializable]
	public class ScrollingItem {
		[SerializeField] private GameObject item;
		[SerializeField] private int bufferSize = 1;
		[SerializeField] private int backBufferSize = 1;

		[Header("scrolling config")] [SerializeField]
		public float scrollSpeed = 1;

		private Vector2 _bounds;
		private Vector3 _startingPoint;
		private GameObject[] _itemBuffer;
		private Transform transform;
		
		// the middle Idx is the item after the middle one
		private int nextInLine => backBufferSize;
		private int bufferLen => backBufferSize + 1 + bufferSize;

		public void Start(Transform parent) {
			transform = parent;
			_bounds = item.GetComponent<Renderer>().bounds.size;
			CreateBuffer();
		}

		public IEnumerator WatchProcess(Func<bool> scrolling) {
			while (scrolling()) {
				item = _itemBuffer[nextInLine];
				yield return new WaitUntil(() => item.transform.position.x < _startingPoint.x);
				
				Vector3 newPos = _itemBuffer[bufferLen - 1].transform.position;
				newPos.x += _bounds.x - 0.0005f;
				_itemBuffer[0].transform.position = newPos;
				
				shiftBuffer();
			}
			
			yield return new WaitForSeconds(1);
		}

		// shiftBuffer shifts the whole buffer to the left, moving the first item to the last position
		// this is done in order to maintain buffer sorting
		void shiftBuffer() {
			var first = _itemBuffer[0];
			for (int i = 0; i < bufferLen - 1; i++) {
				_itemBuffer[i] = _itemBuffer[i + 1];
			}
			_itemBuffer[bufferLen - 1] = first;
		}

		public void Update() {
			MoveTiles();
		}

		private void MoveTiles() {
			foreach (var item in _itemBuffer) {
				item.transform.Translate(Vector3.left * (scrollSpeed * Time.deltaTime));
			}
		}

		// allocates the item in the buffer, and creates new ones to be allocated
		private void CreateBuffer() {
			_itemBuffer = new GameObject[backBufferSize + 1 + bufferSize];
			for (int i = 0; i < backBufferSize; i++) {
				Debug.Log($"creating back item {i}");
				_itemBuffer[i] = CreateItem((backBufferSize - i) * -1);
			}
			
			_itemBuffer[backBufferSize] = item;
			_startingPoint = item.transform.position;
			
			for (int i = 1; i <= bufferSize; i++) {
				Debug.Log($"creating front item {i} at idx {backBufferSize+1+i}");
				_itemBuffer[backBufferSize+i] = CreateItem(i);
			}

		}

		private GameObject CreateItem(float offset) {
			Vector3 pos = item.transform.position;
			pos.x += _bounds.x * offset;
			
			var obj = GameObject.Instantiate(item.gameObject, pos, Quaternion.identity);
			obj.transform.SetParent(transform);
			
			//obj.transform.Translate(pos);
			
			return obj;
		}

		// this is called at editor time so we can't afford to rely on local transform being filled;
		public void Validate(Transform parent) {
			// if it's a prefab
			if (item.scene.name == null) {
				var obj = GameObject.Instantiate(item, parent.position, Quaternion.identity);
				obj.transform.SetParent(parent);
			}
			
			// if the object is not a parent of this one
			if (item.transform.parent != parent) {
				item.transform.SetParent(parent);		
			}
		}
	}
}