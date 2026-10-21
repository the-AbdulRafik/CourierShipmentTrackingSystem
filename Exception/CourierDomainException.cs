using System;

namespace CourierShipmentTrackingSystem.Exceptions
{
    public class CourierDomainException : Exception
    {
        public CourierDomainException(string message) : base(message) { }
    }
}