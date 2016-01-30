using UnityEngine;
using System.Collections;

using GGJ2016.Routines.Model;

namespace GGJ2016.Routines.Controller {
	public class GameCtrl : MonoBehaviour {

		private static GameCtrl _singleton = null;
		public static GameCtrl Get() {
			return _singleton;
		}// Get

		#region Model data
		protected Match _match = null;
		#endregion Model data

		#region Debug
		// TODO
		#endregion Debug





		protected void Awake() {
			if (_singleton != null) {
				GameObject.Destroy(this.gameObject);
				return;
			}// fi
			_singleton = this;

			//DbgOut.LogEnable = false;

			_match = new Match();

		}// Awake



		protected void Start() {

			CreateInfrastructure();

			//MatchView.SetModel(_match, LevelController.Level);

		}// Start

		
		//protected void Update () {
		//}



		protected void CreateInfrastructure() {
			//AudioHelper.AssignSfx(this.gameObject, ref _asApplause, SfxApplause);
		}// CreateInfrastructure

	}// class
}// namespace

