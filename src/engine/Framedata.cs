using Raylib_cs;
using System.Collections.Generic;

namespace leto
{

    public enum ReplicaOwner {
        Person1, Person2, Protagonist
    }

    class Framedata
    {
        int id;
        public Music music;             
        public Sound sound;      
        public Texture2D background;
        public string text;
        public Texture2D person1;
        public Texture2D person2;     
	    public List<Framedata> answers = new List<Framedata>();
        public ReplicaOwner owner;
        public string nextSciene;

        public override string ToString()
        {   
            string answersString = "\n##answers##";
            foreach(var ans in answers) answersString += $"\n+ {ans.text}";
            answersString+="\n";
            return "background: "+ background
                        + "\ntext: " + text.ToString() 
                        + "\nperson 1: " + person1.ToString()
                        + "\nperson 2: " + person2
                        + "\nmusic: " + music
                        + "\nsound: " + sound
                        + answersString;
        }
    }
}
