using EcommerceWeb.ViewModels;

namespace EcommerceWeb.Helpers
{
    public static class SpecificationHelper
    {
        public static List<SpecificationItem> ParseSpecifications(string specificationsText, out List<string> errors)
        {
            var specs = new List<SpecificationItem>();
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(specificationsText))
                return specs;

            var lines = specificationsText.Split('\n');
            int lineNumber = 1;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    lineNumber++;
                    continue;
                }

                var parts = trimmed.Split(':', 2);
                if (parts.Length != 2)
                {
                    errors.Add($"Dòng {lineNumber} không hợp lệ: \"{line}\" (Thông số cập nhật cần có dạng Thông số: Giá trị)");
                }
                else
                {
                    specs.Add(new SpecificationItem
                    {
                        Key = parts[0].Trim(),
                        Value = parts[1].Trim()
                    });
                }

                lineNumber++;
            }
            return specs;
        }
    }
}
