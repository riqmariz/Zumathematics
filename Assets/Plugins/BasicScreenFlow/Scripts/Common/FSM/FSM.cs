﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


namespace Common.FSM
{
    ///<summary>
    ///This is the main engine of our FSM, without this, you won't be
    ///able to use FSM States and FSM Actions.
    ///</summary>
    public class FSM
    {
        private readonly string name;
        protected FSMState currentState;
        private readonly Dictionary<string, FSMState> stateMap;
        
        public string Name {
            get {
                return name;
            }
        }

        ///<summary>
        /// This is the constructor that will initialize the FSM and give it
        /// a unique name or id.
        ///</summary>
        public FSM (string name) {
            this.name = name;
            this.currentState = null;
            stateMap = new Dictionary<string, FSMState> ();
        }

        ///<summary>
        /// This initializes the FSM. We can indicate the starting State of
        /// the Object that has an FSM.
        ///</summary>
        public void Start (string stateName)
        {
            if (!stateMap.ContainsKey (stateName)) {
                Debug.LogWarning ("The FSM doesn't contain: " + stateName);
                return;
            }
 
            ChangeToState (stateMap [stateName]);
        }

        
        public virtual FSMState AddState (string name)
        {
            if (stateMap.ContainsKey (name)) {
                Debug.LogWarning ("The FSM already contains: " + name);
                return null;
            }
 
            FSMState newState = new FSMState (name, this);
            stateMap [name] = newState;
            return newState;
        }

        ///<summary>
        /// This changes the state of the Object. This also calls the exit
        /// state before doing the next state.
        ///</summary>
        public void ChangeToState (FSMState state)
        {
            if (this.currentState != null) {
                ExitState (this.currentState);
            }
 
            this.currentState = state;
            EnterState (this.currentState);
        }

        ///<summary>
        /// This changes the state of the Object. It is not advisable to
        /// call this to change state.
        ///</summary>
        public void EnterState (FSMState state)
        {
            ProcessStateAction (state, delegate(FSMAction action) {
                action.OnEnter ();	
            });
        }

        private void ExitState (FSMState state)
        {
            FSMState currentStateOnInvoke = this.currentState;
 
            ProcessStateAction (state, delegate(FSMAction action) {
 
                if (this.currentState != currentStateOnInvoke)
                    Debug.LogError ("State cannont be changed on exit of the specified state");
 
                action.OnExit ();	
            });
        }

        ///<summary>
        /// Call this under a MonoBehaviour's Update.
        ///</summary>
        public virtual void Update ()
        {
            if (this.currentState == null)
                return;
 
            ProcessStateAction (this.currentState, delegate(FSMAction action) {
                action.OnUpdate ();	
            });
        }

        public virtual void FixedUpdate()
        {
            if (this.currentState == null)
                return;
            ProcessStateAction(this.currentState, delegate(FSMAction action)
            {
                action.OnFixedUpdate();
            });
        }


        ///<summary>
        /// This handles the events that is bound to a state and changes
        /// the state.
        ///</summary>
        public void SendEvent (string eventId, string owner = "")
        {
            FSMState transitonState = ResolveTransition (eventId);
 
            if (transitonState == null)
                Debug.LogWarning ("The current state "+owner+" has no transition for event " + eventId);
            else
                ChangeToState (transitonState);
	
        }
        
        /// <summary>
        /// This gets the next state from the current state.
        /// </summary>
        /// <returns>The transition.</returns>
        /// <param name="eventId">Event identifier.</param>
        private FSMState ResolveTransition (string eventId)
        {
            FSMState transitionState = this.currentState.GetTransition (eventId);
 
            if (transitionState == null)
                return null;
            else
                return transitionState;
        }
        
        protected delegate void StateActionProcessor (FSMAction action);

        /// <summary>
        /// This gets all the actions that is inside the state and loop them.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="actionProcessor">Action processor.</param>
        protected void ProcessStateAction (FSMState state, StateActionProcessor actionProcessor)
        {
            FSMState currentStateOnInvoke = this.currentState;
            IEnumerable<FSMAction> actions = state.GetActions ();

            foreach (FSMAction action in actions) {
                if (this.currentState != currentStateOnInvoke) {
                    break;
                }

                actionProcessor (action);
            }
        }

        public FSMState GetCurrentState()
        {
            return currentState;
        }
        
    }
}
