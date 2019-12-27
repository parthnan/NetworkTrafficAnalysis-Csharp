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
