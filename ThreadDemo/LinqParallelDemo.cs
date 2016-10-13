using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo
{
  public  class LinqParallelDemo
    {
        public void Demo() {
            //多线程
            var query = from t in System.AppDomain.CurrentDomain.GetAssemblies()[0].GetExportedTypes().AsParallel<Type>() select t;
            query.ForAll((t)=>Console.WriteLine(t.ToString()));
        }
    }
}
