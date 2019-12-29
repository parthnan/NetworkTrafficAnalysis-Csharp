using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenSimulation
{
    public class MM1KSimulation
    {
        #region  乱数 (サイコロ）関連
        private Random rnd;
        private double ExponentialDistribution(double lambda)
        {
            return -Math.Log(rnd.NextDouble()) * 1.0 / lambda;
        }
        #endregion

        #region variables: 変数の宣言
        public List<MyEvent> elist = new List<MyEvent>();//イベントリスト
        public double simtime = 0.0;
        public double prevsimtime = 0.0;
        public int num_succ = 0;
        public int num_fail = 0;
        #endregion

        #region parameter: 定数の宣言
        double lambda;
        int numServer;
        int K;
        int numCustomers;
        #endregion


        #region 最も近いイベントを返す関数 Popの定義
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">イベントリスト</param>
        /// <returns>直近のイベントを返す</returns>
        private static MyEvent Pop(List<MyEvent> list)
        {
            if (list.Count == 0) return null;

            list.Sort((x, y) =>
            {
                if (x.time > y.time) { return 1; }
                else if (x.time < y.time) { return -1; }
                else { return 0; };
            });
            MyEvent ret_value = new MyEvent(list[0].time, list[0].where, list[0].who, list[0].action);
            list.RemoveAt(0);
            return ret_value;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrivalrate">到着率</param>
        /// <param name="seed">乱数のseed。default 0。</param>
        /// <param name="numcustomer">客数。default1000</param>        
        public MM1KSimulation(double arrivalrate, int tmpK=1, 
                              int tmpnumServer=1, int seed = 0, int n = 1000)
        {
            #region パラメータ設定、不正パラメータチェック
            lambda = arrivalrate;
            numCustomers = n;
            numServer = tmpnumServer;
            K = tmpK;
            if (lambda < 0.0 || lambda > 1.0) throw new System.Exception("負荷は1.0未満で");
            rnd = new Random(seed);
            #endregion
            #region 変数の初期化
            //宣言のところに書いた
            #endregion
            #region 前処理：客到着に関するのイベント追加
            {
                double t = 0.0;
                for (int i = 0; i < numCustomers; i++)
                {
                    t += ExponentialDistribution(lambda);
                    elist.Add(new MyEvent(t, 0, new Person(i), "ARRIVE"));
                }
            }
            #endregion
        }

        public void run()
        {
            double queue_length = 0.0;
            int customer_wait = 0;
            this.simtime = 0.0;
            while (true)
            {
                #region 最も近いイベントを発掘
                MyEvent current = Pop(this.elist);
                if (current == null) break;
                #endregion
                #region シミュレーション時間の更新。経過時間分の処理（キュー長を減らす）
                {
                    this.prevsimtime = this.simtime;
                    this.simtime = current.time;
                    queue_length -= this.simtime - this.prevsimtime;
                    if (queue_length < 0.0) queue_length = 0.0;
                }
                #endregion
                #region イベント処理
                if (current.action.Equals("ARRIVE"))
                {
                    #region イベントARRIVEの処理
                    if (customer_wait < this.K)
                    {
                        num_succ++;
                        double servicetime = this.ExponentialDistribution(1.0); //平均1の指数分布
                        double wtime = queue_length + servicetime;
                        this.elist.Add(new MyEvent(this.simtime + wtime, 0, current.who, "DEPARTURE"));//離脱のイベントを追加
                        queue_length += servicetime;
                        customer_wait++;
                    }
                    else
                    {
                        num_fail++;
                    }
                    #endregion
                }
                else if (current.action.Equals("DEPARTURE"))
                {
                    customer_wait--;
                }
                else
                {
                    //何もしない
   
                }
                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>平均待ち時間</returns>
        public double get_result()
        {
            #region 棄却率を計算
            return (double)num_fail / (num_fail + num_succ);
            #endregion
        }
    }
     

}
