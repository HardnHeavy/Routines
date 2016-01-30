using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.Jireugi.U3DExtension {

	public class KeyDebouncer : MonoBehaviour {

		protected class WaitingForKey{
			public float TimeReset = 0.0f;
			public float TimeWaitLeft = 0.0f;
			public WaitingForKey(float timereset){
				TimeReset = timereset;
			}// WaitingForKey
		}// class


		protected Dictionary<KeyCode, WaitingForKey> _setup = null;

		protected List<WaitingForKey> _waitList = null;

		protected List<WaitingForKey> _waitDone = null;

		protected float TimeWaitDefault = 0.5f;





		protected void Awake(){
			_setup = new Dictionary<KeyCode, WaitingForKey> ();
			_waitList = new List<WaitingForKey>();
			_waitDone = new List<WaitingForKey>();
		}// Awake



		protected void LateUpdate () {
			foreach (WaitingForKey wait in _waitList) {
				if (wait.TimeWaitLeft >= 0) {
					wait.TimeWaitLeft -= Time.deltaTime;
					if (wait.TimeWaitLeft <= 0.0f)
						_waitDone.Add (wait);
				}// fi
			}// foreach

			foreach (WaitingForKey wait in _waitDone) {
				DbgOut.Log ("Removing wait.");
				_waitList.Remove (wait);
			}// foreach
			_waitDone.Clear();
		}// LateUpdate



		public bool TryPress(KeyCode key){
			WaitingForKey wait = null;
			if (!_setup.TryGetValue (key, out wait)) {
				//DbgOut.LogError("KeyCode=" + key + " is not setup for wait.");
				wait = new WaitingForKey(TimeWaitDefault);
				_setup.Add (key, wait);
			}// fi
			if (_waitList.Contains (wait)) {
				return false;
			}// fi
			_waitList.Add(wait);
			wait.TimeWaitLeft = wait.TimeReset;
			return true;
		}// TryPress



		public void AddSetup(KeyCode key, float timeReset){
			WaitingForKey wait = null;
			if (_setup.ContainsKey (key)) {
				wait = _setup [key];
			} else {
				wait = new WaitingForKey (timeReset);
				_setup.Add (key, wait);
			}// fi
			wait.TimeReset = timeReset;
		}// AddSetup

	}// class

}// namespace
