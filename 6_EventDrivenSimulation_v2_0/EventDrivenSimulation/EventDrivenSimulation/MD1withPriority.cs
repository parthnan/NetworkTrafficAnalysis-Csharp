using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenSimulation
{
    public class MD1withPriority
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
        public List<double> waitTime_priority = new List<double>();        //客がどの程度の待ち時間行列から抜けた客のリスト

        public List<Person> real_queue = new List<Person>();
        public List<Person> real_priority_queue = new List<Person>();

        public double simtime = 0.0;
        public double prevsimtime = 0.0;
        #endregion

        #region parameter: 定数の宣言
        double D = 1.0;
        double lambda;
        double lambda2;
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
        public MD1withPriority(double arrivalrate,double arrivalrate_prior, int seed = 0, int n = 1000)
        {
            #region パラメータ設定、不正パラメータチェック
            lambda = arrivalrate;
            lambda2 = arrivalrate_prior;
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
            {//通常のお客
                double t = 0.0;
                for (int i = 0; i < numCustomers; i++)
                {
                    t += ExponentialDistribution(lambda);
                    elist.Add(new MyEvent(t, 0, new Person(i,t), "ARRIVE"));
                }
            }
            {
                double t = 0.0;
                for (int i = 0; i < numCustomers; i++)
                {
                    t += ExponentialDistribution(lambda2);
                    elist.Add(new MyEvent(t, 0, new Person(i,t, true), "ARRIVE"));
                }

            }
            elist.Sort((x, y) =>
            {
                if (x.time > y.time) { return 1; }
                else if (x.time < y.time) { return -1; }
                else { return 0; };
            });

            #endregion
        }

        public void run()
        {
            bool ServerInProcess=false;
            double queue_length = 0.0;
            this.simtime = 0.0;
            while (true)
            {
                #region 最も近いイベントを発掘
                MyEvent current = Pop(this.elist);
                if (current == null) break;
                #endregion

                #region シミュレーション時間の更新。経過時間は処理しない
                {
                    this.simtime = current.time;
                }
                #endregion

                #region イベント処理
                if (current.action.Equals("ARRIVE"))
                {
                    #region イベントARRIVEの処理
                    if (current.who.isvip)
                    {
                        real_priority_queue.Add(current.who);
                        if (!ServerInProcess)
                            this.elist.Add(new MyEvent(this.simtime, 0, current.who, "BEGINPROCESS"));
                    }
                    else
                    {
                        real_queue.Add(current.who);
                        if (!ServerInProcess)
                            this.elist.Add(new MyEvent(this.simtime, 0, current.who, "BEGINPROCESS"));
                    }
                    #endregion
                }
                else if (current.action.Equals("BEGINPROCESS"))
                {
                    ServerInProcess = true;
                    if (real_priority_queue.Count > 0)
                    {
                        Person head = real_priority_queue[0];//参照渡し？値渡し?
                        real_priority_queue.RemoveAt(0);
                        this.elist.Add(new MyEvent(this.simtime+D, 0,head, "ENDPROCESS"));
                    }
                    else if (real_queue.Count > 0)
                    {
                        Person head = real_queue[0];//参照渡し？値渡し?
                        real_queue.RemoveAt(0);
                        this.elist.Add(new MyEvent(this.simtime + D, 0, head, "ENDPROCESS"));
                    }
                    else
                    {
                        //お客さんがいない
                    }
                }
                else if (current.action.Equals("ENDPROCESS"))
                {
                    ServerInProcess =false;
                    this.elist.Add(new MyEvent(this.simtime, 0, current.who, "DEPARTURE"));
                    //レーン長のチェック
                    if (real_priority_queue.Count > 0)
                    {
                        Person head = real_priority_queue[0];//参照渡し？値渡し?
                        //real_priority_queue.RemoveAt(0);
                        this.elist.Add(new MyEvent(this.simtime, 0, head, "BEGINPROCESS"));
                    }
                    else if (real_queue.Count > 0)
                    {
                        Person head = real_queue[0];//参照渡し？値渡し?
                        //real_queue.RemoveAt(0);
                        this.elist.Add(new MyEvent(this.simtime, 0, head, "BEGINPROCESS"));
                    }
                    else
                    {
                        //お客さんがいない
                    }
                }
                else if (current.action.Equals("DEPARTURE"))
                {
                    if (current.who.isvip)
                    {
                        this.waitTime_priority.Add(this.simtime - current.who.arrivaltime);
                    }
                    else
                    {
                        this.waitTime.Add(this.simtime-current.who.arrivaltime);
                    }
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
        public string get_result_string()
        {
            #region 平均待ち時間を計算
            string message = string.Empty;
            message += this.waitTime_priority.Average().ToString();
            message += ",";
            message += this.waitTime.Average().ToString();
            return message;
            #endregion
        }
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
