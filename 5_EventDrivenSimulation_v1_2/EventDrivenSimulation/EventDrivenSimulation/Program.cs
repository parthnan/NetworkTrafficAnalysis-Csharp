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
            //mm1k_blocking_vs_lambda(); // スライドP.24
            //mm1k_blocking_vs_numcustomer(); // スライドP.25
            
            // スライドP.25

            #region パラメータの設定
            var lambda_list = new List<double>();
            for (double i = 0.01; i < 1.0; i += 0.01)
                lambda_list.Add(i);
            #endregion

            #region シミュレーションの走らせ方 完成形
            int numCustomer = 1000;
            var result = new List<Tuple<double, double>>();
            Parallel.ForEach(lambda_list, lambda =>
            {
                MM1KSimulation a = new MM1KSimulation(lambda, 1, 1, 0, numCustomer);
                a.run();
                Console.WriteLine("finished {0}", lambda);
                result.Add(new Tuple<double, double>(lambda, a.get_result()));
            });
            System.IO.StreamWriter sw = new System.IO.StreamWriter("result.csv");
            result.Sort();
            result.ForEach(j => sw.WriteLine("{0},{1}", j.Item1, j.Item2));
            sw.Close();
            #endregion

        }

        #region mm1k_blocking_vs_numcustomer
        private static void mm1k_blocking_vs_numcustomer()
        {
            #region パラメータの設定
            double lambda = 0.9;
            var numCustomerList = new List<int>();
            for (int i = 10; i < 10e+04; i += 10)
                numCustomerList.Add(i);
            #endregion

            #region シミュレーションの走らせ方 完成形
            var result = new List<Tuple<double, double>>();
            Parallel.ForEach(numCustomerList, numCustomer =>
            {
                MM1KSimulation a = new MM1KSimulation(lambda, 1, 1, 0, numCustomer);
                a.run();
                Console.WriteLine("{0},{1}", numCustomer, a.get_result());
                result.Add(new Tuple<double, double>(numCustomer, a.get_result()));
            });
            result.Sort();
            result.ForEach(j => Console.WriteLine("{0},{1}", j.Item1, j.Item2));
            #endregion
        }
        #endregion

        #region mm1k_blocking_vs_lambda customer=1000;

        private static void mm1k_blocking_vs_lambda(int numCustomer=10000)
        {
            #region パラメータの設定
            var lambda_list = new List<double>();
            for (double i = 0.01; i < 1.0; i += 0.01)
                lambda_list.Add(i);
            #endregion

            #region シミュレーションの走らせ方 完成形
            var result = new List<Tuple<double, double>>();
            Parallel.ForEach(lambda_list, lambda =>
            {
                MM1KSimulation a = new MM1KSimulation(lambda);
                a.run();
                result.Add(new Tuple<double, double>(lambda, a.get_result()));
            });
            result.Sort();
            result.ForEach(j => Console.WriteLine("{0},{1}", j.Item1, j.Item2));
            #endregion
        }
        #endregion

        private static void md1test()
        {

            #region パラメータの設定
            var lambda_list = new List<double>();
            for (double i = 0.01; i < 1.0; i += 0.01)
                lambda_list.Add(i);
            #endregion

            if (true)
            {
                #region シミュレーションの走らせ方 完成形
                {
                    var result = new List<Tuple<double, double>>();
                    Parallel.ForEach(lambda_list, lambda =>
                    {
                        MD1Simulation a = new MD1Simulation(lambda);
                        a.run();
                        result.Add(new Tuple<double, double>(lambda,
                            a.get_average_waitingTime()));
                    });
                    result.Sort();//Tupleの1個目の昇順に並べ替える。MSの仕様
                    //result.ForEach(Console.WriteLine); //括弧付きで表示される;
                    result.ForEach(j => Console.WriteLine("{0},{1}", j.Item1, j.Item2));
                }
                #endregion
            }

            if (false)
            {
                #region シミュレーションの走らせ方 その3 お金持ちバージョン
                Parallel.ForEach(lambda_list, lambda =>
                {
                    MD1Simulation a = new MD1Simulation(lambda);
                    a.run();
                    Console.WriteLine("{0},{1}", lambda,
                        a.get_average_waitingTime());
                });
                #endregion
            }

            if (false)
            {
                #region シミュレーションの走らせ方 その2
                foreach (var lambda in lambda_list)
                {
                    MD1Simulation a = new MD1Simulation(lambda);
                    a.run();
                    Console.WriteLine("{0},{1}", lambda,
                        a.get_average_waitingTime());
                }
                #endregion
            }

        }
    }
}
