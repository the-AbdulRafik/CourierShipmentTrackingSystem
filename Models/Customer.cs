using CourierShipmentTrackingSystem.utils;

namespace CourierShipmentTrackingSystem.Models
{
    public class Customer : IIdentifiable<string>
    {
        public string CustomerId { get; private set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // Missing validation
        // The constructor currently accepts any values. It should reject:

        // Blank customer ID
        // Blank full name
        // Blank phone number
        // Blank address
        // Invalid phone number format, if your assessment expects valid phone numbers
        
        public Customer(string customerId, string fullName, string phoneNumber, string address)
        {
            CustomerId = customerId;
            if (string.IsNullOrWhiteSpace(customerId) )
            {
                throw new ArgumentException("Customer ID cannot be blank.", nameof(customerId));
            }
            if (!Validation.IsValidName(fullName))
            {
                throw new ArgumentException("Full name cannot be blank.", nameof(fullName));
            }
            FullName = fullName;
            if (!Validation.IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException("Phone number cannot be blank.", nameof(phoneNumber));
            }
            PhoneNumber = phoneNumber;
            if (!Validation.IsValidAddress(address))
            {
                throw new ArgumentException("Address cannot be blank.", nameof(address));
            }
            Address = address;
        }

        public string GetId() => CustomerId;
    }
}