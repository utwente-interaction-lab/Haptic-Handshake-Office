using Dexmo.Unity;
using Dexmo.Unity.Interaction;
using UnityEngine;

public class GestureInteractionTest : MonoBehaviour
{
    private InteractionHand hand_left;
    private void Start()
    {
        foreach (var hand in GameObject.FindObjectsOfType<InteractionHand>())
        {
            if (hand.Handedness == Handedness.Left)
            {
                hand_left = hand;
                break;
            }
        }
        SetChecker();
    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        hand_left.InteractionManager.InteractionSettings.GestureSettings.OnGestureEnter.RemoveListener(GestureEnter);
        hand_left.InteractionManager.InteractionSettings.GestureSettings.OnGestureExit.RemoveListener(GestureExit);
    }

    private void SetChecker()
    {
        hand_left.InteractionManager.InteractionSettings.GestureSettings.OnGestureEnter.AddListener(GestureEnter);
        hand_left.InteractionManager.InteractionSettings.GestureSettings.OnGestureExit.AddListener(GestureExit);
    }

    public void GestureEnter(GestureInteractionData _data)
    {
        Debug.Log("Enter    " + _data.GestureName);
    }

    public void GestureExit(GestureInteractionData _data)
    {
        Debug.Log("Exit    " + _data.GestureName);
    }
}
