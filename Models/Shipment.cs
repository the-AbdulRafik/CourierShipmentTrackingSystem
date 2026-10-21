// ShipmentId/tracking number (unique), SenderCustomerId, ReceiverName, ReceiverPhone, DestinationAddress, CreatedDate, Weight, ShippingFee, CurrentStatus.


using CourierShipmentTrackingSystem.utils;

namespace CourierShipmentTrackingSystem.Models
{
    public class Shipment : IIdentifiable<string>
    {
        public string ShipmentId { get; private set; }
        public string SenderCustomerId { get; private set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Weight { get; set; }
        public decimal ShippingFee { get; set; }
        public string CurrentStatus { get; set; }

        public List<ShipmentStatusHistory> StatusHistory { get; private set; } = new List<ShipmentStatusHistory>();

        public Shipment(string shipmentId, string senderCustomerId, string receiverName, string receiverPhone, string destinationAddress, double weight, decimal shippingFee)
        {
            ShipmentId = shipmentId;
            SenderCustomerId = senderCustomerId;
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            if (!Validation.IsValidName(receiverName))
            {
                throw new ArgumentException("Receiver name cannot be blank.", nameof(receiverName));
            }
            if (!Validation.IsValidPhoneNumber(receiverPhone))
            {
                throw new ArgumentException("Receiver phone number cannot be blank.", nameof(receiverPhone));
            }
            if (!Validation.IsValidAddress(destinationAddress))
            {
                throw new ArgumentException("Destination address cannot be blank.", nameof(destinationAddress));
            }
            DestinationAddress = destinationAddress;
            CreatedDate = DateTime.Now;
            Weight = weight;
            ShippingFee = shippingFee;
            CurrentStatus = "Created";
            StatusHistory.Add(new ShipmentStatusHistory(ShipmentId, "Created", "Unknown", "Shipment created."));
        }

        public string GetId() => ShipmentId;
    }
}