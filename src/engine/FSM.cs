using System.Collections.Generic;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;

using System.Numerics;

namespace leto
{
    public class FSM 
    {
        private Dictionary<int, GameState> states = new Dictionary<int, GameState>();

        private GameState currentState;

        public void SetCurrentState(int id) 
        {
            if(states.ContainsKey(id)) 
            {
                if(currentState != null) currentState.Leave();
                currentState = states[id];
                currentState.Enter();
            } 
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"GameState {id} is missing!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        
        public void AddState(int id, GameState state) 
        {
            if(states.ContainsKey(id)) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"GameState {id} is already added, skipping.");
                Console.ForegroundColor = ConsoleColor.White;
            }else {
                state.Init();
                states.Add(id, state);
            } 
        }

        public void RemoveState(int id) 
        {
            if(states.ContainsKey(id)) 
            {
                if(states[id] == currentState) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Cannot remove state, state {id} is currently running!");
                    Console.ForegroundColor = ConsoleColor.White;
                } else 
                {
                    // системный Dispose не срабатывает, я не могу полагаться на среду исполнения, сам буду всё чистить
                    states[id].Dispose();
                    states.Remove(id);
                }
            } 
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Cannot remove state, no state with id={id}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Render()
        {
            if(currentState != null)currentState.Render();
        }

        public void Update()
        {
            if(currentState != null) currentState.Update(this);
        }

        public GameState GetCurrentState()
        {
            return currentState;
        }

        public void Clear()
        {
            foreach(var temp_id in states.Keys) RemoveState(temp_id);
        }

        public List<GameState> GetAllStates()
        {
            List<GameState> statesOnly = new List<GameState>();
            statesOnly.AddRange(states.Values);
            return statesOnly;
        }

        public bool hasState(int key) {
            return states.ContainsKey(key);
        }
        public GameState GetState(int id){
            return hasState(id) ? states[id] : null;
        }
    }

    public abstract class GameState: IDisposable
    {
        public abstract void Init();
        public abstract void Render(); 
        public abstract void Update(FSM fsm);
        public virtual void Enter(){} 
        public virtual void Leave(){}

        public abstract void Dispose();
    }

}