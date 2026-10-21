namespace CourierShipmentTrackingSystem.Models
{
    public interface IIdentifiable<T>
    {
        T GetId();
    }
}