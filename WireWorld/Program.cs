using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireWorld
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var g = new Game(15, 2, 2);
        }
    }
}