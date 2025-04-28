using System.Text.Json;

namespace SemanticCursorJsonMapper;

public class SemanticCursor
    {
        private readonly JsonElement _root;

        public SemanticCursor(string json)
        {
            _root = JsonDocument.Parse(json).RootElement;
        }

        // Find a property anywhere in the JSON, with optional path constraints
        public JsonElement? FindProperty(string propertyName, string pathPrefix = null)
        {
            return FindPropertyRecursive(_root, propertyName, pathPrefix, "");
        }

        // Find all instances of a property
        public List<JsonElement> FindAllProperties(string propertyName)
        {
            var results = new List<JsonElement>();
            FindAllPropertiesRecursive(_root, propertyName, results, "");
            return results;
        }

        private JsonElement? FindPropertyRecursive(JsonElement element, string propertyName, string pathPrefix, string currentPath)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                        
                        // Check if this property matches
                        if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            // If path prefix is specified, ensure the path starts with it
                            if (string.IsNullOrEmpty(pathPrefix) || newPath.StartsWith(pathPrefix))
                            {
                                return property.Value;
                            }
                        }

                        // Recursively check this property's value
                        var result = FindPropertyRecursive(property.Value, propertyName, pathPrefix, newPath);
                        if (result.HasValue)
                        {
                            return result;
                        }
                    }
                    break;

                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        string newPath = $"{currentPath}[{index}]";
                        var result = FindPropertyRecursive(item, propertyName, pathPrefix, newPath);
                        if (result.HasValue)
                        {
                            return result;
                        }
                        index++;
                    }
                    break;
            }

            return null;
        }

        private void FindAllPropertiesRecursive(JsonElement element, string propertyName, List<JsonElement> results, string currentPath)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                        
                        // Check if this property matches
                        if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            results.Add(property.Value);
                        }

                        // Recursively check this property's value
                        FindAllPropertiesRecursive(property.Value, propertyName, results, newPath);
                    }
                    break;

                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        string newPath = $"{currentPath}[{index}]";
                        FindAllPropertiesRecursive(item, propertyName, results, newPath);
                        index++;
                    }
                    break;
            }
        }

        // Extract a specific address object
        public Address ExtractAddress(JsonElement addressElement)
        {
            var address = new Address();

            if (addressElement.ValueKind == JsonValueKind.Object)
            {
                if (addressElement.TryGetProperty("street", out var streetElement))
                    address.Street = streetElement.GetString();
                else if (addressElement.TryGetProperty("line1", out var line1Element))
                    address.Street = line1Element.GetString();

                if (addressElement.TryGetProperty("city", out var cityElement))
                    address.City = cityElement.GetString();

                if (addressElement.TryGetProperty("state", out var stateElement))
                    address.State = stateElement.GetString();
                else if (addressElement.TryGetProperty("province", out var provinceElement))
                    address.State = provinceElement.GetString();

                if (addressElement.TryGetProperty("postalCode", out var postalCodeElement))
                    address.PostalCode = postalCodeElement.GetString();
                else if (addressElement.TryGetProperty("zipCode", out var zipCodeElement))
                    address.PostalCode = zipCodeElement.GetString();

                if (addressElement.TryGetProperty("country", out var countryElement))
                    address.Country = countryElement.GetString();
            }

            return address;
        }

        // Helper to safely get a string value from a JsonElement
        public string GetStringValue(JsonElement? element)
        {
            if (element.HasValue && element.Value.ValueKind == JsonValueKind.String)
            {
                return element.Value.GetString();
            }
            return null;
        }

        // Helper to safely get a boolean value from a JsonElement
        public bool? GetBoolValue(JsonElement? element)
        {
            if (element.HasValue && element.Value.ValueKind == JsonValueKind.True)
            {
                return true;
            }
            else if (element.HasValue && element.Value.ValueKind == JsonValueKind.False)
            {
                return false;
            }
            return null;
        }
    }