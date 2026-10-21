using System.Text.Json;
using CourierShipmentTrackingSystem.Exceptions;
using CourierShipmentTrackingSystem.Models;

namespace CourierShipmentTrackingSystem.Services
{
    public class JsonDataService
    {
        private readonly string _filePath = "courier-data.json";

        public void SaveData(CustomerService customerService, ShipmentService shipmentService)
        {
            var data = new CourierData
            {
                Customers = customerService.GetAllCustomers(),
                Shipments = shipmentService.GetAllShipments()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            File.WriteAllText(_filePath, JsonSerializer.Serialize(data, options));
        }

        public bool LoadData(CustomerService customerService, ShipmentService shipmentService)
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                return false;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<CourierData>(json);

                if (data == null)
                {
                    return false;
                }

                customerService.LoadCustomers(data.Customers);
                shipmentService.LoadShipments(data.Shipments);
                return true;
            }
            catch (JsonException ex)
            {
                throw new CourierDomainException($"Could not load JSON data: {ex.Message}");
            }
        }

        private class CourierData
        {
            public List<Customer> Customers { get; set; } = new List<Customer>();
            public List<Shipment> Shipments { get; set; } = new List<Shipment>();
        }
    }
}
