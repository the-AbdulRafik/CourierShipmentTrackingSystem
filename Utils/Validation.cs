namespace CourierShipmentTrackingSystem.utils
{
    public static class Validation
    {
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 10;
        }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        public static bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address);
        }

        public static bool IsValidWeight(double weight)
        {
            return weight > 0;
        }

        public static bool IsValidShippingFee(decimal shippingFee)
        {
            return shippingFee >= 0;
        }
        
    }
}