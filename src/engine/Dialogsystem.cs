using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;


namespace leto
{
    class Dialogsystem
    {
	
        public static readonly int WAITING_ANSWER_STATE = -1;
        public static readonly int NORMAL_STATE = 0;
        Framedata currentReplica = null;
        Framedata nextReplica;
        // replica action handler here
        String sciene;

        List<Framedata> options = null;
        int ans = -1;

        public Dialogsystem(String name){
            currentReplica = nextReplica = parse(name);
        }

        public void next(){
            if(hasNext()) {
                currentReplica = nextReplica;

                if(currentReplica.nextSciene != null) {
                    currentReplica.answers.Add(parse(currentReplica.nextSciene));
                    sciene = currentReplica.nextSciene ;
                }

                if(currentReplica.answers.Count == 0) nextReplica = null;
                else if(currentReplica.answers.Count == 1) nextReplica = currentReplica.answers[0];
                else {
                    if(nextReplicaIsMine()){
                        options = currentReplica.answers;
                        if(ans == WAITING_ANSWER_STATE) return;
                        nextReplica = currentReplica.answers[ans];
                        ans = WAITING_ANSWER_STATE;
                        options = null;
                        next();
                    } else { 
                        // ai selecting it's move with replica action handler
                    }
                }
            }
        }
        public bool hasNext() {
            return nextReplica != null;
        }	
        	
        public Framedata getCurrentReplica() {
            return currentReplica;
        }
        
        public List<Framedata> getCurrentOptionsList() {
            return options;
        }
        
        public void setAnswer(Framedata r) {
            ans = options.IndexOf(r);
        }
        
        public void setAnswer(int i) {
            ans = i;
           // Console.WriteLine($"you selected answer {ans}");
        }
        public int getState() {
            if(options != null && nextReplicaIsMine() && ans == -1) return WAITING_ANSWER_STATE;
            else return NORMAL_STATE;
        }
        public bool nextReplicaIsMine() {
            if(currentReplica.answers.Count != 0) return currentReplica.answers[0].owner == ReplicaOwner.Protagonist;
            else return false;
	    }
        public String getCurrentSciene(){ //геттер можно переписать на свойство, но уже на рефакторе
            return sciene;
        }
        private Framedata parse(String name){
            return Frameloader.load(name);
        }
    }
}