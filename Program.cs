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

        var searchService = new SearchService(shipmentService, customerService);
        MainMenu(customerService, shipmentService, searchService, jsonDataService);
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

    static void MainMenu(CustomerService customerService, ShipmentService shipmentService,
        SearchService searchService, JsonDataService jsonDataService)
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("====== Shipment Tracking System ======");
            Console.WriteLine("1. Customer management");
            Console.WriteLine("2. Shipment management");
            Console.WriteLine("3. Update shipment status");
            Console.WriteLine("4. Search and view");
            Console.WriteLine("5. Reports");
            Console.WriteLine("6. Save data");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1":
                        CustomerMenu(customerService, shipmentService);
                        break;
                    case "2":
                        ShipmentMenu(customerService, shipmentService);
                        break;
                    case "3":
                        UpdateShipmentStatus(shipmentService);
                        break;
                    case "4":
                        SearchMenu(shipmentService, searchService);
                        break;
                    case "5":
                        ReportsMenu(shipmentService, searchService);
                        break;
                    case "6":
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

                static void CustomerMenu(CustomerService customerService, ShipmentService shipmentService)
                {
                    Console.WriteLine("1. Display customers");
                    Console.WriteLine("2. Register customer");
                    Console.WriteLine("3. Find customer");
                    Console.WriteLine("4. Search customer");
                    Console.WriteLine("5. Update customer");
                    Console.WriteLine("6. Delete customer");
                    Console.Write("Select an option: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            DisplayCustomers(customerService);
                            break;
                        case "2":
                            Console.Write("Customer ID: ");
                            var id = Console.ReadLine() ?? "";
                            Console.Write("Full name: ");
                            var name = Console.ReadLine() ?? "";
                            Console.Write("Phone: ");
                            var phone = Console.ReadLine() ?? "";
                            Console.Write("Address: ");
                            var address = Console.ReadLine() ?? "";
                            customerService.RegisterCustomer(id, name, phone, address);
                            Console.WriteLine("Customer registered.");
                            break;
                        case "3":
                            Console.Write("Customer ID: ");
                            var customer = customerService.GetCustomerById(Console.ReadLine() ?? "");
                            Console.WriteLine($"{customer.CustomerId} | {customer.FullName} | {customer.PhoneNumber} | {customer.Address}");
                            break;
                        case "4":
                            Console.Write("Name or phone: ");
                            var search = Console.ReadLine() ?? "";
                            var customers = customerService.SearchCustomers(c =>
                                c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase));
                            foreach (var item in customers)
                                Console.WriteLine($"{item.CustomerId} | {item.FullName} | {item.PhoneNumber}");
                            break;
                        case "5":
                            Console.Write("Customer ID: ");
                            var updateId = Console.ReadLine() ?? "";
                            Console.Write("New name: ");
                            var newName = Console.ReadLine() ?? "";
                            Console.Write("New phone: ");
                            var newPhone = Console.ReadLine() ?? "";
                            Console.Write("New address: ");
                            var newAddress = Console.ReadLine() ?? "";
                            customerService.UpdateCustomer(updateId, newName, newPhone, newAddress);
                            Console.WriteLine("Customer updated.");
                            break;
                        case "6":
                            Console.Write("Customer ID: ");
                            customerService.DeleteCustomer(Console.ReadLine() ?? "", shipmentService);
                            Console.WriteLine("Customer deleted.");
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }

                static void ShipmentMenu(CustomerService customerService, ShipmentService shipmentService)
                {
                    Console.WriteLine("1. Display shipments");
                    Console.WriteLine("2. Create shipment");
                    Console.WriteLine("3. Track shipment");
                    Console.WriteLine("4. Update shipment details");
                    Console.Write("Select an option: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            DisplayShipments(shipmentService);
                            break;
                        case "2":
                            CreateShipment(customerService, shipmentService);
                            break;
                        case "3":
                            TrackShipment(shipmentService);
                            break;
                        case "4":
                            UpdateShipmentDetails(shipmentService);
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
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

    static void SearchMenu(ShipmentService shipmentService, SearchService searchService)
    {
        Console.WriteLine("1. Track shipment");
        Console.WriteLine("2. Search receiver, destination, or status");
        Console.WriteLine("3. List shipments for sender");
        Console.WriteLine("4. Filter by destination");
        Console.Write("Select an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                TrackShipment(shipmentService);
                break;
            case "2":
                SearchShipments(shipmentService);
                break;
            case "3":
                Console.Write("Sender customer ID: ");
                DisplayShipmentList(searchService.ShipmentsForCustomer(Console.ReadLine() ?? ""));
                break;
            case "4":
                Console.Write("Destination: ");
                DisplayShipmentList(searchService.ShipmentsByDestination(Console.ReadLine() ?? ""));
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    static void ReportsMenu(ShipmentService shipmentService, SearchService searchService)
    {
        Console.WriteLine("1. Shipment counts by status");
        Console.WriteLine("2. In-transit shipments");
        Console.WriteLine("3. Out-for-delivery shipments");
        Console.WriteLine("4. Fees grouped by status");
        Console.WriteLine("5. Fees grouped by destination");
        Console.WriteLine("6. Delivered, pending, cancelled, and in-transit counts");
        Console.WriteLine("7. Heaviest and average shipment weight");
        Console.Write("Select an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                foreach (var item in searchService.GetShipmentCountsByStatus())
                    Console.WriteLine($"{item.Key}: {item.Value}");
                break;
            case "2":
                DisplayShipmentList(searchService.InTransitShipments());
                break;
            case "3":
                DisplayShipmentList(searchService.OutForDeliveryShipments());
                break;
            case "4":
                foreach (var item in searchService.FeesByStatus())
                    Console.WriteLine($"{item.Key}: {item.Value:C}");
                break;
            case "5":
                foreach (var item in searchService.FeesByDestination())
                    Console.WriteLine($"{item.Key}: {item.Value:C}");
                break;
            case "6":
                Console.WriteLine($"Delivered: {searchService.CountByStatus("Delivered")}");
                Console.WriteLine($"Pending: {searchService.CountPending()}");
                Console.WriteLine($"Cancelled: {searchService.CountByStatus("Cancelled")}");
                Console.WriteLine($"In transit: {searchService.CountByStatus("InTransit")}");
                break;
            case "7":
                var statistics = searchService.GetWeightStatistics();
                Console.WriteLine($"Average weight: {statistics.AvgWeight}");
                var heaviest = searchService.GetHeaviestShipment();
                Console.WriteLine($"Heaviest shipment: {heaviest.ShipmentId} ({heaviest.Weight})");
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    static void DisplayShipmentList(List<Shipment> shipments)
    {
        if (!shipments.Any())
        {
            Console.WriteLine("No shipments found.");
            return;
        }

        foreach (var shipment in shipments)
        {
            Console.WriteLine(
                $"{shipment.ShipmentId} | Sender: {shipment.SenderCustomerId} | " +
                $"{shipment.ReceiverName} | {shipment.DestinationAddress} | {shipment.CurrentStatus}");
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
