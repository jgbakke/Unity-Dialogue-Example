using System;

namespace Utils {
	[Serializable]
	public struct UnityMap<K,V>{
		public K key;
		public V value;

		public UnityMap(K k, V v){
			key = k;
			value = v;
		}
	}

}

