using UnityEngine;
using System.Collections;

namespace com.Jireugi.U3DExtension.Audio {

    public class AudioHelper {

        public static void AssignSfx(GameObject container, ref AudioSource src, AudioClip clip) {
            src = null;
            if (clip != null) {
                src = container.AddComponent<AudioSource>();
                src.clip = clip;
                src.volume = 1.0f;
                src.loop = false;
                src.playOnAwake = false;
            }// fi
        }// AssignSfx



        public static void PlaySfx(AudioSource src) {
            if (src != null) {
                src.Play();
            }// fi
        }// PlaySfx



        public static void StopSfx(AudioSource src) {
            if (src != null) {
                src.Stop();
            }// fi
        }// StopSfx

    }// class

}// namespace
