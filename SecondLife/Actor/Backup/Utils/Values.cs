using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using log4net;
using log4net.Config;

namespace DED.DPGE
{
    class Values
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Values));
        public void SetValues(Node n, Bayes net)
        {
            XmlConfigurator.Configure();
            switch (n.ValueFlag)
            {
                case "":
                    break;
                case "twoColumns":
                case "twoRows":
                    GetValuesTwoRows(n);
                    break;
                case "normalized":
                    GetValuesNormalized(n);
                    break;                 
                case "match":      
                    GetValuesMatched(n,net);
                    break;
                case "first":
                    GetValuesFirst(n, net);
                    break;
                case "allFalseMatch":
                    GetValuesAllFalseMatch(n, net);
                    break;

                default:                        
                    break;
            }   
        }

        private void GetValuesAllFalseMatch(Node n, Bayes net)
        {
            int tablesize = net.TableSize(n);
            
            for (int i = 0; i < tablesize-2; ++i)
            {
                if (i % 2 == 0) n.Values.Add(1);
                else n.Values.Add(0);
            }
            n.Values.Add(0);
            n.Values.Add(1);
        }

        private void GetValuesFirst(Node n, Bayes net)
        {
            n.Values.Add(1);
            n.Values.Add(0);
            for (int i = 2; i < net.TableSize(n); ++i)
            {
                if ( i%2 == 0) n.Values.Add(0);
                else n.Values.Add(1);
            }
        }

        private void GetValuesTwoRows(Node n)
        {
            List<double> l = new List<double>();
            foreach (double v in n.Values)
            {
                l.Add(v);
                l.Add(1 - v);
            }
            n.Values = l;
        }
        

        private void GetValuesNormalized(Node n)
        {
            Bayes b = new Bayes();
            b.Normalize(n);
        }


        private void Match( int count, Node n, Bayes net, List<string> parents ){
            if ( count == 0 ){
                foreach ( string state in n.States ) {                    
                    if (parents[0] == parents[1] && state == "Match" ) n.Values.Add(1);
                    else if ((parents[0] == "Null" || parents[1] == "Null") && state == "Null") n.Values.Add(1);
                    else if (parents.Count == 3 && parents[2] == "False" && state == "Null") n.Values.Add(1);
                    else if (parents[0] != parents[1] && state == "NoMatch" && !(parents[0] == "Null" || parents[1] == "Null")) n.Values.Add(1);
                    else n.Values.Add(0);
                }
            }
            else {
                int idx = n.Parents.Count-count;
                --count;
                //get the states of parents
                List<string> parentsStates = new List<string>(net.GetOutcomeIds(n.Parents[idx].Split('$')[0]));
                foreach (string each in parentsStates)
                {
                    parents[idx] = each;
                    Match( count, n, net, parents );
                }
            }
        }
                
        private void GetValuesMatched( Node n, Bayes net ){
            
            Match(n.Parents.Count, n, net,new List<string>( n.Parents ));
        }

        private void EvaluateNEG(int count, Node n, Bayes net, List<string> parents)
        {
            if (count == 0)
            {
                foreach (string state in n.States)
                {
                    if ( parents[0] == parents[1] && state == Constants.FALSE ) n.Values.Add(1);
                    //else if (parents[0] != parents[1] && state == Constants.TRUE ) n.Values.Add(1); 
                    else if ( parents[0] != parents[1] && state == Constants.TRUE ) n.Values.Add(1); 
                    else n.Values.Add(0);
                }
            }
            else
            {
                int idx = n.Parents.Count - count;
                --count;
                //get the states of parents
                List<string> parentsStates = new List<string>(net.GetOutcomeIds(n.Parents[idx].Split('$')[0]));
                foreach (string each in parentsStates)
                {
                    parents[idx] = each;
                    EvaluateNEG(count, n, net, parents);
                }
            }
        }

        public void SetEvalIntegrity( Node n )
        {
            n.Values = new List<double>();
            for (int i = 0; i < 8; i++)
            {
                n.Values.Add(0);
                n.Values.Add(1);
            }
            n.Values[4] = 1;
            n.Values[5] = 0;
            n.Values[8] = 1;
            n.Values[9] = 0;
        }

        public void SetEvaluation(Node n)
        {
            n.Values = new List<double>();
            
            n.Values.Add(0);
            n.Values.Add(1);
            n.Values.Add(1);
            n.Values.Add(0);
            n.Values.Add(1);
            n.Values.Add(0);
            n.Values.Add(0);
            n.Values.Add(1);
        }

        public void SetEvaluateNegate(Node n, Bayes net)
        {
            EvaluateNEG(n.Parents.Count, n, net, new List<string>(n.Parents));
        }

        private void InitInference( int count, Node n, Bayes net, List<string> parents )
        {
            if ( count == 0 )
            {
                foreach ( string state in n.States )
                {
                    if ( parents[0] == state ) n.Values.Add(1);                    
                    else n.Values.Add(0);
                }
            }
            else
            {
                int idx = n.Parents.Count - count;
                --count;
                //get the states of parents
                List<string> parentsStates = new List<string>( net.GetOutcomeIds(n.Parents[idx].Split('$')[0]) );
                foreach ( string each in parentsStates )
                {
                    parents[idx] = each;
                    InitInference(count, n, net, parents);
                }
            }
        }

        public void InitInference(Node n, Bayes net)
        {
            InitInference(n.Parents.Count, n, net, new List<string>(n.Parents));
        }
    }
}
