using System;

namespace CourierShipmentTrackingSystem.Models
{
    public class ShipmentStatusHistory
    {
        public string ShipmentId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }

        public ShipmentStatusHistory(string shipmentId, string status, string location, string notes = "")
        {
            ShipmentId = shipmentId;
            Status = status;
            UpdatedAt = DateTime.Now;
            Location = location;
            Notes = notes;
        }
    }
}