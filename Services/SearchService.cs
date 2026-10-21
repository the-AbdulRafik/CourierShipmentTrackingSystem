using System;

using CourierShipmentTrackingSystem.Services;
using CourierShipmentTrackingSystem.Exceptions;
using CourierShipmentTrackingSystem.utils;
using CourierShipmentTrackingSystem.Models;

// TODO

// Search and reports
// Track by tracking number and display complete status history.
// List shipments for a selected sender.
// Filter shipments by status or destination.
// Show shipments in transit or out for delivery.
// Calculate shipping fees grouped by status or destination.
// Count delivered, pending, cancelled, and in-transit shipments.
// Find the heaviest shipment and calculate average shipment weight.

class SearchService
{
    private readonly ShipmentService _shipmentService;
    private readonly CustomerService _customerService;

    public SearchService(ShipmentService shipmentService, CustomerService customerService)
    {
        _shipmentService = shipmentService;
        _customerService = customerService;
    }

    public void GetShipment(Func<Shipment, bool> criteria)
        {
            var shipments = _shipmentService.GetAllShipments().Where(criteria).ToList();
            if (!shipments.Any())
            {
                throw new CourierDomainException($"No shipments found.");
            }

            foreach (var shipment in shipments)
            {
                Console.WriteLine($"Shipment ID: {shipment.ShipmentId}, Current Status: {shipment.CurrentStatus}");
            }
        }

        //Filter shipments by status or destination 
        public List<Shipment> FilterShipments(Func<Shipment, bool> criteria)
        {
            return _shipmentService.GetAllShipments().Where(criteria).ToList();
        }

        // Count delivered, pending, cancelled, and in-transit shipments.
        
        public Dictionary<string, int> CountShipments(Func<Shipment, string> criteria)
        {
        return _shipmentService.GetAllShipments()
            .GroupBy(criteria)
            .ToDictionary(g => g.Key, g => g.Count());
        }


         // LINQ Reports
        public Dictionary<string, int> GetShipmentCountsByStatus()
        {
            return _shipmentService.GetAllShipments()
                .GroupBy(s => s.CurrentStatus)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public (double MaxWeight, double AvgWeight) GetWeightStatistics()
        {
            if (!_shipmentService.GetAllShipments().Any()) return (0, 0);
            double max = _shipmentService.GetAllShipments().Max(s => s.Weight);
            double avg = _shipmentService.GetAllShipments().Average(s => s.Weight);
            return (max, avg);
        }

        // Find the heaviest shipment and calculate average shipment weight.
        public Shipment GetHeaviestShipment()
        {
            (double maxWeight, _) = GetWeightStatistics();

            var heaviestShipment = _shipmentService.GetAllShipments().FirstOrDefault(s => s.Weight == maxWeight);
            if(heaviestShipment == null) throw new CourierDomainException("Heaviest shipment not found.");
            return heaviestShipment;
        }
        public void GetAverageWeightShipment()
        {
            (_, double avgWeight) = GetWeightStatistics();
            Console.WriteLine($"Average Weight: {avgWeight}");
        }

        
}
