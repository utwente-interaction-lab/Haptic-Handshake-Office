using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Luuklsl.Scripts
{
    public class ScenarioAnimator : MonoBehaviour
    {
        // == Public Variables ==
        // These are the ones you can change. They are used for locations later on
        public Transform startLocation;
        public Transform firstLocation;
        public Transform nearSeatLocation;
        public Transform playerObject;
        public Transform seatObject;

        // Reference to the agent, could also be retrieved in a Awake(), but this gives more certainty imo
        public NavMeshAgent agent;

        public ThirdPersonCharacter character;

        // The ArmAim rig. This is so we can access it from the code
        public StretchArm arm;

        // == Private, set-able variables ==

        [SerializeField] private Animator animator;

        [SerializeField] [Range(0.1f, 8)] private float strength;

        // == Script only Private variables == 
        private float lerpStrength;

        private KeyCode lastPressed;

        private FixedJoint seatJoint;

        private bool moving;
        private bool location2 = false;
        private bool finishedRotating = false;
        private bool seated;
        private bool seatedLookingAtParticipant = false;
        private bool firstTime = true;
        private bool movingAway = false;
        private Vector3 seatObjectCenter;
        private Rigidbody seatRigidbody;


        /*
         * Default Unity function.
         * We use it to verify if a component is missing.
         * We also define that the agent should not rotate itself.
         */
        private void Start()
        {
            if (firstLocation == null || nearSeatLocation == null || agent == null || character == null)
            {
                Debug.LogError("One of the parameters was NULL, make sure you set all parameters");
                // UnityEditor.EditorApplication.isPlaying = false;
            }

            seatObjectCenter = seatObject.GetComponent<Renderer>().bounds.center;
            agent.updateRotation = false;
        }

        /*
         * The core of this script, exists of three core parts.
         * Part A : InputHandler + navigation location setter
         * Part B : Free moving on navigation targets
         * Part C : Scripted moving on sitting down and standing up, basically a big decision tree based on booleans
         * we set when we get near the seat.
         */
        private void Update()
        {
            // Part A
            /*
             * Check for input, goes down to the simple input handler
             */
            if (Input.anyKeyDown)
            {
                InputHandler();
            }

            // Part B
            // Verify if the agent is enabled before we make it move
            if (agent.enabled == true)
            {
                // If we are more than stoppingDistance away, keep navigating
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    // Move towards point, dont crouch/jump
                    character.Move(agent.desiredVelocity, false, false);
                }
                else
                {
                    // Stop moving, no crouch/jump
                    character.Move(Vector3.zero, false, false);
                }

                /*
                 * Make sure the avatar orients towards the player
                 * Check if the distance is between 2D stopping distance (we ignore Y space)
                 * If this is true, turn the avatar.
                 * We need to do this manually as the avatar does not take into account rotation of its goal (we turned
                 * that off in start) 
                 */
                if (Helpers.HorizontalDistance(gameObject.transform.position, firstLocation.position) <
                    agent.stoppingDistance)
                {
                    Helpers.RotateTowardsObject(playerObject, this.transform, strength: strength);
                }
            }

            /*
             * Part C
             */
            
            /*
             * 
             * Beginning of the "decision tree" for sitting
             *
             * Are we moving towards the chair  (movingAway == false)
             * or away from it?                 (movingAway == true)
             */
            
            if (!movingAway)
            {
                // Use the HorizontalDistance Helper to check location compared near Seat
                if (Helpers.HorizontalDistance(gameObject.transform.position, nearSeatLocation.position) <
                    agent.stoppingDistance)
                {
                    // Set variables if previous check was true. This is the only location we set location2, so we
                    // can't get into any other if-statements if we haven't first entered this one 
                    location2 = true;
                    character.enabled = false;
                    agent.enabled = false;
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                }


                /*
                 * If we are at location2 (near set) and we haven't yet finished rotating, we start doing so
                 * Due to the speed of rotation and the speed of the animation, we trigger them simultaneously
                 */
                if (location2 && !finishedRotating)
                {
                    // Grab the animator component (see ReadMe / Animator screen) and play the state with the name "Stand to Sit"
                    // Note: A Animation event is put the end of this animation state so we know it is finished, this message is caught below and sets seated=true
                    // See below
                    animator.Play("Stand To Sit"); 
                    
                    // After starting to play, start rotating and wait for this to turn true.
                    // Offset is to match look-direction with seat-direction.
                    finishedRotating = Helpers.RotateTowardsObject(seatObject, this.transform, 160, strength);

                    // Try to move towards the position needed, more near the seat center
                    // TODO: make this work better. Feels crappy now
                    Vector3 targetPosition = seatObject.transform.position;
                    lerpStrength = Mathf.Min(strength * Time.deltaTime, 1);
                    transform.position = Vector3.Lerp(transform.position, targetPosition, lerpStrength / 2);
                }

                /*
                 * Check if we are finished rotating, are at location2 and are seated.
                 */
                if (finishedRotating && location2 && seated)
                {
                    // Check if we already have a seatJoint. This joint is used to make sure the avatar doesn't float off
                    // As we immediately set it, this is only done once 
                    if (seatJoint == null)
                    {
                        seatJoint = gameObject.AddComponent<FixedJoint>();
                        seatRigidbody = seatObject.gameObject.GetComponent<Rigidbody>();
                        seatRigidbody.isKinematic = false;
                        seatJoint.connectedBody = seatRigidbody;
                        seatJoint.breakForce = Mathf.Infinity;
                        seatJoint.breakTorque = Mathf.Infinity;
                    }
    
                    /*
                     * Check if we rotate enough to look at the participant. Also rotate the chair, but with an offset
                     * of 90 degrees (chair doesn't point towards user otherwise)
                     */
                    seatedLookingAtParticipant =
                        Helpers.RotateTowardsObject(playerObject, this.transform, strength: strength);
                    Helpers.RotateTowardsObject(playerObject, seatObject.transform, 90, strength);
                }

                /*
                 * Start the animation if we are seated and looking at participant. Only do this once.
                 * Animator object will handle the looping
                 */
                if (seatedLookingAtParticipant)
                {
                    if (firstTime)
                    {
                        seatRigidbody.isKinematic = true;
                        animator.Play("Sit Idle 2");
                        firstTime = false;
                    }
                }
            }

            
            /*
             * We are done and the researcher has pressed the KeyPad9 Button.
             */
            
            else //we are moving away from stuff from this point
            {
                /*
                 * Rotate away from participant towards the right.
                 */
                if (seated && !finishedRotating)
                {
                    Helpers.RotateTowardsObject(nearSeatLocation.transform, this.transform, strength: strength);
                    finishedRotating = Helpers.RotateTowardsObject(nearSeatLocation.transform, seatObject.transform,
                        offset: 90, strength);
                }

                /*
                 * We are still seated, but finished with rotation.
                 * Destroy the seatJoint as we don't need it anymore
                 */
                if (seated && finishedRotating && seatJoint != null)
                {
                    Destroy(seatJoint);
                }

                //play standing animation so we stand up
                if (seated && seatJoint == null && finishedRotating)
                {
                    animator.Play("Sit To Stand");
                    
                    /*
                     * Use an Animation event to set moving. This makes that we start slowly moving the avatar away when it
                     * starts looking less janky.
                     */
                    if (moving)
                    {
                        Vector3 targetPosition = new Vector3(-8.4119997f, 0.495999992f, 18.3659992f);
                        lerpStrength = Mathf.Min(strength * Time.deltaTime, 1);
                        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpStrength / 5);
                    }
                }

                //Get standing info, re-enable the thirdPersonController and agent
                if (!seated && location2 && finishedRotating)
                {
                    gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    agent.enabled = true;
                    character.enabled = true;
                    animator.Play("Locomotion");
                    //move away to shake hand for leaving
                    agent.SetDestination(firstLocation.position);
                }

                //last movement can be controlled by pressing pad0
            }
        }

        
        
        /*
         * Animation Message catchers. These are used to know when an animation has played its last frame and we can
         * continue on with the next part of the movement.
         */
        public void Seated(string message)
        {
            seated = true;
        }

        public void Standing(string message)
        {
            seated = false;
        }

        public void LerpMoving(string message)
        {
            moving = true;
        }


        /*
         * Basic input checker. Instead of doing it all over the place in  a big Update() method, we put it all in a
         * sub-method to keep it cleaner what variables are set by which key.
         */
        private void InputHandler()
        {
            /*
             * Each if is similar, except te last one
             * First, the agent's navigation destination is set to a specific location
             * Then we set the last-pressed to what we just pressed (some movement bugs were otherwise happening)
             * Finally, we re-set finishedRotating to false (tree above set it to true where needed) 
             */
            if (Input.GetKeyDown(KeyCode.Keypad0) && lastPressed != KeyCode.Keypad0)
            {
                agent.SetDestination(startLocation.position);
                lastPressed = KeyCode.Keypad0;
                finishedRotating = false;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1) && lastPressed != KeyCode.Keypad1)
            {
                agent.SetDestination(firstLocation.position);
                lastPressed = KeyCode.Keypad1;
                finishedRotating = false;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2) && lastPressed != KeyCode.Keypad2)
            {
                agent.SetDestination(nearSeatLocation.position);
                lastPressed = KeyCode.Keypad2;
                finishedRotating = false;
            }

            if (Input.GetKeyDown(KeyCode.Keypad9) && lastPressed != KeyCode.Keypad9)
            {
                movingAway = true;
                finishedRotating = false;
            }

            // Debug.Log(lastPressed);
        }
    }
}