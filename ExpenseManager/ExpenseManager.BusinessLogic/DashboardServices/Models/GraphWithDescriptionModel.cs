using System.Collections.Generic;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    public class GraphWithDescriptionModel
    {
        public List<SimpleGraphModel> GraphData { get; set; }
        public string Description { get; set; }
    }
}