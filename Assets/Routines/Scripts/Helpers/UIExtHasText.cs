using UnityEngine;
using UnityEngine.UI;

namespace com.Jireugi.U3DExtension.UI {

    public class UIExtHasText : MonoBehaviour {

        public GameObject TextChild = null;

        protected Text _text = null;
        public Text Text {
            get { return _text; }
        }// property





        protected void Awake() {
            if (TextChild == null) {
                Transform tf = this.transform.FindChild("Text");
                if (tf != null) {
                    TextChild = tf.gameObject;
                }// fi
            }// fi
            if (TextChild != null) {
                _text = TextChild.GetComponent<Text>();
            }// fi
            if (_text == null) {
                DbgOut.LogWarning("GameObject=" + this.gameObject.name + " lacks Text child with Text component.");
            }// fi
            
        }// Awake

    }// class

}// namespace
