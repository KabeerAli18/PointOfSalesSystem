using System;
using POS;


//FOR ASSIGNMNET#01
class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        SetConsoleColors();
        PrintWelcomeMessage();

        bool exit = false;

        while (!exit)
        {
            PrintMainMenu();
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    var user = LogInUser();
                    if (user != null)
                    {
                        if (UserManager.IsAdmin(user))
                        {
                            AdminMenu();
                        }
                        else if (UserManager.IsCashier(user))
                        {
                            CashierMenu();
                        }
                    }
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void SetConsoleColors()
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
    }

    static void PrintWelcomeMessage()
    {
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("****************WELCOME TO POS DEMO CONSOLE APPLICATION****************************");
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine();
        Console.ResetColor();
    }

    static void PrintMainMenu()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("********************************** MAIN MENU **************************************");
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("1. Register User");
        Console.WriteLine("2. Log In");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }

    static void PrintErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void RegisterUser()
    {
        Console.Clear();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("******************************** REGISTER USER ************************************");
        Console.WriteLine("***********************************************************************************");

        try
        {
            Console.Write("Enter Your name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Your email: ");
            string? email = Console.ReadLine();
            Console.Write("Enter Your password: ");
            string? password = Console.ReadLine();
            Console.Write("Enter Your role (Admin/Cashier): ");
            string? roleInput = Console.ReadLine();
            UserRoles role = roleInput?.ToLower() == "admin" ? Roles.Admin : Roles.Cashier;

            UserManager.RegisterUser(name!, email!, password!, role); // Ensure non-null references
            Console.WriteLine("User registered successfully.");
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static Users? LogInUser()
    {
        Console.Clear();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("********************************** LOG IN *****************************************");
        Console.WriteLine("***********************************************************************************");

        try
        {
            Console.Write("Enter Your email: ");
            string? email = Console.ReadLine();
            Console.Write("Enter Your password: ");
            string? password = Console.ReadLine();

            var user = UserManager.LogInUserAuthentication(email!, password!); // Ensure non-null references
            Console.WriteLine("Log in successful.");
            return user;
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
            return null;
        }
    }

    static void AdminMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("********************************** ADMIN MENU *************************************");
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Remove Product");
            Console.WriteLine("4. View Products");
            Console.WriteLine("5. Track Inventory");
            Console.WriteLine("6. Receive New Stock");
            Console.WriteLine("7. Reduce Stock");
            Console.WriteLine("8. Change User Role"); // Option to change user role
            Console.WriteLine("9. Log Out");
            Console.Write("Select an option: ");

            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    UpdateProduct();
                    break;
                case "3":
                    RemoveProduct();
                    break;
                case "4":
                    ViewProducts();
                    break;
                case "5":
                    TrackInventory();
                    break;
                case "6":
                    ReceiveNewStock();
                    break;
                case "7":
                    ReduceStock();
                    break;
                case "8":
                    ChangeUserRole(); // Call to new method for changing user role
                    break;
                case "9":
                    exit = true;
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please try again.");
                    break;
            }
        }
    }


    static void CashierMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("******************************* CASHIER MENU **************************************");
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("1. Add Product to Sale");
            Console.WriteLine("2. Calculate Sales Total Amount");
            Console.WriteLine("3. Generate Receipt for Completed Sales Transactions");
            Console.WriteLine("4. Add Product to Purchase Order");
            Console.WriteLine("5. Calculate Purchase Total Amount");
            Console.WriteLine("6. Generate Purchase Receipt");
            Console.WriteLine("7. Log Out");
            Console.Write("Select an option: ");

            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    AddProductToSale();
                    break;
                case "2":

                    Console.WriteLine($"Total Sales Amount : {SalesTransaction.CalculateTotalSalesAmount()}");
                    Console.ReadKey();
                    break;
                case "3":
                    GenerateSalesReceipt();
                    break;
                case "4":
                    AddProductToPurchaseOrder();
                    break;
                case "5":
                    Console.WriteLine( $"Total Purchase  Amount :{ PurchaseTransactions.CalculateTotalPurchaseAmount()}");
                    Console.ReadKey();
                    
                    break;
                case "6":
                    GeneratePurchaseReceipt();
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please try again.");
                    break;
            }
        }
    }


    static void ChangeUserRole()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("******************************* CHANGE USER ROLE **********************************");
        Console.WriteLine("***********************************************************************************");

        try
        {
            Console.Write("Enter User Email to change role: ");
            string? userEmail = Console.ReadLine();
            Console.Write("Enter New Role (Admin/Cashier): ");
            string? roleInput = Console.ReadLine();

            UserRoles newRole = roleInput?.ToLower() 
            switch
            {
                "admin" => Roles.Admin,
                "cashier" => Roles.Cashier,
                _ => throw new ArgumentException("Invalid role. Please enter 'Admin' or 'Cashier'.")
            };

            bool success = UserManager.ChangeUserRole(userEmail!, newRole); // Ensure non-null references

            if (success)
            {
                Console.WriteLine("User role changed successfully.");
            }
            else
            {
                PrintErrorMessage("User not found.");
            }
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }


    static void AddProduct()
    {
        InventoryManager.DisplayInventoryTable();
        Console.WriteLine();
        try
        {
            Console.Write("Enter Product ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Product Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Product Price: ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Product Quantity: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Product Type: ");
            string? type = Console.ReadLine();
            Console.Write("Enter Product Category: ");
            string? category = Console.ReadLine();

            InventoryManager.AddProduct(id, name!, price, quantity, type!, category!); // Ensure non-null references
            Console.WriteLine("Product added successfully.");
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void UpdateProduct()
    {

        try
        {
            InventoryManager.DisplayInventoryTable();
            Console.WriteLine();
            Console.Write("Enter Product ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter New Product Name (leave empty to skip): ");
            string? name = Console.ReadLine();
            Console.Write("Enter New Product Price (leave empty to skip): ");
            string? priceInput = Console.ReadLine();
            Console.Write("Enter New Product Quantity (leave empty to skip): ");
            string? quantityInput = Console.ReadLine();
            Console.Write("Enter New Product Type (leave empty to skip): ");
            string? type = Console.ReadLine();
            Console.Write("Enter New Product Category (leave empty to skip): ");
            string? category = Console.ReadLine();

            decimal? price = string.IsNullOrEmpty(priceInput) ? null : decimal.Parse(priceInput);
            int? quantity = string.IsNullOrEmpty(quantityInput) ? null : int.Parse(quantityInput);

            bool success = InventoryManager.UpdateProduct(id, name!, price, quantity, type!, category!); // Ensure non-null references

            if (success)
            {
                Console.WriteLine("Product updated successfully.");
            }
            else
            {
                PrintErrorMessage("Product not found.");
            }
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void RemoveProduct()
    {
        try
        {
            InventoryManager.DisplayInventoryTable();
            Console.WriteLine();
            Console.Write("Enter Product ID to remove: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            bool success = InventoryManager.RemoveProduct(id);

            if (success)
            {
                Console.WriteLine("Product removed successfully.");
            }
            else
            {
                PrintErrorMessage("Product not found.");
            }
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ViewProducts()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("******************************* VIEW PRODUCTS *************************************");
        Console.WriteLine("***********************************************************************************");

        var products = InventoryManager.ViewProducts();

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
        }
        else
        {
            InventoryManager.DisplayInventoryTable();
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void TrackInventory()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("***************************** TRACK INVENTORY *************************************");
        Console.WriteLine("***********************************************************************************");

        InventoryManager.TrackInventory();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ReceiveNewStock()
    {
        try
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("***************************** RECEIVE NEW STOCK ***********************************");
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine();
            Console.Write("Enter Product ID to receive stock: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Quantity to receive: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");
           bool flag= InventoryManager.ReceiveNewStock(id, quantity);
           Console.WriteLine($"{id} {quantity} {flag}");
            Console.WriteLine("Stock received successfully.");
            InventoryManager.DisplayInventoryTable();
            var product = InventoryManager.ViewProducts().FirstOrDefault(p => p.Id == id);
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ReduceStock()
    {
        try
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("******************************* REDUCE STOCK *************************************");
            Console.WriteLine("***********************************************************************************");
            InventoryManager.DisplayInventoryTable();
            Console.WriteLine();
            Console.Write("Enter Product ID to reduce stock: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Quantity to reduce: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            bool flag=InventoryManager.ReduceStock(id, quantity);
            Console.WriteLine($"{id} {quantity} {flag}");
            Console.WriteLine("Stock reduced successfully.");
            InventoryManager.DisplayInventoryTable();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ShowInventoryItems()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("***************************** INVENTORY ITEMS ************************************");
        Console.WriteLine("***********************************************************************************");

        InventoryManager.ShowInventoryItems();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void AddProductToSale()
    {
        try
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("***************************** ADD PRODUCT TO SALE *********************************");
            Console.WriteLine("***********************************************************************************");

            Console.Write("Enter Product ID to add to sale: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            // Retrieve product details based on ID
            Product product = InventoryManager.FindProductByID(id);

            if (product == null)
            {
                PrintErrorMessage("Product not found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Quantity to add: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            // Add product to sale with the specified quantity
            SalesTransaction.AddProductToSale(product, quantity);

            Console.WriteLine("Product added to sale.");
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }



    static void GenerateSalesReceipt()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("****************************** GENERATE SALES RECEIPT *****************************");
        Console.WriteLine("***********************************************************************************");

        var receipt = SalesTransaction.GenerateSalesTransactionsReceipt();

        Console.WriteLine(receipt);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ClearSaleItems()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("****************************** CLEAR SALE ITEMS ***********************************");
        Console.WriteLine("***********************************************************************************");

        SalesTransaction.ClearSaleItems();

        Console.WriteLine("Sale items cleared.");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void AddProductToPurchaseOrder()
    {
        try
        {
            Console.Clear();
            SetConsoleColors();
            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("************************ ADD PRODUCT TO PURCHASE ORDER ****************************");
            Console.WriteLine("***********************************************************************************");
            InventoryManager.DisplayInventoryTable();
            Console.WriteLine();
            Console.Write("Enter Product ID to add to purchase order: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            // Retrieve product details based on ID
            Product product = InventoryManager.FindProductByID(id);

            if (product == null)
            {
                PrintErrorMessage("Product not found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Quantity to add: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            // Add product to purchase order
            PurchaseTransactions.AddProductToPurchaseOrder(product, quantity);

            Console.WriteLine("Product added to purchase order.");
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void GeneratePurchaseReceipt()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("************************** GENERATE PURCHASE RECEIPT ******************************");
        Console.WriteLine("***********************************************************************************");

        var receipt = PurchaseTransactions.GeneratePurchaseReceipt();

        Console.WriteLine(receipt);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void ClearPurchaseItems()
    {
        Console.Clear();
        SetConsoleColors();
        Console.WriteLine("***********************************************************************************");
        Console.WriteLine("****************************** CLEAR PURCHASE ITEMS *******************************");
        Console.WriteLine("***********************************************************************************");

        PurchaseTransactions.ClearPurchaseItems();

        Console.WriteLine("Purchase items cleared.");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
