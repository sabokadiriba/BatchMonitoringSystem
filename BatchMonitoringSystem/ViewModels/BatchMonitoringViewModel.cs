using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.ViewModels
{
    public class BatchFilterViewModel
    {
        public int SelectedEquipmentId { get; set; }
        public int SelectedProductId { get; set; }
        public int SelectedBatchId { get; set; }

        public IEnumerable<SelectListItem> EquipmentList { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public IEnumerable<SelectListItem> BatchList { get; set; }
        public Batch SelectedBatch { get; set; }
    }

}

