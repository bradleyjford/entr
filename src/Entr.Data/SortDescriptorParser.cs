using System;
using System.Collections.Generic;

namespace Entr.Data
{
    public static class SortDescriptorParser
    {
        public static IEnumerable<SortDescriptor> Parse(string sortOrder)
        {
            var result = new List<SortDescriptor>();

            if (String.IsNullOrEmpty(sortOrder))
            {
                return result;
            }

            var sortSpecifications = sortOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sortSpecification in sortSpecifications)
            {
                var parts = sortSpecification.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1)
                {
                    result.Add(new SortDescriptor(parts[0], SortDirection.Ascending));
                }
                else if (parts.Length == 2)
                {
                    var direction = SortDirection.Ascending;

                    if (String.Compare("desc", parts[1], StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        direction = SortDirection.Descending;
                    }

                    result.Add(new SortDescriptor(parts[0], direction));
                }
            }

            return result;
        }
    }
}
