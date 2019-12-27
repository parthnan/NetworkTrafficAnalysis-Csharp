using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenSimulation
{

    /// <summary>
    /// Eventを定義する。C# では小文字でeventは予約語になっているので、MyEventとした
    /// </summary>
    #region MyEventクラスの定義
    public class MyEvent
    {
        public double time;		//いつ   
        public int where;		//どこで
        public int who;		    //だれが
        public string action;	//何をする？
        public MyEvent(double a, int b, int c, string d)
        {
            time = a;
            where = b;
            who = c;
            action = d;
        }
    }
    #endregion


}
