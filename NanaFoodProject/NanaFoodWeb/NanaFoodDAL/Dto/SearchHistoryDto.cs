using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    internal class SearchHistoryDto
    {
        public int SearchId { get; set; }
        public string UserId { get; set; }
        public string SearchString { get; set; }
    }
}
