﻿using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class CustomerRequest
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Phone]
        public string CustomerPhone { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerCity { get; set; }

        [Required]
        public string CustomerRegion { get; set; }

        public bool IsActive { get; set; }
        public List<Site> Sites { get; set; } = new List<Site>();
        public List<string> CustomerUsers { get; set; } = new List<string>();
        public List<DigitalService> DigitalServices { get; set; } = new List<DigitalService>();
    }

    public class Site
    {
        [Required]
        [StringLength(100)]
        public string SiteName { get; set; }

        [Required]
        [StringLength(200)]
        public string SiteLocation { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public List<Device> Devices { get; set; } = new List<Device>();
    }

    public class Device
    {
        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; }

        [Required]
        public string ProductType { get; set; }

        public double ThreSholdValue { get; set; }
    }

    public class DigitalService
    {
        [Required]
        public DateTime ServiceStartDate { get; set; }

        [Required]
        public DateTime ServiceEndDate { get; set; }

        public bool IsActive { get; set; }

        public List<string> NotificationUsers { get; set; } = new List<string>();
    }
}

