using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.ViewModels
{
    public class BatchReportModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<BatchDto> Batches { get; set; }
    }
        public class BatchDto
    {


        public int BatchId { get; set; }
        [Required]
        public int EquipmentId { get; set; }

        [Required]
        public string BatchName { get; set; }
        [Required]
        public int ProductId { get; set; }

        [Required]
        public DateTime BatchStartTime { get; set; }
       

        [Required]
        public DateTime BatchEndTime { get; set; }
        public string Comments { get; internal set; }
        public string BatchStatus { get; internal set; }
        public List<BatchParameterDto> BatchParameters { get; set; } = new List<BatchParameterDto>();
        public string EquipmentName { get; internal set; }
        public string ProductName { get; internal set; }
    }

    public class BatchParameterDto
    {
        public int BatchParameterId { get; set; }

        [Required]
        public string ParameterName { get; set; }
        public List<double> ActualValues { get; set; } = new List<double>();
        [Required]
       

        public bool IsWithinRange { get; set; }

        public string Comment { get; set; }
        public double MinValue { get; internal set; }
        public double MaxValue { get; internal set; }
    }
}
