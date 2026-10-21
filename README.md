# Courier Shipment Tracking System

This is a menu-driven .NET console application for managing customers,
shipments, status history, searches, and reports.

## Run

From this folder:

```text
dotnet run
```

The application loads data from `courier-data.json` when the file exists.
If it does not exist, a small set of sample data is created. Use the
`Save data` menu option or exit the application to save the current data.

## Choices used in this project

- Customer IDs and tracking numbers are entered manually and checked for uniqueness.
- Shipping fees are entered manually and must not be negative.
- New shipments start with `Created` status.
- Normal status flow is `Created` -> `PickedUp` -> `InTransit` ->
  `OutForDelivery` -> `Delivered`.
- Cancellation is allowed only while a shipment is `Created`.
- Shipment history is stored inside each shipment and saved to JSON.
