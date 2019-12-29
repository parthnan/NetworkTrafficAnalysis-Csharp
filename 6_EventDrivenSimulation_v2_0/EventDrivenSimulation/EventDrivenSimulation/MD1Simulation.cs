using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenSimulation
{
    public class MD1Simulation
    {
        #region  乱数 (サイコロ）関連
        private Random rnd;
        private double ExponentialDistribution(double lambda)
        {
            return -Math.Log(rnd.NextDouble()) * 1.0 / lambda;
        }
        #endregion

        #region variables: 変数の宣言
        public List<MyEvent> elist;//イベントリスト
        public List<double> waitTime = new List<double>();        //客がどの程度の待ち時間行列から抜けた客のリスト
        public double simtime = 0.0;
        public double prevsimtime = 0.0;
        #endregion

        #region parameter: 定数の宣言
        double D = 1.0;
        double lambda;
        int numCustomers;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrivalrate">到着率</param>
        /// <param name="seed">乱数のseed。default 0。</param>
        /// <param name="numcustomer">客数。default1000</param>        
        //コンストラクター。
        //オブジェクトを作る時に呼び出される関数。
        public MD1Simulation(double arrivalrate, int seed=0, int n=1000)
        {
            #region パラメータ設定、不正パラメータチェック
            lambda = arrivalrate;
            numCustomers = n;
            if (lambda < 0.0 || lambda > 1.0) throw new System.Exception("負荷は1.0未満で");
            rnd = new Random(seed);
            #endregion

            #region 変数の初期化
            elist = new List<MyEvent>();
            waitTime = new List<double>();
            simtime = 0.0; prevsimtime = 0.0;
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
                    double wtime = queue_length + D; //離脱のイベントを追加
                    this.elist.Add(new MyEvent(this.simtime + wtime, 0, current.who, "DEPARTURE"));
                    this.waitTime.Add(wtime);
                    queue_length += D;
                    #endregion
                }
                else if (current.action.Equals("DEPARTURE"))
                {
                    //何もしない
                }
                #endregion
            }
        }

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
        /// <returns>平均待ち時間</returns>
        public double get_average_waitingTime()
        {
            #region 平均待ち時間を計算
            return this.waitTime.Average();
            #endregion
        }
    }
}
