using System;
using System.Collections.Generic;

namespace AspireBudgetApi.Models
{
    public class DashboardRow
    {
        public string Name { get; set; }
        public DashboardRowType Type { get; set; }
        public double Available { get; set; }
        public double Spent { get; set; }
        public double Target { get; set; }
        public double Goal { get; set; }
        public double Budgeted { get; set; }
        public bool IsGoal { get; set; }

        public static DashboardRow FromGoogleRow(IList<object> row)
        {
            if (row.Count == 0 || string.IsNullOrEmpty(row[0].ToString()))
            {
                throw new ArgumentException($"No data in row for {nameof(DashboardRow)}.{nameof(FromGoogleRow)} initializer");
            }

            var result = new DashboardRow
            {
                Name = row[0].ToString().Trim()
            };
            if (row.Count == 1)
            {
                result.Type = DashboardRowType.Group;
                return result;
            }

            if (row.Count != 8)
            {
                throw new ArgumentException($"Wrong data in row for {nameof(DashboardRow)}.{nameof(FromGoogleRow)} initializer");
            }

            result.Type = DashboardRowType.Category;
            double.TryParse(row[1].ToString(), out var available);
            double.TryParse(row[4].ToString(), out var spent);
            double.TryParse(row[5].ToString(), out var target);
            double.TryParse(row[6].ToString(), out var goal);
            double.TryParse(row[7].ToString(), out var budgeted);

            result.IsGoal = !string.IsNullOrEmpty(row[3].ToString());
            result.Available = available;
            result.Spent = spent;
            result.Target = target;
            result.Goal = goal;
            result.Budgeted = budgeted;

            return result;
        }
    }

    public enum DashboardRowType
    {
        Group,
        Category
    }
}
