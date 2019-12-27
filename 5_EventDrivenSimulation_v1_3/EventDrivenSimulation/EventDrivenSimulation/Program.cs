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
            int[] numserver = { 5, 10, 20 };

            #region パラメータの設定
            foreach (var n in numserver)
            {
                int numServer = n;
                var lambda_list = new List<double>();
                for (double i = 0.1; i < 1.0 * numServer; i += 0.1)
                    lambda_list.Add(i);
            #endregion

                #region シミュレーション結果の取得。stringバージョン
                {
                    int numCustomer = 1000;
                    var result = new List<Tuple<double, string>>();
                    Parallel.ForEach(lambda_list, lambda =>
                    {
                        MMKKSimulation a = new MMKKSimulation(lambda, numServer, numServer, 0, numCustomer);
                        a.run();
                        result.Add(new Tuple<double, string>(lambda, a.get_result_string()));
                    });

                    System.IO.StreamWriter sw = new System.IO.StreamWriter("result_S" + numServer + ".csv");
                    result.Sort();
                    result.ForEach(j => sw.WriteLine("{0},{1},{2}", numServer, j.Item1, j.Item2));
                    sw.Close();
                }
                #endregion
            }

        }

        #region シミュレーション結果の取得。HashTableバージョン
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

        }
    }
}
