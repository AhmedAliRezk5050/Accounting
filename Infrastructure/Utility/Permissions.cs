namespace Infrastructure.Utility;

public static class Permissions
{
    public static List<string> GeneratePermissions(string module)
    {
        return new List<string>
        {
            $"Permission.{module}.View",
            $"Permission.{module}.Add",
            $"Permission.{module}.Edit",
            $"Permission.{module}.Delete",
        };
    }

    public static List<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();

        foreach (var item in Enum.GetValues(typeof(Modules)))
        {
            allPermissions.AddRange(GeneratePermissions(item.ToString()));
        }

        return allPermissions;
    }

    public static class Accounts
    {
        public const string View = "Permission.Accounts.View";
        public const string Add = "Permission.Accounts.Add";
        public const string Edit = "Permission.Accounts.Edit";
        public const string Delete = "Permission.Accounts.Delete";
    }

    public static class Entries
    {
        public const string View = "Permission.Entries.View";
        public const string Add = "Permission.Entries.Add";
        public const string Edit = "Permission.Entries.Edit";
        public const string Delete = "Permission.Entries.Delete";
    }

    public static class Users
    {
        public const string View = "Permission.Users.View";
        public const string Add = "Permission.Users.Add";
        public const string Edit = "Permission.Users.Edit";
        public const string Delete = "Permission.Users.Delete";
    }

    public static class UserPermissions
    {
        public const string View = "Permission.UserPermissions.View";
        public const string Add = "Permission.UserPermissions.Add";
        public const string Edit = "Permission.UserPermissions.Edit";
        public const string Delete = "Permission.UserPermissions.Delete";
    }

    public static class Banks
    {
        public const string View = "Permission.Banks.View";
    }

    public static class Customers
    {
        public const string View = "Permission.Customers.View";
        public const string Add = "Permission.Customers.Add";
    }

    public static class Suppliers
    {
        public const string View = "Permission.Suppliers.View";
        public const string Add = "Permission.Suppliers.View";
    }

    public static class GeneralLedger
    {
        public const string View = "Permission.GeneralLedger.View";
    }

    public static class TrialBalance
    {
        public const string View = "Permission.TrialBalance.View";
    }

    public static class IncomeStatement
    {
        public const string View = "Permission.IncomeStatement.View";
    }

    public static class BalanceSheet
    {
        public const string View = "Permission.BalanceSheet.View";
    }

    public static class PostEntry
    {
        public const string Add = "Permission.PostEntry.Add";
    }

    public static class Invoices
    {
        public const string View = "Permission.Invoices.View";
        public const string Add = "Permission.Invoices.Add";
        public const string Edit = "Permission.Invoices.Edit";
        public const string Delete = "Permission.Invoices.Delete";
    }
}