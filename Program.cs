using CourierShipmentTrackingSystem.Exceptions;
using CourierShipmentTrackingSystem.Models;
using CourierShipmentTrackingSystem.Services;

class Program
{
    static void Main(string[] args)
    {
        var customerService = new CustomerService();
        var shipmentService = new ShipmentService();
        var jsonDataService = new JsonDataService();

        if (!jsonDataService.LoadData(customerService, shipmentService))
        {
            AddSampleData(customerService, shipmentService);
        }

        MainMenu(customerService, shipmentService, jsonDataService);
    }

    static void AddSampleData(CustomerService customerService, ShipmentService shipmentService)
    {
        customerService.RegisterCustomer("C001", "John Doe", "1234567890", "123 Main Street");
        customerService.RegisterCustomer("C002", "Jane Smith", "0987654321", "456 Park Road");

        shipmentService.CreateShipment(
            "S001",
            "C001",
            "Robert Brown",
            "9876543210",
            "789 Elm Street",
            2.5,
            15.00m,
            customerService);
    }

    static void MainMenu(CustomerService customerService, ShipmentService shipmentService, JsonDataService jsonDataService)
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("====== Shipment Tracking System ======");
            Console.WriteLine("1. Display customers");
            Console.WriteLine("2. Display shipments");
            Console.WriteLine("3. Create shipment");
            Console.WriteLine("4. Update shipment status");
            Console.WriteLine("5. Track shipment");
            Console.WriteLine("6. Search shipments");
            Console.WriteLine("7. Update shipment details");
            Console.WriteLine("8. Save data");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1":
                        DisplayCustomers(customerService);
                        break;
                    case "2":
                        DisplayShipments(shipmentService);
                        break;
                    case "3":
                        CreateShipment(customerService, shipmentService);
                        break;
                    case "4":
                        UpdateShipmentStatus(shipmentService);
                        break;
                    case "5":
                        TrackShipment(shipmentService);
                        break;
                    case "6":
                        SearchShipments(shipmentService);
                        break;
                    case "7":
                        UpdateShipmentDetails(shipmentService);
                        break;
                    case "8":
                        jsonDataService.SaveData(customerService, shipmentService);
                        Console.WriteLine("Data saved successfully.");
                        break;
                    case "0":
                        jsonDataService.SaveData(customerService, shipmentService);
                        Console.WriteLine("Data saved successfully.");
                        isRunning = false;
                        continue;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (CourierDomainException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a valid number.");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
    }

    static void DisplayCustomers(CustomerService customerService)
    {
        foreach (var customer in customerService.GetAllCustomers())
        {
            Console.WriteLine(
                $"{customer.CustomerId} | {customer.FullName} | {customer.PhoneNumber} | {customer.Address}");
        }
    }

    static void DisplayShipments(ShipmentService shipmentService)
    {
        foreach (var shipment in shipmentService.GetAllShipments())
        {
            Console.WriteLine(
                $"{shipment.ShipmentId} | Sender: {shipment.SenderCustomerId} | " +
                $"Receiver: {shipment.ReceiverName} | Status: {shipment.CurrentStatus}");
        }
    }

    static void CreateShipment(CustomerService customerService, ShipmentService shipmentService)
    {
        Console.Write("Tracking number: ");
        string shipmentId = Console.ReadLine() ?? "";

        Console.Write("Sender customer ID: ");
        string senderId = Console.ReadLine() ?? "";

        Console.Write("Receiver name: ");
        string receiverName = Console.ReadLine() ?? "";

        Console.Write("Receiver phone: ");
        string receiverPhone = Console.ReadLine() ?? "";

        Console.Write("Destination address: ");
        string destination = Console.ReadLine() ?? "";

        Console.Write("Weight: ");
        double weight = double.Parse(Console.ReadLine() ?? "");

        Console.Write("Shipping fee: ");
        decimal fee = decimal.Parse(Console.ReadLine() ?? "");

        shipmentService.CreateShipment(
            shipmentId,
            senderId,
            receiverName,
            receiverPhone,
            destination,
            weight,
            fee,
            customerService);

        Console.WriteLine("Shipment created successfully.");
    }

    static void UpdateShipmentStatus(ShipmentService shipmentService)
    {
        Console.Write("Tracking number: ");
        string shipmentId = Console.ReadLine() ?? "";

        Console.WriteLine("Available statuses:");
        Console.WriteLine("PickedUp");
        Console.WriteLine("InTransit");
        Console.WriteLine("OutForDelivery");
        Console.WriteLine("Delivered");
        Console.WriteLine("Cancelled");

        Console.Write("New status: ");
        string status = Console.ReadLine() ?? "";

        Console.Write("Location: ");
        string location = Console.ReadLine() ?? "";

        Console.Write("Notes: ");
        string notes = Console.ReadLine() ?? "";

        shipmentService.UpdateStatus(shipmentId, status, location, notes);
        Console.WriteLine("Shipment status updated successfully.");
    }

    static void TrackShipment(ShipmentService shipmentService)
    {
        Console.Write("Tracking number: ");
        string shipmentId = Console.ReadLine() ?? "";
        var shipment = shipmentService.GetShipmentById(shipmentId);

        Console.WriteLine($"Shipment: {shipment.ShipmentId}");
        Console.WriteLine($"Sender: {shipment.SenderCustomerId}");
        Console.WriteLine($"Receiver: {shipment.ReceiverName}");
        Console.WriteLine($"Destination: {shipment.DestinationAddress}");
        Console.WriteLine($"Weight: {shipment.Weight}");
        Console.WriteLine($"Shipping fee: {shipment.ShippingFee}");
        Console.WriteLine($"Current status: {shipment.CurrentStatus}");
        Console.WriteLine("Status history:");

        foreach (var history in shipment.StatusHistory.OrderBy(h => h.UpdatedAt))
        {
            Console.WriteLine(
                $"{history.UpdatedAt:G} | {history.Status} | {history.Location} | {history.Notes}");
        }
    }

    static void SearchShipments(ShipmentService shipmentService)
    {
        Console.Write("Search receiver, destination, or status: ");
        string searchText = Console.ReadLine() ?? "";

        var shipments = shipmentService.SearchShipments(s =>
            s.ReceiverName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            s.DestinationAddress.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            s.CurrentStatus.Equals(searchText, StringComparison.OrdinalIgnoreCase));

        foreach (var shipment in shipments)
        {
            Console.WriteLine(
                $"{shipment.ShipmentId} | {shipment.ReceiverName} | " +
                $"{shipment.DestinationAddress} | {shipment.CurrentStatus}");
        }

        if (!shipments.Any())
        {
            Console.WriteLine("No shipments found.");
        }
    }

    static void UpdateShipmentDetails(ShipmentService shipmentService)
    {
        Console.Write("Tracking number: ");
        string shipmentId = Console.ReadLine() ?? "";

        Console.Write("New receiver name: ");
        string receiverName = Console.ReadLine() ?? "";

        Console.Write("New receiver phone: ");
        string receiverPhone = Console.ReadLine() ?? "";

        Console.Write("New destination address: ");
        string destination = Console.ReadLine() ?? "";

        shipmentService.UpdateShipmentDetails(
            shipmentId,
            receiverName,
            receiverPhone,
            destination);

        Console.WriteLine("Shipment details updated successfully.");
    }
}
