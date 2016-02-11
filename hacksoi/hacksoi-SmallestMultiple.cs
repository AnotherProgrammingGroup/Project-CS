using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hacksoiHelloC
{
    class hacksoi_SmallestMultiple
    {
        
        public int getAnswer()
        {
            int num = 1;
            outer: while(true)
            {
                for(int i = 1; i <= 20; i++)
                {
                    if (num % i != 0)
                    {
                        num++;
                        goto outer;
                    }
                }
                break;
            }
            return num;
        }

    }
}
