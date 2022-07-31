using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class TestVibrations : BaseVibrationScript
    {
        public float timer = 0.0f;

        public int seconds;


        // Update is called once per frame
        private new void Update()
        {
            timer += Time.deltaTime;
            seconds = (int)(timer % 60f);

            switch (seconds % 4)
            {
                case 0:
                    UpdateMessage(1, 255);
                    UpdateMessage(2, 0);
                    UpdateMessage(3, 0);
                    UpdateMessage(4, 0);
                    break;
                case 1:
                    UpdateMessage(1, 0);
                    UpdateMessage(2, 255);
                    UpdateMessage(3, 0);
                    UpdateMessage(4, 0);
                    break;
                case 2:
                    UpdateMessage(1, 0);
                    UpdateMessage(2, 0);
                    UpdateMessage(3, 255);
                    UpdateMessage(4, 0);
                    break;
                case 3:
                    UpdateMessage(1, 0);
                    UpdateMessage(2, 0);
                    UpdateMessage(3, 0);
                    UpdateMessage(4, 255);
                    break;
                default:
                    Debug.Log("Stukkiewukkie");
                    break;
            }

            base.Update();
        }
    }
}
