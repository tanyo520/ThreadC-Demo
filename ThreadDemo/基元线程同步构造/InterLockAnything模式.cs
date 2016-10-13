using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.基元线程同步构造
{
    /// <summary>
    /// 该模式类似在修改数据库记录时使用乐观锁并发模式
    /// </summary>
  public  class InterLockAnything模式
    {
        public static Int32 Maxinum(ref Int32 target, Int32 value) {
            Int32 currentVal = target, starVal, desiredVal;
            do
            {
                starVal = currentVal;
                desiredVal = Math.Max(starVal, value);
                //线程可能被抢占，所以以下代码不是原子性的。
                // if (target == starVal) target = desiredVal;
                currentVal = Interlocked.CompareExchange(ref target, desiredVal, starVal);

            } while (starVal != currentVal);
            return desiredVal;
        }

        delegate Int32 Morpher<TReuslt, TArgument>(Int32 startValue, TArgument argument, out TReuslt morphResult);
        static TResult Morph<TResult, TArgument>(ref Int32 target, TArgument argument, Morpher<TResult, TArgument> moAction) {
            TResult moResult;
            Int32 currentVal = target, startVal, desiredVal;
            do
            {
                startVal = currentVal;
                desiredVal = moAction(startVal, argument, out moResult);
                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
            } while (currentVal != startVal);
            return moResult;
        }
    }

}
