using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DED.NPC;

namespace DED.UnitTests
{
    class ActorTest
    {
        Actor actor;
        public ActorTest()
        {
        }

        public void testRespondToQuestion(){
            

        }

        void askActor(string id)
        {
            
            //Variable v = new Variable(a.Variable, a.Subnet, a.State);
            //DEDAction action = new DEDAction(a.ID.ToString(), a.AddressedTo, a.TalkAbout, a.Sender, a.Variable, v, a.IsQuestion, a.GoalName);
            //this.conversations.addSpeech(action, a.Sender, new Variable(a.Variable, a.Subnet, -1), SentenceType.Question);
            //isNew = true;
        }

        void WriteToFile(string txt){
            // create a writer and open the file
            TextWriter tw = new StreamWriter(string.Format("characters/{0}/respond to question.txt",this.actor.Name));

            // write a line of text to the file
            tw.Write(txt);

            // close the stream
            tw.Close();
        }
    }
}
