using System;
using UnityEngine;

namespace Luuklsl.Scripts
{
    public class AudioController : MonoBehaviour
    {
        public AudioSource audioSource;

        /*
         * Set up an Array without a size, which lets the size be set by the Unity Editor.
         * This makes it so that the user can change the amount of frames needed
         */
        [Header("Audio fragments, in order of appearance")]
        public AudioClip[] audioClips;

        [Header("Responses")] public AudioClip positive;
        public AudioClip negative;
        public AudioClip neutral;

        private int counter = -1;



        private void PlayInstruction()
        {
            PlayOnce(audioClips[counter]);
        }

        private void PlayOnce(AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
            Debug.Log(audioClip.name + " selected");
        }


        // Update is called once per frame
        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    counter += 1;
                    counter %= audioClips.Length;
                    Debug.Log(counter);
                    PlayInstruction();
                }
                else if (Input.GetKeyUp(KeyCode.KeypadPlus))
                {
                    PlayOnce(positive);
                }
                else if (Input.GetKeyUp(KeyCode.KeypadMinus))
                {
                    PlayOnce(negative);
                }
                else if (Input.GetKeyUp(KeyCode.KeypadEnter))
                {
                    PlayOnce(neutral);
                }
            }
        }
    }
}
