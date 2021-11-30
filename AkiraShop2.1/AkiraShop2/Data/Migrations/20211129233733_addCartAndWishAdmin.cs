using Microsoft.EntityFrameworkCore.Migrations;

namespace AkiraShop2.Data.Migrations
{
    public partial class addCartAndWishAdmin : Migration
    {
        const string ADMIN_USER_GUID = "da31e9b4-a632-4c72-a610-097e655dceab";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql($"INSERT INTO Order (UserOrderId, Status) VALUES ('{ADMIN_USER_GUID}','CART')");
            //migrationBuilder.Sql($"INSERT INTO Order (UserOrderId, Status) VALUES ('{ADMIN_USER_GUID}','WISH_LIST')");

            migrationBuilder.Sql($"INSERT INTO [dbo].[Order] ([UserOrderId],[Status],[AdditionalInfo],[TotalPrice]) VALUES ('{ADMIN_USER_GUID}','CART','null','0')");
            migrationBuilder.Sql($"INSERT INTO [dbo].[Order] ([UserOrderId],[Status],[AdditionalInfo],[TotalPrice]) VALUES ('{ADMIN_USER_GUID}','WISH_LIST','null','0')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM Order WHERE UserOrderId = '{ADMIN_USER_GUID}'");
        }
    }
}
