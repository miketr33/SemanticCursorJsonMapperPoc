using System.Text.Json;

namespace SemanticCursorJsonMapper;

public class PersonMapper
    {
        // Map from unreliable JSON to our static model
        public PersonModel MapFromJson(string json)
        {
            var cursor = new SemanticCursor(json);
            var person = new PersonModel
            {
                PreviousAddresses = new List<Address>(),
                // Extract basic info - try multiple possible locations/names
                FirstName = cursor.GetStringValue(cursor.FindProperty("firstName")) ?? 
                            cursor.GetStringValue(cursor.FindProperty("first_name")) ??
                            cursor.GetStringValue(cursor.FindProperty("givenName")),
                LastName = cursor.GetStringValue(cursor.FindProperty("lastName")) ?? 
                           cursor.GetStringValue(cursor.FindProperty("last_name")) ??
                           cursor.GetStringValue(cursor.FindProperty("surname")),
                // Try to find the email at multiple possible locations
                Email = cursor.GetStringValue(cursor.FindProperty("email")) ??
                        cursor.GetStringValue(cursor.FindProperty("emailAddress")) ??
                        cursor.GetStringValue(cursor.FindProperty("contact.email")),
                // Look for phone number
                Phone = cursor.GetStringValue(cursor.FindProperty("phone")) ??
                        cursor.GetStringValue(cursor.FindProperty("phoneNumber")) ??
                        cursor.GetStringValue(cursor.FindProperty("contact.phone")),
                // Look for match status
                IsMatch = cursor.GetBoolValue(cursor.FindProperty("isMatch")) ?? 
                          cursor.GetBoolValue(cursor.FindProperty("match")) ?? 
                          false
            };

            // Find all reference IDs and use the first valid one
            var allRefIds = cursor.FindAllProperties("referenceId")
                .Union(cursor.FindAllProperties("reference_id"))
                .Union(cursor.FindAllProperties("refId"))
                .Select(e => e.ValueKind == JsonValueKind.String ? e.GetString() : null)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            if (allRefIds.Count > 0)
            {
                person.ReferenceId = allRefIds[0];
            }

            // Extract primary address
            var primaryAddressElement = cursor.FindProperty("primaryAddress") ?? 
                                        cursor.FindProperty("address") ??
                                        cursor.FindProperty("currentAddress") ??
                                        cursor.FindProperty("details");

            if (primaryAddressElement.HasValue)
            {
                person.PrimaryAddress = cursor.ExtractAddress(primaryAddressElement.Value);
                
                // Check if email is in the address (sometimes APIs put it there)
                if (string.IsNullOrEmpty(person.Email) && 
                    primaryAddressElement.Value.TryGetProperty("email", out var emailInAddress))
                {
                    person.Email = emailInAddress.GetString();
                }
            }

            // Extract previous addresses
            var prevAddressesElement = cursor.FindProperty("previousAddresses") ?? 
                                       cursor.FindProperty("pastAddresses") ??
                                       cursor.FindProperty("addressHistory");

            if (prevAddressesElement.HasValue && prevAddressesElement.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var addrElement in prevAddressesElement.Value.EnumerateArray())
                {
                    person.PreviousAddresses.Add(cursor.ExtractAddress(addrElement));
                }
            }

            return person;
        }
    }