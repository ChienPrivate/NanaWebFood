using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    public class LineChartDto
    {
        public DateTime Period { get; set; }   // Tháng của doanh thu
        public double Revenue { get; set; }    // Doanh thu của tháng
    }

}
