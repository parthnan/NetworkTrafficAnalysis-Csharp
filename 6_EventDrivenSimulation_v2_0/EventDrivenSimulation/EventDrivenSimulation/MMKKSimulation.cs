using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace EventDrivenSimulation
{
    class MMKKSimulation
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
        public int num_succ = 0;
        public int num_fail = 0;
        public List<int> AvailableServer;
        public List<double> ServerUtilization;
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
     
        public MMKKSimulation(double arrivalrate, int tmpK = 1, 
                              int tmpnumServer=1, int seed = 0, int n = 1000)
        {
            #region パラメータ設定、不正パラメータチェック
            lambda = arrivalrate;
            numCustomers = n;
            numServer = tmpnumServer;
            K = tmpK;
            if (tmpK != tmpnumServer) throw new System.Exception("unexpected: S should be K");
            rnd = new Random(seed);
            #endregion
            #region 変数の初期化
            AvailableServer = new List<int>();
            ServerUtilization = new List<double>();
            for (int i = 0; i < tmpnumServer; i++)
            {
                AvailableServer.Add(i);
                ServerUtilization.Add(0.0);
            }
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
            int customer_wait = 0;
            this.simtime = 0.0;
            while (true)
            {
                #region 最も近いイベントを発掘
                MyEvent current = Pop(this.elist);
                if (current == null) break;
                #endregion

                #region シミュレーション時間の更新。経過時間分の処理（キュー長を減らす）←即時系なのでこれは省略
                {
                    this.simtime = current.time;
                }
                #endregion
                #region イベント処理
                if (current.action.Equals("ARRIVE"))
                {
                    #region イベントARRIVEの処理
                    if (AvailableServer.Count > 0)
                    {
                        num_succ++;
                        double servicetime = this.ExponentialDistribution(1.0); //平均1の指数分布
                        int serverid = AvailableServer[0];
                        AvailableServer.RemoveAt(0);
                        ServerUtilization[serverid] += servicetime;
                        this.elist.Add(new MyEvent(this.simtime + servicetime, serverid, current.who, "DEPARTURE"));//離脱のイベントを追加
                        customer_wait++;
                    }
                    else
                    {
                        //サーバが一杯の場合
                        if (customer_wait < this.K)
                        {
                            throw new System.Exception("即時系では起こりえない");
                        }
                        else
                        {
                            num_fail++;
                        }
                    }
                    #endregion
                }
                else if (current.action.Equals("DEPARTURE"))
                {
                    int thiserverid = current.where;
                    AvailableServer.Add(thiserverid); //空いているサーバーリストのおしりに足す。
                    customer_wait--;
                }
                else
                {
                    //何もしない
                }
                #endregion
            }
        }

        public string get_result_string()
        {
            string result = string.Empty;
            result += (double)num_fail / (num_fail + num_succ) + ",";
            result += this.ServerUtilization.Average() / this.simtime + ",";
            result += this.ServerUtilization.Max() / this.simtime + ",";
            result += this.ServerUtilization.Min() / this.simtime;
            return result;
        }
        public Hashtable get_result()
        {
            Hashtable result = new Hashtable();
            #region 評価指標を計算
            result["blocking"] =  (double)num_fail / (num_fail + num_succ);
            result["utilization_average"] = this.ServerUtilization.Average() / this.simtime;
            result["utilization_highest"] = this.ServerUtilization.Max() / this.simtime;
            result["utilization_lowest"] = this.ServerUtilization.Min() / this.simtime;
            return result;
            #endregion
        }
    }
}
