using UnityEngine;
using System.Collections;

namespace com.Jireugi.U3DExtension {

    public class DbgOut : MonoBehaviour {

        public static bool LogEnable = true;
        public static bool LogPrependTime = true;

        public static GUIText TxtDebug = null;
        public const int TxtLinesCount = 30;
        public static string[] TxtLines = null;
        private static int TxtLineEdit = 0;

        public string GUITextName = "TxtDebug";





        public void Awake() {

            if (TxtLines == null) {
                TxtLines = new string[TxtLinesCount];
                TxtLineEdit = 0;
            }// fi

            if (LogEnable) {
                // Try to find panel for debugoutput.
                DbgOut.TxtDebug = null;
                //			GameObject goGUI = GameObject.Find("GUI");
                //			if (goGUI == null){
                //				DbgOut.Log("DbgOut.Awake - no GUI object retrieved.");
                //			} else {
                Transform transTxt = this.gameObject.transform.Find(GUITextName);
                if (transTxt != null) {
                    DbgOut.TxtDebug = transTxt.GetComponent<GUIText>();
                    DbgOut.Log(null); // I. e. rebuild possibly existing debug text.
                }// fi
                //			}// fi

            }// fi
        }// Awake



        public void OnDisable() {
            DbgOut.TxtDebug = null;
        }// OnDisable



        public static void Log(string msg) {
            DbgOut.Log(3, msg);
        }// Log



        public static void LogWarning(string msg) {
            DbgOut.Log(2, msg);
        }// LogWarning



        public static void LogError(string msg) {
            DbgOut.Log(1, msg);
        }// LogError



        // severity:
        // 0 critical error
        // 1 non-critical error
        // 2 warning
        // 3 status
        // 4 annoying status
        public static void Log(int severity, string msg) {
            if (LogEnable) {
                string msgout = ((LogPrependTime) ? (string.Format("{0:0.000000}", Time.time) + " ") : ("")) + msg;
                if (severity >= 3)
                    Debug.Log(msgout);
                else if (severity == 2)
                    Debug.LogWarning(msgout);
                else if (severity < 2)
                    Debug.LogError(msgout);

                AddDebugTxtLine(msgout);
            }// fi
        }// Log



        private static void AddDebugTxtLine(string msg) {
            if (DbgOut.TxtDebug != null) {

                if (msg != null && msg.Length > 0)
                    TxtLines[TxtLineEdit++] = msg;

                if (TxtLineEdit >= TxtLinesCount)
                    TxtLineEdit = 0;

                if (DbgOut.TxtDebug.enabled) {

                    // Build the output text. The latest message is at the lowest line.

                    string msgout = "";

                    if (TxtLineEdit > 0)
                        for (int i = TxtLineEdit - 1; i >= 0; i--)
                            msgout += TxtLines[i] + "\n";

                    //for (int i=TxtLineEdit; i < TxtLinesCount; i++)
                    //msgout += TxtLines[i] + "\n";

                    //if (TxtLineEdit > 0){
                    //for (int i=0; i < TxtLineEdit; i++)
                    //msgout += TxtLines[i] + "\n";
                    for (int i = TxtLinesCount - 1; i >= TxtLineEdit; i--)
                        msgout += TxtLines[i] + "\n";
                    //}// fi

                    DbgOut.TxtDebug.text = msgout;
                }// fi

            }// fi
        }// AddDebugTxtLine

    }// class

}// namespace
