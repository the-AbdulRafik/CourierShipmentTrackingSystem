// TODO 

// Shipment creation and management
// Create a shipment for an existing sender customer.
// Generate or validate a unique tracking number.
// Enter receiver name, phone, destination, weight, and shipping fee.
// Calculate or validate shipping fee using a documented rule; if manually entered, reject negative fees.
// Display all shipments and retrieve a shipment by tracking number.
// Display full shipment details and status history.
// Search by tracking number, sender, receiver, destination, or current status.
// Update permitted shipment details before dispatch without changing tracking identity or deleting history.



using System;
using System.Collections.Generic;
using System.Linq;
using CourierShipmentTrackingSystem.Exceptions;
using CourierShipmentTrackingSystem.Models;
using CourierShipmentTrackingSystem.Services;
using CourierShipmentTrackingSystem.utils;

namespace CourierShipmentTrackingSystem.Services
{
    public class ShipmentService
    {
        private List<Shipment> _shipments = new List<Shipment>();

        private readonly List<string> _statusFlow = new List<string> { "Created", "PickedUp", "InTransit", "OutForDelivery", "Delivered" };

        public void CreateShipment(string shipmentId, string senderCustomerId, string receiverName, string receiverPhone, string destinationAddress, double weight, decimal shippingFee, CustomerService customerService)
        {

            if (string.IsNullOrWhiteSpace(shipmentId))
            {
                throw new CourierDomainException("Shipment tracking number cannot be blank.");
            }
            // Validation rules
            if (!Validation.IsValidName(receiverName))
            {
                throw new CourierDomainException("Receiver name must be valid and cannot be blank.");
            }
            if (!Validation.IsValidPhoneNumber(receiverPhone))
            {
                throw new CourierDomainException("Receiver phone number must be valid and cannot be blank.");
            }
            if (!Validation.IsValidAddress(destinationAddress))
            {
                throw new CourierDomainException("Destination address cannot be blank.");
            }
            if (!Validation.IsValidWeight(weight))
            {
                throw new CourierDomainException("Weight must be greater than zero.");
            }
            if (!Validation.IsValidShippingFee(shippingFee))
            {
                throw new CourierDomainException("Shipping fee cannot be negative.");
            }

            if (_shipments.Any(s => s.ShipmentId.Equals(shipmentId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CourierDomainException($"Shipment tracking number '{shipmentId}' already exists.");
            }

            // Verify sender exists
            customerService.GetCustomerById(senderCustomerId); // Will throw if not found

            var shipment = new Shipment(shipmentId, senderCustomerId, receiverName, receiverPhone, destinationAddress, weight, shippingFee);
            _shipments.Add(shipment);
        }

        public Shipment GetShipmentById(string shipmentId)
        {
            var shipment = _shipments.FirstOrDefault(s => s.ShipmentId.Equals(shipmentId, StringComparison.OrdinalIgnoreCase));
            if (shipment == null)
            {
                throw new CourierDomainException($"Shipment with tracking number '{shipmentId}' not found.");
            }
            return shipment;
        }

        public List<Shipment> GetAllShipments() => _shipments;

        public void LoadShipments(List<Shipment> shipments)
        {
            _shipments.Clear();
            _shipments.AddRange(shipments);
        }

        public void UpdateStatus(string shipmentId, string newStatus, string location, string notes = "")
        {
            var shipment = GetShipmentById(shipmentId);
            if(string.IsNullOrWhiteSpace(newStatus) || string.IsNullOrWhiteSpace(location))
            {
                throw new CourierDomainException("New Values cannot be blank.");
            }

            if(shipment.CurrentStatus == "Delivered" || shipment.CurrentStatus == "Cancelled")
            {
                throw new CourierDomainException(
                    $"Cannot update status for a shipment that is already {shipment.CurrentStatus}.");
            }
            if (newStatus == "Cancelled")
            {
                if (shipment.CurrentStatus != "Created")
                {
                    throw new CourierDomainException(
                        "A shipment can only be cancelled while it is in Created status.");
                }
            }
            else
            {
                if (!_statusFlow.Contains(newStatus))
                {
                    throw new CourierDomainException(
                        $"Invalid status '{newStatus}'.");
                }

                int currentStatusIndex = _statusFlow.IndexOf(shipment.CurrentStatus);
                int newStatusIndex = _statusFlow.IndexOf(newStatus);

                if (newStatusIndex != currentStatusIndex + 1)
                {
                    throw new CourierDomainException(
                        $"Invalid transition: {shipment.CurrentStatus} cannot change directly to {newStatus}.");
                }
            }
            
            shipment.CurrentStatus = newStatus;
            shipment.StatusHistory.Add(new ShipmentStatusHistory(shipmentId, newStatus, location, notes));
        }

       

        

        public bool HasShipmentsForCustomer(string customerId)
        {
            return GetAllShipments().Any(s => s.SenderCustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase));
        }
    

        public void UpdateShipmentDetails(string shipmentId, string newReceiverName, string newReceiverPhone, string newDestinationAddress)
        {
            var shipment = GetShipmentById(shipmentId);
            if(shipment == null) throw new CourierDomainException("Shipment not found.");
            if(!Validation.IsValidName(newReceiverName)) throw new CourierDomainException("Receiver name cannot be blank.");
            if(!Validation.IsValidPhoneNumber(newReceiverPhone)) throw new CourierDomainException("Receiver phone number cannot be blank.");
            if(!Validation.IsValidAddress(newDestinationAddress)) throw new CourierDomainException("Destination address cannot be blank.");
            if(shipment.CurrentStatus == "OutForDelivery" || shipment.CurrentStatus == "Delivered" || shipment.CurrentStatus == "Cancelled")
            {
                throw new CourierDomainException($"Cannot update shipment details (Address) for a shipment that is {shipment.CurrentStatus}.");
            }
            
            shipment.ReceiverName = newReceiverName;
            shipment.ReceiverPhone = newReceiverPhone;
            shipment.DestinationAddress = newDestinationAddress;
        }

        public List<Shipment> SearchShipments(Predicate<Shipment> matchCriteria)
        {
            return _shipments.Where(s => matchCriteria(s)).ToList();
        }

        
    
    
    }
}
