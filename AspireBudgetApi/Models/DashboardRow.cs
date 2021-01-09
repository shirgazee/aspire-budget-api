using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetApi.Models
{
    public class DashboardRow
    {
        public string Name { get; private set; }
        public DashboardRowType Type { get; private set; }
        public double Available { get; private set; }
        public double Spent { get; private set; }
        public double Target { get; private set; }
        public double Goal { get; private set; }
        public double Budgeted { get; private set; }
        public bool IsGoal { get; private set; }

        /// <summary>
        /// Set row type (unknown when extracted from google row)
        /// </summary>
        /// <param name="type"></param>
        public void SetType(DashboardRowType type)
        {
            Type = type;
        }

        /// <summary>
        /// Extract info from google row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
