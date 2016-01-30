using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.Jireugi.U3DExtension;

using GGJ2016.Routines.Model;

namespace GGJ2016.Routines.Controller {
	public class GameCtrl : MonoBehaviour {

		private static GameCtrl _singleton = null;
		public static GameCtrl Get() {
			return _singleton;
		}// Get

		#region Match management

		#region Match Parameters
		public int ParamPlayerCount = 4;
		public int ParamCardsPerPlayer = 9;
		public int ParamBoardRows = 3;
		public int ParamBoardColumns = 3;
		public int ParamCardsMinValue = 1;
		public int ParamCardsMaxValue = 10;
		public int ParamCardsSetsPerColor = 4;
		public bool ParamCardColorForbiddenRed = false;
		public bool ParamCardColorForbiddenGreen = false;
		public bool ParamCardColorForbiddenBlue = false;
		public bool ParamCardColorForbiddenYellow = false;
		#endregion Match Parameters

		protected Match _match = null;

		protected bool _matchRunning = false;

		#endregion Match management



		#region View data

		public GameObject PrefabCard = null;



		// This is the parent object for all cards (and perhaps other items?) on the board.
		public GameObject ParentBoard = null;

		// Defines the rotation to be applied for a card that is placed on the board.
		public Vector3 CardOnBoardRotation = Vector3.zero;

		// Defines the offset between cards placed on the board.
		public Vector3 CardOnBoardOffset = Vector3.one;

		protected CardView[][] _cardViewsBoard = null;



		// These are the parent objects for all cards (and perhaps other items?) of each player.
		public GameObject[] ParentPlayerHands = null;

		// Defines the rotation to be applied for a card that is in a player's hand.
		public Vector3 CardPlayerHandRotation = Vector3.zero;

		// Defines the offset between cards of a player hand.
		public Vector3 CardPlayerHandOffset = Vector3.one;

		public CardView[][] _cardViewsPerPlayer = null;



		// GameObjects that define the position and rotation to be applied to the camera when it looks into the hand of a player.
		public GameObject[] CamPosPlayers = null;

		public GameObject CamPosBoard = null;

		public float TimeCamFlightNextPlayer = 1.0f;
		public float TimeCamFlightBoard = 0.25f;
		protected Vector3 _camFlightStartPos = Vector3.zero;
		protected Vector3 _camFlightStartRot = Vector3.zero;
		protected Vector3 _camFlightTargetPos = Vector3.zero;
		protected Vector3 _camFlightTargetRot = Vector3.zero;
		protected float _camFlightTimeTotal = 0.0f;
		protected float _camFlightTimeLeft = 0.0f;
		protected bool _camFlightActive = false;

		protected GameObject _camPosCurrent = null;
		protected GameObject _camPosStashed = null;

		protected bool _camPosBoardActive = false;

		#endregion View data



		#region input
		protected KeyDebouncer _keys = null;
		#endregion input



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

			_camPosCurrent = CamPosPlayers [0];
		}// Awake



		protected void Start() {

			_keys = this.gameObject.GetComponent<KeyDebouncer> ();
			_keys.AddSetup (KeyCode.Return, 0.1f);

			CreateInfrastructure();

			//MatchView.SetModel(_match, LevelController.Level);

		}// Start

		
		protected void Update () {
			DoCameraFlight();
		}// Update



		protected void LateUpdate() {
			if (Input.GetKeyDown(KeyCode.Space) && _keys.TryPress(KeyCode.Space)) {
				BeginMatch ();
			}// fi

			if (Input.GetKey (KeyCode.Return)){// && _keys.TryPress (KeyCode.Return)) {
				if (!_camPosBoardActive) {
					_camPosBoardActive = true;
					_camPosStashed = _camPosCurrent;
					ApplyCamPosition (CamPosBoard, TimeCamFlightBoard);
				}// fi
			} else {
				if (_camPosBoardActive) {
					ApplyCamPosition (_camPosStashed, TimeCamFlightBoard);
					_camPosBoardActive = false;
				}// fi
			}// fi
		}// LateUpdate



		protected void CreateInfrastructure() {

			Vector3 offsetBoard = Vector3.zero;
			offsetBoard.x = -0.5f * (CardOnBoardOffset.x * (ParamBoardColumns-1));
			offsetBoard.z = -0.5f * (CardOnBoardOffset.z * (ParamBoardRows-1));
			_cardViewsBoard = new CardView[ParamBoardRows][];
			for (int row = 0; row < ParamBoardRows; row++){
				_cardViewsBoard [row] = new CardView[ParamBoardColumns];
				for (int col = 0; col < ParamBoardColumns; col++) {
					CardView cv = SpawnCard (
						(CardColor)System.Enum.ToObject (typeof(CardColor), MathHelper.Rand.Next(System.Enum.GetNames (typeof(CardColor)).Length-1)+1),
						MathHelper.Rand.Next(ParamCardsMaxValue - ParamCardsMinValue) +1 +ParamCardsMinValue
						);
					cv.transform.position =
						ParentBoard.transform.position 
						+ offsetBoard
						+ new Vector3(col * CardOnBoardOffset.x, 0.0f, row * CardOnBoardOffset.z);
					cv.transform.rotation = Quaternion.Euler (CardOnBoardRotation);
					cv.transform.SetParent(ParentBoard.transform, true);

					_cardViewsBoard [row] [col] = cv;
				}// for
			}// for

			_cardViewsPerPlayer = new CardView[ParamPlayerCount][];
			for (int p = 0; p < ParamPlayerCount; p++) {
				_cardViewsPerPlayer[p] = new CardView[ParamCardsPerPlayer];
				for (int pc = 0; pc < ParamCardsPerPlayer; pc++) {
					CardView cv = SpawnCard (
						(CardColor)System.Enum.ToObject (typeof(CardColor), MathHelper.Rand.Next(System.Enum.GetNames (typeof(CardColor)).Length-1)+1),
						MathHelper.Rand.Next(ParamCardsMaxValue - ParamCardsMinValue) +1 +ParamCardsMinValue
					);

					cv.transform.position =
						//ParentPlayerHands[p].transform.position +
						new Vector3(pc * CardPlayerHandOffset.x, CardPlayerHandOffset.y, CardPlayerHandOffset.z);
					cv.transform.rotation = Quaternion.Euler (CardPlayerHandRotation);// + ParentPlayerHands[p].transform.rotation.eulerAngles);
					cv.transform.SetParent(ParentPlayerHands[p].transform, false);

					_cardViewsPerPlayer [p] [pc] = cv;
				}// for
			}// for

			//AudioHelper.AssignSfx(this.gameObject, ref _asApplause, SfxApplause);
		}// CreateInfrastructure



		#region match management
		protected void BeginMatch(){

			DbgOut.Log ("Beginning new match.");

			if (_matchRunning)
				AbortMatch ();

			List<CardColor> forbiddenColors = new List<CardColor> ();
			forbiddenColors.Add (CardColor.None);
			if (ParamCardColorForbiddenRed)
				forbiddenColors.Add (CardColor.Red);
			if (ParamCardColorForbiddenGreen)
				forbiddenColors.Add (CardColor.Green);
			if (ParamCardColorForbiddenBlue)
				forbiddenColors.Add (CardColor.Blue);
			if (ParamCardColorForbiddenYellow)
				forbiddenColors.Add (CardColor.Yellow);

			_match = new Match(
				ParamPlayerCount, // playerCount
				ParamCardsPerPlayer, // cardsPerPlayer
				ParamBoardRows, // boardrows
				ParamBoardColumns, // boardcolumns
				ParamCardsMinValue, // cardsMinValue
				ParamCardsMaxValue, // cardsMaxValue
				ParamCardsSetsPerColor, // cardsSetsPerColor
				forbiddenColors // forbiddenCardColors
			);

			FillBoard ();

			_camPosCurrent = CamPosPlayers [_match.PlayerAction];
			ApplyCamPosition (_camPosCurrent, TimeCamFlightNextPlayer);

			_matchRunning = true;

		}// BeginMatch


		protected void PrepareTurn(){
			// TODO
		}// PrepareTurn



		protected void EndMatch(){
			_matchRunning = false;
			// TODO
		}// EndMatch



		protected void AbortMatch(){
			_matchRunning = false;
			// TODO
		}// AbortMatch


		#endregion match management




		#region view management

		protected CardView SpawnCard(CardColor color, int value){

			GameObject card = GameObject.Instantiate (PrefabCard);
			CardView view = card.GetComponent<CardView> ();
			view.SetCardColor (color);
			view.SetCardNumber (value);

			return view;
		}// SpawnCard



		protected CardView SpawnCard(Card cardModel){

			GameObject card = GameObject.Instantiate (PrefabCard);
			CardView view = card.GetComponent<CardView> ();
			view.CardModel = cardModel;
		
			return view;
		}// SpawnCard



		protected void FillBoard(){
			for (int row = 0; row < _match.PlayBoard.Rows; row++){
				for (int col = 0; col < _match.PlayBoard.Columns; col++) {
					_cardViewsBoard [row] [col].CardModel = _match.PlayDeck.DrawCard ();
				}// for
			}// for
		}// FillBoard



		protected void ApplyCamPosition(GameObject camPosition, float time){
			_camFlightTimeLeft = time;
			_camFlightStartPos = Camera.main.transform.position;
			_camFlightStartRot = Camera.main.transform.rotation.eulerAngles;
			_camFlightTargetPos = camPosition.transform.position;
			_camFlightTargetRot = camPosition.transform.rotation.eulerAngles;
			_camFlightTimeTotal = time;
			_camFlightTimeLeft = time;

			_camFlightActive = true;

			_camPosCurrent = camPosition;
		}// ApplyCamPosition



		protected void DoCameraFlight(){
			if (!_camFlightActive)
				return;

			_camFlightTimeLeft -= Time.deltaTime;
			if (_camFlightTimeLeft <= 0.0f) {
				_camFlightTimeLeft = 0.0f;
				_camFlightActive = false;
			}// fi
			float share = (_camFlightTimeTotal - _camFlightTimeLeft) / _camFlightTimeTotal;

			Camera.main.transform.position = Vector3.Lerp(_camFlightStartPos, _camFlightTargetPos, share);
			Camera.main.transform.rotation = Quaternion.Euler( Vector3.Lerp(_camFlightStartRot, _camFlightTargetRot, share));
		}// DoCameraFlight

		#endregion view management
			

	}// class
}// namespace

