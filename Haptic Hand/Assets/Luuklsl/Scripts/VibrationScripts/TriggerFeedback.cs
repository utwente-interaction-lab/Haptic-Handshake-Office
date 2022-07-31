namespace Luuklsl.Scripts.VibrationScripts
{
    public class TriggerFeedback : ContinuousStrengthPattern
    {
        public void Trigger()
        {
            PeakResponse(1);
            PeakResponse(2);
            PeakResponse(3);
        }
    }
}
