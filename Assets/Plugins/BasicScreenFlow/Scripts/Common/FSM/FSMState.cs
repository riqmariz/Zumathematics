﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.FSM
{
    public class FSMState
    {
        private string name;
        public string Name => name;
        private List<FSMAction> actions;
        private FSM owner;
        private Dictionary<String,FSMState> transitionMap;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Common.FSM.FSMState"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="owner">Owner.</param>
        public FSMState (string name, FSM owner)
        {
            this.name = name;
            this.owner = owner;
            this.transitionMap = new Dictionary<string, FSMState> ();
            this.actions = new List<FSMAction> ();
        }
 
        /// <summary>
        /// Adds the transition.
        /// </summary>
        public void AddTransition (string id, FSMState destinationState)
        {
            if (transitionMap.ContainsKey (id)) {
                Debug.LogError (string.Format ("state {0} already contains transition for {1}", this.name, id));
                return;
            }
 
            transitionMap [id] = destinationState;
        }
 
        /// <summary>
        /// Gets the transition.
        /// </summary>
        public FSMState GetTransition (string eventId)
        {
            if (transitionMap.ContainsKey (eventId)) {
                return transitionMap [eventId];
            }
 
            return null;
        }
 
        /// <summary>
        /// Adds the action.
        /// </summary>
        public void AddAction (FSMAction action)
        {
            if (actions.Contains (action)) {
                Debug.LogWarning ("This state already contains " + action);
                return;
            }
 
            if (action.GetOwner () != this) {
                Debug.LogWarning ("This state doesn't own " + action);
            }
 
            actions.Add (action);
        }
 
        /// <summary>
        /// This gets the actions of this state
        /// </summary>
        /// <returns>The actions.</returns>
        public IEnumerable<FSMAction> GetActions ()
        {
            return actions;
        }
 
        /// <summary>
        /// Sends the event.
        /// </summary>
        public void SendEvent (string eventId, string owner = "")
        {
            this.owner.SendEvent (eventId, owner);
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}
