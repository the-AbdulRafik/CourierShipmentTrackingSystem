// Customer management
// Register a customer with unique ID and valid details.
// Display all customers and retrieve a customer by ID.
// Search customers by name or phone number.
// Update customer phone number or address.
// Prevent deletion if sender references would become invalid; reject or use a safe archival rule.


using CourierShipmentTrackingSystem.Models;
using CourierShipmentTrackingSystem.Exceptions;
using System;
using CourierShipmentTrackingSystem.utils;

namespace CourierShipmentTrackingSystem.Services
{
    public class CustomerService
    {
        private List<Customer> _customers = new List<Customer>();

        public void RegisterCustomer(string customerId, string fullName, string phoneNumber, string address)
        {
            if(string.IsNullOrWhiteSpace(customerId))
            {
                throw new CourierDomainException("Customer ID cannot be blank.");
            }

            if(!Validation.IsValidName(fullName) || !Validation.IsValidPhoneNumber(phoneNumber) || !Validation.IsValidAddress(address))
            {
                throw new CourierDomainException("Customer details are invalid.");
            }

            if (_customers.Any(c => c.CustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CourierDomainException($"Customer with ID '{customerId}' already exists.");
            }

            var customer = new Customer(customerId, fullName, phoneNumber, address);
            _customers.Add(customer);
        }

        public List<Customer> GetAllCustomers() => _customers;

        public void LoadCustomers(List<Customer> customers)
        {
            _customers.Clear();
            _customers.AddRange(customers);
        }

        public Customer GetCustomerById(string customerId)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase));
            if (customer == null)
            {
                throw new CourierDomainException($"Customer with ID '{customerId}' not found.");
            }
            return customer;
        }

        public List<Customer> SearchCustomers(Predicate<Customer> matchCriteria)
        {
            return _customers.Where(c => matchCriteria(c)).ToList();
        }

        public void UpdateCustomer(string customerId, string newFullName, string newPhone, string newAddress)
        {
            var customer = GetCustomerById(customerId);

            if (Validation.IsValidName(newFullName)) customer.FullName = newFullName;
            if (Validation.IsValidPhoneNumber(newPhone)) customer.PhoneNumber = newPhone;
            if (Validation.IsValidAddress(newAddress)) customer.Address = newAddress;
        }

        
        public void DeleteCustomer(string customerId, ShipmentService shipmentService)
        {
            var customer = GetCustomerById(customerId);
            if (shipmentService.HasShipmentsForCustomer(customerId))
            {
                throw new CourierDomainException($"Cannot delete customer '{customerId}' as they have associated shipments.");
            }
            _customers.Remove(customer);
        }    
    }
}
