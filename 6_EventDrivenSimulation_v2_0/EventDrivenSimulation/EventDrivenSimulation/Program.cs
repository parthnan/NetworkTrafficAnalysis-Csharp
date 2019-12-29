using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace EventDrivenSimulation
{

    class Program
    {
        static void Main(string[] args)
        {
            int[] numserver = { 1};            
            foreach (var n in numserver)
            {
                int numServer = n;
                var lambda_list = new List<double>();
                for (double i = 0.1; i < 1.0 * numServer; i += 0.1)
                    lambda_list.Add(i);
				//lambda_list.Add(1.0);
                double[] priority_ratio = { 0.6 };
				double ratio2=0.5;

                #region シミュレーション結果の取得。stringバージョン
                foreach (var ratio in priority_ratio)
                {                    
                    int numCustomer = 1000;
                    var result = new List<Tuple<double, string>>();
					var result2 = new List<Tuple<double, string>>();
                    Parallel.ForEach(lambda_list, lambda =>
                    {
                        MD1withPriority a = new MD1withPriority(ratio2*lambda,ratio*lambda,0,numCustomer); 
						//MM1SimulationLIFO a = new MM1SimulationLIFO(lambda,10,numCustomer);
                        a.run();
                        result.Add(new Tuple<double, string>(lambda, a.get_result_string()));
						//MM1Simulation a2 = new MM1Simulation(lambda,10,numCustomer);
						//a2.run();
                        //result2.Add(new Tuple<double, string>(lambda, a2.get_result_string()));
                    });

                    //System.IO.StreamWriter sw = new System.IO.StreamWriter("result_S" + numServer + ".csv");
					System.IO.StreamWriter sw = new System.IO.StreamWriter("result_S" + numServer + "_YUUSEN.csv");
					//result.ForEach(j => sw.WriteLine("{0},{1},{2}", numServer, j.Item1, j.Item2));
					result.Sort();result.ForEach(j => sw.WriteLine("{0},{1},{2}", ratio2*j.Item1,ratio*j.Item1, j.Item2));
                    sw.Close();
					//System.IO.StreamWriter sw2 = new System.IO.StreamWriter("result_S" + numServer + "_FIFO.csv");
					//result2.Sort();result2.ForEach(j2 => sw2.WriteLine("{0},{1},{2}", numServer, j2.Item1, j2.Item2));
                    //sw2.Close();
                }
                #endregion
            }

        }

        /*#region シミュレーション結果の取得。HashTableバージョン
        private void show_result_via_hash()
        {
            int numServer = 2;
            var lambda_list = new List<double>();
            for (double i = 0.1; i < 1.0 * numServer; i += 0.1)
                lambda_list.Add(i);

            int numCustomer = 1000;
            var result = new List<Tuple<double, Hashtable>>();
            Parallel.ForEach(lambda_list, lambda =>
            {
                MMKKSimulation a = new MMKKSimulation(lambda, numServer, numServer, 0, numCustomer);
                a.run();
                result.Add(new Tuple<double, Hashtable>(lambda, a.get_result()));
            });

            System.IO.StreamWriter sw = new System.IO.StreamWriter("result.csv");
            result.Sort();
            result.ForEach(j => sw.WriteLine("{0},{1}",
                j.Item1,
                ((double)j.Item2["blocking"]),
                ((double)j.Item2["utilization_average"])));
            sw.Close();

            #endregion

        }*/
    }
}
